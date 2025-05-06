using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using System.Threading.Tasks;

namespace AgriEnergyConnect.Controllers
{
    [Authorize]
    public class DiscussionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscussionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.DiscussionPosts.Include(p => p.Farmer).ToListAsync();
            return View(posts);
        }

        [Authorize(Roles = "Farmer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create(DiscussionPost post)
        {
            if (ModelState.IsValid)
            {
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == User.Identity.Name);
                if (farmer == null)
                {
                    ModelState.AddModelError("", "Farmer profile not found. Please ensure your account is linked to a farmer profile.");
                    return View(post);
                }
                post.FarmerId = farmer.Id;
                post.CreatedDate = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }
    }
}