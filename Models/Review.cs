using Microsoft.AspNetCore.Identity;
using BookPlatformMVC.Areas.Identity.Data;

namespace BookPlatformMVC.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string? UserId { get; set; } // User who wrote the review
        public int BookId { get; set; } // The book being reviewed
        public string Content { get; set; } = string.Empty; // Review content
        public int Rating { get; set; } // Rating from 1 to 5
        public DateTime CreatedAt { get; set; } // When the review was created

        public virtual Book? Book { get; set; } // Navigation property to Book
        public virtual User? User { get; set; } // Navigation property to User
    }
}
