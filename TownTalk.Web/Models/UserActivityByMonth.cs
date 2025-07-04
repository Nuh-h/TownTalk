namespace TownTalk.Web.Models;
/// <summary>
/// Represents user activity statistics for a specific month.
/// </summary>
public class UserActivityByMonth
{
    /// <summary>
    /// Gets or sets the month for the activity data.
    /// </summary>
    public string Month { get; set; } = default!;

    /// <summary>
    /// Gets or sets the number of posts in the month.
    /// </summary>
    public int PostCount { get; set; }

    /// <summary>
    /// Gets or sets the number of comments in the month.
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Gets or sets the number of reactions in the month.
    /// </summary>
    public int ReactionCount { get; set; }
}