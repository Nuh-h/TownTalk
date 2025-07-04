namespace TownTalk.Web.Services.Interfaces;

using TownTalk.Web.Models;

/// <summary>
/// Provides methods for retrieving post and user activity data.
/// </summary>
public interface IPostService
{
    /// <summary>
    /// Gets the most popular post for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>
    /// A tuple containing the title of the most popular post (or null if none exists) and the reaction count.
    /// </returns>
    Task<(string? Title, int ReactionCount)> GetMostPopularPostAsync(string userId);
    Task<int> GetNewPostsThisMonthAsync();

    /// <summary>
    /// Gets the total number of posts for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The total number of posts for the user.</returns>
    Task<int> GetPostCountAsync(string userId);

    /// <summary>
    /// Gets the count of posts by month for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of post counts by month.</returns>
    Task<List<PostCountByMonth>> GetPostsByMonthAsync(string userId);
    Task<int> GetTotalPostsAsync();

    /// <summary>
    /// Gets the user activity by month for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of user activities by month.</returns>
    Task<List<UserActivityByMonth>> GetUserActivityByMonthAsync(string userId);
}