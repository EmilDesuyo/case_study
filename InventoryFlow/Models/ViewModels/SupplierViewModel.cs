using System.ComponentModel.DataAnnotations;

namespace InventoryFlow.Models.ViewModels
{
    public class SupplierViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public string? Address { get; set; }
    }
}