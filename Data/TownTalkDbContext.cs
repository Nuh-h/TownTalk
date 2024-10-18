using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TownTalk.Models;

public class TownTalkDbContext : IdentityDbContext<ApplicationUser>
{
    public TownTalkDbContext(DbContextOptions<TownTalkDbContext> options)
        : base(options)
    {

    }

    public DbSet<Post> Posts { get; set; } // For managing posts
    public DbSet<Comment> Comments { get; set; } // For managing comments
    public DbSet<Reaction> Reactions { get; set; } // For managing likes/dislikes
    public DbSet<Notification> Notifications { get; set; } // For managing notifications
    public DbSet<Category> Categories { get; set; } // For managing post categories
    public DbSet<UserFollow> UserFollows { get; set; } // For managing user follow relationships

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


        // Seed a default admin user (or any existing user)
        ApplicationUser defaultUser = userManager.Users.FirstOrDefault();

        if (defaultUser == null)
        {
            defaultUser = new ApplicationUser
            {
                UserName = "admin@town.talk",
                Email = "admin@town.talk",
                DisplayName = "Admin User",
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(defaultUser, "Admin@1234").Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(defaultUser, "Admin").Wait();
            }
        }

        // Seed Posts (one post per category)
        if (!context.Posts.Any())
        {
            context.Posts.AddRange(
                new Post
                {
                    Title = "Community Updates: New Park Coming Soon",
                    Content = "The city has announced a new park being built in the downtown area. The park will feature playgrounds, walking trails, and picnic areas for families.",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Local News").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "Upcoming Charity Event: Run for a Cause",
                    Content = "Join us for the annual charity run to raise funds for the local food bank. The event will take place this Saturday at 10 AM.",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Events").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "For Sale: Used Bicycle",
                    Content = "Selling a used bicycle in great condition. Perfect for commuting around the city. Contact me if you're interested!",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Classifieds").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "Local Coffee Shop Now Open",
                    Content = "New coffee shop 'Brewed Awakening' has just opened in the heart of the city. Come by for some freshly brewed coffee and pastries!",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Local Businesses").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "City Clean-Up Drive Next Week",
                    Content = "Join the local community for a city-wide clean-up drive to help keep our parks and streets clean. Volunteers needed!",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Public Services").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "Town Hall Meeting on Upcoming Zoning Changes",
                    Content = "The town hall meeting to discuss the upcoming zoning changes will be held next Thursday. Everyone is welcome to join and voice their opinions.",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Politics").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "Local Team Wins State Championship!",
                    Content = "Our local high school football team has won the state championship! Congratulations to the players and coaches for their hard work and dedication!",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Sports").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "Healthy Living Tips for Winter",
                    Content = "As winter approaches, here are some tips on staying healthy and active, even when it's cold outside. Bundle up and keep moving!",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Health & Wellness").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "School District's New After-School Program",
                    Content = "The school district has introduced a new after-school program aimed at providing tutoring and enrichment activities for elementary school students.",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Schools & Education").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "Spring Gardening Tips for Beginners",
                    Content = "Spring is just around the corner! Here are some gardening tips for beginners to get started on growing your own flowers and vegetables.",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Gardening").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "Lost Dog in the Neighborhood",
                    Content = "A small brown and white dog has gone missing in the Westside neighborhood. If you see him, please contact me at the provided number.",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Pets").Id,
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "New Bike Lanes Installed",
                    Content = "The city has installed new bike lanes on Main Street, making it easier and safer for cyclists to get around town. Give it a try!",
                    CreatedAt = DateTime.Now,
                    UserId = defaultUser.Id,
                    CategoryId = context.Categories.First(c => c.Name == "Transportation").Id,
                    Comments = new List<Comment>()
                }
            );
            context.SaveChanges();
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserFollow relationships
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


        //Configure Post
        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)  // Post's creator
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict); // If user is deleted, their posts are deleted

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)  // Post's category
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); // Set category to null if the category is deleted


        // Comment Configuration
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)  // Comment is related to a specific post
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade); // If a post is deleted, all related comments are deleted

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)  // Comment is created by a user
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of a user if they have comments

        // Threading Configuration
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)  // Comment can have a parent comment
            .WithMany(c => c.Replies) // A parent comment can have many replies
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict); // If a parent comment is deleted, its replies are also deleted


        // Configure Reaction Type to be stored as an integer
        modelBuilder.Entity<Reaction>()
            .Property(r => r.Type)
            .HasConversion<int>(); // Store as integer

        // Reaction Configuration
        modelBuilder.Entity<Reaction>()
            .HasOne(r => r.Post)  // Reaction is related to a specific post
            .WithMany(p => p.Reactions)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Cascade); // If post is deleted, reactions are deleted

        // Category
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Posts)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); // Set CategoryId to null if category is deleted


        //Indexing
        modelBuilder.Entity<Post>()
            .HasIndex(p => p.UserId);  // Indexing the UserId in posts for faster queries by author

        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.PostId);  // Indexing the PostId in comments for fast lookup of comments on posts

        modelBuilder.Entity<Reaction>()
            .HasIndex(r => r.PostId);  // Indexing the PostId in reactions for faster queries on reactions to posts

        modelBuilder.Entity<UserFollow>()
            .HasIndex(uf => uf.FollowerId);  // Indexing the FollowerId for quicker follower lookups



    }
}
