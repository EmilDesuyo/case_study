using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace InventoryFlow.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public List<SaleItem> SaleItems { get; set; } = new();
    }

    public class SaleItem
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}