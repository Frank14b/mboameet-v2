using API.Data;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace API.Controllers;

[Authorize(Policy = "IsUser")]
[Route("api/v1/groups")]
public class GroupController : BaseApiController
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IGroupService _groupService;
    private readonly ILogger _logger;

    public GroupController(
        DataContext context,
        IMapper mapper,
        IUserService userService,
        IGroupService groupService,
        ILogger logger,
        IConfiguration configuration,
        IMailService mailService)
    {
        _dataContext = context;
        _mapper = mapper;
        _userService = userService;
        _groupService = groupService;
        _logger = logger;
    }

    [HttpPost("")]
    public async Task<ActionResult<GroupListDto>> Create(CreateGroupDto data)
    {
        try
        {
            string id = _userService.GetConnectedUser(User);

            if (!await _groupService.CheckIfUserGroupExist(id, data.Name)) return BadRequest("Group name already exists"); // check if the group already exists

            GroupListDto? group = await _groupService.CreateNewGroup(id, data);
            if (group == null) return BadRequest("An error occured please try again");

            return group;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating group", ex?.Message);
            return BadRequest("An error occured or couldn't send your message");
        }
    }

    [HttpGet("")]
    public async Task<ActionResult<ResultPaginate<GroupListDto>>> GetGroups(int skip = 0, int limit = 30, string sort = "desc", string keyword = "")
    {
        try
        {
            string id = _userService.GetConnectedUser(User);

            var query = _dataContext.Groups.Where(g => (g.Status != (int)StatusEnum.delete && g.UserId.ToString() == id) || (g.Status == (int)StatusEnum.enable && g.UserId.ToString() != id.ToString()));

            if (keyword.Length > 0)
            {
                query = _dataContext.Groups.Where(g => g.Name.Contains(keyword.ToLower()) && ((g.Status != (int)StatusEnum.delete && g.UserId.ToString() == id) || (g.Status == (int)StatusEnum.enable && g.UserId.ToString() != id.ToString())));
            }

            int totalGroups = await query.CountAsync();

            query = sort == "desc" ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);

            var _result = await query.Skip(skip).Take(limit).ToListAsync();

            var result = _mapper.Map<IEnumerable<GroupListDto>>(_result);

            return Ok(new ResultPaginate<GroupListDto>
            {
                Data = result,
                Limit = limit,
                Skip = skip,
                Total = totalGroups
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting group list", ex?.Message);
            return BadRequest("An error occured or no group found");
        }
    }

    [HttpPatch("{id}/joins")]
    public async Task<ActionResult<BooleanReturnDto>> JoinTheGroup(string id, JoinGroupDto data)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);
            //check if the group exist and not belonging to the user
            bool isOwner = await _groupService.CheckIfUserCreatedTheGroup(id, userId);
            if (isOwner) return NotFound("Invalid Group Id");

            //check if the user is not yet in the group
            bool groupUser = await _groupService.CheckIfUserIsInTheGroup(userId, id);
            if (!groupUser) return NotFound("Invalid Group Id");

            bool joinGroup = await _groupService.JoinTheGroup(id, userId, data);
            if (!joinGroup) return BadRequest("An error occured please try again");

            return Ok(new BooleanReturnDto()
            {
                Status = true,
                Message = "Successfully join the group"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating group join", ex?.Message);
            return BadRequest("An error occured or group not found");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> Delete(string id)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);
            //check if the group exist and belongs to the user
            bool group = await _groupService.DeleteGroupById(id, userId);
            if (!group) return NotFound("Invalid Group Id / group not found");

            return Ok(new BooleanReturnDto()
            {
                Status = true,
                Message = "Successfully deleted the group"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while deleting the group", ex?.Message);
            return BadRequest("An error occured or group not found");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BooleanReturnDto>> Update(string id, UpdateGroupDto data)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);
            //check if the group exist and belongs to the user and update
            bool group = await _groupService.UpdateGroupById(id, data, userId);
            if (!group) return NotFound("Invalid Group Id / group not found");

            return Ok(new BooleanReturnDto()
            {
                Status = true,
                Message = "Successfully updated the group"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while updating the group", ex?.Message);
            return BadRequest("An error occured or group not found");
        }
    }
}