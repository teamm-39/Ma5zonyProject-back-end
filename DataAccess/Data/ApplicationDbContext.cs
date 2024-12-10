using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System.Reflection.Emit;

namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Export> Exports { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductExports> ProductsExports { get; set; }
        public DbSet<ProductImports> ProductImports { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreProducts> StoresProducts { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<UserMangeProduct> UserMangeProducts { get; set; }
        public DbSet<UserMangeStore> UserMangeStores { get; set; }
        public DbSet<UserMangeUser> UserMangeUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserMangeUser>()
                .HasOne(umu => umu.ApplicationUser) // المدير
                .WithMany(au => au.ManagedEmployees)
                .HasForeignKey(umu => umu.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserMangeUser>()
                .HasOne(umu => umu.Employee) // الموظف
                .WithMany(au => au.Managers)
                .HasForeignKey(umu => umu.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<ProductExports>()
                .HasKey(e => new { e.ProductId, e.ExportId });

            builder.Entity<ProductImports>()
                .HasKey(e => new { e.ProductId, e.ImportId });

            builder.Entity<StoreProducts>()
                .HasKey(e => new { e.StoreId, e.ProductId });

            builder.Entity<UserMangeProduct>()
                .HasKey(e => new { e.ProductId, e.ApplicationUserId });

            builder.Entity<UserMangeStore>()
                .HasKey(e => new { e.StoreId, e.ApplicationUserId });

            builder.Entity<UserMangeUser>()
                .HasKey(e => new { e.ApplicationUserId, e.EmployeeId });
        }
    }
}
