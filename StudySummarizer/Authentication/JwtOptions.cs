using System.ComponentModel.DataAnnotations;

namespace StudySummarizer.API.Authentication;

public class JwtOptions
{
    public const int MinimumSecretBytes = 32;

    [Required]
    [MinLength(MinimumSecretBytes)]
    public string Secret { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 60;
}
