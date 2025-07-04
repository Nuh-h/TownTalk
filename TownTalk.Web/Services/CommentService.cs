namespace TownTalk.Web.Services;

using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Services.Interfaces;

/// <summary>
/// Provides comment-related operations and queries for the TownTalk application.
/// </summary>
/// <summary>
/// Service for handling comment-related operations.
/// </summary>
public class CommentService : ICommentService
{
    /// <summary>
    /// The application's database context.
    /// </summary>
    private readonly TownTalkDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentService"/> class.
    /// </summary>
    /// <param name="context">The database context to use.</param>
    public CommentService(TownTalkDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the total number of comments.
    /// </summary>
    /// <returns>The total comment count.</returns>
    public async Task<int> GetTotalCommentsAsync()
        => await _context.Comments.CountAsync();

    /// <summary>
    /// Gets the number of comments made by a specific user.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <returns>The comment count for the user.</returns>
    public async Task<int> GetUserCommentCountAsync(string userId)
        => await _context.Comments.CountAsync(c => c.UserId == userId);

    /// <summary>
    /// Gets the number of new comments created in the last month.
    /// </summary>
    /// <returns>The count of new comments this month.</returns>
    public async Task<int> GetNewCommentsThisMonthAsync()
        => await _context.Comments.CountAsync(c => c.CreatedAt >= DateTime.UtcNow.AddMonths(-1));
}