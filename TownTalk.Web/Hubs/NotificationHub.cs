namespace TownTalk.Web.Hubs;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using TownTalk.Web.Models;
using TownTalk.Web.Repositories.Interfaces;

public class NotificationHub : Hub
{
    private readonly INotificationRepository _notificationRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    // Track admin connections in a static dictionary
    private static HashSet<string> _adminConnections = new HashSet<string>();

    public NotificationHub(INotificationRepository notificationRepository, UserManager<ApplicationUser> userManager)
    {
        _notificationRepository = notificationRepository;
        _userManager = userManager;
    }

    /// <inheritdoc/>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);

            // If the user is an admin, add the connection ID to the admin connections set
            if (roles.Contains("Admin"))
            {
                _adminConnections.Add(Context.ConnectionId);
            }
        }

        await base.OnConnectedAsync();
    }

    /// <inheritdoc/>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);

            // If the user is an admin, remove the connection ID from the admin connections set
            if (roles.Contains("Admin"))
            {
                _adminConnections.Remove(Context.ConnectionId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }


    /// <summary>
    /// Sends a notification to a specific user and all connected admins.
    /// </summary>
    /// <param name="userId">The ID of the user to receive the notification.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="postId">The ID of the related post.</param>
    public async Task SendNotification(string userId, string message, int postId)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            Message = message,
            PostId = postId,
            Timestamp = DateTime.UtcNow,
            IsRead = false,
        });

        foreach (var connectionId in _adminConnections)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification", new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false // Initially, the notification is unread
            });
        }
    }

    /// <summary>
    /// Sends all unread notifications to the currently connected user.
    /// </summary>
    public async Task SendUnreadNotifications()
    {
        string? userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId, unread: false);
            foreach (var notification in notifications)
            {
                await Clients.User(userId).SendAsync("ReceiveNotification", notification);
            }
        }
    }

    /// <summary>
    /// Sends all notifications to the currently connected user.
    /// </summary>
    public async Task FetchAllNotifications()
    {
        string? userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            IEnumerable<Notification>? notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
            await Clients.User(userId).SendAsync("ReceiveAllNotifications", notifications);
        }
    }

    /// <summary>
    /// Marks a notification as read for the current user and notifies the client.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to mark as read.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task MarkNotificationAsRead(int notificationId)
    {
        string? userId = this.Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await this._notificationRepository.MarkAsReadAsync(notificationId);
                await this.Clients.User(userId).SendAsync("NotificationMarkedAsRead", notificationId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking notification as read: {ex.Message}");
                await this.Clients.User(userId).SendAsync("ErrorMarkingAsRead", notificationId);
            }
        }
    }

    /// <summary>
    /// Retrieves notifications from the last 30 minutes and broadcasts them to all connected admins.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the recent notifications.</returns>
    public async Task<IEnumerable<Notification>> GetRecentNotifications()
    {
        var thirtyMinutesAgo = DateTime.UtcNow.AddMinutes(-30);

        var notifications = await _notificationRepository.GetRecentNotificationsAsync(thirtyMinutesAgo);

        // If any admins are connected, broadcast the notifications to them
        foreach (var connectionId in _adminConnections)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification", notifications);
        }

        return notifications;
    }
}
