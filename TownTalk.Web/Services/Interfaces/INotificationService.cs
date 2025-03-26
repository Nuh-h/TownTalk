using TownTalk.Web.Models;

namespace TownTalk.Web.Services.Interfaces;

public interface INotificationService
{
    Task NotifyUserAsync(string userId, string message, int postId, string senderId, string type);
    Task NotifyFollowAsync(string followerId, string followedId);
    Task NotifyUnfollowAsync(string followerId, string followedId);
    Task NotifyCommentAsync(string postId, string commenterId, string originalPosterId);
    Task NotifyReactionAsync(string postId, string reactorId, string originalPosterId);
    Task<IEnumerable<Notification>> GetRecentNotificationsAsync();
}
