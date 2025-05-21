using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookPlatformMVC.Areas.Identity.Data;
using BookPlatformMVC.Models;
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

        ViewBag.BookClubId = clubId;
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
        var bookClubExists = await _context.BookClubs.AnyAsync(b => b.Id == clubId);
        if (!bookClubExists)
        {
            return NotFound($"BookClub with ID {clubId} does not exist.");
        }

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

        var thread = await _context.DiscussionThreads.FindAsync(threadId);
        if (thread == null)
        {
            return NotFound($"DiscussionThread with ID {threadId} does not exist.");
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

        return RedirectToAction(nameof(Index), new { clubId = thread.BookClubId });
    }

    // GET: View posts for a specific thread
    public async Task<IActionResult> ThreadDetails(int threadId)
    {
        var thread = await _context.DiscussionThreads
            .Include(t => t.Posts)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(t => t.Id == threadId);

        if (thread == null)
            return NotFound();

        return View(thread);
    }


}
