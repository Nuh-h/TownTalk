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
            ApplicationUser? sender = await _userManager.FindByIdAsync(senderId); // Assuming you have access to UserManager

            // Check if a similar notification already exists
            bool notificationExists = await _notificationRepository.NotificationExistsAsync(userId, postId, sender.Id, type);

            if (notificationExists)
            {
                // Optionally, you can return here or update the existing notification
                return; // Prevent creating a duplicate notification
            }

            // Create the new notification since it does not exist
            Notification? notification = new Notification
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



        // Notifies when a user follows another user
        public async Task NotifyFollowAsync(string followerId, string followedId)
        {
            if (followerId == followedId) return; // Prevent self-notification

            var sender = await _userManager.FindByIdAsync(followerId); // Fetch the sender's details
            var message = $"{sender.DisplayName} is now following you."; // Use display name in the message
            await NotifyUserAsync(followedId, message, 0, followerId, "Follow");
        }

        // Notifies when a comment is added to a post
        public async Task NotifyCommentAsync(string postId, string commenterId, string originalPosterId)
        {
            if (commenterId == originalPosterId) return; // Prevent self-notification

            var sender = await _userManager.FindByIdAsync(commenterId); // Fetch the sender's details
            var message = $"{sender.DisplayName} commented on your post."; // Use display name in the message
            await NotifyUserAsync(originalPosterId, message, int.Parse(postId), commenterId, "Comment");
        }

        // Notifies when a reaction is added to a post
        public async Task NotifyReactionAsync(string postId, string reactorId, string originalPosterId)
        {
            ApplicationUser? sender = await _userManager.FindByIdAsync(reactorId); // Fetch the sender's details
            var message = $"{sender.DisplayName} reacted to your post."; // Use display name in the message
            await NotifyUserAsync(originalPosterId, message, int.Parse(postId), senderId: reactorId, type: "Reaction");
        }


    }
}
