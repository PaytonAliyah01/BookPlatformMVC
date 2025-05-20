using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookPlatformMVC.Models
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }

        public int PageCount { get; set; } // Total number of pages

        public DateTime? PublishedDate { get; set; }

        // Compute average rating based on reviews
        [NotMapped] // Don't store in DB, compute dynamically
        public double AverageRating => Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0;

        // Relationship with Reviews
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}