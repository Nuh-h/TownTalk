using Microsoft.AspNetCore.SignalR;
using TownTalk.Models;
using TownTalk.Repositories.Interfaces;

public class NotificationHub : Hub
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationHub(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task SendNotification(string userId, string message, int postId)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            Message = message,
            PostId = postId,
            Timestamp = DateTime.UtcNow,
            IsRead = false
        });
    }

    public async Task SendUnreadNotifications()
    {
        string? userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);
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
}
