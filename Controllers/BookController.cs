using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookPlatformMVC.Models;
using BookPlatformMVC.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using BookPlatformMVC.Services; // Ensure CsvImporter is accessible

public class BookController : Controller
{
    private readonly BookPlatformMVCIdentityDbContext _context;
    private readonly IWebHostEnvironment _environment;

    // Allowed file extensions for book cover uploads
    private readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

    // Allowed MIME types for validation
    private readonly string[] permittedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp" };

    public BookController(BookPlatformMVCIdentityDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Book(string searchString)
    {
        var books = from b in _context.Books select b;

        if (!string.IsNullOrEmpty(searchString))
        {
            books = books.Where(b => b.Title.Contains(searchString));
        }

        return View("Book", await books.ToListAsync());
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ImportManual(Book book, IFormFile? CoverImageFile, string? CoverImageUrl)
    {
        if (!ModelState.IsValid)
        {
            return View(book);
        }

        // Check if a book with the same title already exists (case-insensitive)
        bool exists = await _context.Books.AnyAsync(b => b.Title.ToLower() == book.Title.ToLower());
        if (exists)
        {
            ModelState.AddModelError("", "A book with this title already exists in the database.");
            return View(book);
        }

        // Save book first to get its ID before saving image
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Folder to save cover images (same folder for all images)
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        if (CoverImageFile != null && CoverImageFile.Length > 0)
        {
            var ext = Path.GetExtension(CoverImageFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                ModelState.AddModelError("CoverImageFile", "Invalid file type. Please upload an image file (jpg, jpeg, png, gif, bmp).");
                return View(book);
            }

            if (!permittedMimeTypes.Contains(CoverImageFile.ContentType.ToLower()))
            {
                ModelState.AddModelError("CoverImageFile", "Invalid image file content.");
                return View(book);
            }

            var fileName = $"book_{book.Id}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await CoverImageFile.CopyToAsync(fileStream);

            book.ImagePath = "/images/" + fileName;
        }
        else if (!string.IsNullOrWhiteSpace(CoverImageUrl))
        {
            book.ImagePath = CoverImageUrl;
        }
        else
        {
            book.ImagePath = "/images/default-cover.png";
        }

        // Update book record with cover image URL if changed
        _context.Books.Update(book);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Book manually added successfully!";
        return RedirectToAction(nameof(Book));
    }

    [HttpPost]
    public async Task<IActionResult> ImportCsv(IFormFile csvFile)
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid CSV file.";
            return RedirectToAction("Import");
        }

        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await csvFile.CopyToAsync(stream);
        }

        List<Book> booksToAdd;
        try
        {
            booksToAdd = CsvImporter.ImportBooks(filePath);
        }
        catch (InvalidDataException)
        {
            TempData["ErrorMessage"] = "Unsupported CSV format. Please upload a Goodreads or StoryGraph CSV file.";
            return RedirectToAction("Import");
        }

        if (booksToAdd.Any())
        {
            // Filter out books already in database by title (case-insensitive)
            var existingTitles = new HashSet<string>(
                await _context.Books.Select(b => b.Title.ToLower()).ToListAsync()
            );

            var newBooks = booksToAdd
                .Where(b => !string.IsNullOrEmpty(b.Title) && !existingTitles.Contains(b.Title.ToLower()))
                .ToList();

            if (newBooks.Any())
            {
                _context.Books.AddRange(newBooks);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{newBooks.Count} new books successfully imported! (Skipped duplicates)";
            }
            else
            {
                TempData["ErrorMessage"] = "All books in the CSV file already exist in the database.";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "No books were imported. The file may be empty or incorrectly formatted.";
        }

        return RedirectToAction(nameof(Book));
    }

    // GET: Book/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
            return NotFound();

        return View(book);
    }

    // GET: Book/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book model, IFormFile? CoverImageFile)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        // Update scalar properties
        book.Title = model.Title;
        book.Author = model.Author;
        book.Description = model.Description;
        book.PageCount = model.PageCount;
        book.PublishedDate = model.PublishedDate;

        // Folder to save cover images (same folder for all images)
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        if (CoverImageFile != null && CoverImageFile.Length > 0)
        {
            var ext = Path.GetExtension(CoverImageFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                ModelState.AddModelError("CoverImageFile", "Invalid file type. Please upload an image file (jpg, jpeg, png, gif, bmp).");
                return View(book);
            }

            if (!permittedMimeTypes.Contains(CoverImageFile.ContentType.ToLower()))
            {
                ModelState.AddModelError("CoverImageFile", "Invalid image file content.");
                return View(book);
            }

            var fileName = $"book_{book.Id}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await CoverImageFile.CopyToAsync(stream);
            }

            book.ImagePath = "/images/" + fileName;
        }

        try
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(book.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToAction(nameof(Details), new { id = book.Id });
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}
