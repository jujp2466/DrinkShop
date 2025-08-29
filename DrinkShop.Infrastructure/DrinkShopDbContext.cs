using Microsoft.EntityFrameworkCore;
using DrinkShop.Domain.Entities;

namespace DrinkShop.Infrastructure
{
    public class DrinkShopDbContext : DbContext
    {
        public DrinkShopDbContext(DbContextOptions<DrinkShopDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users -> users
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Username).HasColumnName("username").IsRequired();
                e.Property(x => x.Password).HasColumnName("password").IsRequired();
                e.Property(x => x.Email).HasColumnName("email").IsRequired();
                e.Property(x => x.Role).HasColumnName("role").HasDefaultValue("user");
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();
            });

            // Products -> products
            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("products");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasColumnName("name").IsRequired();
                e.Property(x => x.Description).HasColumnName("description");
                e.Property(x => x.Price).HasColumnName("price").HasColumnType("REAL");
                e.Property(x => x.Category).HasColumnName("category");
                e.Property(x => x.ImageUrl).HasColumnName("image_url");
                e.Property(x => x.Stock).HasColumnName("stock").HasDefaultValue(0);
                e.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Orders -> orders
            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("orders");
                e.HasKey(x => x.Id);
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.TotalAmount).HasColumnName("total_amount").HasColumnType("REAL");
                e.Property(x => x.Status).HasColumnName("status").HasDefaultValue("pending");
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // OrderItems -> order_items
            modelBuilder.Entity<OrderItem>(e =>
            {
                e.ToTable("order_items");
                e.HasKey(x => x.Id);
                e.Property(x => x.OrderId).HasColumnName("order_id");
                e.Property(x => x.ProductId).HasColumnName("product_id");
                e.Property(x => x.Quantity).HasColumnName("quantity");
                e.Property(x => x.Price).HasColumnName("price").HasColumnType("REAL");
                e.HasOne<Order>()
                    .WithMany(o => o.Items)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
