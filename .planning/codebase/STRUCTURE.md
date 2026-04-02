# Codebase Structure

**Analysis Date:** 2026-04-02

## Directory Layout

```text
ePermitsApplication/
├── Controllers/       # REST API controllers grouped by feature
├── Data/              # EF Core DbContext and seeders
├── DTOs/              # Request/response and UI-facing data contracts
├── EmailTemplates/    # Razor email templates copied to output
├── Entities/          # EF entities and permit subgraphs
├── Extensions/        # IQueryable and startup extension helpers
├── Helpers/           # Shared workflow, file, and config helper types
├── Hubs/              # SignalR hub endpoints
├── Mappings/          # AutoMapper profiles
├── Migrations/        # EF Core migration history and model snapshot
├── Models/            # User/email support models not kept under DTOs
├── Repositories/      # Repository implementations and interfaces
├── Services/          # Business logic and infrastructure services
├── Properties/        # Launch profiles
├── .planning/codebase/# Generated mapping/reference documents
├── Program.cs         # Composition root and app startup
├── ePermitsApp.csproj # Project definition
├── appsettings.json   # Non-secret runtime configuration defaults
└── appsettings.Development.json # Development-only overrides
```

## Directory Purposes

**`Controllers/`:**
- Purpose: Expose the HTTP API.
- Contains: One controller per feature or resource, usually pluralized, such as `Controllers/BarangaysController.cs`, `Controllers/BuildingPermitsController.cs`, `Controllers/ApplicationsController.cs`, `Controllers/AdminEmailNotificationConfigsController.cs`.
- Key files: `Controllers/AuthController.cs`, `Controllers/ApplicationsController.cs`, `Controllers/ChatController.cs`, `Controllers/FileController.cs`.

**`Services/`:**
- Purpose: Hold business logic, orchestration, and infrastructure-side behavior.
- Contains: Feature services, background workers, PDF/email/file services, and a nested `Interfaces/` folder.
- Key files: `Services/ApplicationService.cs`, `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`, `Services/AdminMaintenanceService.cs`, `Services/BackgroundEmailSender.cs`, `Services/Interfaces/IApplicationService.cs`.

**`Repositories/`:**
- Purpose: Isolate EF Core data access and query composition.
- Contains: Repository implementations at the root and interfaces in `Repositories/Interfaces/`.
- Key files: `Repositories/ApplicationRepository.cs`, `Repositories/BuildingPermitRepository.cs`, `Repositories/ProvinceRepository.cs`, `Repositories/Interfaces/IApplicationRepository.cs`.

**`Data/`:**
- Purpose: Define EF Core persistence configuration.
- Contains: `Data/ApplicationDbContext.cs` and seeders in `Data/Seeders/`.
- Key files: `Data/ApplicationDbContext.cs`, `Data/Seeders/DatabaseSeederRunner.cs`, `Data/Seeders/AdminSeeder.cs`.

**`Entities/`:**
- Purpose: Store database entity classes.
- Contains: Top-level lookup/workflow entities plus grouped subdirectories for permit-specific graphs.
- Key files: `Entities/ApplicationDepartmentReview.cs`, `Entities/Province.cs`, `Entities/Department.cs`, `Entities/BuildingPermit/BuildingPermit.cs`, `Entities/CoOApp/CoOApp.cs`, `Entities/Common/BaseEntity.cs`.

**`Models/`:**
- Purpose: Keep support models that are not organized under `Entities/` or `DTOs/`.
- Contains: Auth/user/email settings and email template model objects.
- Key files: `Models/User.cs`, `Models/UserProfile.cs`, `Models/UserRole.cs`, `Models/EmailSettings.cs`, `Models/EmailModels/ApplicationSubmittedModel.cs`.

**`DTOs/`:**
- Purpose: Hold API contracts and UI-facing shapes.
- Contains: One or more related DTOs per feature file, plus pagination/result wrappers.
- Key files: `DTOs/BuildingPermitDtos.cs`, `DTOs/CoOAppDtos.cs`, `DTOs/ApplicationDtos.cs`, `DTOs/AdminMaintenanceDtos.cs`, `DTOs/PagedResult.cs`, `DTOs/PaginationParams.cs`.

**`Mappings/`:**
- Purpose: Hold AutoMapper profiles aligned to feature DTO/entity pairs.
- Contains: One profile per entity or workflow area.
- Key files: `Mappings/ApplicationProfile.cs`, `Mappings/BuildingPermitProfile.cs`, `Mappings/CoOAppProfile.cs`, `Mappings/ProvinceProfile.cs`.

**`Helpers/`:**
- Purpose: Centralize small reusable domain and configuration helpers.
- Contains: Workflow constants, file metadata serialization, maintenance scope normalization, option classes.
- Key files: `Helpers/ApplicationWorkflowDefinitions.cs`, `Helpers/FilePathHelper.cs`, `Helpers/FileStorageSettings.cs`, `Helpers/MaintenanceApplicationScopes.cs`.

**`Extensions/`:**
- Purpose: Keep generic extension methods.
- Contains: Query pagination/sorting and startup database migration helpers.
- Key files: `Extensions/QueryableExtensions.cs`, `Extensions/DatabaseExtensions.cs`.

**`Hubs/`:**
- Purpose: Define SignalR endpoints.
- Contains: The application chat hub.
- Key files: `Hubs/ChatHub.cs`.

**`EmailTemplates/`:**
- Purpose: Store RazorLight `.cshtml` templates used for outbound email.
- Contains: Application/reviewer/admin notification templates.
- Key files: `EmailTemplates/ApplicationSubmitted.cshtml`, `EmailTemplates/ApplicationStatusUpdated.cshtml`, `EmailTemplates/ReviewerAssigned.cshtml`, `EmailTemplates/AdminApplicationSubmitted.cshtml`.

**`Migrations/`:**
- Purpose: Persist schema evolution generated by EF Core.
- Contains: Timestamped migration pairs and `Migrations/ApplicationDbContextModelSnapshot.cs`.
- Key files: `Migrations/20260316064736_AddAdminMaintenanceManagement.cs`, `Migrations/20260309023839_AddDepartmentReviewAssignmentsAndWorkflowOptions.cs`, `Migrations/ApplicationDbContextModelSnapshot.cs`.

**`Properties/`:**
- Purpose: Local launch/runtime metadata.
- Contains: ASP.NET launch profiles.
- Key files: `Properties/launchSettings.json`.

## Key File Locations

**Entry Points:**
- `Program.cs`: Main application startup and middleware/DI configuration.
- `Hubs/ChatHub.cs`: SignalR real-time entry point for chat connections.

**Configuration:**
- `ePermitsApp.csproj`: Target framework, package references, template copy settings.
- `appsettings.json`: Base runtime settings.
- `appsettings.Development.json`: Development overrides.
- `Properties/launchSettings.json`: Local run profiles.

**Core Logic:**
- `Services/ApplicationService.cs`: Reviewer dashboard, assignment, and application status workflow.
- `Services/BuildingPermitService.cs`: Building permit create/edit flow, file storage, notifications.
- `Services/CoOAppService.cs`: Certificate of occupancy create/edit flow.
- `Services/AdminMaintenanceService.cs`: Admin lookup maintenance, usage checks, and spreadsheet import/export.
- `Repositories/ApplicationRepository.cs`: Rich application aggregate queries.
- `Data/ApplicationDbContext.cs`: Relationships, indexes, query filters, and seeds.

**Testing:**
- Not detected. No test project, test directory, or `*.test.*` / `*.spec.*` files are present in the repository root.

## Naming Conventions

**Files:**
- Controllers use `{Feature}Controller.cs`, for example `Controllers/ProvincesController.cs` and `Controllers/AdminMaintenanceController.cs`.
- Services use `{Feature}Service.cs`, with interfaces in `Services/Interfaces/I{Feature}Service.cs`, for example `Services/ProvinceService.cs` and `Services/Interfaces/IProvinceService.cs`.
- Repositories use `{Feature}Repository.cs`, with interfaces in `Repositories/Interfaces/I{Feature}Repository.cs`, for example `Repositories/ProvinceRepository.cs` and `Repositories/Interfaces/IProvinceRepository.cs`.
- AutoMapper profiles use `{Feature}Profile.cs`, for example `Mappings/ApplicationProfile.cs`.
- DTO files usually group multiple related classes under one feature-oriented file, for example `DTOs/BuildingPermitDtos.cs` and `DTOs/ApplicationStatusDtos.cs`.
- EF migration files use the default `{timestamp}_{Name}.cs` and `{timestamp}_{Name}.Designer.cs` pattern in `Migrations/`.

**Directories:**
- Top-level directories are PascalCase and feature- or layer-oriented: `Controllers/`, `Services/`, `Repositories/`, `Mappings/`.
- Nested entity folders are used for subgraphs that belong to one permit type: `Entities/BuildingPermit/`, `Entities/CoOApp/`.
- Nested interface folders are used only where implementations and contracts coexist: `Services/Interfaces/`, `Repositories/Interfaces/`.

## Where to Add New Code

**New REST Feature:**
- Primary code: add a controller under `Controllers/`, a service under `Services/`, and a repository under `Repositories/` if the feature needs new persistence logic.
- DTOs: add request/response shapes under `DTOs/`, usually in a new feature file if the shapes are non-trivial.
- Mappings: add an AutoMapper profile under `Mappings/` if the feature exposes mapped DTOs.
- Registration: wire the new service/repository explicitly in `Program.cs`.

**New Lookup / Maintenance Entity:**
- Entity: add the EF entity under `Entities/`.
- DbContext: add a `DbSet<>` and model configuration in `Data/ApplicationDbContext.cs`.
- Repository/service/controller: follow the existing CRUD pattern in `Repositories/`, `Services/`, and `Controllers/`.
- Admin maintenance support: extend `Services/AdminMaintenanceService.cs` and `Controllers/AdminMaintenanceController.cs` if the entity must participate in the generic maintenance UI/API.
- Migration: add a new EF migration under `Migrations/`.

**New Permit-Type Workflow:**
- Shared workflow root: continue using `Entities/Application.cs` as the parent aggregate.
- Permit-specific graph: create a dedicated entity subfolder under `Entities/` similar to `Entities/BuildingPermit/` or `Entities/CoOApp/`.
- Feature stack: mirror the existing vertical slices with `{PermitType}Controller`, `{PermitType}Service`, `{PermitType}Repository`, DTO file, and AutoMapper profile.
- Review/status integration: extend `Helpers/ApplicationWorkflowDefinitions.cs`, `Services/ApplicationService.cs`, and repository detail queries if the new type participates in review dashboards or status transitions.

**Utilities / Shared Helpers:**
- Pure helper or constants: place in `Helpers/`.
- Query/startup extension: place in `Extensions/`.
- Cross-cutting infrastructure service: place in `Services/` with an interface in `Services/Interfaces/` if it will be injected broadly.

## Special Directories

**`.planning/codebase/`:**
- Purpose: Generated codebase reference documents for the planning workflow.
- Generated: Yes.
- Committed: Yes, intended for orchestrator consumption.

**`Migrations/`:**
- Purpose: EF Core schema migration source.
- Generated: Yes, with developer-generated names.
- Committed: Yes.

**`bin/` and `obj/`:**
- Purpose: Standard .NET build artifacts.
- Generated: Yes.
- Committed: No, expected local build output.

**`temp_build/` and `temp_build_verify/`:**
- Purpose: Local temporary build/publish-style artifact directories containing copied runtime assets and binaries.
- Generated: Yes.
- Committed: No, these behave as disposable output folders and should not be used as source locations.

**`.github/`:**
- Purpose: Repository automation metadata.
- Generated: No.
- Committed: Yes.

---

*Structure analysis: 2026-04-02*
