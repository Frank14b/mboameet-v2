using API.DTOs;

namespace API.Interfaces;

public interface IFeedCommentService {
    Task<ResultPaginate<ResultFeedCommentDto>> GetFeedComments(int feedId, int skip, int limit);
    Task<ResultFeedCommentDto?> CreateComment(CreateCommentDto data, int feedId, int userId);
}