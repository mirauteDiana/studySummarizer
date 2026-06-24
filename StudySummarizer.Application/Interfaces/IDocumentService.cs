using ErrorOr;
using StudySummarizer.Application.DTOs;
using StudySummarizer.Domain.Entities;

namespace StudySummarizer.Application.Interfaces;

public interface IDocumentService
{
    Task<ErrorOr<Guid>> UploadAsync(UploadDocumentRequest request, Guid userId);
    Task<ErrorOr<DocumentResponse>> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<DocumentResponse>> GetAllAsync(Guid userId);
    Task<ErrorOr<(Stream stream, string contentType, string fileName)>> DownloadAsync(Guid id, Guid userId);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id, Guid userId);
    Task<ErrorOr<Document>> GetOwnedAsync(Guid id, Guid userId);
}
