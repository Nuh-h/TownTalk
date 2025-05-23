namespace TownTalk.Web.Repositories.Interfaces;

using TownTalk.Web.Models;

/// <summary>
/// Defines methods for managing posts, categories, and authors in the application.
/// </summary>
public interface IPostRepository
{
    /// <summary>
    /// Retrieves all posts asynchronously.
    /// </summary>
    /// <returns>A list of all posts.</returns>
    Task<List<Post>> GetAllPostsAsync();

    /// <summary>
    /// Retrieves filtered posts based on query parameters.
    /// </summary>
    /// <param name="q">Search query.</param>
    /// <param name="cl">Category filter.</param>
    /// <param name="by">Author filter.</param>
    /// <param name="at">Date filter.</param>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A list of filtered posts.</returns>
    Task<List<Post>> GetFilteredPostsAsync(string? q, string? cl, string? by, string? at, int page = 1, int pageSize = 20);

    /// <summary>
    /// Gets the count of filtered posts based on query parameters.
    /// </summary>
    /// <param name="q">Search query.</param>
    /// <param name="cl">Category filter.</param>
    /// <param name="by">Author filter.</param>
    /// <param name="at">Date filter.</param>
    /// <returns>The count of filtered posts.</returns>
    Task<int> GetFilteredPostsCountAsync(string? q, string? cl, string? by, string? at);

    /// <summary>
    /// Retrieves a post by its ID.
    /// </summary>
    /// <param name="id">The post ID.</param>
    /// <param name="includeReactions">Whether to include reactions.</param>
    /// <returns>The post with the specified ID.</returns>
    Task<Post> GetPostByIdAsync(int id, bool? includeReactions = false);

    /// <summary>
    /// Adds a new post asynchronously.
    /// </summary>
    /// <param name="post">The post to add.</param>
    Task AddPostAsync(Post post);

    /// <summary>
    /// Updates an existing post asynchronously.
    /// </summary>
    /// <param name="post">The post to update.</param>
    Task UpdatePostAsync(Post post);

    /// <summary>
    /// Deletes a post by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the post to delete.</param>
    Task DeletePostAsync(int id);

    /// <summary>
    /// Retrieves all categories asynchronously.
    /// </summary>
    /// <returns>A list of categories.</returns>
    Task<List<Category>> GetCategoriesAsync();

    /// <summary>
    /// Retrieves all authors asynchronously.
    /// </summary>
    /// <returns>A list of authors.</returns>
    Task<List<ApplicationUser>> GetAuthorsAsync();

    /// <summary>
    /// Retrieves all published dates asynchronously.
    /// </summary>
    /// <returns>A list of published dates as strings.</returns>
    Task<List<string>> GetPublishedDatesAsync();

    /// <summary>
    /// Retrieves all posts by a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A list of posts by the user.</returns>
    Task<List<Post>> GetAllPostsByUserIdAsync(string userId);

    /// <summary>
    /// Retrieves posts grouped by month for a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A list of posts grouped by month.</returns>
    Task<List<dynamic>> GetPostsByMonth(string userId);
}
