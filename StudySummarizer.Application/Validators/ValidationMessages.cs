namespace StudySummarizer.Application.Validators;

public static class ValidationMessages
{
    public static readonly HashSet<string> AllowedFileExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".docx", ".txt" };

    public const string TitleRequired = "Title is required.";
    public const string FileRequired = "A file is required.";
    public const string FileEmpty = "The uploaded file is empty.";

    public static readonly string FileTypeNotAllowed =
        $"File type not allowed. Accepted: {string.Join(", ", AllowedFileExtensions)}";
}
