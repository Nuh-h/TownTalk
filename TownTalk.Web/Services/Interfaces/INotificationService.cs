namespace TownTalk.Web.Services.Interfaces;

using TownTalk.Web.Models;

/// <summary>
/// Provides notification-related services for users.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notifies a user with a specific message.
    /// </summary>
    /// <param name="userId">The ID of the user to notify.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="postId">The related post ID.</param>
    /// <param name="senderId">The ID of the sender.</param>
    /// <param name="type">The type of notification.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task NotifyUserAsync(string userId, string message, int postId, string senderId, string type);

    /// <summary>
    /// Notifies a user with a specific message (overload without postId).
    /// </summary>
    /// <param name="userId">The ID of the user to notify.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="senderId">The ID of the sender.</param>
    /// <param name="type">The type of notification.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task NotifyUserAsync(string userId, string message, string senderId, string type);

    /// <summary>
    /// Notifies a user that they have a new follower.
    /// </summary>
    /// <param name="followerId">The ID of the follower.</param>
    /// <param name="followedId">The ID of the followed user.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task NotifyFollowAsync(string followerId, string followedId);

    /// <summary>
    /// Notifies a user that they have been unfollowed.
    /// </summary>
    /// <param name="followerId">The ID of the follower.</param>
    /// <param name="followedId">The ID of the unfollowed user.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task NotifyUnfollowAsync(string followerId, string followedId);

    /// <summary>
    /// Notifies a user about a new comment on their post.
    /// </summary>
    /// <param name="postId">The ID of the post.</param>
    /// <param name="commenterId">The ID of the commenter.</param>
    /// <param name="originalPosterId">The ID of the original poster.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task NotifyCommentAsync(string postId, string commenterId, string originalPosterId);

    /// <summary>
    /// Notifies a user about a reaction to their post.
    /// </summary>
    /// <param name="postId">The ID of the post.</param>
    /// <param name="reactorId">The ID of the reactor.</param>
    /// <param name="originalPosterId">The ID of the original poster.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task NotifyReactionAsync(string postId, string reactorId, string originalPosterId);

    /// <summary>
    /// Notifies a user about a reaction.
    /// </summary>
    /// <param name="reactorId">The ID of the reactor.</param>
    /// <param name="originalPosterId">The ID of the original poster.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task NotifyReactionAsync(string reactorId, string originalPosterId);

    /// <summary>
    /// Gets the recent notifications for the user.
    /// </summary>
    /// <returns>A collection of recent notifications.</returns>
    Task<IEnumerable<Notification>> GetRecentNotificationsAsync();

    /// <summary>
    /// Notifies a user that their profile has been viewed.
    /// </summary>
    /// <param name="viewerId">The ID of the user who viewed the profile.</param>
    /// <param name="viewedUserId">The ID of the user whose profile was viewed.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task NotifyProfileViewAsync(string viewerId, string viewedUserId);

    /// <summary>
    /// Pushes an update message to all active users.
    /// </summary>
    /// <param name="message">The message to send to active users.</param>
    /// <param name="type">The type of the update message.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PushUpdateToActiveUsersAsync(string message, string type);

    /// <summary>
    /// Gets the count of unread notifications for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose unread notifications count is to be retrieved.</param>
    /// <returns>A <see cref="Task{Int32}"/> representing the asynchronous operation, with the count of unread notifications as the result.</returns>
    Task<int> GetUnreadNotificationsCountAsync(string userId);
}