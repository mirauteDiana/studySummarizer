using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySummarizer.Application.DTOs;
using StudySummarizer.Application.Interfaces;

namespace StudySummarizer.API.Controllers;

[Authorize]
[ApiController]
[Route("api/documents")]
public class DocumentsController : BaseController
{
    private readonly IDocumentService _documentService;
    private readonly ISummarizationService _summarizationService;

    public DocumentsController(IDocumentService documentService, ISummarizationService summarizationService)
    {
        _documentService = documentService;
        _summarizationService = summarizationService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request)
    {
        var result = await _documentService.UploadAsync(request, CurrentUserId);
        return result.Match(
            id => Ok(new { message = "Document uploaded successfully", id }),
            errors => Problem(errors));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var documents = await _documentService.GetAllAsync(CurrentUserId);
        return Ok(documents);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _documentService.GetByIdAsync(id, CurrentUserId);
        return result.Match(Ok, errors => Problem(errors));
    }

    [HttpGet("{id:guid}/file")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var result = await _documentService.DownloadAsync(id, CurrentUserId);
        return result.Match<IActionResult>(
            value => File(value.stream, value.contentType, value.fileName),
            Problem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _documentService.DeleteAsync(id, CurrentUserId);
        return result.Match(
            _ => Ok(new { message = "Document deleted successfully" }),
            errors => Problem(errors));
    }

    [HttpPost("{documentId:guid}/summarize")]
    public async Task<IActionResult> Summarize(Guid documentId, [FromBody] SummarizeRequest request)
    {
        var result = await _summarizationService.StartAsync(documentId, request, CurrentUserId);
        return result.Match(
            _ => Ok(new { message = "Document summarized successfully", documentId }),
            errors => Problem(errors));
    }

    [HttpGet("{documentId:guid}/summary")]
    public async Task<IActionResult> GetSummary(Guid documentId)
    {
        var result = await _summarizationService.GetAsync(documentId, CurrentUserId);
        return result.Match(Ok, errors => Problem(errors));
    }

    [HttpPatch("{documentId:guid}/summary")]
    public async Task<IActionResult> RegenerateSummary(Guid documentId, [FromBody] SummarizeRequest request)
    {
        var result = await _summarizationService.RegenerateAsync(documentId, request, CurrentUserId);
        return result.Match(
            _ => Ok(new { message = "Summary regenerated successfully" }),
            errors => Problem(errors));
    }
}
