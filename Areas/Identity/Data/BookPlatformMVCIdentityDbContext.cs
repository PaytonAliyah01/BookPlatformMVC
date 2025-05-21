using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookPlatformMVC.Models;

namespace BookPlatformMVC.Areas.Identity.Data;

public class BookPlatformMVCIdentityDbContext : IdentityDbContext<IdentityUser>
{
    public BookPlatformMVCIdentityDbContext(DbContextOptions<BookPlatformMVCIdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<BookshelfEntry> BookshelfEntries { get; set; }
    public DbSet<BookClub> BookClubs { get; set; }
    public DbSet<BookClubMembership> BookClubMemberships { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<DiscussionThread> DiscussionThreads { get; set; }
    public DbSet<DiscussionPost> DiscussionPosts { get; set; }
    public DbSet<ReadingSession> ReadingSessions { get; set; }
    public DbSet<Statistics> Statistics { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
