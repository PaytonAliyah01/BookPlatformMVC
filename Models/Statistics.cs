using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookPlatformMVC.Areas.Identity.Data;

namespace BookPlatformMVC.Models
{
    public class Statistics
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        // Reading goal - target number of books for the year
        [Range(1, 500)]
        public int TargetBooks { get; set; }

        // Year for this stats record
        public int Year { get; set; } = DateTime.Now.Year;

        // Current reading streak (consecutive days reading)
        public int ReadingStreakDays { get; set; }

        // Total finished books in the year
        public int TotalBooksRead { get; set; }

        // Total pages read in finished books
        public int TotalPagesRead { get; set; }

        // Average days taken to finish a book
        public double AverageDaysToFinish { get; set; }

        // Counts of book formats read
        public int PhysicalBooksRead { get; set; }
        public int DigitalBooksRead { get; set; }

        // Average rating given by user (out of 5)
        public double AverageUserRating { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
