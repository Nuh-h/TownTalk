namespace TownTalk.Web.Services.Interfaces;
using TownTalk.Web.Models;

public interface IUserFollowService
{
    Task FollowUserAsync(string followerId, string followedId);
    Task UnfollowUserAsync(string followerId, string followedId);
    Task<List<ApplicationUser>> GetFollowersAsync(string userId);
    Task<List<ApplicationUser>> GetFollowingAsync(string userId);
    Task<bool> IsFollowingAsync(string followerId, string followedId);
    Task<int> GetFollowerCountAsync(string userId);
    Task<int> GetFollowingCountAsync(string userId);
    Task<List<dynamic>> GetFollowersGrowth(string userId);
}
