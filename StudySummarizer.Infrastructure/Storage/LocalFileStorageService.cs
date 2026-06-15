using StudySummarizer.Application.Interfaces;

namespace StudySummarizer.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadPath;

    private readonly string _uploadPathNormalized;

    public LocalFileStorageService(string uploadPath)
    {
        _uploadPath = uploadPath;
        Directory.CreateDirectory(_uploadPath);
        _uploadPathNormalized = Path.GetFullPath(_uploadPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public async Task<string> SaveAsync(Stream stream, string fileName)
    {
        var filePath = ResolveSafePath(fileName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        return fileName;
    }

    public Task<Stream> OpenReadAsync(string storageKey)
    {
        var filePath = ResolveSafePath(storageKey);
        Stream stream = File.OpenRead(filePath);
        return Task.FromResult(stream);
    }

    public void Delete(string storageKey)
    {
        var filePath = ResolveSafePath(storageKey);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    private string ResolveSafePath(string key)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_uploadPath, key));
        if (!fullPath.StartsWith(_uploadPathNormalized + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Storage key '{key}' resolves outside the upload directory.");
        return fullPath;
    }
}
