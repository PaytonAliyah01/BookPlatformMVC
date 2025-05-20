using Microsoft.AspNetCore.Identity;
using BookPlatformMVC.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace BookPlatformMVC.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty; // Review text

        [Range(1, 5)]
        public double Rating { get; set; } // Rating from 1 to 5

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // When the review was created

        // Foreign Key Relationship with Book
        public int BookId { get; set; }
        public virtual Book? Book { get; set; } // Navigation property to Book

        // Foreign Key Relationship with User
        public string? UserId { get; set; }
        public virtual User? User { get; set; } // Navigation property to User
    }
}