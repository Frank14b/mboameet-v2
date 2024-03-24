using API.AppHubs;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class FeedCommentService : IFeedCommentService
{

    private readonly DataContext _context;
    private readonly ILogger<FeedCommentService> _logger;
    private readonly IMapper _mapper;
    private readonly IHubContext<AppHub> _hubContext;

    public FeedCommentService(
        DataContext context,
        ILogger<FeedCommentService> logger,
        IMapper mapper,
        IHubContext<AppHub> hubContext
    )
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<ResultPaginate<ResultFeedCommentDto>> GetFeedComments(int feedId, int skip = 0, int limit = 10, string sort = "desc")
    {
        var query = _context.FeedComments.Where(fc => fc.FeedId == feedId && fc.Status == (int)StatusEnum.enable);

        // Apply sorting directly in the query
        query = sort == "desc"
            ? query.OrderByDescending(x => x.CreatedAt)
            : query.OrderBy(x => x.CreatedAt);

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
                Content = data.Content,
                FeedId = feedId,
                UserId = userId
            };

            await _context.AddAsync(commentObj);
            await _context.SaveChangesAsync();

            var comment = _mapper.Map<ResultFeedCommentDto>(commentObj);
            await _hubContext.Clients.All.SendAsync(AppHubConstants.FeedCommentCreated, comment);
            return comment;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while creating comment ${message}", e.Message);
            return null;
        }
    }

    public async Task<BooleanReturnDto?> UpdateComment(UpdateCommentDto data, int id, int userId)
    {
        try
        {
            FeedComment? comment = await _context.FeedComments.Where(fc => fc.Id == id && fc.UserId == userId && fc.Status == (int)StatusEnum.enable).FirstAsync();

            if (comment is null)
            {
                return new BooleanReturnDto()
                {
                    Status = false,
                    Message = "Comment not found",
                };
            }

            comment.Content = data.Content;
            _context.Update(comment);
            await _context.SaveChangesAsync();

            var updatedComment = _mapper.Map<ResultFeedCommentDto>(comment);
            await _hubContext.Clients.All.SendAsync(AppHubConstants.FeedCommentUpdated, updatedComment);

            return new BooleanReturnDto()
            {
                Status = true,
                Message = "Comment updated",
                Data = updatedComment
            };
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while updating comment ${message}", e.Message);
            return null;
        }
    }

    public async Task<BooleanReturnDto?> DeleteComment(int id, int userId)
    {
        try
        {
            FeedComment? comment = await _context.FeedComments.Where(fc => fc.Id == id && fc.UserId == userId && fc.Status == (int)StatusEnum.enable).FirstAsync();

            if (comment is null)
            {
                return new BooleanReturnDto()
                {
                    Status = false,
                    Message = "Comment not found",
                };
            }

            comment.Status = (int)StatusEnum.delete;
            _context.Update(comment);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync(AppHubConstants.FeedCommentDeleted, id);

            return new BooleanReturnDto()
            {
                Status = true,
                Message = "Comment deleted"
            };
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while updating comment ${message}", e.Message);
            return null;
        }
    }
}