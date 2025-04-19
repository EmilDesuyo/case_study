using InventoryFlow.Models;
using System.Threading.Tasks;

namespace InventoryFlow.Services
{
    public interface IInventoryAlertService
    {
        Task CheckStockLevels();
        Task SendLowStockNotification(Product product);
    }
}