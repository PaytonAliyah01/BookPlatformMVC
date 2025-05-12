using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookPlatformMVC.Areas.Identity.Data; // Make sure this matches your actual namespace for DbContext
using BookPlatformMVC.Models; // If your User model is here
using Microsoft.EntityFrameworkCore;



public class DiscussionController : Controller
{
    private readonly BookPlatformMVCIdentityDbContext _context;
    private readonly UserManager<User> _userManager;

    public DiscussionController(BookPlatformMVCIdentityDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: List all Threads for a Book Club
    public async Task<IActionResult> Index(int clubId)
    {
        var threads = await _context.DiscussionThreads
            .Where(t => t.BookClubId == clubId)
            .Include(t => t.Posts)
            .ToListAsync();

        return View(threads);
    }

    // GET: Create a Discussion Thread
    public IActionResult Create(int clubId)
    {
        ViewBag.BookClubId = clubId;
        return View();
    }

    // POST: Create a Discussion Thread
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int clubId, string title)
    {
        var thread = new DiscussionThread
        {
            BookClubId = clubId,
            Title = title,
            CreatedAt = DateTime.Now
        };

        _context.DiscussionThreads.Add(thread);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { clubId });
    }

    // POST: Add a Post to a Discussion Thread
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post(int threadId, string content)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return BadRequest("User ID cannot be null.");
        }

        var post = new DiscussionPost
        {
            ThreadId = threadId,
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.Now
        };

        _context.DiscussionPosts.Add(post);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { clubId = threadId });
    }
}
