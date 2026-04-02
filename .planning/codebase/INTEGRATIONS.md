# External Integrations

**Analysis Date:** 2026-04-02

## APIs & External Services

**Email Delivery:**
- SMTP mail relay - outbound notification delivery from `Services/BackgroundEmailSender.cs`
  - SDK/Client: `MailKit`
  - Auth: `EmailSettings:SmtpUsername` and `EmailSettings:SmtpPassword`
  - Configuration model: `Models/EmailSettings.cs`
  - Message templating: `RazorLight` with templates in `EmailTemplates/` via `Services/EmailService.cs` and `Services/RazorViewRenderer.cs`

**Realtime Messaging:**
- SignalR hub - authenticated real-time chat updates for permit applications in `Hubs/ChatHub.cs`, `Controllers/ChatController.cs`, and `Program.cs`
  - SDK/Client: `Microsoft.AspNetCore.SignalR`
  - Auth: JWT bearer token passed through the `/chatHub` query string in `Program.cs`

**Document Processing:**
- Local document conversion/packaging pipeline - not an external network API, but a significant integration surface in `Services/ApplicationPdfService.cs` and `Services/AdminMaintenanceService.cs`
  - SDK/Client: `QuestPDF`, `PdfSharpCore`, `DocumentFormat.OpenXml`
  - Auth: Not applicable

**External HTTP APIs:**
- Not detected. No `HttpClient`, REST SDK, or third-party HTTP integrations were found in `Controllers/`, `Services/`, `Repositories/`, `Data/`, or `Hubs/`.

## Data Storage

**Databases:**
- Microsoft SQL Server
  - Connection: `ConnectionStrings:DefaultConnection`
  - Client: Entity Framework Core SQL Server provider in `ePermitsApp.csproj` and `Program.cs`
  - Schema/migrations: `Data/ApplicationDbContext.cs`, `Migrations/`, and `Extensions/DatabaseExtensions.cs`

**File Storage:**
- Local filesystem only
  - Base path config: `FileStorage:BasePath`
  - Write paths: `Services/BuildingPermitService.cs` and `Services/CoOAppService.cs`
  - Read/download paths: `Services/DocumentDownloadService.cs` and `Controllers/FileController.cs`

**Caching:**
- In-memory template caching inside `RazorLightEngine` in `Program.cs`
- No distributed cache service detected

## Authentication & Identity

**Auth Provider:**
- Custom username/email plus password authentication with JWT issuance
  - Implementation: `Services/AuthService.cs` hashes passwords with `SHA256`, issues JWTs from `Jwt` config values, and `Program.cs` configures `JwtBearer` authentication for API routes and SignalR
  - Token consumers: authorized controllers in `Controllers/` and `Hubs/ChatHub.cs`

## Monitoring & Observability

**Error Tracking:**
- None detected

**Logs:**
- Built-in ASP.NET Core / `ILogger<T>` logging configured through `Logging` sections in `appsettings.json` and `appsettings.Development.json`
- Examples: `Services/BackgroundEmailSender.cs`, `Services/EmailService.cs`, and `Extensions/DatabaseExtensions.cs`

## CI/CD & Deployment

**Hosting:**
- Not explicitly declared
- Inference from `.github/workflows/build-backend.yml`: the repository builds a self-contained `win-x64` publish artifact, which suggests Windows-based deployment packaging

**CI Pipeline:**
- GitHub Actions in `.github/workflows/build-backend.yml`
  - Restores with `dotnet restore`
  - Publishes with `dotnet publish -c Release -r win-x64 --self-contained true`
  - Uploads the `publish/` directory as an artifact

## Environment Configuration

**Required env vars:**
- No standalone environment variable files were detected
- Required configuration keys consumed by code:
- `ConnectionStrings:DefaultConnection` for SQL Server in `Program.cs`
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, and `Jwt:ExpiryInHours` in `Program.cs` and `Services/AuthService.cs`
- `FileStorage:BasePath` in `Program.cs`, `Services/DocumentDownloadService.cs`, and `Controllers/FileController.cs`
- `EmailSettings:SmtpHost`, `EmailSettings:SmtpPort`, `EmailSettings:SmtpUsername`, `EmailSettings:SmtpPassword`, `EmailSettings:SenderEmail`, `EmailSettings:SenderName`, and `EmailSettings:EnableSsl` in `Program.cs` and `Services/BackgroundEmailSender.cs`

**Secrets location:**
- `appsettings.json` currently contains secret-bearing values
- ASP.NET Core user secrets are enabled by `<UserSecretsId>` in `ePermitsApp.csproj`
- `.env` files were not detected

## Webhooks & Callbacks

**Incoming:**
- None detected
- Realtime callback-style endpoint: SignalR hub mapped at `/chatHub` in `Program.cs`, but it is not implemented as a webhook receiver

**Outgoing:**
- SMTP outbound mail traffic from `Services/BackgroundEmailSender.cs`
- No outbound webhook or callback POST clients detected

---

*Integration audit: 2026-04-02*
