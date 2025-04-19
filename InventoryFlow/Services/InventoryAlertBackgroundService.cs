using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryFlow.Services
{
    public class InventoryAlertBackgroundService : BackgroundService
    {
        private readonly ILogger<InventoryAlertBackgroundService> _logger;
        private readonly IServiceProvider _services;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

        public InventoryAlertBackgroundService(
            IServiceProvider services,
            ILogger<InventoryAlertBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Inventory Alert Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var alertService = scope.ServiceProvider
                            .GetRequiredService<IInventoryAlertService>();
                        await alertService.CheckStockLevels();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in inventory alert service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}