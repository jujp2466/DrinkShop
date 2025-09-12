using DrinkShop.Application.Interfaces;
using DrinkShop.Application.Services;
using DrinkShop.Infrastructure;
using DrinkShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// CORS 設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
    policy.WithOrigins(
        "https://blue-island-07506ba00.1.azurestaticapps.net",
        "http://localhost:5173"
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

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        RoleClaimType = ClaimTypes.Role
    };
});

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DrinkShop API", Version = "v1" });

    // Define the BearerAuth security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "請直接輸入 JWT token，不需加 Bearer 前綴",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication(); // <-- 啟用驗證
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
