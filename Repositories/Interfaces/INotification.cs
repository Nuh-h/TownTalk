using TownTalk.Models;

namespace TownTalk.Repositories.Interfaces;
public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
    Task AddNotificationAsync(Notification notification);
    Task MarkAsReadAsync(int notificationId);
    Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
    Task<bool> NotificationExistsAsync(string userId, int postId, string senderId, string type);
}
