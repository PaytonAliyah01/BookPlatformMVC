namespace BookPlatformMVC.Models
{
    public class StoryGraphBook
    {
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string ISBN { get; set; }
        public required string Format { get; set; }
        public required string ReadStatus { get; set; }
        public required string DateAdded { get; set; }
        public required string LastDateRead { get; set; }
        public int? ReadCount { get; set; }
        public double? StarRating { get; set; }
        public required string Review { get; set; }
    }
}