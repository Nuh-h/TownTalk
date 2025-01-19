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


}