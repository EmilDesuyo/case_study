using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryFlow.Models
{
    public class PurchaseOrder
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Received Date")]
        public DateTime? ReceivedDate { get; set; }

        [Required]
        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }

        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Received, Cancelled
    }

    public class PurchaseOrderItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string PurchaseOrderId { get; set; }
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        [Display(Name = "Product")]
        public string ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price")]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}