namespace TownTalk.Web.Models;

/// <summary>
/// Represents a user follow relationship between two users in the application.
/// </summary>
public class UserFollow
{
    /// <summary>
    /// Gets or sets the unique identifier for the user follow relationship.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who is following.
    /// </summary>
    public string FollowerId { get; set; }

    /// <summary>
    /// Gets or sets the follower user.
    /// </summary>
    public ApplicationUser Follower { get; set; } // Navigation property for follower

    /// <summary>
    /// Gets or sets the ID of the user being followed.
    /// </summary>
    public string FollowedId { get; set; }

    /// <summary>
    /// Gets or sets the followed user.
    /// </summary>
    public ApplicationUser Followed { get; set; } // Navigation property for followed user

    /// <summary>
    /// Gets or sets the date and time when the follow occurred.
    /// </summary>
    public DateTime FollowedAt { get; set; }
}
