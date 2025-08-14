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

// ====== 設定 SQLite DB 路徑到 Azure 持久化資料夾 ======
var dataPath = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? ".", "data");
if (!Directory.Exists(dataPath))
{
    Directory.CreateDirectory(dataPath);
}
var dbFilePath = Path.Combine(dataPath, "drinkshop.db");

// Add services to the container.
builder.Services.AddControllers();
// DI 註冊
builder.Services.AddScoped<IDrinkService, DrinkService>();
builder.Services.AddScoped<IDrinkRepository, DrinkRepository>();
builder.Services.AddDbContext<DrinkShopDbContext>(options =>
    options.UseSqlite($"Data Source={dbFilePath}"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 啟用 CORS
app.UseCors("AllowFrontend");

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

app.Run();
