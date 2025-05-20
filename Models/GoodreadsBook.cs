namespace BookPlatformMVC.Models
{
    public class GoodreadsBook
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string ISBN { get; set; }
        public double AverageRating { get; set; }
        public required string Publisher { get; set; }
        public int? NumberOfPages { get; set; }
        public int? YearPublished { get; set; }
        public required string ExclusiveShelf { get; set; }
        public required string MyReview { get; set; }
        public int ReadCount { get; set; }
    }
}