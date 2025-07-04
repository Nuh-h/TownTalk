namespace TownTalk.Web.ViewModels;

using TownTalk.Web.Models;

/// <summary>
/// ViewModel for displaying and simulating notifications.
/// </summary>
public class SimulateNotificationsViewModel
{
    /// <summary>
    /// Gets or sets the list of users.
    /// </summary>
    public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    /// <summary>
    /// Gets or sets the collection of notifications.
    /// </summary>
    public IEnumerable<Notification> Notifications { get; set; } = new List<Notification>();

    /// <summary>
    /// Gets or sets the label for the sender.
    /// </summary>
    public required string SenderLabel { get; set; }

    /// <summary>
    /// Gets or sets the label for the receiver.
    /// </summary>
    public required string ReceiverLabel { get; set; }

    /// <summary>
    /// Gets or sets the select sender label.
    /// </summary>
    public required string SelectSender { get; set; }

    /// <summary>
    /// Gets or sets the select receiver label.
    /// </summary>
    public required string SelectReceiver { get; set; }

    /// <summary>
    /// Gets or sets the notification type label.
    /// </summary>
    public required string NotificationTypeLabel { get; set; }

    /// <summary>
    /// Gets or sets the select notification type label.
    /// </summary>
    public required string SelectNotificationType { get; set; }

    /// <summary>
    /// Gets or sets the new follower label.
    /// </summary>
    public required string NewFollower { get; set; }

    /// <summary>
    /// Gets or sets the new reaction label.
    /// </summary>
    public required string NewReaction { get; set; }

    /// <summary>
    /// Gets or sets the new post label.
    /// </summary>
    public required string NewPost { get; set; }

    /// <summary>
    /// Gets or sets the send notification label.
    /// </summary>
    public required string SendNotification { get; set; }

    /// <summary>
    /// Gets or sets the send notification header.
    /// </summary>
    public required string SendNotificationHeader { get; set; }

    /// <summary>
    /// Gets or sets the recent notifications label.
    /// </summary>
    public required string RecentNotifications { get; set; }

    /// <summary>
    /// Gets or sets the sender column label.
    /// </summary>
    public required string SenderColumn { get; set; }

    /// <summary>
    /// Gets or sets the receiver column label.
    /// </summary>
    public required string ReceiverColumn { get; set; }

    /// <summary>
    /// Gets or sets the message column label.
    /// </summary>
    public required string MessageColumn { get; set; }

    /// <summary>
    /// Gets or sets the time column label.
    /// </summary>
    public required string TimeColumn { get; set; }

    /// <summary>
    /// Gets or sets the status column label.
    /// </summary>
    public required string StatusColumn { get; set; }

    /// <summary>
    /// Gets or sets the read status label.
    /// </summary>
    public required string ReadStatus { get; set; }

    /// <summary>
    /// Gets or sets the delivered status label.
    /// </summary>
    public required string DeliveredStatus { get; set; }
}