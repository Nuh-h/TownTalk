using Microsoft.EntityFrameworkCore;
using TownTalk.Models;
using TownTalk.Repositories.Interfaces;

namespace TownTalk.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly TownTalkDbContext _context;

        public NotificationRepository(TownTalkDbContext context)
        {
            _context = context;
        }

        // Adds a new notification to the database
        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        // Retrieves all notifications for a specific user
        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                        .Include(n => n.Sender) // Include the Sender to populate its properties
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // Marks a specific notification as read
        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        // Retrieves all unread notifications for a specific user
        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
            .Include(n => n.Sender) // Include the Sender to populate its properties
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();
        }

        // Optionally: Deletes a notification if needed
        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> NotificationExistsAsync(string userId, int postId, string senderId, string type)
        {
            return await _context.Notifications
                .AnyAsync(n => n.UserId == userId && n.PostId == postId && n.SenderId == senderId && n.Type == type && !n.IsRead);
        }

    }
}
