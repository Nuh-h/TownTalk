namespace TownTalk.Web.Repositories.Interfaces;

using TownTalk.Web.Models;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, bool unread = false);
    Task AddNotificationAsync(Notification notification);
    Task MarkAsReadAsync(int notificationId);
    Task<bool> NotificationExistsAsync(string userId, int postId, string senderId, string type);
    Task<IEnumerable<Notification>> GetRecentNotificationsAsync(DateTime fromDate);
}
