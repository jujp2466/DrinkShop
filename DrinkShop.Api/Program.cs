using DrinkShop.Application.Interfaces;
using DrinkShop.Application.Services;
using DrinkShop.Infrastructure;
using DrinkShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// CORS 設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
            "http://localhost:5173",
            "https://drinkshop-c5ccheftavfvh0av.japaneast-01.azurewebsites.net",
            "https://blue-island-07506ba00.1.azurestaticapps.net"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
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
// DI 註冊
builder.Services.AddScoped<IDrinkService, DrinkService>();
builder.Services.AddScoped<IDrinkRepository, DrinkRepository>();
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ====== 自動建立資料表 (EF Core Migration) ======
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DrinkShopDbContext>();
    db.Database.Migrate(); // 如果資料表不存在就自動建立
}
// ===============================================

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DrinkShopDbContext>();
    db.Database.ExecuteSqlRaw("DELETE FROM __EFMigrationsLock;");
}

app.Run();
