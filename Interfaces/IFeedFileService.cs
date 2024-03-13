using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IFeedFileService {
    Task<bool> CreateFiles(IFormFileCollection files, int feedId, int userId);
}