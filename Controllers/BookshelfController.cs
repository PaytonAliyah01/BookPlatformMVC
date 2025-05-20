using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookPlatformMVC.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using BookPlatformMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
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

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("Bookshelf");
        }

        var entry = new BookshelfEntry
        {
            BookId = bookId,
            UserId = userId,
            Status = status,
            Book = book,
            User = user
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
            _context.BookshelfEntries.Remove(entry);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Book '{entry.Book.Title}' removed from bookshelf.";
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

        int? currentPage = null;
        if (entry.ProgressPercent.HasValue && entry.Book.PageCount > 0)
        {
            currentPage = (int)Math.Round(entry.ProgressPercent.Value / 100.0 * entry.Book.PageCount);
        }

        var model = new BookshelfEntryEditViewModel
        {
            Id = entry.Id,
            BookId = entry.BookId,
            BookTitle = entry.Book.Title,
            Status = entry.Status,
            ProgressPercent = entry.ProgressPercent,
            CurrentPage = currentPage,
            PageCount = entry.Book.PageCount,
            StartedReadingDate = entry.StartedReadingDate,
            FinishedReadingDate = entry.FinishedReadingDate,
            Ownership = entry.Ownership
        };

        return View(model);
    }



    // POST: Bookshelf/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookshelfEntryEditViewModel model)
    {
        var userId = _userManager.GetUserId(User);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entry = await _context.BookshelfEntries
            .Include(e => e.Book)
            .FirstOrDefaultAsync(e => e.Id == model.Id && e.UserId == userId);

        if (entry == null)
        {
            return NotFound();
        }

        // Update entry properties from model
        entry.Status = model.Status;
        entry.StartedReadingDate = model.StartedReadingDate;
        entry.FinishedReadingDate = model.FinishedReadingDate;
        entry.Ownership = model.Ownership;

        // Calculate ProgressPercent based on CurrentPage and PageCount, if available
        if (model.CurrentPage.HasValue && model.PageCount > 0)
        {
            entry.ProgressPercent = (int?)Math.Round((model.CurrentPage.GetValueOrDefault() / (double)model.PageCount) * 100.0);
        }
        else
        {
            entry.ProgressPercent = model.ProgressPercent;
        }

        try
        {
            _context.Update(entry);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (userId == null || !BookshelfEntryExists(entry.Id, userId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToAction("Details", new { id = entry.Id });
    }

    private bool BookshelfEntryExists(int id, string userId)
    {
        return _context.BookshelfEntries.Any(e => e.Id == id && e.UserId == userId);
    }



}