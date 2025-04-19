using System.Collections.Generic;
using System.Linq;

namespace InventoryFlow.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalSuppliers { get; set; }
        public List<Product> RecentProducts { get; set; } = new();
        public List<Product> CriticalStockProducts { get; set; } = new();

        // New properties
        public decimal InventoryValue => RecentProducts.Sum(p => p.Price * p.Quantity);
        public int OutOfStockItems => RecentProducts.Count(p => p.Quantity == 0);
        public int WarningStockItems => RecentProducts.Count(p => p.Quantity > 0 && p.Quantity < p.MinStockLevel);
    }
}