using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class FeedController: BaseApiController {
    private readonly IUserService _userService;
    private readonly IFeedService _feedService;
    public FeedController(IUserService userService, IFeedService feedService) {
        _userService = userService;
        _feedService = feedService;
    }

    [HttpPost("")]
    public async Task<ActionResult<FeedResultDto>> AddFeed (CreateFeedDto data) {
         int userId = _userService.GetConnectedUser(User);
         AppFeed? feed = await _feedService.CreateNewFeed(data, userId);

         if(feed is null) return BadRequest("Couldn't create the feed. please retry later");

         return Ok(feed);
    }
}