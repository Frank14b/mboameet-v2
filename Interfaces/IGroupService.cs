using API.DTOs;

namespace API.Interfaces;

public interface IGroupService
{
    Task<bool> CheckIfUserGroupExist(int userId, string name);
    Task<bool> CheckIfUserIsInTheGroup(int userId, int groupId);
    Task<bool> UpdateGroupById(int id, UpdateGroupDto data, int userId);
    Task<bool> DeleteGroupById(int id, int userId);
    Task<bool> CheckIfUserCreatedTheGroup(int id, int userId);
    Task<bool> JoinTheGroup(int id, int userId, JoinGroupDto data);
    Task<GroupListDto?> CreateNewGroup(int userId, CreateGroupDto data);
}
