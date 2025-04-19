namespace TownTalk.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TownTalk.Web.Data;
using TownTalk.Web.Models;
using TownTalk.Web.Services.Interfaces;

[Authorize]
public class AdminController : Controller
{

    private readonly TownTalkDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IUserFollowService _userFollowService;
    private readonly IGraphService _graphService;


    public AdminController(TownTalkDbContext context, UserManager<ApplicationUser> userManager, INotificationService notificationService, IUserFollowService userFollowService,
    IGraphService graphService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
        _userFollowService = userFollowService;
        _graphService = graphService;
    }

    public async Task<IActionResult> Index()
    {
        List<ApplicationUser> users = _context.Users.ToList();
        IEnumerable<Notification> notifications = await _notificationService.GetRecentNotificationsAsync();
        ViewData["notifications"] = notifications;
        return View(users);
    }

    // [HttpGet]
    // public async Task<IActionResult> Moderation(){

    // }

    // [HttpGet]
    // public async Task<IActionResult> Analytics(){

    // }

    [HttpPost]
    public async Task<ActionResult> SimulateNotification(string senderId, string receiverId, string notificationType) {

        ApplicationUser? sender = await _userManager.FindByIdAsync(senderId);
        ApplicationUser? receiver = await _userManager.FindByIdAsync(receiverId);

        switch(notificationType){
            case "reaction":
                await _notificationService.NotifyReactionAsync(senderId, receiverId);
                break;
            case "follow":
                await _notificationService.NotifyFollowAsync(senderId, receiverId);
                break;
            // case "new-post":
            //     _notificationService.NotifyUserAsync();
            //     break;
            default:
                break;
        }

        return RedirectToAction("Index");

    }

    // Endpoint to get mutual followers and connection details
    [HttpGet("connections")]
    public async Task<IActionResult> GetConnections(string userId1, string userId2)
    {
        ApplicationUser? user1 = await _userManager.FindByIdAsync(userId1);
        ApplicationUser? user2 = await _userManager.FindByIdAsync(userId2);

        if (user1 == null || user2 == null)
        {
            return NotFound("One or both users not found.");
        }

        // Get followers and followings for both users
        List<ApplicationUser>? user1Followers = await _userFollowService.GetFollowersAsync(userId1);
        List<ApplicationUser>? user1Following = await _userFollowService.GetFollowingAsync(userId1);

        List<ApplicationUser>? user2Followers = await _userFollowService.GetFollowersAsync(userId2);
        List<ApplicationUser>? user2Following = await _userFollowService.GetFollowingAsync(userId2);

        // Get mutual connections by finding common users in both lists
        List<ApplicationUser>? mutualConnections = user1Followers
        .Where(f => user2Followers.Any(f2 => f2.Id == f.Id)) // Find users who follow both
        .ToList();
        // mutualConnections.AddRange(user1Following.Intersect(user2Following).ToList());

        // Get the degree of separation (path) between the two users
        var path = await _graphService.FindConnectionPath(userId1, userId2);
        var degreeOfSeparation = path.Count > 0 ? path.Count - 1 : 0; // Degree is length of path - 1

        // Prepare the result with mutual connections and path
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
                user.DisplayName
            }).ToList()
        };

        return Ok(result);
    }

}