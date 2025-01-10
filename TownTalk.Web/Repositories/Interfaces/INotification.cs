using TownTalk.Models;

namespace TownTalk.Repositories.Interfaces;
public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, bool unread = false);
    Task AddNotificationAsync(Notification notification);
    Task MarkAsReadAsync(int notificationId);
    Task<bool> NotificationExistsAsync(string userId, int postId, string senderId, string type);
}
