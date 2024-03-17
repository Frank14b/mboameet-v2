using API.AppHubs;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class FeedService : IFeedService
{
    private readonly DataContext _context;
    private readonly IMailService _mailService;
    private readonly ILogger<FeedService> _logger;
    private readonly IMapper _mapper;
    private readonly IHubContext<AppHub> _hubContext;
    private readonly IFeedFileService _feedFileService;
    private readonly IFeedLikeService _feedLikeService;
    public FeedService(
        DataContext context,
        IMailService mailService,
        ILogger<FeedService> logger,
        IAppFileService appFileService,
        IMapper mapper,
        IHubContext<AppHub> hubContext,
        IFeedFileService feedFileService,
        IFeedLikeService feedLikeService)
    {
        _context = context;
        _mailService = mailService;
        _logger = logger;
        _mapper = mapper;
        _hubContext = hubContext;
        _feedFileService = feedFileService;
        _feedLikeService = feedLikeService;
    }

    public async Task<FeedResultDto?> CreateNewFeed(CreateFeedDto data, int userId)
    {
        try
        {
            Feed feed = new() // and and create feed in db
            {
                Message = data?.Message,
                UserId = userId,
            };
            await _context.AddAsync(feed);
            await _context.SaveChangesAsync();

            if (data?.Images is not null) // add and create feed images if available
            {
                bool fileSaved = await _feedFileService.CreateFiles(data.Images, feed.Id, userId);
            }

            FeedResultDto result = _mapper.Map<FeedResultDto>(feed);

            await _hubContext.Clients.All.SendAsync(AppHubConstants.NewFeedAdded, result.Id);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during feed creation ${message}", e.Message);
            return null;
        }
    }

    public async Task<ResultPaginate<FeedResultDto>?> GetAllFeeds(int userId, int skip = 0, int limit = 10, string sort = "desc")
    {
        try
        {
            var query = _context.Feeds.Where(x => x.Status == (int)StatusEnum.enable);

            // Apply sorting directly in the query
            query = sort == "desc"
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt);

            int feedCount = await query.CountAsync();

            query = query.Skip(skip).Take(limit);
            query = query.Include(e => e.FeedLikes.Where(fl => fl.UserId == userId));
            query = query.Include(e => e.User).Include(e => e.FeedFiles);

            List<Feed> feeds = await query.ToListAsync();

            IEnumerable<FeedResultDto> result = _mapper.Map<IEnumerable<FeedResultDto>>(feeds);

            ResultPaginate<FeedResultDto> response = new()
            {
                Data = result,
                Skip = 0,
                Limit = 10,
                Total = feedCount
            };

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while getting feeds ${message}", e.Message);
            return null;
        }
    }

    public async Task<BooleanReturnDto?> UpdateFeed(int feedId, int userId, UpdateFeedDto data)
    {
        try
        {
            Feed? feed = await _context.Feeds.Where(
                x => x.Id == feedId && x.Status != (int)StatusEnum.delete && x.UserId == userId
            ).FirstAsync();

            if (feed is null)
            {
                return new BooleanReturnDto()
                {
                    Status = false,
                    Message = $"Invalid Feed id: {feed}"
                };
            }

            feed.Message = data.Message;
            feed.UpdatedAt = DateTime.Now;
            _context.Update(feed);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<FeedResultDto>(feed);

            await _hubContext.Clients.All.SendAsync(AppHubConstants.FeedUpdated, result);

            return new BooleanReturnDto()
            {
                Status = true,
                Message = $"The provided feed: {feed} have been updated",
                Data = result
            };
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during feed creation ${message}", e.Message);
            return null;
        }
    }

    public async Task<BooleanReturnDto?> DeleteFeed(int feedId, int userId)
    {
        try
        {
            Feed? feed = await GetUserFeedById(feedId, userId);

            if (feed is null)
            {
                return new BooleanReturnDto()
                {
                    Status = false,
                    Message = $"Invalid Feed id: {feed}"
                };
            }

            feed.Status = (int)StatusEnum.delete;
            feed.UpdatedAt = DateTime.Now;
            _context.Update(feed);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync(AppHubConstants.FeedDeleted, feedId);

            return new BooleanReturnDto()
            {
                Status = true,
                Message = $"The provided feed: {feed} have been deleted"
            };
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during feed creation ${message}", e.Message);
            return null;
        }
    }

    public async Task<bool> IsValidFeedId(int id)
    {
        int feed = await _context.Feeds.Where(f => f.Id == id && f.Status == (int)StatusEnum.enable).CountAsync();
        if (feed  == 0) return false;
        return true;
    }

    public async Task<Feed?> GetFeedById(int id)
    {
        Feed? feed = await _context.Feeds.Where(f => f.Id == id && f.Status == (int)StatusEnum.enable).FirstAsync();
        return feed;
    }

    public async Task<Feed?> GetFeedByIdLinq(int id, int userId)
    {
        var query = _context.Feeds.Where(f => f.Id == id && f.Status == (int)StatusEnum.enable);
        query = query.Include(e => e.FeedLikes.Where(fl => fl.UserId == userId));
        query = query.Include(e => e.User).Include(e => e.FeedFiles);

        Feed? feed = await query.FirstAsync();

        return feed;
    }

    public async Task<Feed?> GetUserFeedById(int id, int userId)
    {
        Feed? feed = await _context.Feeds.Where(f => f.Id == id && f.Status == (int)StatusEnum.enable && f.UserId == userId).FirstAsync();
        return feed;
    }

    public async Task<BooleanReturnDto?> AddFeedLikes(int feedId, int userId)
    {
        Feed? feed = await GetFeedById(feedId);

        if (feed is null)
        {
            return new BooleanReturnDto()
            {
                Status = false,
                Message = $"Invalid Feed id: {feed}"
            };
        }

        BooleanReturnDto? like = await _feedLikeService.CreateFeedLike(feedId, userId);
        if (like is null) return null;

        if (like.Status == true)
        {
            feed.Likes += 1;
            feed.UpdatedAt = DateTime.Now;
            _context.Update(feed);
            await _context.SaveChangesAsync();
        }
        else
        {
            return like;
        }

        List<FeedLike> userLike = new(1)
            {
                like.Data
            };
        feed.FeedLikes = userLike;

        var result = _mapper.Map<FeedResultDto>(feed);
        await _hubContext.Clients.All.SendAsync(AppHubConstants.FeedUpdated, result);

        return new BooleanReturnDto()
        {
            Status = true,
            Message = $"The provided feed: {feedId} have been updated",
            Data = result
        };
    }

    public async Task<BooleanReturnDto?> RemoveFeedLikes(int feedId, int userId)
    {
        Feed? feed = await GetFeedById(feedId);

        if (feed is null)
        {
            return new BooleanReturnDto()
            {
                Status = false,
                Message = $"Invalid Feed id: {feed}"
            };
        }

        BooleanReturnDto? like = await _feedLikeService.DeleteFeedLike(feedId, userId);
        if (like is null) return null;

        if (like.Status == true)
        {
            feed.Likes -= 1;
            feed.UpdatedAt = DateTime.Now;
            _context.Update(feed);
            await _context.SaveChangesAsync();
        }
        else
        {
            return like;
        }

        feed.FeedLikes = null;

        var result = _mapper.Map<FeedResultDto>(feed);
        await _hubContext.Clients.All.SendAsync(AppHubConstants.FeedUpdated, result);

        return new BooleanReturnDto()
        {
            Status = true,
            Message = $"The provided feed: {feedId} have been updated",
            Data = result
        };
    }
}