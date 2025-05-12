using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookPlatformMVC.Models;
using BookPlatformMVC.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class BookClubController : Controller
{
    private readonly BookPlatformMVCIdentityDbContext _context;
    private readonly UserManager<User> _userManager;

    public BookClubController(BookPlatformMVCIdentityDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: All Book Clubs
    public async Task<IActionResult> BookClub()
    {
        var userId = _userManager.GetUserId(User);

        // Fetch the book clubs and their memberships
        var clubs = await _context.BookClubs
            .Include(c => c.BookClubMemberships)
            .ThenInclude(m => m.User)  // Include the user details for each member
            .ToListAsync();

        // Add the IsCreator property to each club
        var clubsWithIsCreator = clubs.Select(club => new BookClub
        {
            Id = club.Id,
            Name = club.Name,
            Description = club.Description,
            UserId = club.UserId,
            CreatedAt = club.CreatedAt,
            IsCreator = club.UserId == userId,  // Check if the current user is the creator
            BookClubMemberships = club.BookClubMemberships
        }).ToList();

        return View("BookClub", clubsWithIsCreator); // Pass modified clubs to the view
    }

    // GET: Create a Book Club
    public IActionResult Create()
    {
        var newBookClub = new BookClub();  // Initialize the model
        return View(newBookClub);  // Pass the model to the view
    }

    // POST: Create a Book Club
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookClub bookClub)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return BadRequest("User ID cannot be null.");
            }

            bookClub.UserId = userId;
            bookClub.CreatedAt = DateTime.Now;

            _context.BookClubs.Add(bookClub);
            await _context.SaveChangesAsync();

            // Add the creator as the first member
            var membership = new BookClubMembership
            {
                BookClubId = bookClub.Id,
                UserId = userId,
                JoinedAt = DateTime.Now
            };
            _context.BookClubMemberships.Add(membership);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BookClub));
        }

        // If validation fails, return the view with the model to show errors
        return View(bookClub);
    }

    // GET: Edit a Book Club
    public async Task<IActionResult> Edit(int id)
    {
        var club = await _context.BookClubs.FindAsync(id);
        if (club == null)
        {
            return NotFound();
        }

        // Check if the current user is the creator of the club
        if (club.UserId != _userManager.GetUserId(User))
        {
            return Forbid(); // If not the creator, return forbidden
        }

        return View("Edit", club); // Displays the edit form for a specific club
    }

    // POST: Update a Book Club
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string name, string description)
    {
        var club = await _context.BookClubs.FindAsync(id);
        if (club == null)
        {
            return NotFound();
        }

        // Ensure the user is the creator of the club
        if (club.UserId != _userManager.GetUserId(User))
        {
            return Forbid(); // If not the creator, return forbidden
        }

        club.Name = name;
        club.Description = description;

        _context.Update(club);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(BookClub));
    }

    // POST: Delete a Book Club
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var club = await _context.BookClubs.FindAsync(id);
        if (club != null)
        {
            // Ensure the user is the creator of the club
            if (club.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // If not the creator, return forbidden
            }

            // Remove associated members first
            var memberships = await _context.BookClubMemberships.Where(m => m.BookClubId == id).ToListAsync();
            _context.BookClubMemberships.RemoveRange(memberships);

            _context.BookClubs.Remove(club);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(BookClub));
    }

    // POST: Join a Book Club
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(int clubId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return BadRequest("User ID cannot be null.");
        }

        var membership = new BookClubMembership
        {
            BookClubId = clubId,
            UserId = userId,
            JoinedAt = DateTime.Now
        };

        _context.BookClubMemberships.Add(membership);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(BookClub));
    }

    // POST: Leave a Book Club
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(int clubId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return BadRequest("User ID cannot be null.");
        }

        var membership = await _context.BookClubMemberships
            .FirstOrDefaultAsync(m => m.BookClubId == clubId && m.UserId == userId);

        if (membership != null)
        {
            _context.BookClubMemberships.Remove(membership);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(BookClub));
    }
}
