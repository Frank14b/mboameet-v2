using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IFeedLikeService {
    Task<BooleanReturnDto?> CreateFeedLike(int feedId, int userId);
    Task<FeedLike?> GetUserLike(int userId, int feedId);
    Task<int> CountUserLike(int userId, int feedId);
    Task<BooleanReturnDto?> DeleteFeedLike(int feedId, int userId);
}