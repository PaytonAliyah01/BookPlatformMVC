using BookPlatformMVC.Areas.Identity.Data; // or your actual ApplicationUser namespace

namespace BookPlatformMVC.Models
{
    public enum ReadingStatus
    {
        WantToRead,
        Reading,
        Finished
    }

    public enum OwnershipType
    {
        None,
        Physical,
        Digital,
        Both
    }

    public class BookshelfEntry
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int BookId { get; set; }
        public ReadingStatus Status { get; set; }

        public int? ProgressPercent { get; set; } // 0-100 for progress

        public DateTime? StartedReadingDate { get; set; }
        public DateTime? FinishedReadingDate { get; set; }

        public OwnershipType Ownership { get; set; } = OwnershipType.None;

        public required Book Book { get; set; }
        public required User User { get; set; }
    }

}