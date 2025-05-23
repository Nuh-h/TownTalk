namespace TownTalk.Web.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a comment made by a user on a post.
/// </summary>
public class Comment
{
    /// <summary>
    /// Gets or sets the unique identifier for the comment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the foreign key for the related post.
    /// </summary>
    [Required]
    public int PostId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the related post.
    /// </summary>
    public Post Post { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the user who made the comment.
    /// </summary>
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the user who made the comment.
    /// </summary>
    public ApplicationUser User { get; set; }

    /// <summary>
    /// Gets or sets the nullable foreign key for the parent comment, if this is a reply.
    /// </summary>
    public int? ParentCommentId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the parent comment.
    /// </summary>
    public Comment? ParentComment { get; set; }

    /// <summary>
    /// Gets or sets the collection of replies to this comment.
    /// </summary>
    public ICollection<Comment> Replies { get; set; } = [];
}
