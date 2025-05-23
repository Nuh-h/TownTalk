namespace TownTalk.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TownTalk.Web.ViewModels;
using TownTalk.Web.Models;
using TownTalk.Web.Repositories.Interfaces;

/// <summary>
/// Controller for managing posts, including creation, editing, deletion, and displaying posts.
/// </summary>
[Authorize]
public class PostsController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostsController"/> class.
    /// </summary>
    /// <param name="postRepository">The repository for managing posts.</param>
    /// <param name="userManager">The user manager for handling user-related operations.</param>
    public PostsController(IPostRepository postRepository, UserManager<ApplicationUser> userManager)
    {
        _postRepository = postRepository;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays the form for creating a new post.
    /// </summary>
    /// <returns>The view for creating a new post.</returns>
    public async Task<IActionResult> Create()
    {
        ViewData["CategoryId"] = new SelectList(await _postRepository.GetCategoriesAsync(), "Id", "Name");
        return View();
    }

    /// <summary>
    /// Handles the POST request for creating a new post.
    /// </summary>
    /// <param name="post">The post to create, bound from the form.</param>
    /// <returns>Redirects to the index page if successful; otherwise, returns the view with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content,CategoryId")] Post post)
    {
        ApplicationUser user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            post.UserId = user.Id;
            post.User = user;
            ModelState.Remove("UserId");
            ModelState.Remove("User");
        }

        if (ModelState.IsValid)
        {
            await _postRepository.AddPostAsync(post);
            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(await _postRepository.GetCategoriesAsync(), "Id", "Name", post.CategoryId);
        return View(post);
    }

    /// <summary>
    /// Displays a paginated list of posts with optional filtering by query, category, author, and date.
    /// </summary>
    /// <param name="q">Search query string.</param>
    /// <param name="cl">Category filter.</param>
    /// <param name="by">Author filter.</param>
    /// <param name="at">Date filter.</param>
    /// <param name="page">Current page number.</param>
    /// <param name="pageSize">Number of posts per page.</param>
    /// <returns>The view with the filtered and paginated list of posts.</returns>
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? q, string? cl, string? by, string? at, int page = 1, int pageSize = 20)
    {

        List<Post> posts = await _postRepository.GetFilteredPostsAsync(q, cl, by, at, page, pageSize);

        int totalPosts = await _postRepository.GetFilteredPostsCountAsync(q, cl, by, at);
        int totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

        List<string>? dates = await _postRepository.GetPublishedDatesAsync();
        List<ApplicationUser>? authors = await _postRepository.GetAuthorsAsync();
        List<Category>? categories = await _postRepository.GetCategoriesAsync();

        Filters? filters = new Filters()
        {
            Authors = authors,
            AvailableDates = dates,
            Categories = categories
        };

        PostsLandingViewModel? viewModel = new PostsLandingViewModel()
        {
            Posts = posts,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalPosts = totalPosts,
            Query = q,
            Category = cl,
            Author = by,
            Date = at,
            Filters = filters
        };

        return View(viewModel);
    }


    /// <summary>
    /// Returns a partial view with filtered and paginated posts for AJAX requests.
    /// </summary>
    /// <param name="q">Search query string.</param>
    /// <param name="cl">Category filter.</param>
    /// <param name="by">Author filter.</param>
    /// <param name="at">Date filter.</param>
    /// <param name="page">Current page number.</param>
    /// <param name="pageSize">Number of posts per page.</param>
    /// <returns>A partial view containing the filtered and paginated posts.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPosts(string? q, string? cl, string? by, string? at, int page = 1, int pageSize = 20)
    {
        // Get filtered posts
        List<Post> posts = await _postRepository.GetFilteredPostsAsync(q, cl, by, at, page, pageSize);

        // Calculate total pages for pagination
        int totalPosts = await _postRepository.GetFilteredPostsCountAsync(q, cl, by, at);
        int totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

        SearchResultsViewModel? viewModel = new SearchResultsViewModel
        {
            Posts = posts,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalPosts = totalPosts,
            Query = q,
            Category = cl,
            Author = by,
            Date = at
        };

        return PartialView("_SearchResults", viewModel); ;
    }


    /// <summary>
    /// Displays the edit form for a specific post.
    /// </summary>
    /// <param name="id">The ID of the post to edit.</param>
    /// <returns>The edit view for the specified post, or NotFound if not found.</returns>
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var post = await _postRepository.GetPostByIdAsync(id: id.Value);
        if (post == null) return NotFound();

        ViewData["CategoryId"] = new SelectList(await _postRepository.GetCategoriesAsync(), "Id", "Name", post.CategoryId);
        return View(post);
    }


    /// <summary>
    /// Handles the POST request for editing an existing post.
    /// </summary>
    /// <param name="id">The ID of the post to edit.</param>
    /// <param name="post">The updated post object.</param>
    /// <returns>Redirects to the index page if successful; otherwise, returns the view with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Post post)
    {
        if (id != post.Id) return NotFound();

        ApplicationUser user = await _userManager.FindByIdAsync(post.UserId);
        if (user != null)
        {
            post.UserId = user.Id;
            post.User = user;
            ModelState.Remove("UserId");
            ModelState.Remove("User");
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _postRepository.UpdatePostAsync(post);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PostExists(post.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(await _postRepository.GetCategoriesAsync(), "Id", "Name", post.CategoryId);
        return View(post);
    }

    /// <summary>
    /// Displays the delete confirmation view for a specific post.
    /// </summary>
    /// <param name="id">The ID of the post to delete.</param>
    /// <returns>The delete confirmation view for the specified post, or NotFound if not found.</returns>
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        Post? post = await _postRepository.GetPostByIdAsync(id: id.Value);
        if (post == null) return NotFound();

        return View(post);
    }

    /// <summary>
    /// Handles the POST request to confirm deletion of a post.
    /// </summary>
    /// <param name="id">The ID of the post to delete.</param>
    /// <returns>Redirects to the index page after deletion.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _postRepository.DeletePostAsync(id);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Retrieves the reactions for a specific post and returns a partial view.
    /// </summary>
    /// <param name="id">The ID of the post for which to get reactions.</param>
    /// <returns>A partial view displaying the reactions for the specified post.</returns>
    public async Task<IActionResult> GetReactions(int id)
    {
        ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
        string currentUserId = currentUser?.Id ?? string.Empty;

        Post? post = await _postRepository.GetPostByIdAsync(id, includeReactions: true);

        if (post == null)
        {
            return NotFound();
        }

        PostViewModel? postViewModel = new PostViewModel(post, currentUserId);

        return PartialView("_Reactions", postViewModel);
    }

    private async Task<bool> PostExists(int id)
    {
        Post? post = await _postRepository.GetPostByIdAsync(id: id);
        return post != null;
    }
}