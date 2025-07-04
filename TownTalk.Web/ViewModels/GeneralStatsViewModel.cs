namespace TownTalk.Web.ViewModels;
using TownTalk.Web.Models;

/// <summary>
/// ViewModel containing statistics for the admin dashboard, including totals, monthly trends, and per-user stats.
/// </summary>
public class GeneralStatsViewModel
{
    // Totals
    /// <summary>
    /// Gets or sets the total number of users.
    /// </summary>
    public int TotalUsers { get; set; }
    /// <summary>
    /// Gets or sets the total number of posts.
    /// </summary>
    public int TotalPosts { get; set; }

    /// <summary>
    /// Gets or sets the total number of comments.
    /// </summary>
    public int TotalComments { get; set; }

    /// <summary>
    /// Gets or sets the total number of reactions.
    /// </summary>
    public int TotalReactions { get; set; }

    /// <summary>
    /// Gets or sets the number of new users registered this month.
    /// </summary>
    public int NewUsersThisMonth { get; set; }

    /// <summary>
    /// Gets or sets the number of new posts created this month.
    /// </summary>
    public int NewPostsThisMonth { get; set; }

    /// <summary>
    /// Gets or sets the number of new comments made this month.
    /// </summary>
    public int NewCommentsThisMonth { get; set; }

    /// <summary>
    /// Gets or sets the per-user statistics.
    /// </summary>
    public List<UserStatsViewModel> UserStats { get; set; } = new List<UserStatsViewModel>();

    /// <summary>
    /// Gets or sets the list of application users.
    /// </summary>
    public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    public string SelectedUserId;

}