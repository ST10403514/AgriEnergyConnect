using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using System.Threading.Tasks;

namespace AgriEnergyConnect.Controllers
{
    [Authorize(Roles = "Farmer")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var proposals = await _context.ProjectProposals.Include(p => p.Farmer).ToListAsync();
            return View(proposals);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectProposal proposal)
        {
            if (ModelState.IsValid)
            {
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == User.Identity.Name);
                if (farmer == null)
                {
                    ModelState.AddModelError("", "Farmer profile not found. Please ensure your account is linked to a farmer profile.");
                    return View(proposal);
                }
                proposal.FarmerId = farmer.Id;
                proposal.CreatedDate = DateTime.Now;
                _context.Add(proposal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(proposal);
        }
    }
}