using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class FeedService : IFeedService
{
    private readonly DataContext _context;
    private readonly IMailService _mailService;
    private readonly ILogger<FeedService> _logger;
    private readonly IAppFileService _appFileService;
    private readonly IMapper _mapper;
    public FeedService(DataContext context, IMailService mailService, ILogger<FeedService> logger, IAppFileService appFileService, IMapper mapper)
    {
        _context = context;
        _mailService = mailService;
        _logger = logger;
        _appFileService = appFileService;
        _mapper = mapper;
    }

    public async Task<Feed?> CreateNewFeed(CreateFeedDto data, int userId)
    {
        try
        {
            List<string>? fileUrls = null;

            if (data?.Images is not null) // updload feed images if available
            {
                fileUrls = await _appFileService.UploadFiles(data.Images, userId, "feeds");
            }

            Feed feed = new() // and and create feed in db
            {
                Message = data?.Message,
                UserId = userId,
            };
            await _context.AddAsync(feed);
            await _context.SaveChangesAsync();

            if (fileUrls is not null) // add and create feed images if available
            {
                foreach (string fileLink in fileUrls)
                {
                    FeedFiles feedFile = new()
                    {
                        Url = fileLink,
                        PreviewUrl = fileLink,
                        Type = "",
                        DisplayMode = "",
                        FeedId = feed.Id,
                        Feed = feed
                    };

                    await _context.AddAsync(feedFile);
                }
            }
            await _context.SaveChangesAsync();

            return feed;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during feed creation ${message}", e.Message);
            return null;
        }
    }

    public async Task<ResultPaginate<FeedResultDto>?> GetAllFeeds(int userId, int skip = 0, int limit = 10, string sort = "desc") {
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
}