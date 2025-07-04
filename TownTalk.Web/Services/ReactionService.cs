namespace TownTalk.Web.Services;

using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Services.Interfaces;

/// <summary>
/// Provides services for handling reactions in the application.
/// </summary>
/// <summary>
/// Service for managing reactions, such as counting total reactions and reactions received by a user.
/// </summary>
public class ReactionService : IReactionService
{
    /// <summary>
    /// The database context for accessing reaction data.
    /// </summary>
    private readonly TownTalkDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactionService"/> class.
    /// </summary>
    /// <param name="context">The database context to use.</param>
    public ReactionService(TownTalkDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the total number of reactions in the database.
    /// </summary>
    /// <returns>The total count of reactions.</returns>
    public async Task<int> GetTotalReactionsAsync()
        => await _context.Reactions.CountAsync();

    /// <summary>
    /// Gets the total number of reactions received by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose received reactions are counted.</param>
    /// <returns>The count of reactions received by the user.</returns>
    public async Task<int> GetUserReactionsReceivedAsync(string userId)
        => await _context.Reactions.CountAsync(r => r.Post.UserId == userId);
}