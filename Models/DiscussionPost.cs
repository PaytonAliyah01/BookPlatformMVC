using BookPlatformMVC.Areas.Identity.Data;
public class DiscussionPost
{
    public int Id { get; set; }
    public int ThreadId { get; set; }
    public string? UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public virtual DiscussionThread? Thread { get; set; }
    public virtual User? User { get; set; }
}
