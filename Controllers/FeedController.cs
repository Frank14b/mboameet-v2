using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Policy = "IsUser")]
[Route("api/v1/feeds")]
public class FeedController: BaseApiController {
    private readonly IUserService _userService;
    private readonly IFeedService _feedService;
    private readonly IMapper _mapper;

    public FeedController(IUserService userService, IFeedService feedService, IMapper mapper) {
        _userService = userService;
        _feedService = feedService;
        _mapper = mapper;
    }

    [HttpPost("")]
    public async Task<ActionResult<FeedResultDto>> AddFeed ([FromForm] CreateFeedDto data) {
         int userId = _userService.GetConnectedUser(User);

         FeedResultDto? feed = await _feedService.CreateNewFeed(data, userId);

         if(feed is null) return BadRequest("Couldn't create the feed. please retry later");

         return feed;
    }

    [HttpGet("")]
    public async Task<ActionResult<ResultPaginate<FeedResultDto>>> GetFeeds (int skip = 0, int limit = 10, string sort = "desc") {
         int userId = _userService.GetConnectedUser(User);

         ResultPaginate<FeedResultDto>? feeds = await _feedService.GetAllFeeds(userId, skip, limit, sort);

         if(feeds is null) return BadRequest("An Error occured. Can't get feeds");

         return feeds;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> DeleteFeed (int id) {
         int userId = _userService.GetConnectedUser(User);

         BooleanReturnDto? result = await _feedService.DeleteFeed(id, userId);

         if(result is null) return BadRequest("An Error occured. Can't delete the feed");
         if(result.Status == false) return NotFound(result.Message);

         return result;
    }
}