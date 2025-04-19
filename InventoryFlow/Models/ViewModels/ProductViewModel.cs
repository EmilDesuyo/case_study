using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryFlow.Models.ViewModels
{
    public class ProductViewModel
    {
        [Required]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Display(Name = "Minimum Stock Level")]
        [Range(0, int.MaxValue)]
        public int MinStockLevel { get; set; } = 10;

        [Display(Name = "Supplier")]
        public string? SupplierId { get; set; }

        public List<Supplier>? AvailableSuppliers { get; set; }
        public List<Supplier> Suppliers { get; internal set; }
    }
}