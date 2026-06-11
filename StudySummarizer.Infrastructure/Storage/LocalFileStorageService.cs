using StudySummarizer.Application.Interfaces;

namespace StudySummarizer.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadPath;

    public LocalFileStorageService(string uploadPath)
    {
        _uploadPath = uploadPath;
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> SaveAsync(Stream stream, string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        return fileName;
    }

    public Task<Stream> OpenReadAsync(string storageKey)
    {
        var filePath = Path.Combine(_uploadPath, storageKey);
        Stream stream = File.OpenRead(filePath);
        return Task.FromResult(stream);
    }

    public void Delete(string storageKey)
    {
        var filePath = Path.Combine(_uploadPath, storageKey);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
