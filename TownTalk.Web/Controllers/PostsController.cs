using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using TownTalk.Models;
using Microsoft.EntityFrameworkCore;
using TownTalk.ViewModels;
using TownTalk.Repositories.Interfaces;

[Authorize]
public class PostsController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostsController(IPostRepository postRepository, UserManager<ApplicationUser> userManager)
    {
        _postRepository = postRepository;
        _userManager = userManager;
    }

    // GET: Posts/Create
    public async Task<IActionResult> Create()
    {
        ViewData["CategoryId"] = new SelectList(await _postRepository.GetCategoriesAsync(), "Id", "Name");
        return View();
    }

    // POST: Posts/Create
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

    // GET: Posts/Index
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? q, string? cl, string? by, string? at)
    {

        ViewBag.Query = q;
        ViewBag.Category = cl;
        ViewBag.Author = by;
        ViewBag.Date = at;

        List<Post>? posts = await _postRepository.GetFilteredPostsAsync(q, cl, by, at);

        if (string.IsNullOrWhiteSpace(q) && string.IsNullOrWhiteSpace(cl) && string.IsNullOrWhiteSpace(by) && string.IsNullOrWhiteSpace(at))
        {
            posts = await _postRepository.GetAllPostsAsync();
        }
        return View(posts);
    }

    // GET: Posts/Edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var post = await _postRepository.GetPostByIdAsync(id: id.Value);
        if (post == null) return NotFound();

        ViewData["CategoryId"] = new SelectList(await _postRepository.GetCategoriesAsync(), "Id", "Name", post.CategoryId);
        return View(post);
    }

    // POST: Posts/Edit/{id}
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

    // GET: Posts/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        Post? post = await _postRepository.GetPostByIdAsync(id: id.Value);
        if (post == null) return NotFound();

        return View(post);
    }

    // POST: Posts/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _postRepository.DeletePostAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> PostExists(int id)
    {
        Post? post = await _postRepository.GetPostByIdAsync(id: id);
        return post != null;
    }

    //Reactions
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

}