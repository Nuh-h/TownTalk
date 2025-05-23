namespace TownTalk.Web.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the types of reactions that can be made to a post.
/// </summary>
public enum ReactionType
{
    /// <summary>Like reaction (👍)</summary>
    Like = 0,
    /// <summary>Love reaction (❤️)</summary>
    Love = 1,
    /// <summary>Haha reaction (😂)</summary>
    Haha = 2,
    /// <summary>Wow reaction (😮)</summary>
    Wow = 3,
    /// <summary>Heart Eyes reaction (😍)</summary>
    HeartEyes = 4,
    /// <summary>Sad reaction (😢)</summary>
    Sad = 5,
    /// <summary>Angry reaction (😡)</summary>
    Angry = 6,
    /// <summary>Cool reaction (😎)</summary>
    Cool = 7,
    /// <summary>Clap reaction (👏)</summary>
    Clap = 8
}
/// <summary>
/// Represents a reaction made by a user to a post.
/// </summary>
public class Reaction
{
    /// <summary>
    /// Gets or sets the unique identifier for the reaction.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the user who made the reaction.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the user who made the reaction.
    /// </summary>
    public ApplicationUser User { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the post to which the reaction was made.
    /// </summary>
    [Required]
    public int PostId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the post to which the reaction was made.
    /// </summary>
    public Post Post { get; set; }

    /// <summary>
    /// Gets or sets the type of the reaction.
    /// </summary>
    public ReactionType Type { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the reaction was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
