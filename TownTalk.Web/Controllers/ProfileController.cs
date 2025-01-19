using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownTalk.Helpers;
using TownTalk.Models;
using TownTalk.Repositories.Interfaces;
using TownTalk.Services.Interfaces;
using TownTalk.ViewModels;

public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserFollowService _userFollowService;
    private readonly IPostRepository _postRepository;
    private readonly INotificationService _notificationService;

    public ProfileController(UserManager<ApplicationUser> userManager, IUserFollowService userFollowService, IPostRepository postRepository, INotificationService notificationService)
    {
        _userManager = userManager;
        _userFollowService = userFollowService;
        _postRepository = postRepository;
        _notificationService = notificationService;
    }

    async public Task<bool> IsFollowed(string followedId, string followerId)
    {

        bool isFollowing = await _userFollowService.IsFollowingAsync(followerId, followedId);

        return isFollowing;
    }

    // View a user's profile
    public async Task<IActionResult> Index(string userId)
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();
        PerformanceLogger pagePerformanceLogger = new PerformanceLogger();

        performanceLogger.Start();
        pagePerformanceLogger.Start();

        var user = await _userManager.Users
        .Where(u => u.Id == userId)
        .Select(u => new
        {
            u.Id,
            u.DisplayName,
            u.DateJoined,
            u.Bio,
            u.Location,
            u.LastActive,
            u.Posts,
            FollowersCount = u.Followers.Count(),
            FollowingCount = u.Following.Count(),
            PostsCount = u.Posts.Count(),
            CommentsCount = u.Comments.Count(),

        })
        .FirstOrDefaultAsync();

        performanceLogger.Stop("Fetching Profile user data");

        List<Post>? posts = await _postRepository.GetAllPostsByUserIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        performanceLogger.Start();

        string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ApplicationUser? currentUser = currentUserId != null ? await _userManager.FindByIdAsync(currentUserId) : null;

        int followersCount = user.FollowersCount;
        int followingCount = user.FollowingCount;
        int postsCount = user.Posts.Count;
        int commentsCount = user.CommentsCount;
        bool isFollowing = await IsFollowed(userId, currentUserId);

        List<string>? currentUserFollowersIds = currentUser?.Followers.Select(f => f.FollowerId).ToList();

        int mutualFollowersCount = 0;

        if (currentUser?.Followers != null)
        {
            // Create an array of tasks to run IsFollowed for each follower
            Task<bool>[]? tasks = currentUser.Followers
                .Select(f => IsFollowed(f.FollowerId, user.Id))
                .ToArray();

            // Await all tasks to complete concurrently
            bool[]? results = await Task.WhenAll(tasks);

            mutualFollowersCount = results.Count(r => r);
        }

        // Maps names to two-letter or up to first 3 letters if only one word
        string profileDisplayName = user.DisplayName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(w => w[0])
                         .Take(2)
                         .Aggregate("", (a, b) => a + b)
                         ?? user.DisplayName.Substring(0, Math.Min(3, user.DisplayName.Length));

        performanceLogger.Stop("Crunching numbers for profile stats");

        ProfileViewModel? viewModel = new ProfileViewModel
        {
            UserId = user.Id,
            DisplayName = user.DisplayName,
            DateJoined = user.DateJoined,
            ProfilePictureUrl = $"https://placehold.co/800?text={profileDisplayName}&font=Lora",
            IsFollowing = isFollowing,
            Posts = posts.Select(p => new PostViewModel(p)).ToList(),
            Bio = user.Bio,
            Location = user.Location,
            LastActive = user.LastActive,
            FollowersCount = followersCount,
            FollowingCount = followingCount,
            MutualFollowersCount = mutualFollowersCount,
            PostsCount = postsCount,
            CommentsCount = commentsCount
        };

        pagePerformanceLogger.Stop("Profile page's speed is: ");

        return View(viewModel);
    }



    // Edit user profile
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ApplicationUser model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            user.DisplayName = model.DisplayName;

            // Update other properties as needed
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", new { userId = user.Id });
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFollow(string userId)
    {
        string? followerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ApplicationUser? follower = await _userManager.Users
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == followerId);

        if (follower == null)
        {
            return NotFound("Current user not found");
        }

        ApplicationUser? userToFollowOrUnfollow = await _userManager.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (userToFollowOrUnfollow == null)
        {
            return NotFound("User to follow/unfollow not found");
        }

        bool isFollowing = await _userFollowService.IsFollowingAsync(followerId, userId);

        if (isFollowing)
        {
            await _userFollowService.UnfollowUserAsync(followerId, userId);
        }
        else
        {
            await _userFollowService.FollowUserAsync(followerId, userId);
        }

        // Get the updated state
        isFollowing = await _userFollowService.IsFollowingAsync(followerId, userId);
        int followersCount = userToFollowOrUnfollow.Followers.Count;
        int followingCount = userToFollowOrUnfollow.Following.Count;

        // Find mutual followers
        List<string> currentUserFollowersIds = follower.Followers.Select(f => f.FollowerId).ToList();
        List<string> profileUserFollowersIds = userToFollowOrUnfollow.Followers.Select(f => f.FollowerId).ToList();
        List<string> mutualFollowers = currentUserFollowersIds.Intersect(profileUserFollowersIds).ToList();
        int mutualFollowersCount = mutualFollowers.Count;

        var response = new
        {
            IsFollowing = isFollowing,
            FollowersCount = followersCount,
            FollowingCount = followingCount,
            MutualFollowersCount = mutualFollowersCount
        };

        return Json(response);
    }

}
