using DrinkShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// CORS 設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // 無論環境如何，允許所有必要的域名，避免環境檢測問題
        policy.WithOrigins(
                "https://blue-island-07506ba00.1.azurestaticapps.net",
                "http://localhost:5173",
                "http://localhost:5174"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ====== 設定 SQLite DB 路徑到應用程式內容根底下的 data 資料夾（避免不同工作目錄造成不同 DB） ======
var contentRoot = builder.Environment.ContentRootPath; // 明確使用應用程式內容根路徑
// 如果在 Azure App Service 上，系統會提供 HOME 環境變數，該路徑位於持久化儲存 (D:\home)，推薦把 DB 放在這裡以避免被部署覆寫。
var homeEnv = Environment.GetEnvironmentVariable("HOME");

// 增加診斷資訊
Console.WriteLine($"Content Root: {contentRoot}");
Console.WriteLine($"HOME env: {homeEnv ?? "not set"}");

// 在 Azure 上使用 site/wwwroot 下的 App_Data 目錄，這是建議的持久化儲存位置
string defaultDataPath = !string.IsNullOrEmpty(homeEnv)
    ? Path.Combine(homeEnv, "site", "wwwroot", "App_Data")
    : Path.Combine(contentRoot, "data");
    
var defaultDbPath = Path.Combine(defaultDataPath, "drinkshop.db");
// 優先使用環境變數 DB_PATH（可在 Azure App Settings 設定），否則使用預設路徑
var dbFilePath = Environment.GetEnvironmentVariable("DB_PATH") ?? defaultDbPath;
var dbFolder = Path.GetDirectoryName(dbFilePath)!;

Console.WriteLine($"DB Folder: {dbFolder}");
Console.WriteLine($"DB File Path: {dbFilePath}");

// 確保資料夾存在且有權限
try
{
    if (!Directory.Exists(dbFolder))
    {
        Console.WriteLine($"Creating database directory: {dbFolder}");
        Directory.CreateDirectory(dbFolder);
    }
    
    // 檢查是否有寫入權限
    var testFile = Path.Combine(dbFolder, ".write-test");
    File.WriteAllText(testFile, DateTime.Now.ToString());
    File.Delete(testFile);
    Console.WriteLine($"Write permission test passed for: {dbFolder}");
}
catch (Exception ex)
{
    Console.WriteLine($"CRITICAL ERROR: Failed to access database directory: {ex}");
    throw; // 讓應用程式直接失敗，以便在日誌中顯示錯誤
}

// Add services to the container.
builder.Services.AddControllers();
// DI 註冊（已移除舊的 Drink 服務/儲存庫）
builder.Services.AddDbContext<DrinkShopDbContext>(options =>
    options.UseSqlite($"Data Source={dbFilePath}", b => b.MigrationsAssembly("DrinkShop.Infrastructure")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 正確的中介軟體順序
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication(); // 如果有使用
app.UseAuthorization();

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

// 處理預檢請求（避免 404 導致瀏覽器判定無 CORS 標頭）
app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok())
   .RequireCors("AllowFrontend");

app.MapControllers().RequireCors("AllowFrontend");

// ====== 自動建立資料表 (EF Core Migration) ======
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<DrinkShopDbContext>();
        Console.WriteLine($"Attempting to migrate database at {dbFilePath}");
        
        // 確保 SQLite 能正常連接
        Console.WriteLine("Testing database connection...");
        var canConnect = db.Database.CanConnect();
        Console.WriteLine($"Database connection test: {(canConnect ? "SUCCESS" : "FAILED")}");
        
        if (canConnect) {
            Console.WriteLine("Running migrations...");
            db.Database.Migrate(); // 如果資料表不存在就自動建立
            Console.WriteLine("Migrations completed successfully");
        } else {
            throw new InvalidOperationException("Cannot connect to the database");
        }
        
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
        logger.LogError(ex, "Database initialization error. Details: {Message}", ex.ToString());
        Console.WriteLine($"FATAL ERROR: Database initialization failed: {ex.Message}");
        Console.WriteLine(ex.ToString());
        // 重新拋出例外，以防止應用程式在損壞狀態下啟動。
        throw;
    }
}
// ===============================================

app.Run();
