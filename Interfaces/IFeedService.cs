using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IFeedService {
    Task<AppFeed?> CreateNewFeed(CreateFeedDto data, string userId);
}