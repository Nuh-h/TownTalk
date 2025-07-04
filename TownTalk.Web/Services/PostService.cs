using TownTalk.Web.Models;
using TownTalk.Web.Repositories.Interfaces;
using TownTalk.Web.Services.Interfaces;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    /// <inheritdoc/>
    public async Task<List<PostCountByMonth>> GetPostsByMonthAsync(string userId)
    {
        var groupedResult = await _postRepository.GetPostsByMonth(userId);

        // Map dynamic results to DTO
        List<PostCountByMonth>? result = groupedResult
            .Select(g => new PostCountByMonth
            {
                Month = $"{g.year:D4}-{g.month:D2}",
                Count = g.count
            })
            .ToList();

        return result;
    }

    /// <inheritdoc/>
    public async Task<List<UserActivityByMonth>> GetUserActivityByMonthAsync(string userId)
    {
        // Get posts, comments, and reactions grouped by month
        List<dynamic>? posts = await _postRepository.GetPostsByMonth(userId);
        List<dynamic>? comments = await _postRepository.GetCommentsByMonth(userId);
        List<dynamic>? reactions = await _postRepository.GetReactionsByMonth(userId);

        // Get all months present in any series
        var allMonths = posts
            .Concat(comments)
            .Concat(reactions)
            .Select(x => new { x.year, x.month })
            .Distinct()
            .OrderBy(x => x.year).ThenBy(x => x.month)
            .ToList();

        List<UserActivityByMonth>? result = allMonths.Select(m =>
            new UserActivityByMonth
            {
                Month = $"{m.year:D4}-{m.month:D2}",
                PostCount = posts.FirstOrDefault(x => x.year == m.year && x.month == m.month)?.count ?? 0,
                CommentCount = comments.FirstOrDefault(x => x.year == m.year && x.month == m.month)?.count ?? 0,
                ReactionCount = reactions.FirstOrDefault(x => x.year == m.year && x.month == m.month)?.count ?? 0
            }
        ).ToList();

        return result;
    }

    public async Task<(string? Title, int ReactionCount)> GetMostPopularPostAsync(string userId)
    {
        // Call repository and return the tuple (Title, ReactionCount)
        return await _postRepository.GetMostPopularPostAsync(userId);
    }

    public async Task<int> GetNewPostsThisMonthAsync()
    {
        // Call repository and return the count
        return await _postRepository.GetNewPostsThisMonthAsync();
    }

    public async Task<int> GetPostCountAsync(string userId)
    {
        // Placeholder: Call repository or return 0
        return await _postRepository.GetPostCountAsync(userId);
    }

    public async Task<int> GetTotalPostsAsync()
    {
        // Placeholder: Call repository or return 0
        return await _postRepository.GetTotalPostsAsync();
    }
}