namespace TownTalk.Web.ViewModels;

using System.ComponentModel.DataAnnotations;
using TownTalk.Web.Models;

public class PostViewModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = "";

    [Required]
    public string Content { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public string UserId { get; set; }
    public string UserDisplayName { get; set; }

    public int? CategoryId { get; set; }
    public string CategoryName { get; set; }

    public ICollection<CommentViewModel> Comments { get; set; }
    public ICollection<ReactionViewModel> Reactions { get; set; }

    public List<ReactionType> AvailableReactions => new List<ReactionType>
        {
            ReactionType.Like,
            ReactionType.Love,
            ReactionType.Haha,
            ReactionType.Wow,
            ReactionType.HeartEyes,
            ReactionType.Sad,
            ReactionType.Angry,
            ReactionType.Cool,
            ReactionType.Clap
        };

    public ReactionType? ActiveUserReaction { get; set; }
    public bool IsUserOwner { get; set; }

    public int CommentCount => Comments?.Count ?? 0;

    public void SetActiveUserReaction(string userId)
    {
        ActiveUserReaction = Reactions.FirstOrDefault(r => r.UserId == userId)?.Type;
    }

    public PostViewModel(Post post, string? userId = "")
    {

        if (post != null)
        {
            Id = post.Id;
            Title = post.Title;
            Content = post.Content;
            CreatedAt = post.CreatedAt;
            UserId = post.UserId;
            UserDisplayName = post.User?.DisplayName ?? "";
            CategoryId = post.CategoryId;
            CategoryName = post.Category?.Name ?? "";
            Comments = post.Comments.Select(c => new CommentViewModel(c, userId)).ToList() ?? new List<CommentViewModel>(); ;
            Reactions = post.Reactions.Select(r => new ReactionViewModel
            {
                Id = r.Id,
                Type = r.Type,
                UserId = r.UserId,
                Count = post.Reactions.Count(re => re.Type == r.Type)
            }).ToList() ?? new List<ReactionViewModel>();
        }

        if (!string.IsNullOrEmpty(userId))
        {
            SetActiveUserReaction(userId);
            IsUserOwner = post?.UserId == userId; // Set owner status
        }
    }
}