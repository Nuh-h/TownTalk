namespace TownTalk.Web.Models;

/// <summary>
/// Represents filter options for TownTalk web application.
/// </summary>
public class Filters
{
    /// <summary>
    /// Gets or sets the list of available dates as strings.
    /// </summary>
    public List<string> AvailableDates { get; set; }

    /// <summary>
    /// Gets or sets the list of authors.
    /// </summary>
    public List<ApplicationUser> Authors { get; set; }

    /// <summary>
    /// Gets or sets the list of categories.
    /// </summary>
    public List<Category> Categories { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Filters"/> class.
    /// </summary>
    public Filters()
    {
        AvailableDates = new List<string>();
        Authors = new List<ApplicationUser>();
        Categories = new List<Category>();
    }
}
