namespace StudySummarizer.Application.Validators;

public static class ValidationMessages
{
    public static readonly HashSet<string> AllowedFileExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".docx", ".txt" };

    public const string TitleRequired = "Title is required.";
    public const string FileRequired = "A file is required.";
    public const string FileEmpty = "The uploaded file is empty.";

    public static readonly string FileTypeNotAllowed =
        $"File type not allowed. Accepted: {string.Join(", ", AllowedFileExtensions.Order())}";

    public const string UsernameRequired = "Username is required.";
    public const string EmailRequired = "Email is required.";
    public const string EmailInvalidFormat = "A valid email address is required.";
    public const string PasswordRequired = "Password is required.";
    public const string PasswordTooShort = "Password must be at least 8 characters.";
    public const string PasswordTooLong = "Password must be at most 256 characters.";
}
