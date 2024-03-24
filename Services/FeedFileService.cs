using API.Data;
using API.Entities;
using API.Interfaces;

namespace API.Services;

public class FeedFileService : IFeedFileService
{
    private readonly DataContext _context;
    private readonly ILogger<FeedService> _logger;
    private readonly IAppFileService _appFileService;
    public FeedFileService(
        DataContext context,
        ILogger<FeedService> logger,
        IAppFileService appFileService
    )
    {
        _context = context;
        _logger = logger;
        _appFileService = appFileService;
    }

    public async Task<bool> CreateFilesAsync(IFormFileCollection files, int feedId, int userId)
    {
        try
        {
            List<string>? fileUrls = await _appFileService.UploadFiles(files, userId, "feeds");

            if (fileUrls is null) return false;

            foreach (string fileLink in fileUrls)
            {
                FeedFile feedFile = new()
                {
                    Url = fileLink,
                    PreviewUrl = fileLink,
                    Type = "",
                    DisplayMode = "",
                    FeedId = feedId,
                    UserId = userId
                };

                await _context.AddAsync(feedFile);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during feed files creation ${message}", e.Message);
            return false;
        }
    }

    public async Task<bool> CreateFileAsync(IFormFile file, int feedId, int userId)
    {
        try
        {
            string? fileLink = await _appFileService.UploadFile(file, userId, "feeds");

            if (fileLink is null) return false;

            FeedFile feedFile = new()
            {
                Url = fileLink,
                PreviewUrl = fileLink,
                Type = "",
                DisplayMode = "",
                FeedId = feedId,
                UserId = userId
            };

            await _context.AddAsync(feedFile);

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured during feed files creation ${message}", e.Message);
            return false;
        }
    }
}