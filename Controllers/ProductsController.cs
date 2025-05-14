using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using AgriEnergyConnect.Data;
using System.Security.Claims;

namespace AgriEnergyConnect.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index(string category = null, string sort = null)
        {
            var products = _context.Products
                .Include(p => p.Farmer)
                .Include(p => p.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            var productList = await products.ToListAsync();

            if (sort == "price_asc")
            {
                productList = productList.OrderBy(p => p.Price).ToList();
            }
            else if (sort == "price_desc")
            {
                productList = productList.OrderByDescending(p => p.Price).ToList();
            }
            else if (sort == "rating")
            {
                productList = productList.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0).ToList();
            }

            ViewBag.Categories = await _context.Products.Select(p => p.Category).Distinct().ToListAsync();
            return View(productList);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Farmer)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Farmer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Farmer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create([Bind("Name,Description,Category,Price,ImageUrl")] Product product)
        {
            ModelState.Remove("Farmer");
            ModelState.Remove("FarmerId");

            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["Error"] = "User not authenticated or email missing.";
                return View(product);
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null)
            {
                TempData["Error"] = "No Farmer profile found for this user.";
                return View(product);
            }

            product.FarmerId = farmer.Id;
            product.ProductionDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product listed successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["Error"] = "User not authenticated or email missing.";
                return RedirectToAction(nameof(Index));
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null || product.FarmerId != farmer.Id)
            {
                TempData["Error"] = "You are not authorized to edit this product.";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FarmerId,Name,Description,Category,Price,ImageUrl,ProductionDate")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Farmer");

            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["Error"] = "User not authenticated or email missing.";
                return View(product);
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null || product.FarmerId != farmer.Id)
            {
                TempData["Error"] = "You are not authorized to edit this product.";
                return View(product);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Products.AnyAsync(p => p.Id == product.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["Error"] = "User not authenticated or email missing.";
                return RedirectToAction(nameof(Index));
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null || product.FarmerId != farmer.Id)
            {
                TempData["Error"] = "You are not authorized to delete this product.";
                return RedirectToAction(nameof(Index));
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Products/AddReview/5
        [HttpPost]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> AddReview(int id, ProductReview review)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Farmer)
                    .Include(p => p.Reviews)
                    .ThenInclude(r => r.Farmer)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid review data: " + string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return View("Details", product);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.Email.ToLower() == User.Identity.Name.ToLower());

                if (farmer == null)
                {
                    TempData["ErrorMessage"] = "Farmer profile not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Ensure Id is unset to let SQLite auto-generate
                review.Id = 0;
                review.ProductId = id;
                review.FarmerId = farmer.Id;
                review.CreatedDate = DateTime.Now;

                _context.Add(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review added successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the review. Please try again.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Products/PlaceOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> PlaceOrder(int id)
        {
            var product = await _context.Products
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["Error"] = "User not authenticated or email missing.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null)
            {
                TempData["Error"] = "No Farmer profile found for this user.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (product.FarmerId == farmer.Id)
            {
                TempData["Error"] = "You cannot order your own product.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}