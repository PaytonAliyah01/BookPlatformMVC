using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookPlatformMVC.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using BookPlatformMVC.Models;
using System.Threading.Tasks;
using System.Linq;

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

    public async Task<IActionResult> Bookshelf(string status)
    {
        var userId = _userManager.GetUserId(User);

        var query = _context.BookshelfEntries
            .Include(b => b.Book)
            .Where(b => b.UserId == userId);

        ReadingStatus? filterStatus = null;

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ReadingStatus>(status, out var parsedStatus))
        {
            filterStatus = parsedStatus;
            query = query.Where(b => b.Status == parsedStatus);
        }

        var entries = await query.ToListAsync();

        var grouped = entries
            .GroupBy(e => e.Status.ToString())
            .ToDictionary(g => g.Key, g => g.ToList());

        ViewBag.FilterStatus = filterStatus?.ToString(); // So you can optionally show it in the view

        return View(grouped);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int bookId, ReadingStatus status)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            TempData["ErrorMessage"] = "User authentication failed.";
            return RedirectToAction("Bookshelf");
        }

        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
        {
            TempData["ErrorMessage"] = "Book not found.";
            return RedirectToAction("Bookshelf");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("Bookshelf");
        }

        var entry = new BookshelfEntry
        {
            BookId = bookId,
            Book = book,
            UserId = userId,
            User = user,
            Status = status
        };

        _context.BookshelfEntries.Add(entry);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Book '{book.Title}' successfully added to bookshelf!";
        return RedirectToAction("Bookshelf");
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int entryId)
    {
        var entry = await _context.BookshelfEntries
            .Include(e => e.Book)
            .FirstOrDefaultAsync(e => e.Id == entryId);

        if (entry != null)
        {
            var bookTitle = entry.Book?.Title ?? "Unknown";

            _context.BookshelfEntries.Remove(entry);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Book '{bookTitle}' removed from bookshelf.";
        }
        else
        {
            TempData["ErrorMessage"] = "Bookshelf entry not found.";
        }

        return RedirectToAction("Bookshelf");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var userId = _userManager.GetUserId(User);

        var entry = await _context.BookshelfEntries
            .Include(e => e.Book)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (entry == null)
            return NotFound();

        if (entry.ProgressPercent.HasValue && entry.Book.PageCount > 0)
        {
            entry.CurrentPage = (int)Math.Round(entry.ProgressPercent.Value / 100.0 * entry.Book.PageCount);
        }
        else
        {
            entry.CurrentPage = null;
        }

        return View(entry);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ReadingStatus Status, int? CurrentPage, int? ProgressPercent, DateTime? StartedReadingDate, DateTime? FinishedReadingDate, OwnershipType Ownership)
    {
        var userId = _userManager.GetUserId(User);
        var entryId = Convert.ToInt32(Request.Form["Id"]);
        var progressMode = Request.Form["ProgressMode"];

        var entry = await _context.BookshelfEntries
            .Include(e => e.Book)
            .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId);

        if (entry == null)
            return NotFound();

        var bookTitle = entry.Book?.Title ?? 
                        await _context.Books
                            .Where(b => b.Id == entry.BookId)
                            .Select(b => b.Title)
                            .FirstOrDefaultAsync() ?? 
                        "Unknown";

        entry.Status = Status;
        entry.StartedReadingDate = StartedReadingDate;
        entry.FinishedReadingDate = FinishedReadingDate;
        entry.Ownership = Ownership;

        if (Status == ReadingStatus.Reading && entry.StartedReadingDate == null)
            entry.StartedReadingDate = DateTime.Now;

        if (Status == ReadingStatus.Finished)
        {
            if (entry.StartedReadingDate == null)
                entry.StartedReadingDate = DateTime.Now;

            entry.FinishedReadingDate = DateTime.Now;
        }

        if (progressMode == "Page")
        {
            entry.CurrentPage = CurrentPage;
            if (entry.Book != null && entry.Book.PageCount > 0 && CurrentPage.HasValue)
            {
                entry.ProgressPercent = (int?)Math.Round(CurrentPage.Value / (double)entry.Book.PageCount * 100.0);
            }
            else
            {
                entry.ProgressPercent = null;
            }
        }
        else if (progressMode == "Percent")
        {
            entry.ProgressPercent = ProgressPercent;
            if (entry.Book != null && entry.Book.PageCount > 0 && ProgressPercent.HasValue)
            {
                entry.CurrentPage = (int?)Math.Round(ProgressPercent.Value / 100.0 * entry.Book.PageCount);
            }
            else
            {
                entry.CurrentPage = null;
            }
        }

        try
        {
            _context.Update(entry);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Book '{bookTitle}' updated successfully!";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (userId == null || !BookshelfEntryExists(entry.Id, userId))
                return NotFound();

            throw;
        }

        return RedirectToAction("Bookshelf", new { id = entry.Id });
    }

    private bool BookshelfEntryExists(int id, string userId)
    {
        return _context.BookshelfEntries.Any(e => e.Id == id && e.UserId == userId);
    }
}
