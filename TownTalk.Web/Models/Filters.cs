namespace TownTalk.Web.Models;

public class Filters
{
    public List<string> AvailableDates { get; set; }
    public List<ApplicationUser> Authors { get; set; }
    public List<Category> Categories { get; set; }

    public Filters()
    {
        AvailableDates = new List<string>();
        Authors = new List<ApplicationUser>();
        Categories = new List<Category>();
    }
}
