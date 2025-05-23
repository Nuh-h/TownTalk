namespace TownTalk.Web.Services.Interfaces;

using TownTalk.Web.Models;

/// <summary>
/// Provides methods for working with user connections and graph-related operations.
/// </summary>
public interface IGraphService
{
    /// <summary>
    /// Gets the degree of separation between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user.</param>
    /// <param name="userId2">The ID of the second user.</param>
    /// <returns>The degree of separation as an integer.</returns>
    Task<int> GetDegreeOfSeparation(string userId1, string userId2);

    /// <summary>
    /// Finds the connection path between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user.</param>
    /// <param name="userId2">The ID of the second user.</param>
    /// <returns>A list of ApplicationUser objects representing the connection path.</returns>
    Task<List<ApplicationUser>> FindConnectionPath(string userId1, string userId2);
}
