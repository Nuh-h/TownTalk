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
    private readonly IPostRepository _postRepository;
    private readonly IUserFollowService _userFollowService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="postRepository">The post repository.</param>
    /// <param name="userFollowService">The user follow service.</param>
    public UsersController(IPostRepository postRepository, IUserFollowService userFollowService)
    {
        _postRepository = postRepository;
        _userFollowService = userFollowService;
    }

    /// <summary>
    /// Gets the number of posts created by a user, grouped by month.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>A JSON object containing posts by month.</returns>
    [HttpGet("postsbymonth/{userId}")]
    public async Task<IActionResult> GetPostsByMonth(string userId)
    {
        var postsByMonth = await _postRepository.GetPostsByMonth(userId);
        return Ok(postsByMonth); // Return as JSON
    }

    /// <summary>
    /// Gets the growth of followers for a user over time.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>A JSON object containing followers growth data.</returns>
    [HttpGet("followersgrowth/{userId}")]
    public async Task<IActionResult> GetFollowersGrowth(string userId)
    {
        var followersGrowth = await _userFollowService.GetFollowersGrowth(userId);
        return Ok(followersGrowth); // Return as JSON
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
