using ErrorOr;
using StudySummarizer.Application.DTOs;
using StudySummarizer.Application.Interfaces;
using StudySummarizer.Domain.Entities;
using StudySummarizer.Domain.Enums;
using StudySummarizer.Domain.Interfaces;

namespace StudySummarizer.Application.Services;

public class SummarizationService : ISummarizationService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ISummaryRepository _summaryRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILlamaClient _llamaClient;

    public SummarizationService(
        IDocumentRepository documentRepository,
        ISummaryRepository summaryRepository,
        IFileStorageService fileStorageService,
        ILlamaClient llamaClient)
    {
        _documentRepository = documentRepository;
        _summaryRepository = summaryRepository;
        _fileStorageService = fileStorageService;
        _llamaClient = llamaClient;
    }

    public async Task<ErrorOr<Success>> StartAsync(Guid documentId, SummarizeRequest request)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document is null)
            return Error.NotFound("Document.NotFound", $"Document {documentId} was not found.");

        if (document.FileType != FileType.Txt)
            return Error.Validation("Summary.UnsupportedFileType", "Only .txt documents support summarization at this time.");

        var existing = await _summaryRepository.GetByDocumentIdAsync(documentId);
        if (existing is not null)
            return Error.Conflict("Summary.AlreadyExists", "A summary already exists for this document. Use PATCH to regenerate.");

        return await GenerateAndSaveAsync(document, request.SummaryType, existing: null);
    }

    public async Task<ErrorOr<SummaryResponse>> GetAsync(Guid documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document is null)
            return Error.NotFound("Document.NotFound", $"Document {documentId} was not found.");

        var summary = await _summaryRepository.GetByDocumentIdAsync(documentId);
        if (summary is null)
            return Error.NotFound("Summary.NotFound", "No summary has been generated for this document yet.");

        return new SummaryResponse
        {
            Id = summary.Id,
            DocumentId = document.Id,
            Title = document.Title,
            Content = summary.Content,
            CreatedAt = summary.CreatedAt
        };
    }

    public async Task<ErrorOr<Success>> RegenerateAsync(Guid documentId, SummarizeRequest request)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document is null)
            return Error.NotFound("Document.NotFound", $"Document {documentId} was not found.");

        if (document.FileType != FileType.Txt)
            return Error.Validation("Summary.UnsupportedFileType", "Only .txt documents support summarization at this time.");

        var existing = await _summaryRepository.GetByDocumentIdAsync(documentId);
        if (existing is null)
            return Error.NotFound("Summary.NotFound", "No summary has been generated for this document yet. Use POST to create one.");

        return await GenerateAndSaveAsync(document, request.SummaryType, existing);
    }

    private async Task<ErrorOr<Success>> GenerateAndSaveAsync(Document document, SummaryType summaryType, Summary? existing)
    {
        string text;
        try
        {
            var stream = await _fileStorageService.OpenReadAsync(document.FilePath);
            using var reader = new StreamReader(stream);
            text = await reader.ReadToEndAsync();
        }
        catch (FileNotFoundException)
        {
            return Error.NotFound("Document.FileNotFound", $"The file for document {document.Id} is missing from storage.");
        }

        if (string.IsNullOrWhiteSpace(text))
            return Error.Validation("Summary.EmptyDocument", "The document contains no text to summarize.");

        string content;
        try
        {
            content = await _llamaClient.SummarizeAsync(text, summaryType);
        }
        catch (Exception ex)
        {
            return Error.Failure("Summary.LlamaError", $"Summarization failed: {ex.Message}");
        }

        if (existing is null)
        {
            var summary = new Summary
            {
                DocumentId = document.Id,
                SummaryType = summaryType,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };
            await _summaryRepository.AddAsync(summary);
        }
        else
        {
            existing.SummaryType = summaryType;
            existing.Content = content;
        }

        document.Status = DocumentStatus.Summarized;
        await _documentRepository.SaveChangesAsync();

        return Result.Success;
    }
}
