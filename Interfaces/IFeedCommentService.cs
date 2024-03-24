using API.DTOs;

namespace API.Interfaces;

public interface IFeedCommentService {
    Task<ResultPaginate<ResultFeedCommentDto>> GetFeedComments(int feedId, int skip, int limit, string sort);
    Task<ResultFeedCommentDto?> CreateComment(CreateCommentDto data, int feedId, int userId);
    Task<BooleanReturnDto?> UpdateComment(UpdateCommentDto data, int id, int userId);
    Task<BooleanReturnDto?> DeleteComment(int id, int userId);
}