using AutoMapper;
using ErrorOr;
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
    private readonly IMapper _mapper;

    public DocumentService(IDocumentRepository documentRepository, IFileStorageService fileStorageService, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<ErrorOr<Guid>> UploadAsync(UploadDocumentRequest request)
    {
        var ext = Path.GetExtension(request.File!.FileName).ToLowerInvariant();

        var fileType = ext switch
        {
            ".pdf" => (FileType?)FileType.Pdf,
            ".docx" => FileType.Docx,
            ".txt" => FileType.Txt,
            _ => null
        };

        if (fileType is null)
            return Error.Validation("Document.UnsupportedFileType", $"Unsupported file type: {ext}");

        var document = new Document
        {
            Title = request.Title,
            FileType = fileType.Value,
            Status = DocumentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _documentRepository.AddAsync(document);
        await _documentRepository.SaveChangesAsync();

        var storageKey = $"{document.Id}{ext}";
        string savedKey;
        try
        {
            savedKey = await _fileStorageService.SaveAsync(request.File.OpenReadStream(), storageKey);
        }
        catch
        {
            await _documentRepository.DeleteAsync(document);
            await _documentRepository.SaveChangesAsync();
            throw;
        }

        document.FilePath = savedKey;
        await _documentRepository.SaveChangesAsync();

        return document.Id;
    }

    public async Task<ErrorOr<DocumentResponse>> GetByIdAsync(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document is null)
            return Error.NotFound("Document.NotFound", $"Document {id} was not found.");

        return _mapper.Map<DocumentResponse>(document);
    }

    public async Task<IEnumerable<DocumentResponse>> GetAllAsync()
    {
        var documents = await _documentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DocumentResponse>>(documents);
    }

    public async Task<ErrorOr<(Stream stream, string contentType, string fileName)>> DownloadAsync(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document is null)
            return Error.NotFound("Document.NotFound", $"Document {id} was not found.");

        Stream stream;
        try
        {
            stream = await _fileStorageService.OpenReadAsync(document.FilePath);
        }
        catch (FileNotFoundException)
        {
            return Error.NotFound("Document.FileNotFound", $"The file for document {id} is missing from storage.");
        }

        var contentType = document.FileType switch
        {
            FileType.Pdf => "application/pdf",
            FileType.Docx => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            FileType.Txt => "text/plain",
            _ => "application/octet-stream"
        };

        var ext = Path.GetExtension(document.FilePath).ToLowerInvariant();
        var fileName = $"{document.Title}{ext}";

        return (stream, contentType, fileName);
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document is null)
            return Error.NotFound("Document.NotFound", $"Document {id} was not found.");

        await _documentRepository.DeleteAsync(document);
        await _documentRepository.SaveChangesAsync();

        return Result.Deleted;
    }

}
