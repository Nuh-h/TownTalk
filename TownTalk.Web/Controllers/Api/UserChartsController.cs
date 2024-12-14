using Microsoft.AspNetCore.Mvc;
using TownTalk.Data;
using TownTalk.Repositories.Interfaces;
using TownTalk.Services.Interfaces;

namespace TownTalk.Controllers.Api;

[Route("api/usercharts")]
[ApiController]
public class UserChartsController : ControllerBase
{
    private readonly IPostRepository _postRepository;
    private readonly IUserFollowService _userFollowService;

    public UserChartsController(IPostRepository postRepository, IUserFollowService userFollowService)
    {
        _postRepository = postRepository;
        _userFollowService = userFollowService;
    }

    // Get posts created by user over months
    [HttpGet("postsbymonth/{userId}")]
    public async Task<IActionResult> GetPostsByMonth(string userId)
    {
        var postsByMonth = await _postRepository.GetPostsByMonth(userId);
        return Ok(postsByMonth); // Return as JSON
    }

    // Get followers growth over time
    [HttpGet("followersgrowth/{userId}")]
    public async Task<IActionResult> GetFollowersGrowth(string userId)
    {
        var followersGrowth = await _userFollowService.GetFollowersGrowth(userId);
        return Ok(followersGrowth); // Return as JSON
    }

    public async Task<IActionResult> SeedData([FromServices] UserDataSeeder seeder)
    {
        await seeder.SeedDataAsync();
        return Ok("Data seeded successfully");
    }
}
