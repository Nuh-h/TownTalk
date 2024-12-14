using Microsoft.EntityFrameworkCore;
using TownTalk.Models;
using TownTalk.Repositories.Interfaces;
using System.Linq;
using TownTalk.Data;

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
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(navigationPropertyPath: c => c.Replies)
            .ToListAsync();
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
        return await _context.Categories.ToListAsync();
    }

    public async Task<List<Post>> GetAllPostsByUserIdAsync(string userId)
    {
        return await _context.Posts
            .Where(post => post.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<dynamic>> GetPostsByMonth(string userId)
    {

        List<dynamic>? result = await _context.Posts
            .Where(p => p.UserId == userId)
            .Select(p => new { p.CreatedAt })
            .ToListAsync<dynamic>();

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

        return groupedResult;
    }


}