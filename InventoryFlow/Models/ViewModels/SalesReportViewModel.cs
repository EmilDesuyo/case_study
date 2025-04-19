using System.Collections.Generic;
using System;
using System.Linq;

namespace InventoryFlow.Models.ViewModels
{
    public class SalesReportViewModel
    {
        public List<Sale> Sales { get; set; } = new();
        public decimal TotalSales { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalItemsSold => Sales.Sum(s => s.SaleItems.Sum(i => i.Quantity));
        public Dictionary<string, int> TopProducts =>
            Sales.SelectMany(s => s.SaleItems)
                .GroupBy(i => i.Product.Name)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));
    }
}