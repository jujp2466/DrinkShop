using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DrinkShop.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DrinkShopDbContext>
    {
        public DrinkShopDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DrinkShopDbContext>();

            var contentRoot = Directory.GetCurrentDirectory();
            var homeEnv = Environment.GetEnvironmentVariable("HOME");
            string defaultDataPath = !string.IsNullOrEmpty(homeEnv)
                ? Path.Combine(homeEnv, "data")
                : Path.Combine(contentRoot, "DrinkShop.Api", "data");
            var defaultDbPath = Path.Combine(defaultDataPath, "drinkshop.db");
            var dbFilePath = Environment.GetEnvironmentVariable("DB_PATH") ?? defaultDbPath;

            var folder = Path.GetDirectoryName(dbFilePath)!;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            optionsBuilder.UseSqlite($"Data Source={dbFilePath}", b => b.MigrationsAssembly("DrinkShop.Infrastructure"));
            return new DrinkShopDbContext(optionsBuilder.Options);
        }
    }
}
