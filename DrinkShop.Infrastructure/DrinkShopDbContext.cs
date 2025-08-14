using Microsoft.EntityFrameworkCore;
using DrinkShop.Domain.Entities;

namespace DrinkShop.Infrastructure
{
    public class DrinkShopDbContext : DbContext
    {
        public DrinkShopDbContext(DbContextOptions<DrinkShopDbContext> options) : base(options) { }

        public DbSet<Drink> Drinks => Set<Drink>();
    }
}
