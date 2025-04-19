using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryFlow.Models;
using InventoryFlow.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace InventoryFlow.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        public async Task<IActionResult> Create()
        {
            var model = new PurchaseOrderViewModel
            {
                Suppliers = await _context.Suppliers.OrderBy(s => s.Name).ToListAsync(),
                Products = await _context.Products.OrderBy(p => p.Name).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var po = new PurchaseOrder
                {
                    SupplierId = model.SupplierId,
                    Status = "Pending",
                    OrderDate = DateTime.Now
                };

                foreach (var item in model.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null) continue;

                    po.Items.Add(new PurchaseOrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                po.TotalAmount = po.Items.Sum(i => i.TotalPrice);

                _context.PurchaseOrders.Add(po);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.Suppliers = await _context.Suppliers.OrderBy(s => s.Name).ToListAsync();
            model.Products = await _context.Products.OrderBy(p => p.Name).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReceiveOrder(string id)
        {
            var order = await _context.PurchaseOrders
                .Include(po => po.Items)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Update inventory
            foreach (var item in order.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                    product.LastUpdated = DateTime.Now;
                }
            }

            order.Status = "Received";
            order.ReceivedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}