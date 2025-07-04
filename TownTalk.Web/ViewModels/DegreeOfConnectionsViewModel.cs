
namespace TownTalk.Web.ViewModels;
using TownTalk.Web.Models;

public class DegreeOfConnectionsViewModel
{
    public string DegreeOfConnectionsLabel { get; set; } = "Degree of Connections";
    public string User1Label { get; set; } = "User 1";
    public string User2Label { get; set; } = "User 2";
    public string SelectUserLabel { get; set; } = "Select user";
    public string FindDegreeOfConnectionsLabel { get; set; } = "Find Degree of Connections";
    public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}