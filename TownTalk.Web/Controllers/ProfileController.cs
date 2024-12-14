using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    // View a user's profile
    public async Task<IActionResult> Index(string userId)
    {
        ApplicationUser? user = await _userManager.Users
        .Include(u => u.Followers)
        .Include(u => u.Following)
        .Include(u => u.Posts)
        .Include(u => u.Comments)
        .FirstOrDefaultAsync(u => u.Id == userId);
        ;
        List<Post>? posts = await _postRepository.GetAllPostsByUserIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ApplicationUser? currentUser = currentUserId != null ? await _userManager.FindByIdAsync(currentUserId): null;

        bool isFollowing = currentUser != null ? await _userFollowService.IsFollowingAsync(followerId: currentUserId, followedId: userId): false;

        int followersCount = user.Followers.Count;
        int followingCount = user.Following.Count;
        int postsCount = user.Posts.Count;
        int commentsCount = user.Comments.Count;

        List<string>? currentUserFollowersIds = currentUser?.Followers.Select(f => f.FollowerId).ToList();
        List<string>? profileUserFollowersIds = user.Followers.Select(f => f.FollowerId).ToList();

        // Compute mutual followers by finding the intersection of these two lists
        List<string>? mutualFollowers = currentUserFollowersIds?.Intersect(profileUserFollowersIds).ToList();
        int mutualFollowersCount = mutualFollowers == null ? 0 : mutualFollowers.Count;

        // Maps names to two-letter or up to first 3 letters if only one word
        string profileDisplayName = user.DisplayName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(w => w[0])
                         .Take(2)
                         .Aggregate("", (a, b) => a + b)
                         ?? user.DisplayName.Substring(0, Math.Min(3, user.DisplayName.Length));

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
