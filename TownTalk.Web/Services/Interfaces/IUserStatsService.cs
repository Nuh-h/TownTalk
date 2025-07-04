namespace TownTalk.Web.Services.Interfaces;

/// <summary>
/// Provides methods to retrieve user statistics.
/// </summary>
public interface IUserStatsService
{
    /// <summary>
    /// Gets the number of posts for the specified user.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>The post count.</returns>
    Task<int> GetPostCountAsync(string userId);

    /// <summary>
    /// Gets the number of comments for the specified user.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>The comment count.</returns>
    Task<int> GetCommentCountAsync(string userId);

    /// <summary>
    /// Gets the number of followers for the specified user.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>The followers count.</returns>
    Task<int> GetFollowersCountAsync(string userId);

    /// <summary>
    /// Gets the number of users the specified user is following.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>The following count.</returns>
    Task<int> GetFollowingCountAsync(string userId);

    /// <summary>
    /// Gets the number of unread notifications for the specified user.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>The unread notifications count.</returns>
    Task<int> GetUnreadNotificationsCountAsync(string userId);

    /// <summary>
    /// Gets the most popular post for the specified user.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>A tuple containing the post title and reaction count.</returns>
    Task<(string? Title, int ReactionCount)> GetMostPopularPostAsync(string userId);
}