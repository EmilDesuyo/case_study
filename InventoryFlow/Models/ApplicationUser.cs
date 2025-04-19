using Microsoft.AspNetCore.Identity;
using System;

namespace InventoryFlow.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FullName { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}