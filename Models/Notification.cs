namespace TownTalk.Models;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } // Foreign key for User
    public ApplicationUser User { get; set; } // Navigation property for User
    public string Message { get; set; }
    public bool IsRead { get; set; } = false; // Default to unread
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatically set the creation date
}
