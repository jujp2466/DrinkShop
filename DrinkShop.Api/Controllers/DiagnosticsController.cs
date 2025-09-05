using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrinkShop.Infrastructure;
using DrinkShop.Domain.Entities;
using Microsoft.Data.Sqlite;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly DrinkShopDbContext _context;
        private readonly IConfiguration _configuration;

        public DiagnosticsController(DrinkShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<object> Get()
        {
            var envVars = new Dictionary<string, string>
            {
                ["ASPNETCORE_ENVIRONMENT"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                ["HOME"] = Environment.GetEnvironmentVariable("HOME"),
                ["DB_PATH"] = Environment.GetEnvironmentVariable("DB_PATH"),
                ["TEMP"] = Environment.GetEnvironmentVariable("TEMP"),
                ["WEBSITE_SITE_NAME"] = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")
            };

            var dbPath = Environment.GetEnvironmentVariable("DB_PATH") ?? 
                        Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", "data", "drinkshop.db");
            
            return new
            {
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                databasePath = dbPath,
                databaseExists = System.IO.File.Exists(dbPath),
                directoryExists = Directory.Exists(Path.GetDirectoryName(dbPath)),
                environmentVariables = envVars,
                canConnect = CanConnectToDatabase(),
                runtimeVersion = Environment.Version.ToString()
            };
        }

        [HttpGet("rebuild-database")]
        public async Task<ActionResult<object>> RebuildDatabase()
        {
            try
            {
                var dbPath = Environment.GetEnvironmentVariable("DB_PATH") ?? 
                            Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", "data", "drinkshop.db");
                
                // 確保目錄存在
                var directory = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 釋放資料庫連線，以便刪除檔案
                await _context.Database.CloseConnectionAsync();
                SqliteConnection.ClearAllPools();

                // 刪除現有資料庫檔案
                if (System.IO.File.Exists(dbPath))
                {
                    System.IO.File.Delete(dbPath);
                }

                // 重建資料庫結構
                await _context.Database.EnsureCreatedAsync();
                
                // 執行種子資料
                await SeedDatabaseAsync();

                var productCount = await _context.Products.CountAsync();

                return Ok(new
                {
                    success = true,
                    message = "資料庫已成功重建",
                    databasePath = dbPath,
                    timestamp = DateTime.UtcNow,
                    tablesCreated = productCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        private bool CanConnectToDatabase()
        {
            try
            {
                return _context.Database.CanConnect();
            }
            catch
            {
                return false;
            }
        }

        private async Task SeedDatabaseAsync()
        {
            try
            {
                if (!await _context.Products.AnyAsync())
                {
                    var products = new List<Product>
                    {
                        new Product
                        {
                            Name = "綠茶",
                            Description = "清爽的綠茶飲品",
                            Price = 30,
                            Category = "茶類",
                            ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=300",
                            Stock = 100,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "珍珠奶茶",
                            Description = "香濃奶茶配上 Q 彈珍珠",
                            Price = 60,
                            Category = "奶茶",
                            ImageUrl = "https://images.unsplash.com/photo-1525385133512-2f3bdd039054?w=300",
                            Stock = 80,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "芒果冰沙",
                            Description = "夏日消暑特調芒果冰沙",
                            Price = 80,
                            Category = "冰沙",
                            ImageUrl = "https://images.unsplash.com/photo-1570197788417-0e82375c9371?w=300",
                            Stock = 50,
                            IsActive = true
                        }
                    };
                    
                    _context.Products.AddRange(products);
                    await _context.SaveChangesAsync();
                }

                // 確保有 admin 用戶
                if (!await _context.Users.AnyAsync(u => u.UserName == "admin"))
                {
                    var adminUser = new User
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
                    _context.Users.Add(adminUser);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding error: {ex.Message}");
                throw;
            }
        }
    }
}
