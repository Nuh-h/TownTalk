using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownTalk.Data;
using TownTalk.Models;
using TownTalk.Services.Interfaces;

namespace TownTalk.Controllers
{
    [Authorize]
    public class ReactionsController : Controller
    {
        private readonly TownTalkDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public ReactionsController(TownTalkDbContext context, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // POST: Reactions/Create
        [HttpPost("Reactions/Create/")]
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

        // DELETE: Reactions/Delete
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
}
