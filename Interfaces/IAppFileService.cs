namespace API.Interfaces;

public interface IAppFileService {
    Task<string?> UploadFile(IFormFile file, int userId, string folder);
    Task<List<string>?> UploadFiles(IFormFileCollection dataFiles, int userId, string folder);
}