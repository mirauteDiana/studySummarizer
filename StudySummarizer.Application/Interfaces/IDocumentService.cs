using StudySummarizer.Application.DTOs;

namespace StudySummarizer.Application.Interfaces;

public interface IDocumentService
{
    Task<Guid> UploadAsync(UploadDocumentRequest request);
    Task<DocumentResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<DocumentResponse>> GetAllAsync();
    Task<(Stream stream, string contentType, string fileName)?> DownloadAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}
