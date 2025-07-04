namespace TownTalk.Web.Controllers.Api;

using Microsoft.AspNetCore.Mvc;
using TownTalk.Web.Data;
using TownTalk.Web.Repositories.Interfaces;
using TownTalk.Web.Services.Interfaces;

/// <summary>
/// API controller for user-related operations.
/// </summary>
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IUserFollowService _userFollowService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="postService">The post service.</param>
    /// <param name="userFollowService">The user follow service.</param>
    public UsersController(IPostService postService, IUserFollowService userFollowService)
    {
        _postService = postService;
        _userFollowService = userFollowService;
    }

    /// <summary>
    /// Gets the user's activity grouped by month.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of user activities by month.</returns>
    [HttpGet("activitiesbymonth/{userId}")]
    public async Task<IActionResult> GetUserActivityByMonth(string userId)
    {
        List<Models.UserActivityByMonth>? userActivity = await _postService.GetUserActivityByMonthAsync(userId);
        return Ok(userActivity);
    }

    /// <summary>
    /// Gets the growth of followers for a user over time.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>A JSON object containing followers growth data.</returns>
    [HttpGet("followersgrowth/{userId}")]
    public async Task<IActionResult> GetFollowersGrowth(string userId)
    {
        List<dynamic>? followersGrowth = await _userFollowService.GetFollowersGrowthAsync(userId);
        return Ok(followersGrowth);
    }

    /// <summary>
    /// Gets the follow trends for a user grouped by month.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of follow trends by month.</returns>
    [HttpGet("followtrendsbymonth/{userId}")]
    public async Task<IActionResult> GetFollowTrendsByMonth(string userId)
    {
        var trends = await _userFollowService.GetFollowTrendsByMonthAsync(userId);
        return Ok(trends);
    }

    /// <summary>
    /// Seeds user data using the provided seeder service.
    /// </summary>
    /// <param name="seeder">The user data seeder service.</param>
    /// <returns>Action result indicating success.</returns>
    public async Task<IActionResult> SeedData([FromServices] UserDataSeeder seeder)
    {
        await seeder.SeedDataAsync();
        return Ok("Data seeded successfully");
    }
}
