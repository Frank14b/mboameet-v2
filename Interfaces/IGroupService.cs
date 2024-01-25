using API.DTOs;

namespace API.Interfaces;

public interface IGroupService
{
    Task<bool> CheckIfUserGroupExist(string userId, string name);
    Task<bool> CheckIfUserIsInTheGroup(string userId, string groupId);
    Task<bool> UpdateGroupById(string id, UpdateGroupDto data, string userId);
    Task<bool> DeleteGroupById(string id, string userId);
    Task<bool> CheckIfUserCreatedTheGroup(string id, string userId);
    Task<bool> JoinTheGroup(string id, string userId, JoinGroupDto data);
    Task<GroupListDto?> CreateNewGroup(string userId, CreateGroupDto data);
}
