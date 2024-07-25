using Mango.Services.ShoppnigCartAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppnigCartAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }        
    }
}
