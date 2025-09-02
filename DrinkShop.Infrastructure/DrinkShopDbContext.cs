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
                e.Property(x => x.UserName).HasColumnName("userName").IsRequired();
                e.Property(x => x.Email).HasColumnName("email").IsRequired();
                e.Property(x => x.PasswordHash).HasColumnName("passwordHash");
                e.Property(x => x.IsActive).HasColumnName("isActive").HasDefaultValue(true);
                e.Property(x => x.Password).HasColumnName("password").IsRequired();
                e.Property(x => x.Role).HasColumnName("role").HasDefaultValue("user");
                e.Property(x => x.Phone).HasColumnName("phone");
                e.Property(x => x.Address).HasColumnName("address");
                e.Property(x => x.Status).HasColumnName("status").HasDefaultValue("active");
                e.Property(x => x.LastLoginAt).HasColumnName("lastLoginAt");
                e.Property(x => x.CreatedAt).HasColumnName("createdAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.HasIndex(x => x.UserName).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();
            });            // Products -> products
            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("products");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasColumnName("name").IsRequired();
                e.Property(x => x.Description).HasColumnName("description");
                e.Property(x => x.Price).HasColumnName("price").HasColumnType("REAL");
                e.Property(x => x.Category).HasColumnName("category");
                e.Property(x => x.ImageUrl).HasColumnName("imageUrl");
                e.Property(x => x.Stock).HasColumnName("stock").HasDefaultValue(0);
                e.Property(x => x.IsActive).HasColumnName("isActive").HasDefaultValue(true);
                e.Property(x => x.CreatedAt).HasColumnName("createdAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Orders -> orders
            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("orders");
                e.HasKey(x => x.Id);
                e.Property(x => x.UserId).HasColumnName("userId");
                e.Property(x => x.TotalAmount).HasColumnName("totalAmount").HasColumnType("REAL");
                e.Property(x => x.ShippingFee).HasColumnName("shippingFee").HasColumnType("REAL").HasDefaultValue(0);
                e.Property(x => x.Status).HasColumnName("status").HasDefaultValue("pending");
                e.Property(x => x.ShippingAddress).HasColumnName("shippingAddress");
                e.Property(x => x.PaymentMethod).HasColumnName("paymentMethod");
                e.Property(x => x.Notes).HasColumnName("notes");
                e.Property(x => x.CreatedAt).HasColumnName("createdAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.HasOne(x => x.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItems -> order_items
            modelBuilder.Entity<OrderItem>(e =>
            {
                e.ToTable("orderItems");
                e.HasKey(x => x.Id);
                e.Property(x => x.OrderId).HasColumnName("orderId");
                e.Property(x => x.ProductId).HasColumnName("productId");
                e.Property(x => x.Quantity).HasColumnName("quantity");
                e.Property(x => x.Price).HasColumnName("price").HasColumnType("REAL");
                e.HasOne(x => x.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
