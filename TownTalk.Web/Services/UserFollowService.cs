using Microsoft.EntityFrameworkCore;
using TownTalk.Models;

public class UserFollowService : IUserFollowService
{
    private readonly TownTalkDbContext _context;

    public UserFollowService(TownTalkDbContext context)
    {
        _context = context;
    }

    public async Task FollowUserAsync(string followerId, string followedId)
    {
        var existingFollow = await _context.UserFollows
            .AnyAsync(userFollow => userFollow.FollowerId == followerId && userFollow.FollowedId == followedId);

        if (!existingFollow)
        {
            var userFollow = new UserFollow { FollowerId = followerId, FollowedId = followedId };
            await _context.UserFollows.AddAsync(userFollow);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnfollowUserAsync(string followerId, string followedId)
    {
        var userFollow = await _context.UserFollows
            .FirstOrDefaultAsync(userFollow => userFollow.FollowerId == followerId && userFollow.FollowedId == followedId);

        if (userFollow != null)
        {
            _context.UserFollows.Remove(userFollow);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<ApplicationUser>> GetFollowersAsync(string userId)
    {
        return await _context.UserFollows
            .Where(userFollow => userFollow.FollowedId == userId)
            .Select(userFollow => userFollow.Follower)
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetFollowingAsync(string userId)
    {
        return await _context.UserFollows
            .Where(userFollow => userFollow.FollowerId == userId)
            .Select(userFollow => userFollow.Followed)
            .ToListAsync();
    }

    public async Task<bool> IsFollowingAsync(string followerId, string followedId)
    {
        return await _context.UserFollows
            .AnyAsync(userFollow => userFollow.FollowerId == followerId && userFollow.FollowedId == followedId);
    }

    public async Task<int> GetFollowerCountAsync(string userId)
    {
        return await _context.UserFollows.CountAsync(userFollow => userFollow.FollowedId == userId);
    }

    public async Task<int> GetFollowingCountAsync(string userId)
    {
        return await _context.UserFollows.CountAsync(userFollow => userFollow.FollowerId == userId);
    }
}
