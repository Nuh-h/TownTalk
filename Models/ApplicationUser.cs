using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TownTalk.Models;
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty; // For user-friendly display
    public DateTime DateJoined { get; set; } = DateTime.UtcNow; // Automatically set the join date
    public ICollection<Post> Posts { get; set; } = [];// Navigation property for user posts
    public ICollection<Comment> Comments { get; set; } = []; // Navigation property for user comments
    public ICollection<Reaction> Reactions { get; set; } = []; // Navigation property for user reactions
    public ICollection<Notification> Notifications { get; set; } = []; // Navigation property for notifications
    public ICollection<UserFollow> Following { get; set; } = []; // Users this user follows
    public ICollection<UserFollow> Followers { get; set; } = []; // Users following this user
}
