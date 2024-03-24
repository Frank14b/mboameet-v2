using API.DTOs;

namespace API.Interfaces;

public interface IFeedService {
    Task<FeedResultDto?> CreateNewFeedAsync(CreateFeedDto data, int userId);
    Task<ResultPaginate<FeedResultDto>?> GetAllFeedAsync(int userId, int skip, int limit, string sort = "desc");
    Task<BooleanReturnDto?> DeleteFeedAsync(int feedId, int userId);
    Task<BooleanReturnDto?> UpdateFeedAsync(int feedId, int userId, UpdateFeedDto data);
    Task<bool> IsValidFeedIdAsync(int id);
    Task<BooleanReturnDto?> AddFeedLikeAsync(int feedId, int userId);
    Task<BooleanReturnDto?> RemoveFeedLikeAsync(int feedId, int userId);
}