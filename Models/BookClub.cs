using Microsoft.AspNetCore.Identity;
using BookPlatformMVC.Areas.Identity.Data;
using BookPlatformMVC.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookPlatformMVC.Models
{
public class BookClub
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? UserId { get; set; }  // Creator
    public DateTime CreatedAt { get; set; }

    [NotMapped]
    public bool IsCreator { get; set; }

    // Add this line to support member listing
    public virtual ICollection<BookClubMembership> BookClubMemberships { get; set; } = new List<BookClubMembership>();
}

}
