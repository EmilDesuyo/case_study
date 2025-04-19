using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryFlow.Models;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryFlow.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                LowStockProducts = await _context.Products
                    .Where(p => p.Quantity < p.MinStockLevel)
                    .CountAsync(),
                TotalSuppliers = await _context.Suppliers.CountAsync(),
                RecentProducts = await _context.Products
                    .Include(p => p.Supplier)
                    .OrderByDescending(p => p.LastUpdated)
                    .Take(5)
                    .ToListAsync(),
                CriticalStockProducts = await _context.Products
                    .Where(p => p.Quantity < 5) // Critical threshold
                    .OrderBy(p => p.Quantity)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}