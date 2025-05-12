using BookPlatformMVC.Models;
public class DiscussionThread
{
    public int Id { get; set; }
    public int BookClubId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public virtual BookClub? BookClub { get; set; }
    public virtual ICollection<DiscussionPost> Posts { get; set; } = new List<DiscussionPost>();
}
