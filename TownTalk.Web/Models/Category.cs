namespace TownTalk.Web.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a category that can contain multiple posts.
/// </summary>
public class Category
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the collection of posts associated with this category.
    /// </summary>
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
