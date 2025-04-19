using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryFlow.Models
{
    public class Supplier
    {
        [Key]
        public required string Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [Phone]
        public required string Phone { get; set; }

        public string? Address { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}