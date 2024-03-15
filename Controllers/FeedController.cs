using API.DTOs;
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
    private readonly IFeedCommentService _feedCommentService;
    private readonly IMapper _mapper;

    public FeedController(
     IUserService userService, 
     IFeedService feedService, 
     IMapper mapper,
     IFeedCommentService feedCommentService
   ) {
        _userService = userService;
        _feedService = feedService;
        _mapper = mapper;
        _feedCommentService = feedCommentService;
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

    [HttpPut("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> UpdateFeed (int id, UpdateFeedDto data) {
         int userId = _userService.GetConnectedUser(User);

         BooleanReturnDto? result = await _feedService.UpdateFeed(id, userId, data);

         if(result is null) return BadRequest("An Error occured. Can't update the feed");
         if(result.Status == false) return NotFound(result.Message);

         return result;
    }

    [HttpPost("{id}/like")]
    public async Task<ActionResult<BooleanReturnDto>> AddFeedLike (int id) {
         int userId = _userService.GetConnectedUser(User);

         BooleanReturnDto? feed = await _feedService.AddFeedLikes(id, userId);

         if(feed is null) return BadRequest("Couldn't create the feed like. please retry later");

         return feed;
    }

    [HttpDelete("{id}/like")]
    public async Task<ActionResult<BooleanReturnDto>> DeleteFeedLike (int id) {
         int userId = _userService.GetConnectedUser(User);

         BooleanReturnDto? feed = await _feedService.RemoveFeedLikes(id, userId);

         if(feed is null) return BadRequest("Couldn't remove the feed like. please retry later");

         return feed;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> DeleteFeed (int id) {
         int userId = _userService.GetConnectedUser(User);

         bool validFeedId = await _feedService.IsValidFeedId(id);
         if(!validFeedId) return NotFound("Invalid Feed Id");

         BooleanReturnDto? result = await _feedService.DeleteFeed(id, userId);

         if(result is null) return BadRequest("An Error occured. Can't delete the feed");
         if(result.Status == false) return NotFound(result.Message);

         return result;
    }

    [HttpGet("{id}/comments")]
    public async Task<ActionResult<ResultPaginate<ResultFeedCommentDto>>> GetFeedComments (int id, int skip=0, int limit=10) {

         ResultPaginate<ResultFeedCommentDto> comments = await _feedCommentService.GetFeedComments(id, skip, limit);

         return comments;
    }

    [HttpPost("{id}/comments")]
    public async Task<ActionResult<ResultFeedCommentDto>> CreateComment (CreateCommentDto data, int id) {
       int userId = _userService.GetConnectedUser(User);
       ResultFeedCommentDto? comment = await _feedCommentService.CreateComment(data, id, userId);

       if(comment is null) return BadRequest("An Error occured. Can't create the feed comment");

       return comment;
    }
}