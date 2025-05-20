using BookPlatformMVC.Models;


public class BookshelfEntryEditViewModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public required string BookTitle { get; set; }

    public ReadingStatus Status { get; set; }
    public int? ProgressPercent { get; set; }       // Optional, for % tracking
    public int? CurrentPage { get; set; }           // Optional, for page-based tracking
    public int? PageCount { get; set; }             // Read-only or for display
    public DateTime? StartedReadingDate { get; set; }
    public DateTime? FinishedReadingDate { get; set; }
    public OwnershipType Ownership { get; set; }
}