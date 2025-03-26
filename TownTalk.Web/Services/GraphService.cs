using Microsoft.AspNetCore.Identity;
using TownTalk.Web.Services.Interfaces;
using TownTalk.Web.Models;

namespace TownTalk.Web.Services;
public class GraphService : IGraphService
{
    private readonly IUserFollowService _userFollowService;
    private readonly UserManager<ApplicationUser> _userManager;


    public GraphService(IUserFollowService userFollowService, UserManager<ApplicationUser> userManager)
    {
        _userFollowService = userFollowService;
        _userManager = userManager;
    }

    // This function calculates the shortest path using BFS
    public async Task<int> GetDegreeOfSeparation(string userId1, string userId2)
    {
        if (userId1 == userId2) return 0;

        HashSet<string>? visited = new HashSet<string>();
        Queue<(string UserId, int Degree)>? queue = new Queue<(string UserId, int Degree)>();

        // Start with the first user
        queue.Enqueue((userId1, 0));

        while (queue.Any())
        {
            var (currentUserId, degree) = queue.Dequeue();

            if (currentUserId == userId2) return degree;

            if (visited.Contains(currentUserId)) continue;
            visited.Add(currentUserId);

            // Get the followers (or people that the current user follows)
            List<ApplicationUser>? followers = await _userFollowService.GetFollowersAsync(currentUserId);

            foreach (ApplicationUser follower in followers)
            {
                if (!visited.Contains(follower.Id))
                {
                    queue.Enqueue((follower.Id, degree + 1));
                }
            }
        }

        return -1; // If no connection is found, return -1 (not connected)
    }

    public async Task<List<ApplicationUser>> FindConnectionPath(string userId1, string userId2)
    {
        HashSet<string>? visited = new HashSet<string>();
        Queue<List<ApplicationUser>>? queue = new Queue<List<ApplicationUser>>();
        List<ApplicationUser>? initialPath = new List<ApplicationUser> { await _userManager.FindByIdAsync(userId1) };
        queue.Enqueue(initialPath);
        visited.Add(userId1);

        while (queue.Count > 0)
        {
            List<ApplicationUser>? path = queue.Dequeue();
            ApplicationUser? currentUser = path.Last();

            // If we reached the target user, return the path
            if (currentUser.Id == userId2)
            {
                return path;
            }

            // Get the neighbors of the current user (followers and followings)
            List<ApplicationUser>? neighbors = await GetConnectionsForUser(currentUser.Id); // This could be GetFollowers() + GetFollowing()

            foreach (ApplicationUser neighbor in neighbors)
            {
                if (!visited.Contains(neighbor.Id))
                {
                    visited.Add(neighbor.Id);
                    List<ApplicationUser>? newPath = new List<ApplicationUser>(path) { neighbor };
                    queue.Enqueue(newPath);
                }
            }
        }

        return new List<ApplicationUser>(); // No path found
    }

    private async Task<List<ApplicationUser>> GetConnectionsForUser(string userId)
    {
        List<ApplicationUser>? followers = await _userFollowService.GetFollowersAsync(userId);
        List<ApplicationUser>? following = await _userFollowService.GetFollowingAsync(userId);
        return followers.Concat(following).ToList();
    }


}
