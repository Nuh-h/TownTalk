using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TownTalk.Models;
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Reaction> Reactions { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<UserFollow> Following { get; set; } = [];
    public ICollection<UserFollow> Followers { get; set; } = [];
}
