using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class FeedCommentService : IFeedCommentService
{

    private readonly DataContext _context;
    private readonly ILogger<FeedCommentService> _logger;
    private readonly IMapper _mapper;
    public FeedCommentService(
        DataContext context,
        ILogger<FeedCommentService> logger,
        IMapper mapper
    )
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ResultPaginate<ResultFeedCommentDto>> GetFeedComments(int feedId, int skip = 0, int limit = 10)
    {
        var query = _context.FeedComments.Where(fc => fc.FeedId == feedId && fc.Status == (int)StatusEnum.enable);

        int total = await query.CountAsync();

        query = query.Skip(skip).Take(limit).Include(fc => fc.User);

        List<FeedComment> comments = await query.ToListAsync();

        var result = _mapper.Map<IEnumerable<ResultFeedCommentDto>>(comments);

        return new ResultPaginate<ResultFeedCommentDto>()
        {
            Skip = skip,
            Limit = limit,
            Total = total,
            Data = result
        };
    }

    public async Task<ResultFeedCommentDto?> CreateComment(CreateCommentDto data, int feedId, int userId)
    {
        try
        {
            FeedComment commentObj = new()
            {
                Content = data.Context,
                FeedId = feedId,
                UserId = userId
            };

            await _context.AddAsync(commentObj);
            await _context.SaveChangesAsync();

            var comment = _mapper.Map<ResultFeedCommentDto>(commentObj);
            return comment;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while creating comment ${message}", e.Message);
            return null;
        }
    }
}