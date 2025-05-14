using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using Microsoft.Extensions.Logging;

namespace AgriEnergyConnect.Controllers
{
    public class DiscussionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DiscussionsController> _logger;

        public DiscussionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<DiscussionsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Loading discussion posts");
            var posts = await _context.DiscussionPosts
                .Include(p => p.Farmer)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Farmer)
                .ToListAsync();
            _logger.LogInformation("Retrieved {Count} discussion posts", posts.Count);
            return View(posts);
        }

        [Authorize(Roles = "Farmer")]
        public IActionResult Create()
        {
            _logger.LogInformation("Rendering Create discussion post form");
            return View();
        }

        [Authorize(Roles = "Farmer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscussionPost post)
        {
            _logger.LogInformation("Attempting to create discussion post: Title={Title}, Content={Content}", post.Title, post.Content);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("ModelState invalid for discussion post creation: {Errors}", string.Join("; ", errors));
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(post);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found for creating discussion post");
                ModelState.AddModelError("", "User not found. Please log in again.");
                return View(post);
            }

            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
            if (farmer == null)
            {
                _logger.LogWarning("Farmer profile not found for email: {Email}", user.Email);
                ModelState.AddModelError("", "Farmer profile not found. Please ensure your account is linked to a farmer profile.");
                return View(post);
            }

            try
            {
                post.FarmerId = farmer.Id;
                post.CreatedAt = DateTime.UtcNow;
                _context.Add(post);
                int changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Discussion post {Title} created successfully with {Changes} changes", post.Title, changes);
                TempData["Success"] = "Discussion post created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error saving discussion post {Title}: {ErrorMessage}", post.Title, ex.InnerException?.Message ?? ex.Message);
                ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                return View(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error saving discussion post {Title}: {ErrorMessage}", post.Title, ex.Message);
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View(post);
            }
        }

        [Authorize(Roles = "Farmer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int discussionPostId, string content)
        {
            _logger.LogInformation("Attempting to add comment to discussion post {PostId}", discussionPostId);

            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("Comment content is empty for discussion post {PostId}", discussionPostId);
                ModelState.AddModelError("", "Comment content cannot be empty.");
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found for adding comment to discussion post {PostId}", discussionPostId);
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction(nameof(Index));
            }

            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.Email == user.Email);
            if (farmer == null)
            {
                _logger.LogWarning("Farmer profile not found for email: {Email}", user.Email);
                TempData["Error"] = "Farmer profile not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var comment = new Comment
                {
                    DiscussionPostId = discussionPostId,
                    FarmerId = farmer.Id,
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Comments.Add(comment);
                int changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Comment added to discussion post {PostId} with {Changes} changes", discussionPostId, changes);
                TempData["Success"] = "Comment added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error adding comment to discussion post {PostId}: {ErrorMessage}", discussionPostId, ex.InnerException?.Message ?? ex.Message);
                TempData["Error"] = $"Database error: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding comment to discussion post {PostId}: {ErrorMessage}", discussionPostId, ex.Message);
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            _logger.LogInformation("Attempting to delete discussion post {PostId}", id);

            var post = await _context.DiscussionPosts.FindAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Discussion post {PostId} not found", id);
                TempData["Error"] = "Discussion post not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.DiscussionPosts.Remove(post);
                int changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Discussion post {PostId} deleted successfully with {Changes} changes", id, changes);
                TempData["Success"] = "Discussion post deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting discussion post {PostId}: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                TempData["Error"] = $"Database error: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting discussion post {PostId}: {ErrorMessage}", id, ex.Message);
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            _logger.LogInformation("Attempting to delete comment {CommentId}", id);

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found", id);
                TempData["Error"] = "Comment not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Comments.Remove(comment);
                int changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Comment {CommentId} deleted successfully with {Changes} changes", id, changes);
                TempData["Success"] = "Comment deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting comment {CommentId}: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                TempData["Error"] = $"Database error: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting comment {CommentId}: {ErrorMessage}", id, ex.Message);
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}