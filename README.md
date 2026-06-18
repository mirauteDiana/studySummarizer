# StudySummarizer

REST API for uploading, managing, and AI-summarizing academic documents (PDF, DOCX, TXT).

## Stack

- ASP.NET Core 10 / C#
- SQLite + Entity Framework Core
- Ollama (local LLM) for summarization
- Clean architecture (Domain / Application / Infrastructure / API)

## Setup

```bash
dotnet restore
dotnet ef database update --project StudySummarizer.Infrastructure --startup-project StudySummarizer
dotnet run --project StudySummarizer
```

API available at `https://localhost:{port}/swagger`.

Summarization requires a running [Ollama](https://ollama.com) instance. Configure its URL and the model in `appsettings.json` under the `Ollama` section.

## Endpoints

### Documents

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/documents` | Upload a document (multipart/form-data) |
| GET | `/api/documents` | List all documents |
| GET | `/api/documents/{id}` | Get document metadata |
| GET | `/api/documents/{id}/file` | Download file |
| DELETE | `/api/documents/{id}` | Delete document |

### Summaries

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/documents/{id}/summarize` | Generate a summary |
| GET | `/api/documents/{id}/summary` | Get existing summary |
| PATCH | `/api/documents/{id}/summary` | Regenerate summary |
