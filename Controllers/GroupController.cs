using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
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

            if (!await _groupService.CheckIfUserGroupExist(id, data.Name)) return BadRequest("Group name already exists");

            var newGroup = _mapper.Map<AppGroup>(data);
            newGroup.UserId = ObjectId.Parse(id);

            await _dataContext.AddAsync(newGroup);
            await _dataContext.SaveChangesAsync();

            var result = _mapper.Map<GroupListDto>(newGroup);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating group", ex?.Message);
            return BadRequest("An error occured or couldn't send your message");
        }
    }

    [HttpGet("")]
    public async Task<ActionResult<ResultPaginate>> GetGroups(int skip = 0, int limit = 30, string sort = "desc", string keyword = "")
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

            return Ok(new ResultPaginate
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
            bool group = await _dataContext.Groups.AnyAsync(g => g.Id.ToString() == id && g.UserId.ToString() != userId);
            if (!group) return NotFound("Invalid Group Id");

            //check if the user is not yet in the group
            bool groupUser = await _groupService.CheckIfUserIsInTheGroup(userId, id);
            if (!groupUser) return NotFound("Invalid Group Id");

            AppGroupUser newGroupUser = new()
            {
                UserId = ObjectId.Parse(userId),
                GroupId = ObjectId.Parse(id),
                GroupAccesId = ObjectId.Parse(data.GroupAccesId)
            };

            await _dataContext.GroupUsers.AddAsync(newGroupUser);
            await _dataContext.SaveChangesAsync();

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
    public async Task<ActionResult<BooleanReturnDto>> DeleteGroup(string id)
    {
        try
        {
            string userId = _userService.GetConnectedUser(User);
            //check if the group exist and not belonging to the user
            AppGroup? group = await _dataContext.Groups.FirstOrDefaultAsync(g => g.Id.ToString() == id && g.UserId.ToString() == userId);
            if (group == null) return NotFound("Invalid Group Id / group not found");

            group.Status = (int)StatusEnum.delete;
            await _dataContext.SaveChangesAsync();

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
}