using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Services;

public class FeedService : IFeedService
{
    private readonly DataContext _context;
    private readonly IMailService _mailService;
    private readonly ILogger<FeedService> _logger;
    private readonly IAppFileService _appFileService;
    public FeedService(DataContext context, IMailService mailService, ILogger<FeedService> logger, IAppFileService appFileService)
    {
        _context = context;
        _mailService = mailService;
        _logger = logger;
        _appFileService = appFileService;
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
                        FeedId = feed.Id
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
}