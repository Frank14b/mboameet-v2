using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IFeedService {
    Task<Feed?> CreateNewFeed(CreateFeedDto data, int userId);
    Task<ResultPaginate<FeedResultDto>?> GetAllFeeds(int userId, int skip, int limit, string sort = "desc");
}