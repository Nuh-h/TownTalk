using TownTalk.Web.Models;

namespace TownTalk.Web.Services.Interfaces;
public interface IGraphService
{
    Task<int> GetDegreeOfSeparation(string userId1, string userId2);
    Task<List<ApplicationUser>> FindConnectionPath(string userId1, string userId2);
}
