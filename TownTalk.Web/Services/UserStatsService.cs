namespace TownTalk.Web.Services;

using System.Threading.Tasks;
using TownTalk.Web.Services.Interfaces;

/// <summary>
/// Provides user statistics such as post count, comment count, followers, following, unread notifications, and most popular post.
/// </summary>
/// <summary>
/// Service for retrieving user statistics.
/// </summary>
public class UserStatsService : IUserStatsService
{
    /// <summary>
    /// Service for post-related operations.
    /// </summary>
    protected readonly IPostService _postService;

    /// <summary>
    /// Service for comment-related operations.
    /// </summary>
    protected readonly ICommentService _commentService;

    /// <summary>
    /// Service for user follow operations.
    /// </summary>
    protected readonly IUserFollowService _userFollowService;

    /// <summary>
    /// Service for notification-related operations.
    /// </summary>
    protected readonly INotificationService _notificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserStatsService"/> class.
    /// </summary>
    /// <param name="postService">The post service.</param>
    /// <param name="commentService">The comment service.</param>
    /// <param name="userFollowService">The user follow service.</param>
    /// <param name="notificationService">The notification service.</param>
    public UserStatsService(
        IPostService postService,
        ICommentService commentService,
        IUserFollowService userFollowService,
        INotificationService notificationService)
    {
        _postService = postService;
        _commentService = commentService;
        _userFollowService = userFollowService;
        _notificationService = notificationService;
    }

    /// <inheritdoc/>
    public virtual Task<int> GetPostCountAsync(string userId)
        => _postService.GetPostCountAsync(userId);

    /// <inheritdoc/>
    public virtual Task<int> GetCommentCountAsync(string userId)
        => _commentService.GetUserCommentCountAsync(userId);

    /// <inheritdoc/>
    public virtual Task<int> GetFollowersCountAsync(string userId)
        => _userFollowService.GetFollowerCountAsync(userId);

    /// <inheritdoc/>
    public virtual Task<int> GetFollowingCountAsync(string userId)
        => _userFollowService.GetFollowingCountAsync(userId);

    /// <inheritdoc/>
    public virtual Task<int> GetUnreadNotificationsCountAsync(string userId)
        => _notificationService.GetUnreadNotificationsCountAsync(userId);

    /// <inheritdoc/>
    public virtual Task<(string? Title, int ReactionCount)> GetMostPopularPostAsync(string userId)
        => _postService.GetMostPopularPostAsync(userId);
}
