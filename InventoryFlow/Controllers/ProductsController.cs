using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryFlow.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using InventoryFlow.Models.ViewModels;


namespace InventoryFlow.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string categoryFilter, int page = 1)
        {
            var productsQuery = _context.Products.Include(p => p.Supplier).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(searchString) ||
                    p.ItemCode.ToLower().Contains(searchString) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                productsQuery = productsQuery.Where(p => p.Category == categoryFilter);
            }

            var totalItems = await productsQuery.CountAsync();
            var products = await productsQuery
                .OrderBy(p => p.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Categories = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            ViewBag.SearchString = searchString;
            ViewBag.CategoryFilter = categoryFilter;

            return View(products);
        }

    // [Rest of your existing actions...]


        // GET: Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ItemCode == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine($"Item Code: {model.ItemCode}");
                Console.WriteLine($"Name: {model.Name}");
                Console.WriteLine($"Description: {model.Description}");
                // Log other properties

                var product = new Product
                {
                    ItemCode = model.ItemCode,
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,
                    Price = model.Price,
                    Quantity = model.Quantity,
                    MinStockLevel = model.MinStockLevel,
                    SupplierId = model.SupplierId,
                    LastUpdated = DateTime.Now
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.AvailableSuppliers = await _context.Suppliers.ToListAsync();
            return View(model);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Suppliers = await _context.Suppliers.ToListAsync();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ItemCode,Name,Description,Category,Price,Quantity,MinStockLevel,SupplierId")] Product product)
        {
            if (id != product.ItemCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.LastUpdated = DateTime.Now;
                    _context.Products.Update(product);  // Changed from _context.Update
                    await _context.SaveChangesAsync();  // Make sure to await
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ItemCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Suppliers = await _context.Suppliers.ToListAsync();
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ItemCode == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();  // Make sure to await
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(string id)
        {
            return _context.Products.Any(e => e.ItemCode == id);
        }
    }
}