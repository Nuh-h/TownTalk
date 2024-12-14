namespace TownTalk.Models;
public class UserFollow
{
    public int Id { get; set; }
    public string FollowerId { get; set; }
    public ApplicationUser Follower { get; set; } // Navigation property for follower
    public string FollowedId { get; set; }
    public ApplicationUser Followed { get; set; } // Navigation property for followed user
    public DateTime FollowedAt { get; set; }
}
