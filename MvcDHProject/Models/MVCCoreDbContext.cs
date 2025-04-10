using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCCoreDBF.Models;

namespace MvcDHProject.Models
{
    public class MVCCoreDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public MVCCoreDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Customer>Customers { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important!

            modelBuilder.Entity<Student>().HasKey(s => s.Sid);
            modelBuilder.Entity<Customer>().HasKey(c => c.CustId);
        }

    }
}
