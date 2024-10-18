using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TownTalk.Models;

namespace TownTalk.ViewModels
{

    public class ReactionViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ReactionType Type { get; set; }
    }
}
