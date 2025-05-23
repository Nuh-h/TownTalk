namespace TownTalk.Web.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a post in the TownTalk application.
/// </summary>
public class Post
{
    /// <summary>
    /// Gets or sets the unique identifier for the post.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the post.
    /// </summary>
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the post.
    /// </summary>
    [Required]
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time of the post.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the user ID of the post's author.
    /// </summary>
    public string UserId { get; set; } // Foreign key for User

    /// <summary>
    /// Gets or sets the user who created the post.
    /// </summary>
    public ApplicationUser User { get; set; } // Navigation property for User

    /// <summary>
    /// Gets or sets the collection of comments on the post.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    /// Gets or sets the collection of reactions to the post.
    /// </summary>
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    /// <summary>
    /// Gets or sets the optional category ID for the post.
    /// </summary>
    public int? CategoryId { get; set; } // Optional foreign key for Category

    /// <summary>
    /// Gets or sets the category of the post.
    /// </summary>
    public Category? Category { get; set; } // Navigation property for Category
}
