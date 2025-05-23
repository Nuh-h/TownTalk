namespace TownTalk.Web.Repositories;

using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Repositories.Interfaces;
using TownTalk.Web.Models;

/// <summary>
/// Repository for managing notifications in the TownTalk application.
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly TownTalkDbContext _context;

    public NotificationRepository(TownTalkDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task AddNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, bool unread)
    {

        IQueryable<Notification>? query = _context.Notifications
            .Include(n => n.Sender)
            .Where(n => n.UserId == userId);

        if (unread)
        {
            query = query.Where(n => n.IsRead == true);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Notification>> GetRecentNotificationsAsync(DateTime fromDate)
    {
        return await _context.Notifications
            .Where(n => n.CreatedAt >= fromDate)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task MarkAsReadAsync(int notificationId)
    {
        Notification? notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task DeleteNotificationAsync(int notificationId)
    {
        Notification? notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> NotificationExistsAsync(string userId, int postId, string senderId, string type)
    {
        return await _context.Notifications
                .AnyAsync(n => n.UserId == userId && n.PostId == postId && n.SenderId == senderId && n.Type == type && !n.IsRead);
    }

}
