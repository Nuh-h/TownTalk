using Microsoft.AspNetCore.Identity;
using TownTalk.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    public string? Bio { get; set; } // Nullable for optional bio
    public string? Location { get; set; } // Nullable for optional location
    public DateTime? LastActive { get; set; } // Nullable for optional last active date
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
    public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
}
