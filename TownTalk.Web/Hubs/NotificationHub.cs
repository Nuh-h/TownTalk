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

    // When a user disconnects from the hub
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

    public async Task FetchAllNotifications()
    {
        string? userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            IEnumerable<Notification>? notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
            await Clients.User(userId).SendAsync("ReceiveAllNotifications", notifications);
        }
    }

    public async Task MarkNotificationAsRead(int notificationId)
    {
        string? userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await _notificationRepository.MarkAsReadAsync(notificationId);
                await Clients.User(userId).SendAsync("NotificationMarkedAsRead", notificationId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking notification as read: {ex.Message}");
                await Clients.User(userId).SendAsync("ErrorMarkingAsRead", notificationId);
            }
        }
    }

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
