using DrinkShop.Application.Interfaces;
using DrinkShop.Application.Services;
using DrinkShop.Infrastructure;
using DrinkShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// CORS 設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
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

// 簡化的資料庫設定
string dbPath;
var dbPathFromEnv = Environment.GetEnvironmentVariable("DB_PATH");

if (!string.IsNullOrWhiteSpace(dbPathFromEnv))
{
    dbPath = dbPathFromEnv!;
}
else if (builder.Environment.IsProduction())
{
    var homeEnv = Environment.GetEnvironmentVariable("HOME") ?? "D:\\home";
    var dataDir = Path.Combine(homeEnv, "data");
    dbPath = Path.Combine(dataDir, "drinkshop.db");
    
    // 確保目錄存在
    if (!Directory.Exists(dataDir))
    {
        Directory.CreateDirectory(dataDir);
    }
}
else
{
    var dataDir = Path.Combine(builder.Environment.ContentRootPath, "data");
    if (!Directory.Exists(dataDir))
    {
        Directory.CreateDirectory(dataDir);
    }
    dbPath = Path.Combine(dataDir, "drinkshop.db");
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Repository DI 註冊
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// 配置 DbContext
var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = dbPath,
    Mode = SqliteOpenMode.ReadWriteCreate,
    Cache = SqliteCacheMode.Shared
}.ToString();

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

// Configure the HTTP request pipeline
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthorization();

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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok())
   .RequireCors("AllowFrontend");

app.MapControllers().RequireCors("AllowFrontend");

// 簡化的資料庫初始化 - 移到背景執行，不阻擋應用程式啟動
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DrinkShopDbContext>();
    
    try
    {
        await db.Database.EnsureCreatedAsync();
        
        // 檢查是否需要創建 admin 用戶
        if (!await db.Users.AnyAsync(u => u.UserName == "admin"))
        {
            var adminUser = new DrinkShop.Domain.Entities.User
            {
                UserName = "admin",
                Password = "admin123",
                Email = "admin@drinkshop.com",
                Role = "admin",
                Phone = "0000000000",
                PasswordHash = "admin123",
                IsActive = true,
                Status = "active"
            };
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Background database initialization error");
    }
});

app.Run();
