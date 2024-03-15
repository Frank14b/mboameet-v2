using API.DTOs;

namespace API.Interfaces;

public interface IFeedService {
    Task<FeedResultDto?> CreateNewFeed(CreateFeedDto data, int userId);
    Task<ResultPaginate<FeedResultDto>?> GetAllFeeds(int userId, int skip, int limit, string sort = "desc");
    Task<BooleanReturnDto?> DeleteFeed(int feedId, int userId);
    Task<BooleanReturnDto?> UpdateFeed(int feedId, int userId, UpdateFeedDto data);
    Task<bool> IsValidFeedId (int id);
    Task<BooleanReturnDto?> AddFeedLikes(int feedId, int userId);
    Task<BooleanReturnDto?> RemoveFeedLikes(int feedId, int userId);
}