using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[Authorize(Policy = "IsUser")]
[Route("/api/v1/matches")]
public class MatchController : BaseApiController
{
    private readonly IMatchService _matchService;
    private readonly IUserService _userService;
    private readonly ILogger<MatchController> _logger;

    public MatchController(
        IMatchService matchService,
        IUserService userService,
        ILogger<MatchController> logger)
    {
        _matchService = matchService;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("")]
    public async Task<ActionResult<MatchesResultDto>> AddMatch(AddMatchDto data)
    {
        try
        {
            int userId = _userService.GetConnectedUser(User);

            if (data.MatchedUserId == userId) return BadRequest("Invalid user id");

            if ((await _matchService.CheckIfMatchRequestExist(userId, data.MatchedUserId)).Status) return BadRequest("User match request already exists");
            if ((await _matchService.CheckIfUserSendMatchRequest(userId, data.MatchedUserId, (int)MatchStateEnum.inititated)).Status)
            {
                return BadRequest("A previous request was already sent");
            }

            MatchesResultDto? match = await _matchService.CreateUserMatch(userId, data);

            if (match == null) return BadRequest("An error occurred when trying to add");
            return match;
        }
        catch (Exception)
        {
            return BadRequest("An error occurred");
        }
    }

    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<ResultPaginate<MatchesResultDto>>>> GetUserMatches(int skip = 0, int limit = 50, string sort = "desc")
    {
        try
        {
            int userId = _userService.GetConnectedUser(User);

            ResultPaginate<MatchesResultDto>? matches = await _matchService.GetUserMatches(userId, skip, limit, sort);

            if (matches == null) return BadRequest("An error occurred when trying to get matches");

            return Ok(matches);
        }
        catch (Exception e)
        {
            return BadRequest("An error occurred or no matches found " + e);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> CancelMatchRequest(int id)
    {
        try
        {
            int userId = _userService.GetConnectedUser(User);

            BooleanReturnDto result = await _matchService.CancelMatchRequest(userId, id);
            return result;
        }
        catch (Exception e)
        {
            return BadRequest("An error occurred or request found " + e);
        }
    }

    [HttpPatch("{id}/review")]
    public async Task<ActionResult<BooleanReturnDto>> ReplyMatchRequest(int id, string action = "approved")
    {
        try
        {
            int userId = _userService.GetConnectedUser(User);

            if (action != "approved" && action != "declined") return BadRequest("Invalid action : approved or declined");

            BooleanReturnDto result = await _matchService.ReplyMatchRequest(userId, action, id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while reviewing", ex.Message);
            return BadRequest("An error occurred or request found");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> DeleteMyRequest(int id)
    {
        try
        {
            int userId = _userService.GetConnectedUser(User);

            BooleanReturnDto match = await _matchService.DeleteUserMatchRequest(userId, id);
            return match;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting", ex.Message);
            return BadRequest("An error occurred while deleting");
        }
    }
}
