namespace TownTalk.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Services.Interfaces;
using TownTalk.Web.Models;

/// <summary>
/// Controller for managing reactions to posts, including creating, updating, and deleting reactions.
/// </summary>
[Authorize]
public class ReactionsController : Controller
{
    private readonly TownTalkDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactionsController"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="userManager">The user manager for handling user-related operations.</param>
    /// <param name="notificationService">The notification service for sending notifications.</param>
    public ReactionsController(TownTalkDbContext context, UserManager<ApplicationUser> userManager, INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Creates or updates a reaction for a post by the current user.
    /// </summary>
    /// <param name="reaction">The reaction to create or update.</param>
    /// <returns>A JSON result indicating success or failure.</returns>
    [HttpPost("Reactions/Create")]
    public async Task<IActionResult> Create([FromBody] Reaction reaction)
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        Post? post = await _context.Posts.FindAsync(reaction.PostId);

        if (user != null)
        {
            reaction.UserId = user.Id;
            reaction.User = user;
            ModelState.Remove(key: "User");
            ModelState.Remove("UserId");
        }

        if (post != null)
        {
            reaction.Post = post;
            ModelState.Remove(key: "Post");
            ModelState.Remove(key: "PostId");
        }

        reaction.CreatedAt = DateTime.Now;

        if (ModelState.IsValid)
        {
            Reaction? existingReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.PostId == reaction.PostId && r.UserId == user.Id);

            if (existingReaction != null && existingReaction.Type == reaction.Type)
            {
                _context.Reactions.Remove(existingReaction);
            }
            else if (existingReaction != null)
            {
                existingReaction.Type = reaction.Type;
                _context.Reactions.Update(existingReaction);
            }
            else
            {
                _context.Reactions.Add(reaction);
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"{reaction.PostId} - {reaction.UserId} - {reaction.Post.UserId}");

            await _notificationService.NotifyReactionAsync(reaction.PostId.ToString(), reactorId: reaction.UserId, originalPosterId: post.UserId);

            return Json(new
            {
                success = true,
                id = reaction.Id,
                postId = reaction.PostId,
                type = reaction.Type,
                createdAt = reaction.CreatedAt
            });
        }

        return Json(new { success = false, errors = ModelState });
    }

    /// <summary>
    /// Deletes a reaction for a post by the current user.
    /// </summary>
    /// <param name="reaction">The reaction to delete.</param>
    /// <returns>A JSON result indicating success or failure.</returns>
    [HttpDelete(template: "Reactions/Delete")]
    public async Task<IActionResult> Delete([FromBody] Reaction reaction)
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);

        if (user == null || reaction == null || reaction.PostId == null || reaction.Type == null)
        {
            return Json(new { success = false, message = "Invalid request." });
        }

        Reaction? existingReaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.PostId == reaction.PostId && r.UserId == user.Id && r.Type == reaction.Type);

        if (existingReaction != null)
        {
            string? originalPosterId = (await _context.Posts.FindAsync(reaction.PostId))?.UserId;

            _context.Reactions.Remove(existingReaction);
            await _context.SaveChangesAsync();

            var existingNotification = await _context.Notifications
       .FirstOrDefaultAsync(n => n.UserId == user.Id && n.PostId == reaction.PostId && n.Type == reaction.Type.ToString() && !n.IsRead);

            if (existingNotification != null)
            {
                // Remove the existing unread notification
                _context.Notifications.Remove(existingNotification);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, id = existingReaction.Id });
        }

        return Json(new { success = false, message = "Reaction not found or you do not have permission to delete this reaction." });
    }

}
