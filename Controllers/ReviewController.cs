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
            .Where(r => r.BookId == bookId)
            .ToListAsync();
        
        // Specify the Review folder explicitly
        return View("Review", reviews); // Assuming 'Review.cs' is inside 'Views/Review' folder
    }

    // POST: Create a Review
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int bookId, string content, int rating)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return BadRequest("User is not logged in.");
        }

        var review = new Review
        {
            BookId = bookId,
            UserId = userId,
            Content = content,
            Rating = rating,
            CreatedAt = DateTime.Now
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Review), new { bookId });
    }
}