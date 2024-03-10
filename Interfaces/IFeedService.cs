using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IFeedService {
    Task<Feed?> CreateNewFeed(CreateFeedDto data, int userId);
}