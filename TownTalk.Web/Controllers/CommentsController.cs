namespace TownTalk.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Models;
using TownTalk.Web.Services.Interfaces;


/// <summary>
/// Controller for handling comment creation and deletion.
/// </summary>
[Authorize]
public class CommentsController : Controller
{
    private readonly TownTalkDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentsController"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="userManager">The user manager for handling user-related operations.</param>
    /// <param name="notificationService">The notification service for sending notifications.</param>
    public CommentsController(TownTalkDbContext context, UserManager<ApplicationUser> userManager, INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Creates a new comment for a post or as a reply to another comment.
    /// </summary>
    /// <param name="comment">The comment to create, bound from the request.</param>
    /// <returns>A JSON result with the created comment data or validation errors.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([Bind("PostId,ParentCommentId,Content")] Comment comment)
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        Post? post = await _context.Posts.FindAsync(comment.PostId);

        if (user != null)
        {
            comment.UserId = user.Id;
            comment.User = user;
            ModelState.Remove("UserId");
            ModelState.Remove("User");
        }

        if (post != null)
        {
            comment.Post = post;
            ModelState.Remove("Post");
        }

        if (ModelState.IsValid)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            await _notificationService.NotifyCommentAsync(comment.PostId.ToString(), comment.UserId, comment.Post.UserId);

            // Return JSON with the newly created comment data
            return Json(new
            {
                success = true,
                id = comment.Id,
                content = comment.Content,
                userDisplayName = comment.User.DisplayName,
                createdAt = comment.CreatedAt.ToString("g"),
                comment.ParentCommentId,
                comment.PostId,
            });
        }

        return Json(new { success = false, errors = ModelState });
    }

    /// <summary>
    /// Deletes a comment and its replies by comment ID.
    /// </summary>
    /// <param name="id">The ID of the comment to delete.</param>
    /// <returns>A JSON result indicating success or failure.</returns>
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment != null)
        {
            // Delete all replies first
            _context.Comments.RemoveRange(comment.Replies);
            // Then delete the comment itself
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id = comment.Id });
        }

        return Json(new { success = false });
    }
}

