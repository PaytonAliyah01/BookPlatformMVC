using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookPlatformMVC.Areas.Identity.Data;

namespace BookPlatformMVC.Models
{
    public class BookClubMembership
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookClubId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation property: the user who is a member
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Navigation property: the club this membership belongs to
        public virtual BookClub? BookClub { get; set; }

        // Navigation: any discussion threads created by this user in the club
        public virtual ICollection<DiscussionThread> Threads { get; set; } = new List<DiscussionThread>();
    }
}
