# Money2Word

Converts monetary decimal amounts to English words. Supports amounts from $0.01 up to $999,999,999,999,999.99.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Running Locally

```bash
dotnet run --project Money2Word --launch-profile https
```

- **Web UI:** https://localhost:7220
- **Swagger (dev only):** https://localhost:7220/swagger

## Running Tests

```bash
dotnet test
```

## Running E2E Tests

One-time browser install (run after first build):

```powershell
pwsh Money2Word.E2ETests/bin/Debug/net10.0/playwright.ps1 install chromium
```

Run E2E tests only (headless Chromium, ~17s):

```bash
dotnet test Money2Word.E2ETests
```

## API

```
POST /api/show
Content-Type: application/json

{ "Amount": 1234.56 }
```

**Success response (200):**
```json
{ "Words": "ONE THOUSAND TWO HUNDRED AND THIRTY-FOUR DOLLARS AND FIFTY-SIX CENTS" }
```

**Error response (400) — RFC 7807:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Conversion failed",
  "status": 400,
  "detail": "Amount exceeds the maximum supported value of 999,999,999,999,999.99"
}
```

## Architecture

- **`Money2Word/Services/IMoney2WordService`** — core conversion contract; accepts `decimal`, returns `ConversionResult`
- **`Money2Word/Models/ConversionResult`** — discriminated result type; `IsSuccess` / `Words` / `ErrorMessage`
- **`Money2Word/Controllers/ApiController`** — REST endpoint at `POST /api/show`; returns RFC 7807 errors
- **`Money2Word/Controllers/HomeController`** — serves the glassmorphism dark UI (jQuery AJAX, no Bootstrap)
- **`Money2Word.Tests/`** — 32 unit tests with FluentAssertions and NSubstitute
- **`Money2Word.E2ETests/`** — 12 Playwright E2E tests using headless Chromium against a real Kestrel server

## Deployment

Azure DevOps pipelines are in `.azure-pipelines/`. The app deploys to the Azure App Service **Money2Word** via `Build and Deploy.yml`.
