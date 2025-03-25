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
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreProducts> StoresProducts { get; set; }
        public DbSet<CustomerSupplier> CustomersSuppliers { get; set; }
        public DbSet<LookupOperationType> LookupOperationTypes { get; set; }
        public DbSet<OperationStoreProduct> OperationStoreProducts { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StoreProducts>()
                .HasKey(e => new { e.StoreId, e.ProductId });

            builder.Entity<LookupOperationType>()
                .HasMany(l => l.Operations)
                .WithOne(o => o.LookupOperationType);

            builder.Entity<OperationStoreProduct>()
        .HasOne(osp => osp.FromStore)
        .WithMany(s => s.FromOperations)
        .HasForeignKey(osp => osp.FromStoreId);

            builder.Entity<OperationStoreProduct>()
    .HasOne(osp => osp.ToStore)
    .WithMany(s => s.ToOperations)
    .HasForeignKey(osp => osp.ToStoreId);
        }
    }
}
