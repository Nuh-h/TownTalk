using System.ComponentModel.DataAnnotations;

namespace TownTalk.Models;

public class Comment
{
    public int Id { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public int PostId { get; set; } // Foreign key for Post
    public Post Post { get; set; } // Navigation property for Post
    [Required]
    public string UserId { get; set; } // Foreign key for User
    public ApplicationUser User { get; set; } // Navigation property for User
    public int? ParentCommentId { get; set; } // Nullable foreign key for the parent comment
    public Comment? ParentComment { get; set; } // Navigation property for the parent comment
    public ICollection<Comment> Replies { get; set; } = [];
}
