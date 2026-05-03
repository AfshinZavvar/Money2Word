# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Money2WordCore is an ASP.NET Core 10.0 web application that converts monetary amounts (decimal numbers) to English words. It exposes both an MVC web UI and a REST API endpoint.

## Build & Run Commands

```powershell
# Restore dependencies
dotnet restore Money2WordCore.sln

# Build
dotnet build Money2WordCore.sln

# Run tests
dotnet test

# Run (development, HTTPS on localhost:7220)
dotnet run --project Money2Word --launch-profile https
```

Swagger UI is available at `/swagger` in development mode.

### Docker

```powershell
# Build and start (HTTP on localhost:8080)
docker compose up --build

# Stop
docker compose down
```

The compose file expects `ApplicationInsights__ConnectionString` — supply it via a `.env` file (format documented in `.env.example`) or as an environment variable; leave it empty to run without telemetry.

## Architecture

### Request Flow

1. **MVC UI** — `HomeController` → `Views/Home/Index.cshtml` (glassmorphism dark UI, jQuery AJAX)
2. **REST API** — `POST /api/show` via `ApiController` → `IMoney2WordService.Convert(decimal)` → `ConversionResult`

### Client-side (`wwwroot/js/site.js`)

Input is enforced on every keystroke via `FormatCurrencyInput`:
- Whole-number part clamped to **15 digits** (mirrors `MaxSupportedAmount = 999,999,999,999,999.99`)
- Decimal part clamped to **2 digits**
- Comma thousands-separators inserted automatically; stripped before the API call

### Core Service

`IMoney2WordService` / `Money2WordService` (registered as **singleton** — it is stateless) is the single business logic component. It:
- Accepts a raw `decimal` (not an HTTP model — the service is independent of the web layer)
- Returns a `ConversionResult` discriminated type (`IsSuccess` / `Words` / `ErrorMessage`)
- Supports amounts up to $999,999,999,999,999.99 (enforced by a `MaxSupportedAmount` constant)
- Splits dollars and cents using `decimal` arithmetic with `Math.Round(..., MidpointRounding.AwayFromZero)` to avoid floating-point precision loss
- Builds scale chunks using a `Stack<string>` (LIFO, avoids O(n²) `List.Insert(0,...)`)
- Lookup tables (`Ones`, `Tens`) use `FrozenDictionary` — read-optimised, allocated once at startup
- Uses `ILogger` throughout

### Result Pattern

`ConversionResult` (`Money2Word/Models/ConversionResult.cs`) is a `readonly record struct` with private constructors and two factory methods:
- `ConversionResult.Success(string words)`
- `ConversionResult.Failure(string errorMessage)`

`IsSuccess` carries `[MemberNotNullWhen(true, nameof(Words))]` and `[MemberNotNullWhen(false, nameof(ErrorMessage))]` so the compiler enforces null-safety at call sites — no null-forgiving operators needed after an `IsSuccess` check.

### Models

- `InputModel` — `record` with `[Required]`, `[Range(0.01, 999...)]`, and `init`-only `decimal Amount`
- `ConversionResult` — `readonly record struct` discriminated result type (see above)

### API Errors

The API returns RFC 7807 `application/problem+json` for all error responses:
- Invalid model: `ValidationProblem(ModelState)` — 400 with field-level errors
- Business failure: `BadRequest(new ProblemDetails {...})` — 400 with `title` + `detail`

### Telemetry (`Money2Word/Telemetry/`)

- `RequestFilterProcessor` — `ITelemetryProcessor` that drops `/swagger` requests and successful home-page GETs to reduce noise
- `AppVersionTelemetryInitializer` — `ITelemetryInitializer` that stamps `ApplicationVersion` and `Environment` on every telemetry item
- `Money2WordService` tracks an `AmountConverted` custom event via `TelemetryClient`
- `ApiController` tracks `ConversionDurationMs` metric via `TelemetryClient`

### Dependency Injection (Program.cs)

```csharp
builder.Services.AddSingleton<IMoney2WordService, Money2WordService>();
builder.Services.AddSingleton(TimeProvider.System);   // injected into HomeController
builder.Services.AddProblemDetails();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddApplicationInsightsTelemetryProcessor<RequestFilterProcessor>();
builder.Services.AddSingleton<ITelemetryInitializer, AppVersionTelemetryInitializer>();
builder.Services.AddSwaggerGen(...);
```

### C# Conventions

- All C# files use **file-scoped namespaces** (`namespace Foo.Bar;`)
- Primary constructors on all controllers, services, and telemetry types — no backing fields; parameters are used directly in method bodies
- `FrozenDictionary<int, string>` for `Ones` / `Tens` lookup tables (read-optimised, allocated once at startup)
- `[MemberNotNullWhen]` on `ConversionResult.IsSuccess` for compiler-enforced null flow at call sites
- **Central Package Management** — all NuGet versions live in `Directory.Packages.props`; csproj files have no `Version` attributes
- `Directory.Build.props` sets `TargetFramework`, `Nullable`, `ImplicitUsings`, `LangVersion` for both projects
- SDK pinned to `10.0.203` (latestPatch roll-forward) in `global.json`
- `Program` is declared as `public class` (not `public static class`) so it can be used as a type argument for `WebApplicationFactory<Program>` in E2E tests

## Tests

**44 tests total** across two test projects.

`Money2Word.Tests/` uses **xunit.v3** (3.2.2) + FluentAssertions + NSubstitute (32 unit tests, >80% coverage):
- `Services/Money2WordServiceTests.cs` — parametrised correctness (including teens, zero-dollar, multi-scale with gap chunks), precision regression, boundary tests
- `Controllers/ApiControllerTests.cs` — controller tests with mocked `IMoney2WordService`

`Money2Word.E2ETests/` uses **xunit.v3** + Microsoft.Playwright 1.52.0 + FluentAssertions + Microsoft.AspNetCore.Mvc.Testing (12 E2E tests):
- `Fixtures/AppHostFixture.cs` — extends `WebApplicationFactory<Program>`; starts a real Kestrel server on a free port (via `FindFreePort()` using a socket bind) so Playwright can connect; exposes `BaseUrl`; implements `IAsyncLifetime`
- `Fixtures/PlaywrightFixture.cs` — launches headless Chromium via `Playwright.CreateAsync()`; implements `IAsyncLifetime`; shared per test class via `IClassFixture<T>`
- `Tests/ConverterPageTests.cs` — 12 tests covering: page load, input auto-formatting, 15-digit cap, 2-decimal cap, valid conversion result, word highlighting, empty/below-min validation, max amount, Enter key, button disabled during AJAX, result cleared on new submit

**One-time Playwright browser install** (run after first build):
```powershell
pwsh Money2Word.E2ETests/bin/Debug/net10.0/playwright.ps1 install chromium
```

**Run E2E tests only:**
```powershell
dotnet test Money2Word.E2ETests   # headless, ~17s
```

**Headed mode** (local debugging): flip `Headless = true` → `Headless = false` in `PlaywrightFixture.cs` and optionally add `SlowMo = 500`.

## CI/CD

Azure DevOps pipelines are in `.azure-pipelines/`:
- **Build and Deploy.yml** — manually triggered; builds then deploys to Azure App Service "Money2Word" via the `Prod` environment gate
- **Build Only.yml** — CI pipeline; triggers on push and PRs targeting `main` or `develop`
- **build-template.yml** — shared build steps: `UseDotNet@2` (reads `global.json`) → `DotNetCoreCLI@2 restore` → `DotNetCoreCLI@2 build` → `DotNetCoreCLI@2 test (unit, **/*.Tests.csproj)` → `PowerShell@2 playwright.ps1 install chromium --with-deps` → `DotNetCoreCLI@2 test (E2E, **/*.E2ETests.csproj)` → `DotNetCoreCLI@2 publish` → `PublishPipelineArtifact@1` (artifact: `drop`)

## Changesets

To add a changeset, write a new file to the `.changeset` directory.
The file should be named `0000-your-change.md`.
