using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookPlatformMVC.Areas.Identity.Data;
using BookPlatformMVC.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class StatisticsController : Controller
{
    private readonly BookPlatformMVCIdentityDbContext _context;
    private readonly UserManager<User> _userManager;

    public StatisticsController(BookPlatformMVCIdentityDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var year = DateTime.Now.Year;

        var goal = await _context.Statistics.FirstOrDefaultAsync(rg => rg.UserId == userId && rg.Year == year);

        var finishedBooks = await _context.BookshelfEntries
            .Where(be => be.UserId == userId && be.Status == ReadingStatus.Finished)
            .Include(be => be.Book)
            .ToListAsync();

        var readCount = finishedBooks.Count;

        var totalPages = finishedBooks
            .Where(be => be.Book != null)
            .Sum(be => be.Book!.PageCount);

        var avgDaysToFinish = finishedBooks
            .Where(be => be.StartedReadingDate.HasValue && be.FinishedReadingDate.HasValue)
            .Select(be => (be.FinishedReadingDate!.Value - be.StartedReadingDate!.Value).TotalDays)
            .DefaultIfEmpty(0)
            .Average();

        var physicalCount = finishedBooks.Count(be => be.Ownership == OwnershipType.Physical || be.Ownership == OwnershipType.Both);
        var digitalCount = finishedBooks.Count(be => be.Ownership == OwnershipType.Digital || be.Ownership == OwnershipType.Both);

        // Reading streak logic
        var readingDates = await _context.ReadingSessions
            .Where(rs => rs.UserId == userId)
            .Select(rs => rs.Date.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync();

        int readingStreak = 0;
        if (readingDates.Count > 0)
        {
            var currentDate = DateTime.Today;
            foreach (var date in readingDates)
            {
                if ((currentDate - date).Days == readingStreak)
                {
                    readingStreak++;
                }
                else
                {
                    break;
                }
            }
        }

        var mostReadAuthors = finishedBooks
            .GroupBy(be => be.Book!.Author)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new { Author = g.Key, Count = g.Count() })
            .ToList();

        var avgRating = await _context.Reviews
            .Where(r => r.UserId == userId && r.Rating > 0)
            .AverageAsync(r => (double?)r.Rating) ?? 0.0;

        // Pages per month chart
        var pagesPerMonth = finishedBooks
            .Where(be => be.FinishedReadingDate.HasValue)
            .GroupBy(be => new { be.FinishedReadingDate!.Value.Year, be.FinishedReadingDate!.Value.Month })
            .Select(g => new
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM", CultureInfo.InvariantCulture),
                TotalPages = g.Sum(be => be.Book!.PageCount)
            })
            .ToList();

        var pagesPerMonthDict = pagesPerMonth
            .GroupBy(p => p.Month)
            .ToDictionary(g => g.Key, g => g.First().TotalPages);

        // Reading streak heatmap dates
        var readingDayStrings = readingDates
            .Select(d => d.ToString("yyyy-MM-dd"))
            .ToList();

        ViewData["ReadCount"] = readCount;
        ViewData["GoalCount"] = goal?.TargetBooks ?? 0;
        ViewData["TotalPages"] = totalPages;
        ViewData["AvgDaysToFinish"] = avgDaysToFinish;
        ViewData["PhysicalCount"] = physicalCount;
        ViewData["DigitalCount"] = digitalCount;
        ViewData["ReadingStreak"] = readingStreak;
        ViewData["MostReadAuthors"] = mostReadAuthors;
        ViewData["AvgRating"] = avgRating;
        ViewData["PagesPerMonth"] = pagesPerMonthDict;
        ViewData["ReadingDays"] = readingDayStrings;

        return View(goal ?? new Statistics { Year = year, UserId = userId! });
    }

    [HttpPost]
    public async Task<IActionResult> SetGoal(int targetBooks)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var year = DateTime.Now.Year;
        var goal = await _context.Statistics.FirstOrDefaultAsync(rg => rg.UserId == userId && rg.Year == year);

        if (goal == null)
        {
            goal = new Statistics { UserId = userId, TargetBooks = targetBooks, Year = year };
            _context.Statistics.Add(goal);
        }
        else
        {
            goal.TargetBooks = targetBooks;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
