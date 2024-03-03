using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces;

public interface IAppFileService {
    Task<string?> UploadFile([FromForm] IFormFile file, string userId, string folder);
}