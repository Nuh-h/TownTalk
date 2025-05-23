namespace TownTalk.Web.Services.Interfaces;

using TownTalk.Web.Models;

/// <summary>
/// Provides methods for managing user follow relationships and related queries.
/// </summary>
public interface IUserFollowService
{
    /// <summary>
    /// Creates a new follow relationship between two users.
    /// </summary>
    /// <param name="followerId">The ID of the user who is following another user.</param>
    /// <param name="followedId">The ID of the user being followed.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task FollowUserAsync(string followerId, string followedId);

    /// <summary>
    /// Removes the follow relationship between two users.
    /// </summary>
    /// <param name="followerId">The ID of the user who is following another user.</param>
    /// <param name="followedId">The ID of the user being followed.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task UnfollowUserAsync(string followerId, string followedId);

    /// <summary>
    /// Gets the list of followers for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose followers are being retrieved.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<List<ApplicationUser>> GetFollowersAsync(string userId);

    /// <summary>
    /// Gets the list of users that a specific user is following.
    /// </summary>
    /// <param name="userId">The ID of the user whose following list is being retrieved.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<List<ApplicationUser>> GetFollowingAsync(string userId);

    /// <summary>
    /// Checks if a user is following another user.
    /// </summary>
    /// <param name="followerId">The ID of the user who is following another user.</param>
    /// <param name="followedId">The ID of the user being followed.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<bool> IsFollowingAsync(string followerId, string followedId);

    /// <summary>
    /// Gets the count of followers for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose followers list total is being retrieved.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<int> GetFollowerCountAsync(string userId);

    /// <summary>
    /// Gets the count of users that a specific user is following.
    /// </summary>
    /// <param name="userId">The ID of the user whose following list count is being retrieved.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<int> GetFollowingCountAsync(string userId);

    /// <summary>
    /// Gets the growth of followers for a specific user over time.
    /// </summary>
    /// <param name="userId">The ID of the user whose followers growth rate is being retrieved.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<List<dynamic>> GetFollowersGrowth(string userId);
}
