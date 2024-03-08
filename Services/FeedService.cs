using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using MongoDB.Bson;

namespace API.Services;

public class FeedService: IFeedService {
    private readonly DataContext _context;
    private readonly IMailService _mailService;
    private readonly ILogger<FeedService> _logger;
    public FeedService(DataContext context, IMailService mailService, ILogger<FeedService> logger) {
        _context = context;
        _mailService = mailService;
        _logger = logger;
    }

    public async Task<AppFeed?> CreateNewFeed(CreateFeedDto data, int userId) {
        try
        {
            AppFeed feed = new() {
                Message = data?.Message,
                UserId = userId,
                VideoLink = data?.VideoLink,
            };

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