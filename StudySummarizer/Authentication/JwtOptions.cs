namespace StudySummarizer.API.Authentication;

public class JwtOptions
{
    public const string PlaceholderSecret = "change-this-to-a-secure-secret-at-least-32-chars";
    public const int MinimumSecretBytes = 32;

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}
