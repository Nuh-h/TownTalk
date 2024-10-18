using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TownTalk.Models;

namespace TownTalk.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public string UserDisplayName { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }
        public ICollection<ReactionViewModel> Reactions { get; set; }

        public int LikeCount => Reactions.Count(r => r.Type == ReactionType.Like);
        public int LoveCount => Reactions.Count(r => r.Type == ReactionType.Love);
        public int SadCount => Reactions.Count(r => r.Type == ReactionType.Sad);

        public ReactionType? ActiveUserReaction { get; set; }

        // Simple method to set the active user's reaction
        public void SetActiveUserReaction(string userId)
        {
            ActiveUserReaction = Reactions.FirstOrDefault(r => r.UserId == userId)?.Type;
        }
    }
}
