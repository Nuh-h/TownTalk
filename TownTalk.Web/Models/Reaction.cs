using System.ComponentModel.DataAnnotations;

namespace TownTalk.Models;

public enum ReactionType
{
    Like = 0,  // 😊
    Love = 1,  // ❤️
    Sad = 2    // 😢
}
public class Reaction
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; } // Foreign key for User
    public ApplicationUser User { get; set; } // Navigation property for User
    [Required]
    public int PostId { get; set; } // Foreign key for Post
    public Post Post { get; set; } // Navigation property for Post
    public ReactionType Type { get; set; }
}
