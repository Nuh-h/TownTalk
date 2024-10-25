using TownTalk.Models;

namespace TownTalk.Repositories;
public interface IPostRepository
{
    Task<List<Post>> GetAllPostsAsync();
    Task<Post> GetPostByIdAsync(int id);
    Task<Post> GetPostByIdAsync(int id, bool includeReactions);
    Task AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(int id);
    Task<List<Category>> GetCategoriesAsync();

}
