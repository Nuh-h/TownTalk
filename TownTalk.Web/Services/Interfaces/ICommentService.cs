namespace TownTalk.Web.Services.Interfaces;

/// <summary>
/// Provides methods for comment-related operations.
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Gets the total number of comments.
    /// </summary>
    /// <returns>The total comment count.</returns>
    Task<int> GetTotalCommentsAsync();

    /// <summary>
    /// Gets the number of comments made by a specific user.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>The comment count for the user.</returns>
    Task<int> GetUserCommentCountAsync(string userId);

    /// <summary>
    /// Gets the number of new comments made this month.
    /// </summary>
    /// <returns>The count of new comments this month.</returns>
    Task<int> GetNewCommentsThisMonthAsync();
}