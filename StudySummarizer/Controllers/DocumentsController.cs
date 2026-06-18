using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using StudySummarizer.Application.DTOs;
using StudySummarizer.Application.Interfaces;

namespace StudySummarizer.API.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
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
        var result = await _documentService.UploadAsync(request);
        return result.Match(
            id => Ok(new { message = "Document uploaded successfully", id }),
            errors => Problem(errors));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var documents = await _documentService.GetAllAsync();
        return Ok(documents);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _documentService.GetByIdAsync(id);
        return result.Match(Ok, errors => Problem(errors));
    }

    [HttpGet("{id:guid}/file")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var result = await _documentService.DownloadAsync(id);
        return result.Match<IActionResult>(
            value => File(value.stream, value.contentType, value.fileName),
            Problem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _documentService.DeleteAsync(id);
        return result.Match(
            _ => Ok(new { message = "Document deleted successfully" }),
            errors => Problem(errors));
    }

    [HttpPost("{documentId:guid}/summarize")]
    public async Task<IActionResult> Summarize(Guid documentId, [FromBody] SummarizeRequest request)
    {
        var result = await _summarizationService.StartAsync(documentId, request);
        return result.Match(
            _ => Ok(new { message = "Document summarized successfully", documentId }),
            errors => Problem(errors));
    }

    [HttpGet("{documentId:guid}/summary")]
    public async Task<IActionResult> GetSummary(Guid documentId)
    {
        var result = await _summarizationService.GetAsync(documentId);
        return result.Match(Ok, errors => Problem(errors));
    }

    [HttpPatch("{documentId:guid}/summary")]
    public async Task<IActionResult> RegenerateSummary(Guid documentId, [FromBody] SummarizeRequest request)
    {
        var result = await _summarizationService.RegenerateAsync(documentId, request);
        return result.Match(
            _ => Ok(new { message = "Summary regenerated successfully" }),
            errors => Problem(errors));
    }

    private ObjectResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
            return Problem(statusCode: StatusCodes.Status500InternalServerError);

        var first = errors[0];
        var statusCode = first.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: first.Code, detail: first.Description);
    }
}
