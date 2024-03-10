using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Policy = "IsUser")]
[Route("api/v1/feeds")]
public class FeedController: BaseApiController {
    private readonly IUserService _userService;
    private readonly IFeedService _feedService;
    public FeedController(IUserService userService, IFeedService feedService) {
        _userService = userService;
        _feedService = feedService;
    }

    [HttpPost("")]
    public async Task<ActionResult<FeedResultDto>> AddFeed ([FromForm] CreateFeedDto data) {
         int userId = _userService.GetConnectedUser(User);

         Feed? feed = await _feedService.CreateNewFeed(data, userId);

         if(feed is null) return BadRequest("Couldn't create the feed. please retry later");

         return Ok(feed);
    }

    [HttpGet("")]
    public async Task<ActionResult<FeedResultDto>> GetFeeds (int skip = 0, int limit = 10, string sort = "desc") {
         int userId = _userService.GetConnectedUser(User);

         Feed? feed = null; // await _feedService.CreateNewFeed(data, userId);

         if(feed is null) return BadRequest("Couldn't create the feed. please retry later");

         return Ok(feed);
    }
}