using API.Data;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public class AppFileService: IAppFileService {
    private readonly DataContext _dataContext;
    private readonly ILogger<AppFileService> _logger;

    public AppFileService(DataContext dataContext, ILogger<AppFileService> logger) {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<string?> UploadFile([FromForm] IFormFile image, string userId, string folder) {
        try
        {
            if(image is not IFormFile file) return null;

            string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "storage", userId, folder);

            //create the storage folder if not exist
            if(!Directory.Exists(uploadPath)) {
                Directory.CreateDirectory(uploadPath);
            }

            string filePath = Path.Combine(uploadPath, fileName);
            FileStream stream = new(filePath, FileMode.Create);
            
            await file.CopyToAsync(stream);
            
            return $"{folder}/{fileName}";
        }
        catch (Exception e)
        {
            _logger.LogError("an error occured during file upload ${message}", e.Message);
            return null;
        }
    }
}