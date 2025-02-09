using TownTalk.Models;

namespace TownTalk.Repositories.Interfaces;
public interface IPostRepository
{
    Task<List<Post>> GetAllPostsAsync();
    Task<List<Post>> GetFilteredPostsAsync(string? q, string? cl, string? by, string? at);
    Task<Post> GetPostByIdAsync(int id, bool? includeReactions = false);
    Task AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(int id);
    Task<List<Category>> GetCategoriesAsync();
    Task<List<Post>> GetAllPostsByUserIdAsync(string userId);
    Task<List<dynamic>> GetPostsByMonth(string userId);
}
