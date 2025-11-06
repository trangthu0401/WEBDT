using Microsoft.EntityFrameworkCore;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Data
{
    public class DemoWebBanDienThoaiDbContext : DbContext
    {
        public DemoWebBanDienThoaiDbContext(DbContextOptions<DemoWebBanDienThoaiDbContext> options)
            : base(options)
        {
        }

        // Khai báo tất cả các bảng (DbSet)
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<FavoriteDetail> FavoriteDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewDetail> ReviewDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các mối quan hệ 1-1 và ON DELETE CASCADE
            // (Như trong SQL gốc của bạn)

            // 1-1: Account -> Customer (Khi xóa Account -> Xóa Customer)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Account)
                .WithOne(a => a.Customer)
                .HasForeignKey<Customer>(c => c.AccountID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-Nhiều: Customer -> Address (Khi xóa Customer -> Xóa Address)
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-1: Order -> Shipping
            modelBuilder.Entity<Shipping>()
                .HasOne(s => s.Order)
                .WithOne(o => o.Shipping)
                .HasForeignKey<Shipping>(s => s.OrderId);
        }
    }
}