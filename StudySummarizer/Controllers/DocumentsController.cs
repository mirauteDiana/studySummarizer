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
    public async Task<IActionResult> Upload([FromForm] string title, IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest(new { message = "Title is required." });

        if (file is null || file.Length == 0)
            return BadRequest(new { message = "A file is required." });

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".pdf", ".docx", ".txt" };
        var ext = Path.GetExtension(file.FileName);
        if (!allowed.Contains(ext))
            return BadRequest(new { message = $"File type not allowed. Accepted: {string.Join(", ", allowed)}" });

        var request = new UploadDocumentRequest
        {
            Title = title,
            FileStream = file.OpenReadStream(),
            FileName = file.FileName,
        };

        var id = await _documentService.UploadAsync(request);
        return Ok(new { message = "Document uploaded successfully", id });
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
        var document = await _documentService.GetByIdAsync(id);
        return document is null ? NotFound() : Ok(document);
    }

    [HttpGet("{id:guid}/file")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var result = await _documentService.DownloadAsync(id);
        if (result is null)
            return NotFound();

        var (stream, contentType, fileName) = result.Value;
        return File(stream, contentType, fileName);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _documentService.DeleteAsync(id);
        return deleted ? Ok(new { message = "Document deleted successfully" }) : NotFound();
    }
}
