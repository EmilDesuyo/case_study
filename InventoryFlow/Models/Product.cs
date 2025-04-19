using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryFlow.Models
{
    public class Product
    {
        [Key]
        public required string ItemCode { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public required string Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Display(Name = "Minimum Stock Level")]
        [Range(0, int.MaxValue)]
        public int MinStockLevel { get; set; } = 10;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Foreign Key
        public string? SupplierId { get; set; }

        // Navigation Properties
        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

        // Helper property (not mapped to database)
        [NotMapped]
        public string StockStatus => Quantity switch
        {
            0 => "Out of Stock",
            int n when n < MinStockLevel => "Low Stock",
            _ => "In Stock"
        };
    }
}