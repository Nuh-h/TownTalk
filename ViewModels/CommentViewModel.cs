using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TownTalk.Models;

namespace TownTalk.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
