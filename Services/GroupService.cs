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

    public async Task<bool> CheckIfUserGroupExist(int userId, string name)
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

    public async Task<bool> CheckIfUserIsInTheGroup(int userId, int groupId)
    {
        try
        {
            bool groupUser = await _dataContext.GroupUsers.AnyAsync(gu => gu.Status != (int)StatusEnum.delete && gu.UserId == userId && gu.GroupId == groupId);
            if (groupUser) return true;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to check if user belongs to the group", ex?.Message);
            return true;
        }
    }

    public async Task<bool> UpdateGroupById(int id, UpdateGroupDto data, int userId)
    {
        try
        {
            Group? group = await _dataContext.Groups.FirstOrDefaultAsync(g => g.Status != (int)StatusEnum.delete && g.UserId == userId && g.Id == id);
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

    public async Task<bool> DeleteGroupById(int id, int userId)
    {
        try
        {
            Group? group = await _dataContext.Groups.FirstOrDefaultAsync(g => g.Status != (int)StatusEnum.delete && g.UserId == userId && g.Id == id);
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

    public async Task<bool> CheckIfUserCreatedTheGroup(int id, int userId)
    {
        bool group = await _dataContext.Groups.AnyAsync(g => g.Id == id && g.UserId == userId);
        if (!group) return false;

        return true;
    }

    public async Task<bool> JoinTheGroup(int id, int userId, JoinGroupDto data)
    {
        try
        {
            GroupUser newGroupUser = new()
            {
                UserId = userId,
                GroupId = id,
                GroupAccesId = data.GroupAccesId
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

    public async Task<GroupListDto?> CreateNewGroup(int userId, CreateGroupDto data)
    {
        try
        {
            var newGroup = _mapper.Map<Group>(data);
            newGroup.UserId = userId;

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
