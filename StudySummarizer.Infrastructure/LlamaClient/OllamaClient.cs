using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using StudySummarizer.Application.Interfaces;
using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Infrastructure.LlamaClient;

public class OllamaClient : ILlamaClient
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public OllamaClient(HttpClient httpClient, IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> SummarizeAsync(string text, SummaryType summaryType, CancellationToken cancellationToken = default)
    {
        if (text.Length > _options.MaxInputChars)
            text = text[.._options.MaxInputChars];

        var prompt = summaryType switch
        {
            SummaryType.Concise =>
                $"Summarize the following text in 2-3 sentences, capturing only the most important points:\n\n{text}",
            SummaryType.Detailed =>
                $"Provide a detailed summary of the following text, covering all key concepts, arguments, and conclusions:\n\n{text}",
            _ => throw new ArgumentOutOfRangeException(nameof(summaryType))
        };

        var requestBody = new
        {
            model = _options.Model,
            prompt,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", requestBody, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        return json.GetProperty("response").GetString()
               ?? throw new InvalidOperationException("Ollama returned an empty response.");
    }
}
