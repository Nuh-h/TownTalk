namespace TownTalk.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Models;
using TownTalk.Web.Services.Interfaces;
using TownTalk.Web.ViewModels;

/// <summary>
/// Controller for administrative actions, including user management, notifications, and user connections.
/// </summary>
[Authorize]
public class AdminController : Controller
{
    private readonly TownTalkDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IUserFollowService _userFollowService;
    private readonly IGraphService _graphService;
    private readonly IUserStatsService _userStatsService;
    private readonly IPostService _postService;
    private readonly ICommentService _commentService;
    private readonly IReactionService _reactionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminController"/> class.
    /// </summary>
    public AdminController(
        TownTalkDbContext context,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService,
        IUserFollowService userFollowService,
        IGraphService graphService,
        IUserStatsService userStatsService,
        IPostService postService,
        ICommentService commentService,
        IReactionService reactionService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
        _userFollowService = userFollowService;
        _graphService = graphService;
        _userStatsService = userStatsService;
        _postService = postService;
        _commentService = commentService;
        _reactionService = reactionService;
    }

    /// <summary>
    /// Renders the admin dashboard shell. All dashboard sections are loaded via AJAX partials.
    /// </summary>
    public IActionResult Index()
    {
        AdminDashboardViewModel model = new AdminDashboardViewModel
        {
            PageTitle = "Admin Dashboard"
        };
        return View(model);
    }

    /// <summary>
    /// Returns the site-wide statistics partial.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SiteStatsPartial()
    {
        List<ApplicationUser>? users = await _context.Users.ToListAsync();
        GeneralStatsViewModel? model = new GeneralStatsViewModel
        {
            TotalUsers = users.Count,
            TotalPosts = await _postService.GetTotalPostsAsync(),
            TotalComments = await _commentService.GetTotalCommentsAsync(),
            TotalReactions = await _reactionService.GetTotalReactionsAsync(),
            NewUsersThisMonth = users.Count(u => u.DateJoined >= DateTime.UtcNow.AddMonths(-1)),
            NewPostsThisMonth = await _postService.GetNewPostsThisMonthAsync(),
            NewCommentsThisMonth = await _commentService.GetNewCommentsThisMonthAsync(),
            Users = users
        };
        return PartialView("_GeneralStats", model);
    }

    /// <summary>
    /// Returns the per-user statistics partial.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> UserStatsPartial(string userId)
    {
        ApplicationUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();

        int posts = await _userStatsService.GetPostCountAsync(user.Id);
        int comments = await _userStatsService.GetCommentCountAsync(user.Id);
        int reactionsReceived = await _reactionService.GetUserReactionsReceivedAsync(user.Id);
        int followers = await _userStatsService.GetFollowersCountAsync(user.Id);
        int following = await _userStatsService.GetFollowingCountAsync(user.Id);
        int unreadNotifications = await _userStatsService.GetUnreadNotificationsCountAsync(user.Id);
        (string? mostPopularPostTitle, int mostPopularPostReactions) = await _userStatsService.GetMostPopularPostAsync(user.Id);

        UserStatsViewModel? model = new UserStatsViewModel
        {
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Posts = posts,
            Comments = comments,
            ReactionsReceived = reactionsReceived,
            Followers = followers,
            Following = following,
            UnreadNotifications = unreadNotifications,
            MostPopularPostTitle = mostPopularPostTitle,
            MostPopularPostReactions = mostPopularPostReactions
        };

        return PartialView("_UserStats", model);
    }

    /// <summary>
    /// Returns the notifications partial.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> NotificationsPartial()
    {
        IEnumerable<Notification>? notifications = await _notificationService.GetRecentNotificationsAsync();
        // You may want to create a NotificationsViewModel if needed
        return PartialView("_SimulateNotifications", new SimulateNotificationsViewModel()
        {
            Notifications = notifications.ToList(),
            Users = await _context.Users.ToListAsync(),
            SenderLabel = "Sender",
            ReceiverLabel = "Receiver",
            SelectSender = "Select Sender",
            SelectReceiver = "Select Receiver",
            NotificationTypeLabel = "Notification Type",
            SelectNotificationType = "Select Notification Type",
            NewFollower = "New Follower",
            NewReaction = "New Reaction",
            NewPost = "New Post",
            SenderColumn = "Sender",
            ReceiverColumn = "Receiver",
            SendNotificationHeader = "Send Notification",
            RecentNotifications = "Recent Notifications",
            SendNotification = "Send Notification",
            MessageColumn = "Message",
            TimeColumn = "Time",
            StatusColumn = "Status",
            ReadStatus = "Status",
            DeliveredStatus = "Delivered",
        });
    }

    /// <summary>
    /// Returns the degree of connections partial.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DegreeOfConnectionsPartial()
    {
        List<ApplicationUser>? users = await _context.Users.ToListAsync();
        DegreeOfConnectionsViewModel? model = new DegreeOfConnectionsViewModel
        {
            Users = users
        };
        return PartialView("_DegreeOfConnections", model);
    }

    [HttpGet]
    public IActionResult Moderation()
    {
        return Ok();
    }

    [HttpGet]
    public IActionResult Analytics()
    {
        return Ok();
    }

    /// <summary>
    /// Simulates sending a notification of a specified type from one user to another.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> SimulateNotification(string senderId, string receiverId, string notificationType)
    {
        ApplicationUser? sender = await _userManager.FindByIdAsync(senderId);
        ApplicationUser? receiver = await _userManager.FindByIdAsync(receiverId);

        switch (notificationType)
        {
            case "reaction":
                await _notificationService.NotifyReactionAsync(senderId, receiverId);
                break;
            case "follow":
                await _notificationService.NotifyFollowAsync(senderId, receiverId);
                break;
            default:
                break;
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Retrieves mutual followers and connection details between two users, including degree of separation and connection path.
    /// </summary>
    [HttpGet("connections")]
    public async Task<IActionResult> GetConnections(string userId1, string userId2)
    {
        ApplicationUser? user1 = await _userManager.FindByIdAsync(userId1);
        ApplicationUser? user2 = await _userManager.FindByIdAsync(userId2);

        if (user1 == null || user2 == null)
        {
            return NotFound("One or both users not found.");
        }

        List<ApplicationUser>? user1Followers = await _userFollowService.GetFollowersAsync(userId1);
        List<ApplicationUser>? user1Following = await _userFollowService.GetFollowingAsync(userId1);

        List<ApplicationUser>? user2Followers = await _userFollowService.GetFollowersAsync(userId2);
        List<ApplicationUser>? user2Following = await _userFollowService.GetFollowingAsync(userId2);

        List<ApplicationUser>? mutualConnections = user1Followers
            .Where(f => user2Followers.Any(f2 => f2.Id == f.Id))
            .ToList();

        var path = await _graphService.FindConnectionPath(userId1, userId2);
        var degreeOfSeparation = path.Count > 0 ? path.Count - 1 : 0;

        var result = new
        {
            User1 = user1.DisplayName,
            User2 = user2.DisplayName,
            AllConnectionsUser1 = user1Followers.Select(f => new
            {
                f.Id,
                f.DisplayName,
            }).ToList(),
            AllConnectionsUser2 = user2Followers.Select(f => new
            {
                f.Id,
                f.DisplayName,
            }).ToList(),
            MutualConnections = mutualConnections.Select(f => new
            {
                f.Id,
                f.DisplayName,
            }).ToList(),
            DegreeOfSeparation = degreeOfSeparation,
            Path = path.Select(user => new
            {
                user.Id,
                user.DisplayName,
            }).ToList(),
        };

        return Ok(result);
    }
}