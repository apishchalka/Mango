using Mango.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon() { CouponId = 100, CouponCode = "1ABC", DiscountAmount = 100, MinAmount = 1000 },
                new Coupon() { CouponId = 200, CouponCode = "ZYTS", DiscountAmount = 200, MinAmount = 2000 },
                new Coupon() { CouponId = 300, CouponCode = "YULKA", DiscountAmount = 300, MinAmount = 3000 }
            );
        }
    }
}
