using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using InventoryFlow.Models;
using System.Linq;

namespace InventoryFlow.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles
            string[] roleNames = { "Admin", "Manager", "Staff" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminUser = new ApplicationUser
            {
                UserName = "admin@inventoryflow.com",
                Email = "admin@inventoryflow.com",
                FullName = "Admin User",
                EmailConfirmed = true
            };

            string adminPassword = "Admin@123";
            var user = await userManager.FindByEmailAsync(adminUser.Email);

            if (user == null)
            {
                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed sample data if empty
            if (!await context.Suppliers.AnyAsync())
            {
                var suppliers = new[]
                {
                    new Supplier
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Global Suppliers",
                        Email = "contact@globalsuppliers.com",
                        Phone = "555-123-4567",
                        Address = "123 Main St"
                    },
                    new Supplier
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Tech Parts Inc.",
                        Email = "sales@techparts.com",
                        Phone = "555-987-6543",
                        Address = "456 Tech Ave"
                    }
                };

                await context.Suppliers.AddRangeAsync(suppliers);
                await context.SaveChangesAsync();

                if (!await context.Products.AnyAsync())
                {
                    var products = new[]
                    {
                        new Product
                        {
                            ItemCode = "PROD001",
                            Name = "Laptop",
                            Description = "15-inch business laptop",
                            Category = "Electronics",
                            Price = 999.99m,
                            Quantity = 15,
                            MinStockLevel = 5,
                            SupplierId = suppliers[0].Id
                        },
                        new Product
                        {
                            ItemCode = "PROD002",
                            Name = "Mouse",
                            Description = "Wireless mouse",
                            Category = "Accessories",
                            Price = 25.50m,
                            Quantity = 42,
                            MinStockLevel = 10,
                            SupplierId = suppliers[1].Id
                        },
                    };

                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}