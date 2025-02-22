using Microsoft.EntityFrameworkCore;
using TownTalk.Models;
using TownTalk.Repositories.Interfaces;
using TownTalk.Data;
using TownTalk.Helpers;

namespace TownTalk.Repositories;
public class PostRepository : IPostRepository
{
    private readonly TownTalkDbContext _context;

    public PostRepository(TownTalkDbContext context)
    {
        _context = context;
    }

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

    public async Task<Post> GetPostByIdAsync(int id, bool? includeReactions)
    {
        IQueryable<Post> query = _context.Posts.Include(p => p.Category);

        if (includeReactions.Value)
        {
            query = query.Include(p => p.Reactions);
        }

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddPostAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();

        performanceLogger.Start();

        List<Category>? results = await _context.Categories.ToListAsync();

        performanceLogger.Stop("GetCategories");

        return results;
    }

    public async Task<List<ApplicationUser>> GetAuthorsAsync()
    {
        PerformanceLogger performanceLogger = new PerformanceLogger();

        performanceLogger.Start();

        List<ApplicationUser>? results = await _context.Users.Where(u => u.Posts.Count > 0).ToListAsync();

        performanceLogger.Stop("GetAuthors");

        return results;
    }

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
            int month, year;
            int.TryParse(at.Split("/")[0], out month);
            int.TryParse(at.Split("/")[1], out year);
            query = query.Where(p => p.CreatedAt.Year == year && p.CreatedAt.Month == month);
        }

        return query;
    }

}