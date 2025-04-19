using System.Collections.Generic;
using System.Linq;

namespace InventoryFlow.Models.ViewModels
{
    public class InventoryReportViewModel
    {
        public List<Product> Products { get; set; } = new();
        public decimal TotalValue { get; set; }
        public int LowStockItems => Products.Count(p => p.Quantity < p.MinStockLevel);
        public int OutOfStockItems => Products.Count(p => p.Quantity == 0);
    }
}