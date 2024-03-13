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
    public FeedService(
        DataContext context,
        IMailService mailService,
        ILogger<FeedService> logger,
        IAppFileService appFileService,
        IMapper mapper,
        IHubContext<AppHub> hubContext,
        IFeedFileService feedFileService)
    {
        _context = context;
        _mailService = mailService;
        _logger = logger;
        _mapper = mapper;
        _hubContext = hubContext;
        _feedFileService = feedFileService;
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

            List<Feed> feeds = await query.Skip(skip).Take(limit).Include(e => e.User).Include(e => e.FeedFiles).ToListAsync();

            Console.WriteLine(feeds.ToArray()[0].ToString());

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

    public async Task<BooleanReturnDto?> DeleteFeed(int feedId, int userId)
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
}