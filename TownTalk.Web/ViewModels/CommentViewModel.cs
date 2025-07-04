namespace TownTalk.Web.ViewModels;
using TownTalk.Web.Models;

public class CommentViewModel
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string UserDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CommentViewModel> Replies { get; set; }
    public int PostId { get; set; }
    public bool IsUserOwner { get; set; }
    public int? ParentCommentId { get; set; }

    public CommentViewModel(Comment comment, string? userId = "")
    {
        Id = comment.Id;
        Content = comment.Content;
        UserDisplayName = comment.User.DisplayName;
        CreatedAt = comment.CreatedAt;
        PostId = comment.PostId;
        Replies = comment.Replies.Select(r => new CommentViewModel(r, userId)).ToList();
        IsUserOwner = comment.UserId == userId;
        ParentCommentId = comment.ParentCommentId ?? null;
    }

}
