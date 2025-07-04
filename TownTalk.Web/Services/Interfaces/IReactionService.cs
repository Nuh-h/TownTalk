namespace TownTalk.Web.Services.Interfaces;

/// <summary>
/// Provides methods to handle reaction-related operations.
/// </summary>
public interface IReactionService
{
    /// <summary>
    /// Gets the total number of reactions.
    /// </summary>
    /// <returns>The total number of reactions as an integer.</returns>
    Task<int> GetTotalReactionsAsync();

    /// <summary>
    /// Gets the total number of reactions received by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The number of reactions received by the user as an integer.</returns>
    Task<int> GetUserReactionsReceivedAsync(string userId);
}