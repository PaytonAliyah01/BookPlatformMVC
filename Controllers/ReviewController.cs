using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookPlatformMVC.Models;
using BookPlatformMVC.Areas.Identity.Data;

public class ReviewController : Controller
{
    private readonly BookPlatformMVCIdentityDbContext _context;
    private readonly UserManager<User> _userManager;

    public ReviewController(BookPlatformMVCIdentityDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Book Reviews
    public async Task<IActionResult> Review(int bookId)
    {
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Book)
            .ToListAsync();

        ViewBag.BookId = bookId; // Pass bookId to view

        return View("Review", reviews);
    }

    // POST: Create a Review
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int bookId, string content, double rating)
    {
        if (bookId <= 0)
        {
            ModelState.AddModelError("", "You must select a valid book to review.");
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return BadRequest("User is not logged in.");
        }

        if (rating < 1 || rating > 5)
        {
            ModelState.AddModelError("Rating", "Rating must be between 1 and 5.");
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            ModelState.AddModelError("Content", "Review content is required.");
        }
        if (!ModelState.IsValid)
        {
            // Save submitted content for repopulation in the view
            ViewData["SubmittedContent"] = content;

            // Re-fetch reviews for the view in case of error
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            ViewBag.BookId = bookId; // Make sure bookId is passed too

            return View("Review", reviews);
        }

        var review = new Review
        {
            BookId = bookId,
            UserId = userId,
            Content = content,
            Rating = rating,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Review), new { bookId });
    }

}