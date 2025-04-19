using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryFlow.Models.ViewModels
{
    public class PurchaseOrderViewModel
    {
        [Required]
        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }

        public List<PurchaseOrderItemViewModel> Items { get; set; } = new();
        public List<Supplier>? Suppliers { get; set; }
        public List<Product>? Products { get; set; }
    }

    public class PurchaseOrderItemViewModel
    {
        [Required]
        [Display(Name = "Product")]
        public string ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
    }
}