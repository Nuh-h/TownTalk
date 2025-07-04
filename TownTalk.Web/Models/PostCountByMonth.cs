namespace TownTalk.Web.Models;

/// <summary>
/// Represents the count of posts for a specific month.
/// </summary>
public class PostCountByMonth
{
    /// <summary>
    /// Gets or sets the month.
    /// </summary>
    public string Month { get; set; }

    /// <summary>
    /// Gets or sets the count of posts for the month.
    /// </summary>
    public int Count { get; set; }
}