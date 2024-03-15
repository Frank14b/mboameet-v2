using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class FeedLikeService : IFeedLikeService
{

    private readonly DataContext _context;
    private readonly ILogger<FeedLikeService> _logger;

    public FeedLikeService(DataContext context, ILogger<FeedLikeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BooleanReturnDto?> CreateFeedLike(int feedId, int userId)
    {
        try
        {
            int likeCount = await CountUserLike(userId, feedId);
            if (likeCount > 0)
            {
                return new BooleanReturnDto()
                {
                    Status = false,
                    Message = "User like already exist",
                };
            }

            FeedLike like = new()
            {
                UserId = userId,
                FeedId = feedId
            };
            await _context.AddAsync(like);
            await _context.SaveChangesAsync();
            return new BooleanReturnDto()
            {
                Status = true,
                Message = "User like created",
                Data = like
            };
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during like save ${message}", e.Message);
            return null;
        }
    }

    public async Task<FeedLike?> GetUserLike(int userId, int feedId)
    {
        FeedLike like = await _context.FeedLikes.Where(fl => fl.UserId == userId && fl.FeedId == feedId).FirstAsync();
        return like;
    }

    public async Task<int> CountUserLike(int userId, int feedId)
    {
        int like = await _context.FeedLikes.Where(fl => fl.UserId == userId && fl.FeedId == feedId).CountAsync();
        return like;
    }

    public async Task<BooleanReturnDto?> DeleteFeedLike(int feedId, int userId)
    {
        try
        {
            FeedLike? like = await GetUserLike(userId, feedId);
            if (like is null)
            {
                return new BooleanReturnDto()
                {
                    Status = false,
                    Message = "User like doesn't exist",
                };
            }

            _context.Remove(like);
            await _context.SaveChangesAsync();
            return new BooleanReturnDto()
            {
                Status = true,
                Message = "User like has been removed",
                Data = like
            };
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during like removal ${message}", e.Message);
            return null;
        }
    }
}