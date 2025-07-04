namespace TownTalk.Web.Services;

using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Services.Interfaces;
using TownTalk.Web.Models;

public class UserFollowService : IUserFollowService
{
    private readonly TownTalkDbContext _context;

    public UserFollowService(TownTalkDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task FollowUserAsync(string followerId, string followedId)
    {
        bool existingFollow = await _context.UserFollows
            .AnyAsync(userFollow => userFollow.FollowerId == followerId && userFollow.FollowedId == followedId);

        if (!existingFollow)
        {
            UserFollow? userFollow = new UserFollow { FollowerId = followerId, FollowedId = followedId };
            await _context.UserFollows.AddAsync(userFollow);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task UnfollowUserAsync(string followerId, string followedId)
    {
        UserFollow? userFollow = await _context.UserFollows
            .FirstOrDefaultAsync(userFollow => userFollow.FollowerId == followerId && userFollow.FollowedId == followedId);

        if (userFollow != null)
        {
            _context.UserFollows.Remove(userFollow);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<List<ApplicationUser>> GetFollowersAsync(string userId)
    {
        return await _context.UserFollows
            .Where(userFollow => userFollow.FollowedId == userId)
            .Select(userFollow => userFollow.Follower)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ApplicationUser>> GetFollowingAsync(string userId)
    {
        return await _context.UserFollows
            .Where(userFollow => userFollow.FollowerId == userId)
            .Select(userFollow => userFollow.Followed)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> IsFollowingAsync(string followerId, string followedId)
    {
        return await _context.UserFollows
            .AnyAsync(userFollow => userFollow.FollowerId == followerId && userFollow.FollowedId == followedId);
    }

    /// <inheritdoc/>
    public async Task<int> GetFollowerCountAsync(string userId)
    {
        return await _context.UserFollows.CountAsync(userFollow => userFollow.FollowedId == userId);
    }

    /// <inheritdoc/>
    public async Task<int> GetFollowingCountAsync(string userId)
    {
        return await _context.UserFollows.CountAsync(userFollow => userFollow.FollowerId == userId);
    }

    /// <inheritdoc/>
    public async Task<List<dynamic>> GetFollowersGrowthAsync(string userId)
    {
        // Load all necessary data first
        var result = await _context.UserFollows
            .Where(u => u.FollowedId == userId) // Only consider followers for the specified user
            .Select(u => new { u.FollowedAt, u.FollowedId }) // Select the FollowedAt and FollowedId fields
            .ToListAsync(); // Load the data into memory

        // Group by year and month in memory (client-side)
        List<dynamic>? groupedResult = result
            .GroupBy(u => new { u.FollowedAt.Year, u.FollowedAt.Month }) // Group by Year and Month
            .Select(g => new
            {
                year = g.Key.Year,
                month = g.Key.Month,
                count = g.Count()
            })
            .OrderBy(g => g.year)
            .ThenBy(g => g.month)
            .ToList<dynamic>();

        return groupedResult;
    }

    /// <inheritdoc/>
    public async Task<List<dynamic>> GetFollowingGrowthAsync(string userId)
    {
        var result = await _context.UserFollows
            .Where(u => u.FollowerId == userId) // Only consider users this user followed
            .Select(u => new { u.FollowedAt, u.FollowerId })
            .ToListAsync();

        var groupedResult = result
            .GroupBy(u => new { u.FollowedAt.Year, u.FollowedAt.Month })
            .Select(g => new
            {
                year = g.Key.Year,
                month = g.Key.Month,
                count = g.Count()
            })
            .OrderBy(g => g.year)
            .ThenBy(g => g.month)
            .ToList<dynamic>();

        return groupedResult;
    }

    /// <inheritdoc/>
    public async Task<List<FollowTrendsByMonth>> GetFollowTrendsByMonthAsync(string userId)
    {
        List<DateTime>? followers = await _context.UserFollows
            .Where(u => u.FollowedId == userId)
            .Select(u => u.FollowedAt)
            .ToListAsync();

        List<DateTime>? following = await _context.UserFollows
            .Where(u => u.FollowerId == userId)
            .Select(u => u.FollowedAt)
            .ToListAsync();

        var allMonths = followers
            .Concat(following)
            .Select(d => new { d.Year, d.Month })
            .Distinct()
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToList();

        List<FollowTrendsByMonth>? trends = allMonths.Select(m => new FollowTrendsByMonth
        {
            Month = $"{m.Year:D4}-{m.Month:D2}",
            FollowersGained = followers.Count(d => d.Year == m.Year && d.Month == m.Month),
            FollowingGained = following.Count(d => d.Year == m.Year && d.Month == m.Month)
        }).ToList();

        return trends;
    }
}
