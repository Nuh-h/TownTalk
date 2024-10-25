using Microsoft.EntityFrameworkCore;
using TownTalk.Models;

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
                .ThenInclude(c => c.Replies)
            .ToListAsync();
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post> GetPostByIdAsync(int id, bool includeReactions = false)
    {
        IQueryable<Post> query = _context.Posts.Include(p => p.Category);

        if (includeReactions)
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
}
