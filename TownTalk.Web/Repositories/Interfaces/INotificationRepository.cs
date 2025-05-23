namespace TownTalk.Web.Repositories.Interfaces;

using TownTalk.Web.Models;

/// <summary>
/// Provides methods for managing notifications in the application.
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Gets the notifications for a user.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="unread">Whether to get only unread notifications.</param>
    /// <returns>A collection of notifications.</returns>
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, bool unread = false);

    /// <summary>
    /// Adds a new notification.
    /// </summary>
    /// <param name="notification">The notification to add.</param>
    Task AddNotificationAsync(Notification notification);

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    /// <param name="notificationId">The notification ID.</param>
    Task MarkAsReadAsync(int notificationId);

    /// <summary>
    /// Checks if a notification exists for a user, post, sender, and type.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="postId">The post ID.</param>
    /// <param name="senderId">The sender's ID.</param>
    /// <param name="type">The notification type.</param>
    /// <returns>True if the notification exists; otherwise, false.</returns>
    Task<bool> NotificationExistsAsync(string userId, int postId, string senderId, string type);

    /// <summary>
    /// Gets recent notifications from a specific date.
    /// </summary>
    /// <param name="fromDate">The date from which to get notifications.</param>
    /// <returns>A collection of recent notifications.</returns>
    Task<IEnumerable<Notification>> GetRecentNotificationsAsync(DateTime fromDate);

    /// <summary>
    /// Deletes a notification by its ID asynchronously.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteNotificationAsync(int notificationId);
}
