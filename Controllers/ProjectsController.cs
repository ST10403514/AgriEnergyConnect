using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AgriEnergyConnect.Data;

namespace AgriEnergyConnect.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.Farmer)
                .Include(p => p.Collaborators)
                .ThenInclude(c => c.Farmer)
                .ToListAsync();
            ViewBag.FundingOpportunities = await _context.FundingOpportunities.ToListAsync();
            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Farmer)
                .Include(p => p.Collaborators)
                .ThenInclude(c => c.Farmer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.FundingOpportunities = await _context.FundingOpportunities.ToListAsync();
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Farmer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create(Project project)
        {
            // Log incoming project data
            Console.WriteLine($"Create Project: Title={project.Title}, Status={project.Status}, FarmerId={project.FarmerId}");

            // Remove ModelState errors for Farmer and FarmerId (set server-side)
            ModelState.Remove("Farmer");
            ModelState.Remove("FarmerId");

            // Get the current user and their email
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("Error: User not found.");
                ModelState.AddModelError("", "User not found.");
                return View(project);
            }
            Console.WriteLine($"User found: Email={user.Email}");

            // Find the Farmer record by email (case-insensitive)
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null)
            {
                Console.WriteLine($"Error: No Farmer profile found for email={user.Email}");
                ModelState.AddModelError("", "No Farmer profile found for the current user.");
                return View(project);
            }
            Console.WriteLine($"Farmer found: Id={farmer.Id}, Name={farmer.Name}");

            // Set required fields
            project.FarmerId = farmer.Id;
            project.Status = project.Status ?? "Open"; // Use form value or default to "Open"
            project.CreatedDate = DateTime.UtcNow;

            // Log ModelState errors if invalid
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Validation errors: " + string.Join(", ", errors));
                return View(project);
            }

            try
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Project created: Id={project.Id}");
                TempData["Success"] = "Project created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving project: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the project.");
                return View(project);
            }
        }

        // POST: Projects/Join/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer,GreenEnergyExpert")]
        public async Task<IActionResult> Join(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Collaborators)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null || project.Status != "Open")
            {
                TempData["Error"] = "Project not found or not open for collaboration.";
                return RedirectToAction(nameof(Index));
            }

            // Get the current user and their email
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Find the Farmer record by email (case-insensitive)
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.Email.ToLower() == user.Email.ToLower());

            if (farmer == null)
            {
                TempData["Error"] = "No Farmer profile found for the current user.";
                return RedirectToAction(nameof(Index));
            }

            if (project.Collaborators.Any(c => c.FarmerId == farmer.Id))
            {
                TempData["Error"] = "You are already a collaborator on this project.";
                return RedirectToAction(nameof(Index));
            }

            var collaborator = new ProjectCollaborator
            {
                ProjectId = project.Id,
                FarmerId = farmer.Id
            };

            _context.ProjectCollaborators.Add(collaborator);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Successfully joined the project!";
            return RedirectToAction(nameof(Index));
        }
    }
}