
using TownTalk.Models;

namespace TownTalk.ViewModels
{

    public class ReactionViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public ReactionType Type { get; set; }

        public int Count { get; set; }

        public bool IsUserOwner { get; set; }

        public string GetEmoji()
        {
            return Type switch
            {
                ReactionType.Like => "😊",
                ReactionType.Love => "❤️",
                ReactionType.Sad => "😢",
                _ => ""
            };
        }
    }
}
