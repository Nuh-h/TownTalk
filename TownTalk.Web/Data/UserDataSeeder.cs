namespace TownTalk.Web.Data;

using Bogus;
using Microsoft.AspNetCore.Identity;
using TownTalk.Web.Models;
using TownTalk.Web.Repositories.Interfaces;

public class UserDataSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TownTalkDbContext _context;
    private readonly IPostRepository _postRepository;
    private readonly Random _random;

    public UserDataSeeder(
        UserManager<ApplicationUser> userManager,
        TownTalkDbContext context,
        IPostRepository postRepository)
    {
        _userManager = userManager;
        _context = context;
        _postRepository = postRepository;
        _random = new Random();
    }

    public async Task SeedDataAsync()
    {

        // Generate 50 fake users
        Faker<ApplicationUser> userFaker = new Faker<ApplicationUser>()
        .RuleFor(u => u.UserName, f => f.Person.FullName)
        .RuleFor(u => u.Email, (f, u) => $"{string.Join(".", u.UserName.Split(' '))}@town.talk")
        .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
        .RuleFor(u => u.EmailConfirmed, f => true)
        .RuleFor(u => u.DisplayName, (f, u) => u.UserName)
        .RuleFor(u => u.Bio, f => f.Lorem.Sentence())
        .RuleFor(u => u.Location, f => f.Address.City())
        .RuleFor(u => u.Bio, f => f.Lorem.Paragraph())
        .RuleFor(u => u.DateJoined, f => f.Date.Past(5, DateTime.UtcNow)) // Random date from 5 years ago (2020-2025)
        .RuleFor(u => u.LastActive, f => f.Date.Between(new DateTime(2020, 1, 1), DateTime.UtcNow))
        .RuleFor(u => u.DateJoined, f => f.Date.Between(new DateTime(2020, 1, 1), new DateTime(2025, 12, 31)));

        List<ApplicationUser>? users = userFaker.Generate(50);
        List<ApplicationUser>? createdUsers = new List<ApplicationUser>();

        ApplicationUser defaultUser = _userManager.Users.FirstOrDefault();

        if (defaultUser == null)
        {
            defaultUser = new ApplicationUser
            {
                UserName = "admin@town.talk",
                Email = "admin@town.talk",
                DisplayName = "Admin User",
                EmailConfirmed = true,
                Bio = "King of the castle"
            };

            IdentityResult? result = _userManager.CreateAsync(user: defaultUser, "Admin@1234").Result;

            if (result.Succeeded)
            {
                _userManager.AddToRoleAsync(defaultUser, "Admin").Wait();
                createdUsers.Add(defaultUser);

            }
        }
        else
        {
            createdUsers.Add(defaultUser);
        }

        // Create users in the database
        foreach (ApplicationUser user in users)
        {
            string? password = "Password123!";
            IdentityResult? result = _userManager.CreateAsync(user, password).Result;
            if (result.Succeeded)
            {
                createdUsers.Add(user);
            }
        }

        // Generate random follows
        List<UserFollow>? follows = new List<UserFollow>();
        foreach (ApplicationUser user in createdUsers)
        {
            List<ApplicationUser>? availableUsersToFollow = createdUsers
                .Where(u => u.Id != user.Id)
                .OrderBy(x => _random.Next())
                .Take(_random.Next(1, 45)) // Follow 1-20 random users
                .ToList();

            foreach (ApplicationUser userToFollow in availableUsersToFollow)
            {
                DateTime followDate = GetRandomDateBetween(
                    new DateTime(2020, 1, 1),
                    new DateTime(2025, 12, 31));

                UserFollow? userFollow = new UserFollow
                {
                    FollowerId = user.Id,
                    FollowedId = userToFollow.Id,
                    FollowedAt = followDate
                };
                follows.Add(userFollow);
            }
        }
        await _context.UserFollows.AddRangeAsync(follows);
        await _context.SaveChangesAsync();

        // Seed Categories (shared across all localities)
        if (!_context.Categories.Any())
        {
            _context.Categories.AddRange(
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
            _context.SaveChanges();
        }

        // Generate and save posts using PostRepository
        List<Post>? posts = new List<Post>();

        // Retrieve all categories from the database to use them when creating posts
        List<Category> categories = _context.Categories.ToList();

        Faker<Post>? postFaker = new Faker<Post>()
            .RuleFor(p => p.Title, f => f.Lorem.Sentence()) // Random title for post
            .RuleFor(p => p.Content, f => f.Lorem.Paragraph()) // Random content
            .RuleFor(p => p.CreatedAt, f => GetRandomDateBetween(
                new DateTime(2020, 1, 1),
                new DateTime(2025, 12, 31))) // Random created date between 2020 and 2025
            .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id) // Pick a random category from seeded categories
            .RuleFor(p => p.Category, (f, p) => categories.FirstOrDefault(c => c.Id == p.CategoryId)) // Set the category based on the CategoryId
            .RuleFor(p => p.UserId, f => f.Random.Guid().ToString()) // Generate random UserId (or use actual user ID later)
            .RuleFor(p => p.User, (f, p) => createdUsers.FirstOrDefault(u => u.Id == p.UserId)); // Link to existing users

        // Loop through created users and generate posts for each user
        foreach (ApplicationUser user in createdUsers)
        {
            List<Post>? userPosts = postFaker
                .RuleFor(p => p.UserId, f => user.Id) // Assign actual user ID to the post
                .Generate(_random.Next(5, 21)); // Generate 5 to 20 posts per user

            foreach (Post post in userPosts)
            {
                await _postRepository.AddPostAsync(post);
                posts.Add(post);
            }
        }


        // Get all posts for reactions and comments
        List<Post>? allPosts = await _postRepository.GetAllPostsAsync();

        // Generate reactions and comments
        List<Reaction>? reactions = new List<Reaction>();
        List<Comment>? comments = new List<Comment>();
        Faker<Comment>? commentFaker = new Faker<Comment>()
            .RuleFor(c => c.Content, f => f.Lorem.Sentence());

        foreach (var post in allPosts)
        {
            // Random reactions
            IEnumerable<ApplicationUser>? reactingUsers = createdUsers
                .OrderBy(x => _random.Next())
                .Take(_random.Next(0, 31)); // 0-30 reactions per post

            foreach (ApplicationUser reactingUser in reactingUsers)
            {
                DateTime reactionDate = GetRandomDateBetween(post.CreatedAt, new DateTime(2025, 12, 31));

                Reaction? reaction = new Reaction
                {
                    PostId = post.Id,
                    UserId = reactingUser.Id,
                    Type = (ReactionType)_random.Next(0, 9), // Using your enum values 0-8
                    CreatedAt = reactionDate
                };
                reactions.Add(reaction);
            }

            // Random comments
            IEnumerable<ApplicationUser>? commentingUsers = createdUsers
                .OrderBy(x => _random.Next())
                .Take(_random.Next(0, 11)); // 0-10 comments per post

            foreach (ApplicationUser commentingUser in commentingUsers)
            {
                DateTime commentDate = GetRandomDateBetween(post.CreatedAt, new DateTime(2025, 12, 31));

                var comment = commentFaker
                    .RuleFor(c => c.PostId, f => post.Id)
                    .RuleFor(c => c.UserId, f => commentingUser.Id)
                    .RuleFor(c => c.CreatedAt, f => commentDate)
                    .RuleFor(c => c.Content, f => f.Lorem.Paragraph())
                    .Generate();

                comments.Add(comment);
            }
        }

        await _context.Reactions.AddRangeAsync(reactions);
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();
    }

    private DateTime GetRandomDateBetween(DateTime start, DateTime end)
    {
        TimeSpan timeSpan = end - start;
        TimeSpan newSpan = new TimeSpan(0, _random.Next(0, (int)timeSpan.TotalMinutes), 0);
        return start + newSpan;
    }
}