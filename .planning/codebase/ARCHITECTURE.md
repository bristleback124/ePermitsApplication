# Architecture

**Analysis Date:** 2026-04-02

## Pattern Overview

**Overall:** Layered ASP.NET Core Web API using Controller -> Service -> Repository -> EF Core `DbContext`, with AutoMapper-based DTO translation and a few direct infrastructure services for cross-cutting work.

**Key Characteristics:**
- `Program.cs` wires almost every feature explicitly in the composition root rather than through assembly scanning.
- HTTP endpoints live in `Controllers/`, most business logic lives in `Services/`, and persistence logic lives in `Repositories/` over `Data/ApplicationDbContext.cs`.
- Domain data is split between reference entities in `Entities/`, workflow entities in `Entities/BuildingPermit/` and `Entities/CoOApp/`, and user/application models in `Models/` plus `Entities/Application.cs` and `Entities/Message.cs`.

## Layers

**Composition Root / Host Setup:**
- Purpose: Build the web host, register dependencies, enable middleware, and map endpoints.
- Location: `Program.cs`
- Contains: DI registrations, JWT auth setup, CORS policy, Swagger, SignalR hub mapping, database migration startup, options binding.
- Depends on: `Data/ApplicationDbContext.cs`, `Extensions/DatabaseExtensions.cs`, `Mappings/*.cs`, `Services/*.cs`, `Repositories/*.cs`, `Hubs/ChatHub.cs`.
- Used by: The ASP.NET Core host process.

**HTTP API Layer:**
- Purpose: Receive HTTP requests, enforce route/authorization metadata, validate model state, and translate service results to HTTP responses.
- Location: `Controllers/`
- Contains: Feature controllers such as `Controllers/ApplicationsController.cs`, `Controllers/BuildingPermitsController.cs`, `Controllers/CoOAppsController.cs`, `Controllers/AuthController.cs`, `Controllers/AdminMaintenanceController.cs`, `Controllers/FileController.cs`.
- Depends on: Service interfaces from `Services/Interfaces/`, AutoMapper in some CRUD controllers, `Microsoft.AspNetCore.Mvc`.
- Used by: Frontend/API consumers over REST endpoints.

**Service Layer:**
- Purpose: Own business rules, workflow transitions, permission checks, file persistence orchestration, email notifications, and DTO shaping that is too feature-specific for repositories.
- Location: `Services/`
- Contains: CRUD services like `Services/ProvinceService.cs`, workflow services like `Services/ApplicationService.cs`, form services like `Services/BuildingPermitService.cs` and `Services/CoOAppService.cs`, infrastructure services like `Services/EmailService.cs`, `Services/ApplicationPdfService.cs`, `Services/DocumentDownloadService.cs`, `Services/CurrentUserService.cs`.
- Depends on: Repository interfaces, `Data/ApplicationDbContext.cs` in some cases, helpers such as `Helpers/ApplicationWorkflowDefinitions.cs` and `Helpers/FilePathHelper.cs`, AutoMapper, options/configuration, logging.
- Used by: Controllers and other services.

**Repository Layer:**
- Purpose: Encapsulate EF Core queries and persistence operations for aggregate roots and lookup entities.
- Location: `Repositories/` and `Repositories/Interfaces/`
- Contains: Repositories such as `Repositories/ApplicationRepository.cs`, `Repositories/BuildingPermitRepository.cs`, `Repositories/MessageRepository.cs`, `Repositories/ProvinceRepository.cs`.
- Depends on: `Data/ApplicationDbContext.cs`, EF Core include/query APIs, `Extensions/QueryableExtensions.cs` for sorting and pagination.
- Used by: Services.

**Persistence / Data Model Layer:**
- Purpose: Define the relational model, entity constraints, relationships, global query filters, and seed data.
- Location: `Data/ApplicationDbContext.cs`, `Data/Seeders/`, `Migrations/`
- Contains: `DbSet<>` declarations, `OnModelCreating` configuration, startup seed runner `Data/Seeders/DatabaseSeederRunner.cs`, migration history in `Migrations/`.
- Depends on: Entities in `Entities/`, `Entities/BuildingPermit/`, `Entities/CoOApp/`, plus models in `Models/`.
- Used by: Repositories, some services, startup migration extension.

**DTO / Mapping Layer:**
- Purpose: Separate API/request-response shapes from EF entities and map between them.
- Location: `DTOs/` and `Mappings/`
- Contains: Request/response DTOs such as `DTOs/BuildingPermitDtos.cs`, `DTOs/ApplicationDtos.cs`, `DTOs/AdminMaintenanceDtos.cs`, and AutoMapper profiles such as `Mappings/BuildingPermitProfile.cs`, `Mappings/ApplicationProfile.cs`, `Mappings/CoOAppProfile.cs`.
- Depends on: Entities, helper utilities such as `Helpers/FilePathHelper.cs`.
- Used by: Controllers and services.

**Real-Time / Background Infrastructure:**
- Purpose: Support chat and queued email delivery outside the request pipeline.
- Location: `Hubs/ChatHub.cs`, `Services/ChatService.cs`, `Services/BackgroundEmailSender.cs`, `Services/EmailService.cs`
- Contains: SignalR hub grouping, chat persistence/service logic, `Channel<EmailMessage>` queue processing, Razor template rendering.
- Depends on: Authenticated user context, repositories, RazorLight, MailKit configuration.
- Used by: `Controllers/ChatController.cs`, `Controllers/EmailController.cs`, and `Program.cs`.

## Data Flow

**Lookup CRUD Flow:**

1. A controller such as `Controllers/ProvincesController.cs` receives a request and calls an interface like `IProvinceService`.
2. The service in `Services/ProvinceService.cs` applies business rules such as active/deleted handling and usage checks, then calls `IProvinceRepository`.
3. The repository in `Repositories/ProvinceRepository.cs` queries `Data/ApplicationDbContext.cs` and returns entities.
4. AutoMapper profiles such as `Mappings/ProvinceProfile.cs` convert entities to DTOs returned by the controller.

**Permit Submission Flow:**

1. `Controllers/BuildingPermitsController.cs` accepts a multipart form DTO from `DTOs/BuildingPermitDtos.cs`.
2. `Services/BuildingPermitService.cs` maps the form DTO into `Entities/BuildingPermit/BuildingPermit.cs`, creates an `Entities/Application.cs` workflow record, attaches required `ApplicationDepartmentReview` rows from `Helpers/ApplicationWorkflowDefinitions.cs`, and writes uploaded files under the configured base path.
3. `Repositories/BuildingPermitRepository.cs` persists the entity graph through `Data/ApplicationDbContext.cs`.
4. The service updates the generated formatted ID, stores serialized file metadata with `Helpers/FilePathHelper.cs`, then enqueues templated emails through `Services/EmailService.cs`.

**Application Review Flow:**

1. `Controllers/ApplicationsController.cs` calls `Services/ApplicationService.cs` for dashboard, assignment, and status actions.
2. `Services/ApplicationService.cs` loads rich aggregates through `Repositories/ApplicationRepository.cs`, filters them using the authenticated user from `Services/CurrentUserService.cs`, and validates transitions against `Helpers/ApplicationWorkflowDefinitions.cs`.
3. The service persists changes through repository updates, sends notification emails, and maps entities to dashboard/detail DTOs through `Mappings/ApplicationProfile.cs`.

**Chat Flow:**

1. `Controllers/ChatController.cs` handles REST chat actions and uses `IHubContext<ChatHub>` to notify subscribed clients.
2. `Services/ChatService.cs` validates the caller against the owning application and creates or reads `Entities/Message.cs` records through `Repositories/MessageRepository.cs`.
3. `Hubs/ChatHub.cs` manages membership in per-application SignalR groups at `/chatHub`.

**Document Download / PDF Assembly Flow:**

1. `Controllers/ApplicationsController.cs` exposes `download-all` and delegates to `Services/ApplicationPdfService.cs`.
2. `Services/ApplicationPdfService.cs` requests normalized file paths from `Services/DocumentDownloadService.cs`, which itself reuses `IApplicationService` detail DTOs instead of querying the database directly.
3. `Services/ApplicationPdfService.cs` renders a summary PDF with QuestPDF, inlines images and DOCX text when possible, then merges uploaded PDFs with PdfSharpCore.

**State Management:**
- Request state is mostly stateless and database-backed.
- Authenticated user context is derived per request through `Services/CurrentUserService.cs` and `IHttpContextAccessor`.
- Long-running email work is offloaded through `Channel<EmailMessage>` plus `Services/BackgroundEmailSender.cs`.
- Real-time client session state is limited to SignalR group membership in `Hubs/ChatHub.cs`.

## Key Abstractions

**Application Workflow Aggregate:**
- Purpose: Represent a submitted permit request plus its review state.
- Examples: `Entities/Application.cs`, `Entities/ApplicationDepartmentReview.cs`, `Services/ApplicationService.cs`, `Repositories/ApplicationRepository.cs`
- Pattern: Aggregate root with specialized child graphs for building permits and certificates of occupancy.

**Permit Form Subgraphs:**
- Purpose: Separate the two major permit types while keeping a shared parent workflow.
- Examples: `Entities/BuildingPermit/BuildingPermit.cs`, `Entities/BuildingPermit/BuildingPermitAppInfo.cs`, `Entities/CoOApp/CoOApp.cs`, `Entities/CoOApp/CoOAppProf.cs`
- Pattern: One-to-one owned-by-application subgraphs loaded with eager `Include` chains in repositories.

**Reference Data Maintenance Model:**
- Purpose: Centralize management of lookup tables such as provinces, departments, requirements, and permit metadata.
- Examples: `Controllers/AdminMaintenanceController.cs`, `Services/AdminMaintenanceService.cs`, `Entities/Province.cs`, `Entities/RequirementCategory.cs`
- Pattern: Generic admin service over a shared EF context instead of one repository/service pair per admin workflow.

**DTO + AutoMapper Boundary:**
- Purpose: Prevent controllers from exposing EF entities directly and reshape nested graphs for UI consumption.
- Examples: `DTOs/ApplicationDtos.cs`, `DTOs/BuildingPermitDtos.cs`, `Mappings/ApplicationProfile.cs`, `Mappings/BuildingPermitProfile.cs`
- Pattern: Feature-specific DTO files paired with a matching AutoMapper profile.

**File Metadata Serialization:**
- Purpose: Persist uploaded file references inside entity string fields while still returning structured metadata to clients.
- Examples: `Helpers/FilePathHelper.cs`, `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`, `Mappings/ApplicationProfile.cs`
- Pattern: Serialize `FileMetadataDto` collections into delimited strings at write time and deserialize during mapping/read flows.

## Entry Points

**Web Application Startup:**
- Location: `Program.cs`
- Triggers: Process start.
- Responsibilities: Register services/repositories, bind config, configure auth/CORS/Swagger, apply migrations, map controllers, map `/chatHub`, and start the app.

**Database Migration Bootstrap:**
- Location: `Extensions/DatabaseExtensions.cs`
- Triggers: `await app.MigrateDatabaseAsync()` inside `Program.cs`.
- Responsibilities: Open a scope, inspect pending migrations, apply them, and log startup migration status.

**REST API Surface:**
- Location: `Controllers/`
- Triggers: Incoming HTTP requests under routes such as `api/building-permits`, `api/applications`, `api/auth`, `api/admin/maintenance`.
- Responsibilities: Request binding, response status mapping, auth attributes, and service invocation.

**SignalR Hub Surface:**
- Location: `Hubs/ChatHub.cs`
- Triggers: WebSocket/SignalR connection to `/chatHub`.
- Responsibilities: Authorize clients and join or leave per-application groups.

**Background Email Worker:**
- Location: `Services/BackgroundEmailSender.cs`
- Triggers: Host startup plus messages written into the channel by `Services/EmailService.cs`.
- Responsibilities: Dequeue `EmailMessage` work items and deliver them using configured email settings and templates.

## Error Handling

**Strategy:** Service methods enforce domain rules and return either nullable results, tuples with success/message payloads, or throw `InvalidOperationException` for unrecoverable validation errors. Controllers translate those outcomes into HTTP responses.

**Patterns:**
- Controllers often guard with `ModelState.IsValid` and return `BadRequest(ModelState)` as in `Controllers/ApplicationsController.cs` and `Controllers/BuildingPermitsController.cs`.
- Service methods frequently return `(bool Success, string Message, ...)` tuples instead of custom result types, as in `Services/ApplicationService.cs` and `Services/BuildingPermitService.cs`.
- Notification code catches and logs exceptions internally so email failures do not fail the main request path, as in `Services/ApplicationService.cs` and `Services/BuildingPermitService.cs`.
- Startup auth configuration throws immediately if `Jwt:Key` is missing in `Program.cs`.

## Cross-Cutting Concerns

**Logging:** `ILogger<T>` is used in services and startup extensions, especially in `Services/ApplicationService.cs`, `Services/BuildingPermitService.cs`, `Services/EmailService.cs`, and `Extensions/DatabaseExtensions.cs`.

**Validation:** Validation is a mix of ASP.NET model binding, manual service guard clauses, and EF Core constraints in `Data/ApplicationDbContext.cs`. There is no separate validation pipeline layer.

**Authentication:** JWT bearer authentication is configured in `Program.cs`; request identity is read through `Services/CurrentUserService.cs`; controllers use `[Authorize]` and `[Authorize(Roles = "...")]`.

**Authorization:** Coarse-grained role checks are declared at controller/action level, while record-level checks happen in services such as `Services/ApplicationService.cs`, `Services/BuildingPermitService.cs`, and `Services/ChatService.cs`.

**Persistence Conventions:** Soft-delete query filters are configured for many lookup entities in `Data/ApplicationDbContext.cs`; repositories use eager loading for detail views and `AsNoTracking()` for paginated lists like `Repositories/BuildingPermitRepository.cs`.

**Templated Email Rendering:** RazorLight templates under `EmailTemplates/` are rendered by `Services/RazorViewRenderer.cs` and queued through `Services/EmailService.cs`.

---

*Architecture analysis: 2026-04-02*
