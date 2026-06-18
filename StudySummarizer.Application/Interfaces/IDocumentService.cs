using ErrorOr;
using StudySummarizer.Application.DTOs;

namespace StudySummarizer.Application.Interfaces;

public interface IDocumentService
{
    Task<ErrorOr<Guid>> UploadAsync(UploadDocumentRequest request);
    Task<ErrorOr<DocumentResponse>> GetByIdAsync(Guid id);
    Task<IEnumerable<DocumentResponse>> GetAllAsync();
    Task<ErrorOr<(Stream stream, string contentType, string fileName)>> DownloadAsync(Guid id);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id);
}
