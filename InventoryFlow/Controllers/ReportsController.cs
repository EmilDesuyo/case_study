using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryFlow.Models;
using InventoryFlow.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System; // Add this using directive

namespace InventoryFlow.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Inventory()
        {
            var report = new InventoryReportViewModel
            {
                Products = await _context.Products
                    .Include(p => p.Supplier)
                    .OrderBy(p => p.Quantity)
                    .ToListAsync(),
                TotalValue = await _context.Products
                    .SumAsync(p => p.Price * p.Quantity)
            };
            return View(report);
        }

        public async Task<IActionResult> Sales(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value.Date.AddDays(1).AddTicks(-1));

            var report = new SalesReportViewModel
            {
                Sales = await query.ToListAsync(),
                TotalSales = await query.SumAsync(s => s.TotalAmount),
                StartDate = startDate,
                EndDate = endDate
            };

            return View(report);
        }
    }
}