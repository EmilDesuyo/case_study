using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryFlow.Models;

namespace InventoryFlow.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Sale to SaleItems relationship
            builder.Entity<Sale>()
                .HasMany(s => s.SaleItems)
                .WithOne(si => si.Sale)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product to SaleItems relationship
            builder.Entity<Product>()
                .HasMany(p => p.SaleItems)
                .WithOne(si => si.Product)
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Supplier to Products relationship
            builder.Entity<Supplier>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            // Purchase Order relationships
            builder.Entity<PurchaseOrder>()
                .HasMany(po => po.Items)
                .WithOne(poi => poi.PurchaseOrder)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.Product)
                .WithMany()
                .HasForeignKey(poi => poi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}