using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace API.Services;

public class GroupService : IGroupService
{
    private readonly DataContext _dataContext;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public GroupService(DataContext context, ILogger logger, IMapper mapper)
    {
        _dataContext = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> CheckIfUserGroupExist(string userId, string name)
    {
        try
        {
            bool group = await _dataContext.Groups.AnyAsync(g => g.Status != (int)StatusEnum.delete && g.Name.ToLower() == name.ToLower());
            if (group) return true;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to check if user group exists", ex?.Message);
            return true;
        }
    }

    public async Task<bool> CheckIfUserIsInTheGroup(string userId, string groupId)
    {
        try
        {
            bool groupUser = await _dataContext.GroupUsers.AnyAsync(gu => gu.Status != (int)StatusEnum.delete && gu.UserId.ToString() == userId && gu.GroupId.ToString() == groupId);
            if (groupUser) return true;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to check if user belongs to the group", ex?.Message);
            return true;
        }
    }

    public async Task<bool> UpdateGroupById(string id, UpdateGroupDto data, string userId)
    {
        try
        {
            AppGroup? group = await _dataContext.Groups.FirstOrDefaultAsync(g => g.Status != (int)StatusEnum.delete && g.UserId.ToString() == userId && g.Id.ToString() == id);
            if (group == null) return false;

            group.Name = data?.Name ?? group.Name;
            group.Description = data?.Name ?? group.Description;
            group.Type = data?.Name ?? group.Type;

            await _dataContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update the group", ex?.Message);
            return false;
        }
    }

    public async Task<bool> DeleteGroupById(string id, string userId)
    {
        try
        {
            AppGroup? group = await _dataContext.Groups.FirstOrDefaultAsync(g => g.Status != (int)StatusEnum.delete && g.UserId.ToString() == userId && g.Id.ToString() == id);
            if (group == null) return false;

            group.Status = (int)StatusEnum.delete;
            await _dataContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete the group", ex?.Message);
            return false;
        }
    }

    public async Task<bool> CheckIfUserCreatedTheGroup(string id, string userId)
    {
        bool group = await _dataContext.Groups.AnyAsync(g => g.Id.ToString() == id && g.UserId.ToString() == userId);
        if (!group) return false;

        return true;
    }

    public async Task<bool> JoinTheGroup(string id, string userId, JoinGroupDto data)
    {
        try
        {
            AppGroupUser newGroupUser = new()
            {
                UserId = ObjectId.Parse(userId),
                GroupId = ObjectId.Parse(id),
                GroupAccesId = ObjectId.Parse(data.GroupAccesId)
            };

            await _dataContext.GroupUsers.AddAsync(newGroupUser);
            await _dataContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to join the group", ex?.Message);
            return false;
        }
    }

    public async Task<GroupListDto?> CreateNewGroup(string userId, CreateGroupDto data)
    {
        try
        {
            var newGroup = _mapper.Map<AppGroup>(data);
            newGroup.UserId = ObjectId.Parse(userId);

            await _dataContext.AddAsync(newGroup);
            await _dataContext.SaveChangesAsync();

            var result = _mapper.Map<GroupListDto>(newGroup);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create the group", ex?.Message);
            return null;
        }
    }
}
