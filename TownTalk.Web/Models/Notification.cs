namespace TownTalk.Web.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a notification sent to a user.
/// </summary>
public class Notification
{
    /// <summary>
    /// Gets or sets the unique identifier for the notification.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the user receiving the notification.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the user receiving the notification.
    /// </summary>
    [JsonIgnore]
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Gets or sets the brief description of the notification.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the type of notification (e.g., Comment, Reaction, New Post, Tag, Direct Message, Follow).
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated post (nullable).
    /// </summary>
    public int? PostId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated comment (nullable).
    /// </summary>
    public int? CommentId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the tagged user (nullable).
    /// </summary>
    public string? TaggedUserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the tagged user.
    /// </summary>
    [JsonIgnore]
    public ApplicationUser? TaggedUser { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the user sending a direct message.
    /// </summary>
    public string SenderId { get; set; }

    /// <summary>
    /// Gets the display name of the sender, if available.
    /// </summary>
    public string? SenderDisplayName => Sender?.DisplayName;

    /// <summary>
    /// Gets or sets the navigation property for the sender.
    /// </summary>
    [JsonIgnore]
    public ApplicationUser? Sender { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the notification has been read.
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// Gets or sets the creation date of the notification.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
