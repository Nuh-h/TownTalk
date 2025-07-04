namespace TownTalk.Web.ViewModels;

/// <summary>
/// ViewModel representing user statistics for display purposes.
/// </summary>
public class UserStatsViewModel
{
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public int Posts { get; set; }
    public int Comments { get; set; }
    public int ReactionsReceived { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public int UnreadNotifications { get; set; }
    public string? MostPopularPostTitle { get; set; }
    public int MostPopularPostReactions { get; set; }
}