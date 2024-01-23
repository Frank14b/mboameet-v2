using API.Data;
using API.DTOs;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class GroupService : IGroupService
{
    private readonly DataContext _dataContext;
    private readonly ILogger _logger;

    public GroupService(DataContext context, ILogger logger)
    {
        _dataContext = context;
        _logger = logger;
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
}
