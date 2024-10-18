namespace TownTalk.Models;
public class UserFollow
{
    public int Id { get; set; }
    public string FollowerId { get; set; } // User who follows
    public ApplicationUser Follower { get; set; } // Navigation property for follower
    public string FollowedId { get; set; } // User being followed
    public ApplicationUser Followed { get; set; } // Navigation property for followed user
}
