namespace TownTalk.Web.Models;

using System.Text.Json.Serialization;

public class Notification
{
    public int Id { get; set; }

    public string UserId { get; set; } // Foreign key for User
    [JsonIgnore]
    public ApplicationUser User { get; set; } // Navigation property for User

    public string Message { get; set; } // Brief description of the notification

    public string Type { get; set; } // Type of notification (e.g., Comment, Reaction, New Post, Tag, Direct Message, Follow)

    public int? PostId { get; set; } // Foreign key for the associated post (nullable)
    public int? CommentId { get; set; } // Foreign key for the associated comment (nullable)
    public string? TaggedUserId { get; set; } // Foreign key for the tagged user (nullable)
    [JsonIgnore]
    public ApplicationUser TaggedUser { get; set; } // Navigation property for User

    public string SenderId { get; set; } // Foreign key for the user sending a direct message (nullable)
    public string? SenderDisplayName => Sender?.DisplayName;

    [JsonIgnore]
    public ApplicationUser Sender { get; set; } // Navigation property for User

    public bool IsRead { get; set; } = false; // Default to unread
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatically set the creation date
}
