namespace StudySummarizer.Application.DTOs;

public class UploadDocumentRequest
{
    public string Title { get; set; } = string.Empty;
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
}
