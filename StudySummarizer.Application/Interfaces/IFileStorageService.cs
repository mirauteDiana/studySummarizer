namespace StudySummarizer.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream stream, string fileName);
    Task<Stream> OpenReadAsync(string storageKey);
    void Delete(string storageKey);
}
