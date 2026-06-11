using StudySummarizer.Application.DTOs;
using StudySummarizer.Application.Interfaces;
using StudySummarizer.Domain.Entities;
using StudySummarizer.Domain.Enums;
using StudySummarizer.Domain.Interfaces;

namespace StudySummarizer.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;

    public DocumentService(IDocumentRepository documentRepository, IFileStorageService fileStorageService)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Guid> UploadAsync(UploadDocumentRequest request)
    {
        var id = Guid.NewGuid();
        var ext = Path.GetExtension(request.FileName).ToLowerInvariant();
        var storageKey = $"{id}{ext}";

        var savedKey = await _fileStorageService.SaveAsync(request.FileStream, storageKey);

        var fileType = ext switch
        {
            ".pdf" => FileType.Pdf,
            ".docx" => FileType.Docx,
            ".txt" => FileType.Txt,
            _ => throw new InvalidOperationException($"Unsupported file type: {ext}")
        };

        var document = new Document
        {
            Id = id,
            Title = request.Title,
            FileType = fileType,
            FilePath = savedKey,
            Status = DocumentStatus.Pending,
            UploadDate = DateTime.UtcNow
        };

        try
        {
            await _documentRepository.AddAsync(document);
            await _documentRepository.SaveChangesAsync();
        }
        catch
        {
            _fileStorageService.Delete(savedKey);
            throw;
        }

        return id;
    }

    public async Task<DocumentResponse?> GetByIdAsync(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        return document is null ? null : MapToResponse(document);
    }

    public async Task<IEnumerable<DocumentResponse>> GetAllAsync()
    {
        var documents = await _documentRepository.GetAllAsync();
        return documents.Select(MapToResponse);
    }

    public async Task<(Stream stream, string contentType, string fileName)?> DownloadAsync(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document is null)
            return null;

        Stream stream;
        try
        {
            stream = await _fileStorageService.OpenReadAsync(document.FilePath);
        }
        catch (FileNotFoundException)
        {
            return null;
        }

        var ext = Path.GetExtension(document.FilePath).ToLowerInvariant();
        var contentType = ext switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };

        var fileName = $"{document.Title}{ext}";

        return (stream, contentType, fileName);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document is null)
            return false;

        var filePath = document.FilePath;
        _fileStorageService.Delete(filePath);

        await _documentRepository.DeleteAsync(document);
        await _documentRepository.SaveChangesAsync();

        return true;
    }

    private static DocumentResponse MapToResponse(Document document) => new()
    {
        Id = document.Id,
        Title = document.Title,
        FileType = document.FileType,
        Status = document.Status,
        UploadDate = document.UploadDate,
    };
}
