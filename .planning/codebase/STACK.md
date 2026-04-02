# Technology Stack

**Analysis Date:** 2026-04-02

## Languages

**Primary:**
- C# with nullable reference types enabled - application code in `Program.cs`, `Controllers/`, `Services/`, `Repositories/`, `Data/`, `Entities/`, and `Hubs/`

**Secondary:**
- JSON - runtime configuration in `appsettings.json`, `appsettings.Development.json`, and `Properties/launchSettings.json`
- YAML - CI workflow definition in `.github/workflows/build-backend.yml`
- CSHTML - email templates copied as content from `EmailTemplates/**/*.cshtml` via `ePermitsApp.csproj`

## Runtime

**Environment:**
- .NET 8 / ASP.NET Core Web SDK targeting `net8.0` in `ePermitsApp.csproj`

**Package Manager:**
- NuGet via SDK-style project restore in `ePermitsApp.csproj`
- Lockfile: missing (`packages.lock.json` not detected)

## Frameworks

**Core:**
- ASP.NET Core Web API - HTTP host, dependency injection, controllers, auth, CORS, and SignalR startup in `Program.cs`
- Entity Framework Core 8 with SQL Server provider - ORM, migrations, and schema management in `ePermitsApp.csproj`, `Data/ApplicationDbContext.cs`, and `Extensions/DatabaseExtensions.cs`

**Testing:**
- Not detected

**Build/Dev:**
- Swashbuckle.AspNetCore 6.6.2 - Swagger/OpenAPI generation in `ePermitsApp.csproj` and `Program.cs`
- GitHub Actions - restore/publish pipeline in `.github/workflows/build-backend.yml`
- Launch profiles for local development - `http`, `https`, and IIS Express profiles in `Properties/launchSettings.json`

## Key Dependencies

**Critical:**
- `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.23 - JWT bearer authentication for API endpoints and SignalR in `ePermitsApp.csproj`, `Program.cs`, and `Services/AuthService.cs`
- `Microsoft.EntityFrameworkCore` 8.0.23 - data access abstraction used across repositories and `Data/ApplicationDbContext.cs`
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.23 - SQL Server database provider configured in `Program.cs`
- `AutoMapper` 16.0.0 - DTO/entity mapping profiles registered from `Mappings/*.cs` in `Program.cs`
- `MailKit` 4.15.1 - SMTP email delivery in `Services/BackgroundEmailSender.cs`

**Infrastructure:**
- `RazorLight` 2.3.1 - renders email templates from `EmailTemplates/` in `Program.cs` and `Services/RazorViewRenderer.cs`
- `QuestPDF` 2024.12.2 - builds application summary PDFs in `Services/ApplicationPdfService.cs`
- `PdfSharpCore` 1.3.65 - merges uploaded PDF files into generated packets in `Services/ApplicationPdfService.cs`
- `DocumentFormat.OpenXml` 3.2.0 - reads DOCX uploads and imports/exports XLSX maintenance sheets in `Services/ApplicationPdfService.cs` and `Services/AdminMaintenanceService.cs`

## Configuration

**Environment:**
- Configuration sources are ASP.NET Core defaults plus user secrets support signaled by `<UserSecretsId>` in `ePermitsApp.csproj`
- Runtime sections consumed directly in code are `ConnectionStrings:DefaultConnection`, `Jwt`, `FileStorage`, `EmailSettings`, and `SampleUsers` in `Program.cs`, `Services/AuthService.cs`, `Services/DocumentDownloadService.cs`, and `Controllers/FileController.cs`
- `appsettings.json` and `appsettings.Development.json` exist; `appsettings.json` currently contains secret-bearing sections and should be treated as sensitive configuration, not source-of-truth documentation

**Build:**
- Project and package configuration lives in `ePermitsApp.csproj`
- Development launch configuration lives in `Properties/launchSettings.json`
- CI publish configuration lives in `.github/workflows/build-backend.yml`

## Platform Requirements

**Development:**
- .NET 8 SDK
- SQL Server / SQL Server Express reachable through `ConnectionStrings:DefaultConnection`
- Writable local filesystem path for `FileStorage:BasePath` used by `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`, and `Controllers/FileController.cs`
- SMTP credentials for `EmailSettings` used by `Services/BackgroundEmailSender.cs`

**Production:**
- Inference from `.github/workflows/build-backend.yml`: publish target is `win-x64` self-contained, so the current delivery artifact is a Windows x64 deployment package
- Database migrations are applied automatically on startup by `Extensions/DatabaseExtensions.cs`

---

*Stack analysis: 2026-04-02*
