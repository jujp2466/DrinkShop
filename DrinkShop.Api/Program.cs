using DrinkShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Text;

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

// ==================== 資料庫設定 ====================
// 輸出環境資訊
bool isProduction = builder.Environment.IsProduction();
bool isDevelopment = builder.Environment.IsDevelopment();
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"IsProduction: {isProduction}, IsDevelopment: {isDevelopment}");

// 確定資料庫檔案路徑
string contentRoot = builder.Environment.ContentRootPath;
Console.WriteLine($"ContentRootPath: {contentRoot}");

// 在 Azure 上使用特殊路徑，這是一個可寫的路徑
string dbPath;
if (isProduction)
{
    // 在 Azure App Service 上使用 TEMP 目錄，這是可寫的
    var tempDir = Environment.GetEnvironmentVariable("TEMP") ?? Path.Combine(contentRoot, "App_Data");
    dbPath = Path.Combine(tempDir, "drinkshop.db");
    Console.WriteLine($"PRODUCTION: Using Azure App Service TEMP directory for database: {dbPath}");
}
else
{
    // 本機開發環境使用實體檔案
    var dataDir = Path.Combine(contentRoot, "data");
    if (!Directory.Exists(dataDir))
    {
        Directory.CreateDirectory(dataDir);
    }
    dbPath = Path.Combine(dataDir, "drinkshop.db");
    Console.WriteLine($"DEVELOPMENT: Using file-based SQLite database at: {dbPath}");
}

// 確保目錄存在
var dbDir = Path.GetDirectoryName(dbPath);
if (!Directory.Exists(dbDir))
{
    Directory.CreateDirectory(dbDir);
    Console.WriteLine($"Created database directory: {dbDir}");
}

// Add services to the container.
builder.Services.AddControllers();

// 配置 DbContext - 使用 SQLite 共享快取模式
var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = dbPath,
    Mode = SqliteOpenMode.ReadWriteCreate,
    Cache = SqliteCacheMode.Shared
}.ToString();

Console.WriteLine($"Using connection string: {connectionString}");

// 使用連接池模式，確保可靠性
builder.Services.AddDbContext<DrinkShopDbContext>(options =>
{
    options.UseSqlite(connectionString, sqliteOptions =>
    {
        sqliteOptions.MigrationsAssembly("DrinkShop.Infrastructure");
    });
}, ServiceLifetime.Scoped);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 正確的中介軟體順序
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication(); // 如果有使用
app.UseAuthorization();

// Log actual DB path for debugging
Console.WriteLine($"Using database file: {dbPath}");

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
        
        // 處理記憶體內資料庫的邏輯
        if (dbFilePath == ":memory:")
        {
            Console.WriteLine("Initializing in-memory database...");
            
            // 記憶體內資料庫需要顯式建立資料表結構
            db.Database.EnsureCreated();
            Console.WriteLine("In-memory database schema created");
            
            // 確保有基本資料
            SeedDatabaseWithSampleData(db);
        }
        else
        {
            // 檔案型資料庫正常進行遷移
            Console.WriteLine("Running migrations on file-based database...");
            db.Database.Migrate();
            Console.WriteLine("Database migrations completed");
        }
        
        // 確保有 admin 用戶
        var adminUser = db.Users.FirstOrDefault(u => u.UserName == "admin");
        if (adminUser == null)
        {
            // 創建新的 admin 用戶
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
        else
        {
            Console.WriteLine($"Admin user '{adminUser.UserName}' already exists");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database initialization error");
        Console.WriteLine($"FATAL ERROR: Database initialization failed: {ex.Message}");
        Console.WriteLine(ex.ToString());
        
        // 為生產環境進行特殊處理 - 在 Azure 上不要使應用程式崩潰
        if (!isProduction)
        {
            throw; // 開發環境下重新拋出例外
        }
        
        // 生產環境下記錄錯誤但繼續運行，使用應急方案
        Console.WriteLine("PRODUCTION ENVIRONMENT: Continuing despite database error");
    }
}

// 範例資料填充方法
void SeedDatabaseWithSampleData(DrinkShopDbContext db)
{
    try
    {
        Console.WriteLine("Checking for existing products...");
        if (!db.Products.Any())
        {
            Console.WriteLine("No products found. Adding sample products...");
            
            // 建立一些範例產品
            var products = new List<DrinkShop.Domain.Entities.Product>
            {
                new DrinkShop.Domain.Entities.Product
                {
                    Name = "綠茶",
                    Description = "清爽的綠茶飲品",
                    Price = 30,
                    Category = "茶類",
                    ImageUrl = "https://example.com/greentea.jpg",
                    Stock = 100,
                    IsActive = true
                },
                new DrinkShop.Domain.Entities.Product
                {
                    Name = "珍珠奶茶",
                    Description = "香濃奶茶配上 Q 彈珍珠",
                    Price = 60,
                    Category = "奶茶",
                    ImageUrl = "https://example.com/bubbletea.jpg",
                    Stock = 80,
                    IsActive = true
                },
                new DrinkShop.Domain.Entities.Product
                {
                    Name = "芒果冰沙",
                    Description = "夏日消暑特調芒果冰沙",
                    Price = 80,
                    Category = "冰沙",
                    ImageUrl = "https://example.com/mangosmoothie.jpg",
                    Stock = 50,
                    IsActive = true
                }
            };
            
            db.Products.AddRange(products);
            db.SaveChanges();
            Console.WriteLine($"Added {products.Count} sample products");
        }
        else
        {
            Console.WriteLine("Products already exist in the database");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding database: {ex.Message}");
        // 記錄但不拋出，避免初始化失敗
    }
}
// ===============================================

app.Run();
