namespace TownTalk.Web.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using TownTalk.Web.Repositories.Interfaces;
using TownTalk.Web.Services.Interfaces;
using TownTalk.Web.Models;
using TownTalk.Web.Hubs;

/// <summary>
/// Provides notification-related services, including sending notifications to users and managing notification data.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext, UserManager<ApplicationUser> userManager)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
        _userManager = userManager;
    }

    /// <inheritdoc/>
    public async Task NotifyUserAsync(string userId, string message, int postId, string senderId, string type)
    {
        ApplicationUser? sender = await _userManager.FindByIdAsync(senderId);

        bool notificationExists = await _notificationRepository.NotificationExistsAsync(userId, postId, sender.Id, type);

        if (notificationExists || senderId == userId)
        {
            return;
        }

        Notification? notification = new Notification()
        {
            UserId = userId,
            Message = message,
            PostId = postId,
            SenderId = sender.Id,
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
            Type = type,
        };

        await _notificationRepository.AddNotificationAsync(notification);

        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
    }

    /// <inheritdoc/>
    public async Task NotifyUserAsync(string userId, string message, string senderId, string type)
    {
        ApplicationUser? sender = await _userManager.FindByIdAsync(senderId);

        Notification? notification = new Notification()
        {
            UserId = userId,
            Message = message,
            SenderId = sender.Id,
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
            Type = type
        };

        await _notificationRepository.AddNotificationAsync(notification);

        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Notification>> GetRecentNotificationsAsync()
    {
        // DateTime thirtyMinutesAgo = DateTime.UtcNow.AddMinutes(-30);
        DateTime thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        return await _notificationRepository.GetRecentNotificationsAsync(thirtyDaysAgo);
    }

    /// <inheritdoc/>
    public async Task NotifyFollowAsync(string followerId, string followedId)
    {
        if (followerId == followedId) return; // Don't notify if the user is following themselves

        ApplicationUser? sender = await _userManager.FindByIdAsync(followerId);
        string? message = $"{sender.DisplayName} is now following you.";

        // Send notification to the followed user
        await NotifyUserAsync(followedId, message, 0, followerId, "Follow");
    }


    /// <inheritdoc/>
    public async Task NotifyCommentAsync(string postId, string commenterId, string originalPosterId)
    {
        if (commenterId == originalPosterId) return;

        ApplicationUser? sender = await _userManager.FindByIdAsync(commenterId);
        string? message = $"{sender.DisplayName} commented on your post.";
        await NotifyUserAsync(originalPosterId, message, int.Parse(postId), commenterId, "Comment");
    }

    /// <inheritdoc/>
    public async Task NotifyReactionAsync(string? postId, string reactorId, string originalPosterId)
    {
        ApplicationUser? sender = await _userManager.FindByIdAsync(reactorId);
        string? message = $"{sender?.DisplayName} reacted to your post.";
        await NotifyUserAsync(originalPosterId, message, int.Parse(postId), senderId: reactorId, type: "Reaction");
    }

    /// <inheritdoc/>
    public async Task NotifyReactionAsync(string reactorId, string originalPosterId)
    {
        ApplicationUser? sender = await _userManager.FindByIdAsync(reactorId);
        string? message = $"{sender?.DisplayName} reacted to your post.";
        await NotifyUserAsync(originalPosterId, message, senderId: reactorId, type: "Reaction");
    }

    /// <inheritdoc/>
    public async Task NotifyProfileViewAsync(string viewerId, string viewedUserId)
    {
        if (viewerId == viewedUserId) return;

        ApplicationUser? sender = await _userManager.FindByIdAsync(viewerId);
        string? message = $"{sender.DisplayName} viewed your profile.";
        await NotifyUserAsync(viewedUserId, message, 0, viewerId, "ProfileView");
    }

    /// <inheritdoc/>
    public async Task NotifyUnfollowAsync(string followerId, string unfollowedId)
    {
        if (followerId == unfollowedId) return;

        ApplicationUser? sender = await _userManager.FindByIdAsync(followerId);
        string? message = $"{sender.DisplayName} unfollowed you.";
        await NotifyUserAsync(unfollowedId, message, 0, followerId, "Unfollow");
    }

    /// <inheritdoc/>
    public async Task PushUpdateToActiveUsersAsync(string message, string type)
    {
        List<ApplicationUser>? activeUsers = _userManager.Users.Where(u => u.LastActive.HasValue && u.LastActive.Value > DateTime.UtcNow.AddMinutes(-5)).ToList();

        foreach (ApplicationUser user in activeUsers)
        {
            await _hubContext.Clients.User(user.Id).SendAsync("ReceiveUpdate", new { Message = message, Type = type });
        }
    }

    /// <inheritdoc/>
    public async Task<int> GetUnreadNotificationsCountAsync(string userId)
    {
        return await _notificationRepository.GetUnreadNotificationsCountAsync(userId);
    }

}
