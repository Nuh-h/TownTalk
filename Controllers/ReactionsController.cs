using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownTalk.Models;

namespace TownTalk.Controllers
{
    [Authorize]
    public class ReactionsController : Controller
    {
        private readonly TownTalkDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReactionsController(TownTalkDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Reactions/Create
        [HttpPost]
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

            if (ModelState.IsValid)
            {
                // Check if the user already reacted to this post
                var existingReaction = await _context.Reactions
                    .FirstOrDefaultAsync(r => r.PostId == reaction.PostId && r.UserId == user.Id);

                if (existingReaction != null)
                {
                    // If exists, update the reaction type
                    existingReaction.Type = reaction.Type;
                    _context.Reactions.Update(existingReaction);
                }
                else
                {
                    // Otherwise, add new reaction
                    _context.Reactions.Add(reaction);
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    id = reaction.Id,
                    postId = reaction.PostId,
                    type = reaction.Type
                });
            }

            return Json(new { success = false, errors = ModelState }); // Return errors if invalid
        }

        // POST: Reactions/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var reaction = await _context.Reactions.FindAsync(id);

            if (reaction != null)
            {
                // Check if the user is the owner of the reaction
                var user = await _userManager.GetUserAsync(User);
                if (reaction.UserId == user.Id)
                {
                    _context.Reactions.Remove(reaction);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, id = reaction.Id }); // Return JSON on success
                }
                return Json(new { success = false, message = "You do not have permission to delete this reaction." });
            }

            return Json(new { success = false, message = "Reaction not found." }); // Return failure if reaction not found
        }
    }
}
