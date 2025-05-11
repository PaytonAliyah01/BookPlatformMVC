using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookPlatformMVC.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

public class BookController : Controller
{
    private readonly BookPlatformMVCIdentityDbContext _context;

    public BookController(BookPlatformMVCIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Book(string searchString)
    {
        var books = from b in _context.Books select b;

        if (!string.IsNullOrEmpty(searchString))
        {
            books = books.Where(b => b.Title.Contains(searchString));
        }

        return View("Book" ,await books.ToListAsync());
    }
}
