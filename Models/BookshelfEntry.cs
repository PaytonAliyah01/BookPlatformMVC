public enum ReadingStatus { ToRead, Reading, Read, Owned }

public class BookshelfEntry
{
    public int Id { get; set; }
    public string? UserId { get; set; }

    public int BookId { get; set; }
    public Book? Book { get; set; }

    public ReadingStatus Status { get; set; }
}
