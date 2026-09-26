# Money2Word

[![CI](https://github.com/AfshinZavvar/Money2WordCore/actions/workflows/ci.yml/badge.svg)](https://github.com/AfshinZavvar/Money2WordCore/actions/workflows/ci.yml)

Money2Word is an ASP.NET Core web application that converts dollar amounts into uppercase English words. It provides a browser UI and a JSON API, supports cents, and accepts values from `$0.01` through `$999,999,999,999,999.99`.

Example: `$1,234.56` becomes `ONE THOUSAND TWO HUNDRED AND THIRTY-FOUR DOLLARS AND FIFTY-SIX CENTS`.

This README is the single source of truth for project documentation. Third-party license files remain beside their vendored libraries.

## Technology

- ASP.NET Core MVC targeting .NET 11
- jQuery for the browser interaction
- Swagger/OpenAPI in the Development environment
- Application Insights for request, conversion, and version telemetry
- xUnit v3, FluentAssertions, and NSubstitute for automated tests
- Playwright with headless Chromium for browser tests
- GitHub Actions for CI and Azure App Service deployment

NuGet versions are managed centrally in `Directory.Packages.props`. Shared compiler settings and the target framework are in `Directory.Build.props`; `global.json` pins the required SDK.

## Prerequisites

- The .NET SDK version specified in `global.json`
- Chromium installed through Playwright when running end-to-end tests
- Docker Desktop or another Compose-compatible runtime only when running the containerized app

The repository currently targets a .NET 11 release-candidate SDK. Install that exact SDK, or a compatible patch allowed by `global.json`, before restoring the solution.

## Build and run

```powershell
dotnet restore Money2WordCore.sln
dotnet build Money2WordCore.sln
dotnet run --project Money2Word --launch-profile https
```

The HTTPS launch profile serves the UI at `https://localhost:7220`; Swagger is available at `https://localhost:7220/swagger` in Development. The `http` profile uses `http://localhost:5000`.

To run with Docker on `http://localhost:8080`:

```powershell
docker compose up --build
```

The default command automatically applies `docker-compose.override.yml` and runs the app in Development. Use `docker compose -f docker-compose.yml up --build` for the production-shaped configuration. Copy `.env.example` to `.env` to supply `ApplicationInsights__ConnectionString` or change `MONEY2WORD_PORT`; leave telemetry empty to run without it. Stop the containers with `docker compose down`.

## API

`POST /api/show` accepts JSON with an `Amount` property. The server also permits a numeric string because the browser removes grouping separators before sending the request.

```http
POST /api/show
Content-Type: application/json

{ "Amount": 1234.56 }
```

A successful request returns HTTP 200:

```json
{
  "Words": "ONE THOUSAND TWO HUNDRED AND THIRTY-FOUR DOLLARS AND FIFTY-SIX CENTS"
}
```

Invalid input or a conversion failure returns HTTP 400 using RFC 7807 problem details. Model-validation failures include field-level errors; business failures include a `title` and `detail`.

## Architecture

The main request paths are:

```text
Browser -> HomeController -> Razor view -> site.js -> POST /api/show
                                             |
API client -> ApiController -----------------+-> IMoney2WordService -> ConversionResult
```

- `Money2Word/Services/Money2WordService.cs` contains the stateless conversion algorithm. It uses decimal arithmetic, rounds cents away from zero, and assembles three-digit chunks through trillion scale.
- `Money2Word/Models/ConversionResult.cs` is a readonly result type with success and failure factories plus compiler-assisted null-state annotations.
- `Money2Word/Controllers/ApiController.cs` validates requests, invokes the converter, records conversion duration, and returns JSON or problem details.
- `Money2Word/Controllers/HomeController.cs` serves the MVC UI and error page.
- `Money2Word/Telemetry/` filters low-value requests and enriches Application Insights telemetry with application version and environment.
- `Money2Word.Tests/` contains service and controller tests.
- `Money2Word.E2ETests/` starts a real Kestrel server and exercises the UI with Playwright.

`Money2WordService` is registered as a singleton because it is stateless. `TimeProvider.System` is injected into `HomeController` to keep time-dependent behavior testable.

## Tests

Run service and controller tests:

```powershell
dotnet test Money2Word.Tests
```

After the first build, install the Playwright browser once and run the browser suite:

```powershell
pwsh Money2Word.E2ETests/bin/Debug/net11.0/playwright.ps1 install chromium
dotnet test Money2Word.E2ETests
```

Run the full solution with `dotnet test Money2WordCore.sln`. To target a test class or method, use `--filter`, for example:

```powershell
dotnet test Money2Word.Tests --filter "FullyQualifiedName~Money2WordServiceTests"
```

## Front-end maintenance

The UI uses a warm editorial design: warm ivory surfaces, deep forest green actions, and copper accents. `Playfair Display` is reserved for display text and `Plus Jakarta Sans` is used for interface and body text; both are loaded by `Money2Word/Views/Shared/_Layout.cshtml`.

The canonical palette is defined as CSS custom properties at the start of `Money2Word/wwwroot/css/site.css`. Preserve the intentional `card::before` accent, the single-line card title, and the content-dependent error panel when changing the layout.

The following selectors form a contract between `Money2Word/Views/Home/Index.cshtml`, the stylesheet, the browser tests, and `Money2Word/wwwroot/js/site.js`:

| Selector | Purpose |
| --- | --- |
| `#Amount` | Formatted amount input and validation state |
| `#btnSubmit` | Convert action and in-flight disabled state |
| `.btn-text` | Button status text |
| `#resultPanel` | Result visibility |
| `#responseAmount` | Generated word output |
| `#responseError` | Accessible validation and request errors |
| `.word-highlight` | Emphasis for scale and currency words |

The browser formatter limits the whole-number portion to 15 digits, limits cents to two digits, and inserts thousands separators. Keep those rules aligned with `InputModel` and `Money2WordService.MaxSupportedAmount`.

## Code conventions

- Use file-scoped namespaces and nullable reference types.
- Keep package versions in `Directory.Packages.props`, not individual project files.
- Keep common framework and compiler settings in `Directory.Build.props`.
- Preserve `Program` as a public class because the end-to-end host uses it as `WebApplicationFactory<Program>`.
- Keep the service independent of HTTP types; web-specific validation and responses belong in controllers and models.

## CI/CD

GitHub Actions definitions live in `.github/workflows/`:

- `ci.yml` runs the complete build and all tests for pushes and pull requests targeting `main` or `develop`.
- `deploy.yml` is manually triggered from `main`; it repeats the complete quality gate, packages the web application, authenticates to Azure through OIDC, and deploys to the `Money2Word` App Service.
- `build.yml` is the reusable build, test, and optional packaging workflow shared by CI and deployment. Deployment packages are self-contained for the App Service's Windows x86 worker because the application targets a .NET preview runtime that App Service does not install globally.

Production uses the protected `production` GitHub environment. Its `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, and `AZURE_SUBSCRIPTION_ID` secrets identify the federated Azure identity; no client secret or publish profile is stored in GitHub.

## Repository layout

```text
Money2Word/              Web application, conversion service, static assets, and views
Money2Word.Tests/        Unit and controller tests
Money2Word.E2ETests/     Playwright browser tests and test web host
.github/workflows/       GitHub Actions CI/CD definitions
mockup/                  Static UI mockup
Directory.Build.props    Shared .NET project settings
Directory.Packages.props Central NuGet package versions
global.json              Required .NET SDK
```
