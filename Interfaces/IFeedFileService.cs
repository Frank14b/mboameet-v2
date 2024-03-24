using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IFeedFileService {
    Task<bool> CreateFilesAsync(IFormFileCollection files, int feedId, int userId);
    Task<bool> CreateFileAsync(IFormFile file, int feedId, int userId);
}