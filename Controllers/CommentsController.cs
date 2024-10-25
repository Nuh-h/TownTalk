using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownTalk.Models;

namespace TownTalk.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly TownTalkDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(TownTalkDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Comments/Create
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

                // Return JSON with the newly created comment data
                return Json(new
                {
                    success = true,
                    id = comment.Id,
                    content = comment.Content,
                    userDisplayName = comment.User.DisplayName,
                    createdAt = comment.CreatedAt.ToString("g"),
                    ParentCommentId = comment.ParentCommentId,
                    PostId = comment.PostId
                });
            }

            return Json(new { success = false, errors = ModelState });
        }

        // POST: Comments/Delete/5
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
}
