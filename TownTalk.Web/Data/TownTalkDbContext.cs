namespace TownTalk.Web.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Models;

/// <summary>
/// The Entity Framework database context for the TownTalk application, including Identity and application-specific entities.
/// </summary>
public class TownTalkDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TownTalkDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public TownTalkDbContext(DbContextOptions<TownTalkDbContext> options)
        : base(options)
    {

    }

    /// <summary>
    /// Gets or sets the posts in the application.
    /// </summary>
    public DbSet<Post> Posts { get; set; }

    /// <summary>
    /// Gets or sets the comments in the application.
    /// </summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>
    /// Gets or sets the reactions in the application.
    /// </summary>
    public DbSet<Reaction> Reactions { get; set; }

    /// <summary>
    /// Gets or sets the notifications in the application.
    /// </summary>
    public DbSet<Notification> Notifications { get; set; }

    /// <summary>
    /// Gets or sets the categories in the application.
    /// </summary>
    public DbSet<Category> Categories { get; set; }

    /// <summary>
    /// Gets or sets the user follow relationships in the application.
    /// </summary>
    public DbSet<UserFollow> UserFollows { get; set; }

    /// <summary>
    /// Seeds initial data into the database, such as default categories.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="userManager">The user manager for handling users.</param>
    public static void SeedData(TownTalkDbContext context, UserManager<ApplicationUser> userManager)
    {

        // Seed Categories (shared across all localities)
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Local News" },
                new Category { Name = "Events" },
                new Category { Name = "Classifieds" },
                new Category { Name = "Local Businesses" },
                new Category { Name = "Public Services" },
                new Category { Name = "Politics" },
                new Category { Name = "Sports" },
                new Category { Name = "Health & Wellness" },
                new Category { Name = "Schools & Education" },
                new Category { Name = "Gardening" },
                new Category { Name = "Pets" },
                new Category { Name = "Transportation" }
            );
            context.SaveChanges();
        }
    }


    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Configure UserFollow relationships

        modelBuilder.Entity<UserFollow>()
            .HasKey(keyExpression: uf => new { uf.FollowerId, uf.FollowedId });

        modelBuilder.Entity<UserFollow>()
            .HasOne(uf => uf.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserFollow>()
            .HasOne(uf => uf.Followed)
            .WithMany(u => u.Followers)
            .HasForeignKey(uf => uf.FollowedId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Configure Post

        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict); // If user is deleted, their posts are deleted

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); // Set category to null if the category is deleted
        #endregion

        #region Comment Configuration

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade); // If a post is deleted, all related comments are deleted

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of a user if they have comments

        // Threading Configuration
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)  // Comment can have a parent comment
            .WithMany(c => c.Replies) // A parent comment can have many replies
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict); // If a parent comment is deleted, its replies are also deleted

        #endregion

        #region Configure Reaction Type to be stored as an integer

        modelBuilder.Entity<Reaction>()
            .Property(r => r.Type)
            .HasConversion<int>(); // Store as integer

        modelBuilder.Entity<Reaction>()
            .HasOne(r => r.Post)
            .WithMany(p => p.Reactions)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Cascade); // If post is deleted, reactions are deleted

        #endregion

        #region Category

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Posts)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); // Set CategoryId to null if category is deleted

        #endregion

        #region Notifications

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User) // Recipient of the notification
            .WithMany() // Assuming User can have many notifications
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if notifications exist

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Sender) // Sender of the notification
            .WithMany() // Assuming Sender can have many notifications
            .HasForeignKey(n => n.SenderId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if notifications exist

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.TaggedUser) // Tagged user
            .WithMany() // Assuming TaggedUser can have many notifications
            .HasForeignKey(n => n.TaggedUserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if notifications exist

        // Configure Post relationship
        modelBuilder.Entity<Notification>()
            .HasOne<Post>() // If you want to configure the relationship explicitly
            .WithMany() // Assuming a Post can have many notifications
            .HasForeignKey(n => n.PostId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if notifications exist

        // Configure Comment relationship
        modelBuilder.Entity<Notification>()
            .HasOne<Comment>() // If you want to configure the relationship explicitly
            .WithMany() // Assuming a Comment can have many notifications
            .HasForeignKey(n => n.CommentId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if notifications exist
        #endregion

        #region Indexing

        modelBuilder.Entity<Post>()
            .HasIndex(p => p.UserId);  // Indexing the UserId in posts for faster queries by author

        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.PostId);  // Indexing the PostId in comments for fast lookup of comments on posts

        modelBuilder.Entity<Reaction>()
            .HasIndex(r => r.PostId);  // Indexing the PostId in reactions for faster queries on reactions to posts

        modelBuilder.Entity<UserFollow>()
            .HasIndex(uf => uf.FollowerId);  // Indexing the FollowerId for quicker follower lookups

        #endregion

    }
}
