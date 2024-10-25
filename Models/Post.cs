using System.ComponentModel.DataAnnotations;

namespace TownTalk.Models;

public class Post
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    public string UserId { get; set; } // Foreign key for User
    public ApplicationUser User { get; set; } // Navigation property for User
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Reaction> Reactions { get; set; } = [];
    public int? CategoryId { get; set; } // Optional foreign key for Category
    public Category? Category { get; set; } // Navigation property for Category
}
