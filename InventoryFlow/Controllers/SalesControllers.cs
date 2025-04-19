using InventoryFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace InventoryFlow.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            return View(await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            var products = await _context.Products
                .Where(p => p.Quantity > 0)
                .Select(p => new ProductSelectionViewModel
                {
                    ItemCode = p.ItemCode,
                    Name = p.Name,
                    Price = p.Price,
                    AvailableQuantity = p.Quantity
                }).ToListAsync();

            return View(new SaleCreateViewModel { AvailableProducts = products });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var sale = new Sale { SaleDate = DateTime.Now };
                    decimal total = 0;

                    foreach (var item in model.SelectedItems)
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product == null || product.Quantity < item.Quantity)
                        {
                            throw new Exception($"Invalid product or quantity for {item.ProductId}");
                        }

                        sale.SaleItems.Add(new SaleItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = product.Price
                        });

                        total += product.Price * item.Quantity;
                        product.Quantity -= item.Quantity;
                        product.LastUpdated = DateTime.Now;
                    }

                    sale.TotalAmount = total;
                    _context.Sales.Add(sale);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model.AvailableProducts = await _context.Products
                .Where(p => p.Quantity > 0)
                .Select(p => new ProductSelectionViewModel
                {
                    ItemCode = p.ItemCode,
                    Name = p.Name,
                    Price = p.Price,
                    AvailableQuantity = p.Quantity
                }).ToListAsync();

            return View(model);
        }
    }
}