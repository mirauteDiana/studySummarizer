namespace StudySummarizer.Infrastructure.LlamaClient;

public class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama3.2";
    public int MaxInputChars { get; set; } = 16_000;
}
