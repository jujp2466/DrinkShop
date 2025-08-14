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
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddControllers();
// DI 註冊
builder.Services.AddScoped<IDrinkService, DrinkService>();
builder.Services.AddScoped<IDrinkRepository, DrinkRepository>();
builder.Services.AddDbContext<DrinkShopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=drinkshop.db"));
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

app.Run();
