using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AgriEnergyConnect.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index(int? farmerId, string category, decimal? minPrice, decimal? maxPrice)
        {
            var products = _context.Products.Include(p => p.Farmer).AsQueryable();

            if (farmerId.HasValue)
                products = products.Where(p => p.FarmerId == farmerId.Value);

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category == category);

            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value);

            ViewData["farmerId"] = farmerId;
            ViewData["category"] = category;
            ViewData["minPrice"] = minPrice;
            ViewData["maxPrice"] = maxPrice;
            ViewData["Farmers"] = await _context.Farmers.ToListAsync(); // For farmer dropdown

            return View(await products.ToListAsync());
        }

        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> MyProducts()
        {
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == User.Identity.Name);
            if (farmer == null)
            {
                return NotFound("Farmer profile not found.");
            }
            var products = await _context.Products
                .Include(p => p.Farmer)
                .Where(p => p.FarmerId == farmer.Id)
                .ToListAsync();
            return View("Index", products); // Reuse Index view
        }

        [Authorize(Roles = "Farmer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == User.Identity.Name);
                if (farmer == null)
                {
                    ModelState.AddModelError("", "Farmer profile not found. Please ensure your account is linked to a farmer profile.");
                    return View(product);
                }
                product.FarmerId = farmer.Id;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyProducts));
            }
            return View(product);
        }
    }
}