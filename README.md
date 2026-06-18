# StudySummarizer

REST API for uploading and managing academic documents (PDF, DOCX, TXT).

## Stack

- ASP.NET Core 10 / C#
- SQLite + Entity Framework Core
- Clean architecture (Domain / Application / Infrastructure / API)

## Setup

```bash
dotnet restore
dotnet ef database update --project StudySummarizer.Infrastructure --startup-project StudySummarizer
dotnet run --project StudySummarizer
```

API available at `https://localhost:{port}/swagger`.

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/documents` | Upload a document (multipart/form-data) |
| GET | `/api/documents` | List all documents |
| GET | `/api/documents/{id}` | Get document metadata |
| GET | `/api/documents/{id}/download` | Download file |
| DELETE | `/api/documents/{id}` | Delete document |
