namespace API.Interfaces;

public interface IGroupService
{
    Task<bool> CheckIfUserGroupExist(string userId, string name);
    Task<bool> CheckIfUserIsInTheGroup(string userId, string groupId);
}
