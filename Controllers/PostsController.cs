using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TownTalk.Models;

namespace TownTalk.Controllers;

[Authorize]
public class PostsController : Controller
{
    private readonly TownTalkDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostsController(TownTalkDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Posts/Create
    public IActionResult Create()
    {
        // Populate the dropdown for categories, ensuring that the current category is selected.
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");

        return View();
    }

    // POST: Posts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content,CategoryId")] Post post)
    {

        ApplicationUser? user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            post.UserId = user.Id;
            post.User = user;

            // Manually clear any ModelState errors for UserId if it's invalid after assigning
            ModelState.Remove("UserId");
            ModelState.Remove("User");

        }

        if (ModelState.IsValid)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));  // Redirect to the post list or details
        }

        // Populate the dropdown for categories, ensuring that the current category is selected.
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
        return View(post);
    }

    // GET: Posts/Index
    public async Task<IActionResult> Index()
    {
        List<Post>? posts = await _context.Posts
    .Include("User") // Includes the User for each Post
    .Include("Category") // Includes the Category for each Post
    .Include("Reactions") // Includes the Category for each Post
    .Include(navigationPropertyPath: "Comments.User") // Includes User for each Comment
    .Include("Comments.Replies") // Includes User for each Comment
    .ToListAsync();


        return View(posts);
    }

    // GET: Posts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.Category) // Optionally include the category for the current post
            .FirstOrDefaultAsync(p => p.Id == id);


        if (post == null)
        {
            return NotFound();
        }

        // Populate the dropdown for categories, ensuring that the current category is selected.
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);

        return View(post);
    }

    // POST: Posts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Post post)
    {
        if (id != post.Id)
        {
            return NotFound();
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(post.UserId);

        if (user != null)
        {
            post.UserId = user.Id;
            post.User = user;

            // Manually clear any ModelState errors for UserId if it's invalid after assigning
            ModelState.Remove("UserId");
            ModelState.Remove("User");

        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(post);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId); // Populate the dropdown again in case of validation errors

        return View(post);
    }


    // GET: Posts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // POST: Posts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PostExists(int id)
    {
        return _context.Posts.Any(e => e.Id == id);
    }
}
