namespace TownTalk.Web.Models;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents an application user with extended profile information.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the user joined.
    /// </summary>
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the user's biography.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the user's location.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the date and time the user was last active.
    /// </summary>
    public DateTime? LastActive { get; set; }

    /// <summary>
    /// Gets or sets the collection of posts created by the user.
    /// </summary>
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    /// <summary>
    /// Gets or sets the collection of comments made by the user.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    /// Gets or sets the collection of reactions made by the user.
    /// </summary>
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    /// <summary>
    /// Gets or sets the collection of users this user is following.
    /// </summary>
    public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();

    /// <summary>
    /// Gets or sets the collection of users following this user.
    /// </summary>
    public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
}
