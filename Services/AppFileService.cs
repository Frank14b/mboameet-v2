using API.Data;
using API.DTOs;
using API.Interfaces;

namespace API.Services;

public class AppFileService : IAppFileService
{
    private readonly DataContext _dataContext;
    private readonly ILogger<AppFileService> _logger;

    public AppFileService(DataContext dataContext, ILogger<AppFileService> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<string?> UploadFile(IFormFile dataFile, int userId, string folder)
    {
        try
        {
            if (dataFile is not IFormFile file) return null;

            string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "storage", userId.ToString(), folder);

            //create the storage folder if not exist
            if (!Directory.Exists(uploadPath))
            {
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

    public async Task<List<string>?> UploadFiles(IFormFileCollection dataFiles, int userId, string folder)
    {
        try
        {
            if (dataFiles is not IFormFileCollection files) return null;

            List<string> links = new();

            foreach (IFormFile file in files)
            {
                string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "storage", userId.ToString(), folder);

                //create the storage folder if not exist
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string filePath = Path.Combine(uploadPath, fileName);
                FileStream stream = new(filePath, FileMode.Create);

                await file.CopyToAsync(stream);

                links.Add($"{folder}/{fileName}");
            }

            return links;
        }
        catch (Exception e)
        {
            _logger.LogError("an error occured during file upload ${message}", e.Message);
            return null;
        }
    }
}