using System.ComponentModel.DataAnnotations;

namespace TownTalk.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } // Name of the category
    public ICollection<Post> Posts { get; set; } = []; // Navigation property for posts
}
