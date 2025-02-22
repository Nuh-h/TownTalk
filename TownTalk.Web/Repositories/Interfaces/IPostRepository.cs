using TownTalk.Models;

namespace TownTalk.Repositories.Interfaces;
public interface IPostRepository
{
    Task<List<Post>> GetAllPostsAsync();
    Task<List<Post>> GetFilteredPostsAsync(string? q, string? cl, string? by, string? at, int page = 1, int pageSize = 20);
    Task<int> GetFilteredPostsCountAsync(string? q, string? cl, string? by, string? at);
    Task<Post> GetPostByIdAsync(int id, bool? includeReactions = false);
    Task AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(int id);
    Task<List<Category>> GetCategoriesAsync();
    Task<List<ApplicationUser>> GetAuthorsAsync();
    Task<List<string>> GetPublishedDatesAsync();
    Task<List<Post>> GetAllPostsByUserIdAsync(string userId);
    Task<List<dynamic>> GetPostsByMonth(string userId);
}
