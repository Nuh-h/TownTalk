namespace TownTalk.Web.ViewModels;

public class PaginationViewModel
{
    public int CurrentPage { get; set; } = 0;
    public int TotalPages { get; set; } = 0;
    public int PageSize { get; set; } = 20;
    public int TotalPosts { get; set; } = 0;
}
