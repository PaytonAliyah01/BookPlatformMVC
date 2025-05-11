using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookPlatformMVC.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class BookshelfController : Controller
{
    private readonly BookPlatformMVCIdentityDbContext _context;
    private readonly UserManager<User> _userManager;

    public BookshelfController(BookPlatformMVCIdentityDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Bookshelf()
    {
        var userId = _userManager.GetUserId(User);
        var entries = await _context.BookshelfEntries
            .Include(b => b.Book)
            .Where(b => b.UserId == userId)
            .ToListAsync();

        var grouped = entries
            .GroupBy(e => e.Status.ToString())
            .ToDictionary(g => g.Key, g => g.ToList());

        return View(grouped);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int bookId, ReadingStatus status)
    {
        var userId = _userManager.GetUserId(User);
        var entry = new BookshelfEntry
        {
            BookId = bookId,
            UserId = userId ?? throw new InvalidOperationException("User ID cannot be null."),
            Status = status
        };

        _context.BookshelfEntries.Add(entry);
        await _context.SaveChangesAsync();

        return RedirectToAction("Bookshelf");
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int entryId)
    {
        var entry = await _context.BookshelfEntries.FindAsync(entryId);

        if (entry != null)
        {
            _context.BookshelfEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Bookshelf");
    }
}
