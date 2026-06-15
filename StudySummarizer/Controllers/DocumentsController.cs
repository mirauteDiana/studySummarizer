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

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
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
