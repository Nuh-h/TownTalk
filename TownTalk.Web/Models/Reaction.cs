namespace TownTalk.Web.Models;

using System.ComponentModel.DataAnnotations;

public enum ReactionType
{
    Like = 0,        // 👍
    Love = 1,        // ❤️
    Haha = 2,        // 😂
    Wow = 3,         // 😮
    HeartEyes = 4,   // 😍
    Sad = 5,         // 😢
    Angry = 6,       // 😡
    Cool = 7,        // 😎
    Clap = 8         // 👏
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
    public DateTime CreatedAt { get; set; }
}
