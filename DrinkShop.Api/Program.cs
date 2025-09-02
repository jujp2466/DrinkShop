using DrinkShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// CORS 設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // 開發環境允許任意來源與方法，避免 Vite 動態埠（5173/5174/…）造成阻擋
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:5174", // 新的本地端口
                    "https://drinkshop-c5ccheftavfvh0av.japaneast-01.azurewebsites.net",
                    "https://blue-island-07506ba00.1.azurestaticapps.net"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // 添加憑證支援
        }
    });
});

// ====== 設定 SQLite DB 路徑到應用程式內容根底下的 data 資料夾（避免不同工作目錄造成不同 DB） ======
var contentRoot = builder.Environment.ContentRootPath; // 明確使用應用程式內容根路徑
// 如果在 Azure App Service 上，系統會提供 HOME 環境變數，該路徑位於持久化儲存 (D:\home)，推薦把 DB 放在這裡以避免被部署覆寫。
var homeEnv = Environment.GetEnvironmentVariable("HOME");
string defaultDataPath = !string.IsNullOrEmpty(homeEnv)
    ? Path.Combine(homeEnv, "data")
    : Path.Combine(contentRoot, "data");
var defaultDbPath = Path.Combine(defaultDataPath, "drinkshop.db");
// 優先使用環境變數 DB_PATH（可在 Azure App Settings 設定），否則使用預設路徑
var dbFilePath = Environment.GetEnvironmentVariable("DB_PATH") ?? defaultDbPath;
var dbFolder = Path.GetDirectoryName(dbFilePath)!;
if (!Directory.Exists(dbFolder))
{
    Directory.CreateDirectory(dbFolder);
}

// Add services to the container.
builder.Services.AddControllers();
// DI 註冊（已移除舊的 Drink 服務/儲存庫）
builder.Services.AddDbContext<DrinkShopDbContext>(options =>
    options.UseSqlite($"Data Source={dbFilePath}", b => b.MigrationsAssembly("DrinkShop.Infrastructure")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 啟用 CORS
app.UseRouting();
app.UseCors("AllowFrontend");

// Log actual DB path for debugging
Console.WriteLine($"Using database file: {dbFilePath}");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DrinkShop API V1");
        c.RoutePrefix = "swagger";
    });
    // 自動導向 swagger 首頁
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/" || context.Request.Path == "/index.html")
        {
            context.Response.Redirect("/swagger/index.html");
            return;
        }
        await next();
    });
}

// 僅在非開發環境啟用 HTTPS 轉址，避免本機只有 http 監聽時造成預檢/跨域被 307 轉址
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();

// ====== 自動建立資料表 (EF Core Migration) ======
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<DrinkShopDbContext>();
        db.Database.Migrate(); // 如果資料表不存在就自動建立
        
        // 確保有admin用戶
        var adminUser = db.Users.FirstOrDefault(u => u.UserName == "admin");
        if (adminUser == null)
        {
            // 創建新的admin用戶
            adminUser = new DrinkShop.Domain.Entities.User
            {
                UserName = "admin",
                Password = "admin123", // 生產環境應該使用加密
                Email = "admin@drinkshop.com",
                Role = "admin",
                Phone = "0000000000", // 預設電話號碼
                PasswordHash = "admin123", // 暫時使用明文，生產環境應加密
                IsActive = true,
                Status = "active"
            };
            db.Users.Add(adminUser);
            db.SaveChanges();
            Console.WriteLine("Default admin user created: admin/admin123");
        }
        else if (adminUser.Role != "admin")
        {
            // 將現有用戶設置為admin
            adminUser.Role = "admin";
            db.SaveChanges();
            Console.WriteLine($"User '{adminUser.UserName}' role updated to admin");
        }
        else
        {
            Console.WriteLine($"Admin user '{adminUser.UserName}' already exists");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization.");
        // 根據你的策略，你可能想在這裡讓應用程式失敗，或只是記錄錯誤並繼續
    }
}
// ===============================================

app.Run();
