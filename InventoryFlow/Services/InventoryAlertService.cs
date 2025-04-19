using System.Threading.Tasks;
using System;
using InventoryFlow.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace InventoryFlow.Services
{
    public class InventoryAlertService : IInventoryAlertService
    {
        private readonly ILogger<InventoryAlertService> _logger;
        private readonly IServiceProvider _services;

        public InventoryAlertService(
            IServiceProvider services,
            ILogger<InventoryAlertService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task CheckStockLevels()
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                try
                {
                    var lowStockProducts = await context.Products
                        .Where(p => p.Quantity < p.MinStockLevel)
                        .ToListAsync();

                    foreach (var product in lowStockProducts)
                    {
                        await SendLowStockNotification(product);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking stock levels");
                }
            }
        }

        public async Task SendLowStockNotification(Product product)
        {
            _logger.LogWarning($"Low stock alert: {product.Name} (Current: {product.Quantity}, Min: {product.MinStockLevel})");
            // Implement actual notification logic here
        }
    }
}