namespace TownTalk.Web.Repositories;

using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Helpers;
using TownTalk.Web.Repositories.Interfaces;
using TownTalk.Web.Models;

/// <summary>
/// Repository for managing Post entities and related data operations.
/// </summary>
public class PostRepository : IPostRepository
{
    private readonly TownTalkDbContext _context;

    public PostRepository(TownTalkDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<int> GetTotalPostsAsync()
    {
        return await _context.Posts.CountAsync();
    }

    /// <inheritdoc/>
    public async Task<int> GetPostCountAsync(string userId)
    {
        return await _context.Posts.CountAsync(p => p.UserId == userId);
    }

    /// <inheritdoc/>
    public async Task<int> GetNewPostsThisMonthAsync()
    {
        DateTime now = DateTime.UtcNow;
        return await _context.Posts
            .CountAsync(p => p.CreatedAt.Year == now.Year && p.CreatedAt.Month == now.Month);
    }

    /// <inheritdoc/>
    public async Task<(string? Title, int ReactionCount)> GetMostPopularPostAsync(string userId)
    {
        var post = await _context.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.Reactions.Count)
            .Select(p => new { p.Title, ReactionCount = p.Reactions.Count })
            .FirstOrDefaultAsync();

        return post == null ? (null, 0) : (post.Title, post.ReactionCount);
    }

    /// <inheritdoc/>
    public async Task<List<Post>> GetAllPostsAsync()
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();

        performanceLogger.Start();

        List<Post>? results = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(navigationPropertyPath: c => c.Replies)
            .ToListAsync();

        performanceLogger.Stop("GetAllPosts");

        return results;
    }

    /// <inheritdoc/>
    public async Task<Post> GetPostByIdAsync(int id, bool? includeReactions)
    {
        IQueryable<Post> query = _context.Posts.Include(p => p.Category);

        if (includeReactions.Value)
        {
            query = query.Include(p => p.Reactions);
        }

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <inheritdoc/>
    public async Task AddPostAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeletePostAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<List<Category>> GetCategoriesAsync()
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();

        performanceLogger.Start();

        List<Category>? results = await _context.Categories.ToListAsync();

        performanceLogger.Stop("GetCategories");

        return results;
    }

    /// <inheritdoc/>
    public async Task<List<ApplicationUser>> GetAuthorsAsync()
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();

        performanceLogger.Start();

        List<ApplicationUser>? results = await _context.Users.Where(u => u.Posts.Count > 0).ToListAsync();

        performanceLogger.Stop("GetAuthors");

        return results;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetPublishedDatesAsync()
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();

        performanceLogger.Start();

        var posts = await _context.Posts
        .Select(p => p.CreatedAt)
        .ToListAsync();

        var uniqueDates = posts
            .Select(p => p.ToString("MM/yyyy"))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        performanceLogger.Stop("GetPublishedDates");

        return uniqueDates;
    }


    /// <inheritdoc/>
    public async Task<List<Post>> GetAllPostsByUserIdAsync(string userId)
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();

        performanceLogger.Start();

        List<Post>? results = await _context.Posts
           .Where(post => post.UserId == userId)
           .ToListAsync();

        performanceLogger.Stop("GetAllPostsByUser");

        return results;
    }

    /// <inheritdoc/>
    public async Task<List<dynamic>> GetPostsByMonth(string userId)
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();

        performanceLogger.Start();

        List<dynamic>? result = await _context.Posts
           .Where(p => p.UserId == userId)
           .Select(p => new { p.CreatedAt })
           .ToListAsync<dynamic>();

        performanceLogger.Stop("Finding posts for user");

        performanceLogger.Start();

        List<dynamic>? groupedResult = result
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
            .Select(g => new
            {
                year = g.Key.Year,
                month = g.Key.Month,
                count = g.Count()
            })
            .OrderBy(g => g.year)
            .ThenBy(g => g.month)
            .ToList<dynamic>();

        performanceLogger.Stop("Grouping posts by month for user");

        return groupedResult;
    }

    /// <inheritdoc/>
    public async Task<List<dynamic>> GetCommentsByMonth(string userId)
    {
        var result = await _context.Comments
            .Where(c => c.UserId == userId)
            .Select(c => new { c.CreatedAt })
            .ToListAsync();

        return result
            .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
            .Select(g => new { year = g.Key.Year, month = g.Key.Month, count = g.Count() })
            .OrderBy(g => g.year).ThenBy(g => g.month)
            .ToList<dynamic>();
    }

    /// <inheritdoc/>
    public async Task<List<dynamic>> GetReactionsByMonth(string userId)
    {
        var result = await _context.Reactions
            .Where(r => r.UserId == userId)
            .Select(r => new { r.CreatedAt })
            .ToListAsync();

        return result
            .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
            .Select(g => new { year = g.Key.Year, month = g.Key.Month, count = g.Count() })
            .OrderBy(g => g.year).ThenBy(g => g.month)
            .ToList<dynamic>();
    }

    /// <inheritdoc/>
    public async Task<List<Post>> GetFilteredPostsAsync(string? q, string? cl, string? by, string? at, int page = 1, int pageSize = 20)
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();
        performanceLogger.Start();

        IQueryable<Post> query = _context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Replies);

        query = ApplyFilters(query, q, cl, by, at);

        query = query.Skip((page - 1) * pageSize).Take(pageSize);

        List<Post> filteredList = await query.ToListAsync();

        performanceLogger.Stop("GetFilteredPosts");

        return filteredList;
    }

    /// <inheritdoc/>
    public async Task<int> GetFilteredPostsCountAsync(string? q, string? cl, string? by, string? at)
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();
        performanceLogger.Start();

        IQueryable<Post> query = _context.Posts;

        query = ApplyFilters(query, q, cl, by, at);

        int count = await query.CountAsync();

        performanceLogger.Stop("GetFilteredCount");

        return count;
    }

    /// <summary>
    /// Applies filtering criteria to a queryable collection of <see cref="Post"/> entities based on the provided parameters.
    /// </summary>
    /// <param name="query">The initial <see cref="IQueryable{Post}"/> to apply filters to.</param>
    /// <param name="q">An optional search term to filter posts by title or content.</param>
    /// <param name="cl">An optional category name to filter posts by category.</param>
    /// <param name="by">An optional user display name to filter posts by author.</param>
    /// <param name="at">
    /// An optional date filter in the format "MM/YYYY" to filter posts by creation month and year.
    /// If parsing fails, this filter is ignored.
    /// </param>
    /// <returns>
    /// The filtered <see cref="IQueryable{Post}"/> based on the provided criteria.
    /// </returns>
    private static IQueryable<Post> ApplyFilters(IQueryable<Post> query, string? q, string? cl, string? by, string? at)
    {
        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(p => p.Title.Contains(q) || p.Content.Contains(q));
        }

        if (!string.IsNullOrEmpty(cl))
        {
            query = query.Where(p => p.Category.Name.Contains(cl));
        }

        if (!string.IsNullOrEmpty(by))
        {
            query = query.Where(p => p.User.DisplayName.Contains(by));
        }

        if (!string.IsNullOrEmpty(at))
        {
            _ = int.TryParse(at.Split("/")[0], out int month);
            _ = int.TryParse(at.Split("/")[1], out int year);

            if (month >= 0 || year >= 0)
            {
                query = query.Where(p => p.CreatedAt.Year == year && p.CreatedAt.Month == month);
            }
        }

        return query;
    }

}