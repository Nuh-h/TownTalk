using System.ComponentModel.DataAnnotations;
using TownTalk.Models;

namespace TownTalk.ViewModels;

public class SearchResultsViewModel
{
    public required List<Post> Posts;
    public int CurrentPage;
    public int TotalPages;
    public int TotalPosts;
    public string? Query;
    public string? Category;

    public string? Author;
    public string? Date;
}
