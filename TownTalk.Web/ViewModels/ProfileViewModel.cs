namespace TownTalk.Web.ViewModels
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
        public bool IsFollowing { get; set; }

        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int MutualFollowersCount { get; set; }
        public int PostsCount { get; set; }
        public int CommentsCount { get; set; }

        public string? Bio { get; set; }
        public string? Location { get; set; }
        public DateTime? LastActive { get; set; }
    }
}
