using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using TownTalk.Models;
using TownTalk.Repositories.Interfaces;
using TownTalk.Services.Interfaces;

namespace TownTalk.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext, UserManager<ApplicationUser> userManager)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        // // Sends a notification to a specific user
        public async Task NotifyUserAsync(string userId, string message, int postId, string senderId, string type)
        {
            ApplicationUser? sender = await _userManager.FindByIdAsync(senderId);

            bool notificationExists = await _notificationRepository.NotificationExistsAsync(userId, postId, sender.Id, type);

            if (notificationExists || senderId == userId)
            {
                return;
            }

            Notification? notification = new()
            {
                UserId = userId,
                Message = message,
                PostId = postId,
                SenderId = sender.Id,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Type = type
            };

            await _notificationRepository.AddNotificationAsync(notification);

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }



        public async Task NotifyFollowAsync(string followerId, string followedId)
        {
            if (followerId == followedId) return; // Don't notify if the user is following themselves

            var sender = await _userManager.FindByIdAsync(followerId);
            var message = $"{sender.DisplayName} is now following you.";

            // Send notification to the followed user
            await NotifyUserAsync(followedId, message, 0, followerId, "Follow");
        }


        // Notifies when a comment is added to a post
        public async Task NotifyCommentAsync(string postId, string commenterId, string originalPosterId)
        {
            if (commenterId == originalPosterId) return;

            var sender = await _userManager.FindByIdAsync(commenterId);
            var message = $"{sender.DisplayName} commented on your post.";
            await NotifyUserAsync(originalPosterId, message, int.Parse(postId), commenterId, "Comment");
        }

        // Notifies when a reaction is added to a post
        public async Task NotifyReactionAsync(string postId, string reactorId, string originalPosterId)
        {
            ApplicationUser? sender = await _userManager.FindByIdAsync(reactorId);
            var message = $"{sender.DisplayName} reacted to your post.";
            await NotifyUserAsync(originalPosterId, message, int.Parse(postId), senderId: reactorId, type: "Reaction");
        }

        // Notifies when a user views another user's profile
        public async Task NotifyProfileViewAsync(string viewerId, string viewedUserId)
        {
            if (viewerId == viewedUserId) return;

            var sender = await _userManager.FindByIdAsync(viewerId);
            var message = $"{sender.DisplayName} viewed your profile.";
            await NotifyUserAsync(viewedUserId, message, 0, viewerId, "ProfileView");
        }

        // Notify when a user unfollows another user
        public async Task NotifyUnfollowAsync(string followerId, string unfollowedId)
        {
            if (followerId == unfollowedId) return;

            var sender = await _userManager.FindByIdAsync(followerId);
            var message = $"{sender.DisplayName} unfollowed you.";
            await NotifyUserAsync(unfollowedId, message, 0, followerId, "Unfollow");
        }

        // Sends a notification to all users when there is an update (reaction, follow, etc.)
        public async Task PushUpdateToActiveUsersAsync(string message, string type)
        {
            List<ApplicationUser>? activeUsers = _userManager.Users.Where(u => u.LastActive.HasValue && u.LastActive.Value > DateTime.UtcNow.AddMinutes(-5)).ToList();

            foreach (ApplicationUser user in activeUsers)
            {
                await _hubContext.Clients.User(user.Id).SendAsync("ReceiveUpdate", new { Message = message, Type = type });
            }
        }

    }
}
