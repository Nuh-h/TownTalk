namespace TownTalk.Web.Models;


/// <summary>
/// Represents the trends of followers and following gained by month.
/// </summary>
public class FollowTrendsByMonth
{
    /// <summary>
    /// Gets or sets the month.
    /// </summary>
    public string Month { get; set; } = default!;

    /// <summary>
    /// Gets or sets the number of followers gained in the month.
    /// </summary>
    public int FollowersGained { get; set; }

    /// <summary>
    /// Gets or sets the number of following gained in the month.
    /// </summary>
    public int FollowingGained { get; set; }
}