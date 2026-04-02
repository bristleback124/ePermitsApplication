# Coding Conventions

**Analysis Date:** 2026-04-02

## Naming Patterns

**Files:**
- Use PascalCase `.cs` filenames that match the primary type in the file, such as `Controllers/ApplicationsController.cs`, `Services/ApplicationService.cs`, `Repositories/ProvinceRepository.cs`, `DTOs/RegisterDto.cs`, and `Mappings/ApplicationProfile.cs`.
- Prefix interfaces with `I` and keep the interface/implementation pair in parallel paths, such as `Services/Interfaces/IApplicationService.cs` with `Services/ApplicationService.cs` and `Repositories/Interfaces/IProvinceRepository.cs` with `Repositories/ProvinceRepository.cs`.
- Use suffixes to signal role: `*Controller`, `*Service`, `*Repository`, `*Dto`, `*Profile`, `*Seeder`, `*Settings`, and `*Extensions`.

**Functions:**
- Use PascalCase for public methods, such as `GetApplicationsByUserIdAsync` in `Services/ApplicationService.cs`, `RegisterAsync` in `Services/AuthService.cs`, and `MigrateDatabaseAsync` in `Extensions/DatabaseExtensions.cs`.
- Append `Async` to asynchronous methods consistently across controllers, services, repositories, and extensions, such as `GetByIdAsync`, `UpdateOverallStatusAsync`, and `SendTemplatedEmailAsync`.
- Use predicate-style names for boolean helpers, such as `CanAccessApplication`, `CanUpdateDepartmentReview`, `IsAdmin`, and `IsDepartmentUser` in `Services/ApplicationService.cs`.

**Variables:**
- Use `_camelCase` for injected private readonly fields, such as `_service` in `Controllers/AdminEmailNotificationConfigsController.cs`, `_context` in `Repositories/ProvinceRepository.cs`, and `_logger` in `Services/EmailService.cs`.
- Use `camelCase` for locals and parameters, such as `applicationId`, `departmentId`, `registerDto`, and `currentUser`.
- Use descriptive domain names rather than generic abbreviations unless the domain model already uses them, such as `RequirementClassification`, `OccupancyNature`, `LGU`, and `CoOApp`.

**Types:**
- Use PascalCase for classes, records, DTOs, and entity types, such as `ApplicationDepartmentReviewDto` in `DTOs/ApplicationDtos.cs` and `DummyReviewerSeed` in `Services/ApplicationService.cs`.
- Keep DTO names explicit about intent: request models use `Create`, `Update`, `Assign`, or `Status` suffixes, while response/read models use `Dto`, `DetailDto`, or `Short`.

## Code Style

**Formatting:**
- No repository-level formatter config was detected. `.editorconfig`, `stylecop.json`, and Roslyn rule sets were not found at the repository root.
- The codebase mixes block-scoped namespaces, such as `namespace ePermitsApp.Controllers { ... }` in `Controllers/ApplicationsController.cs`, with file-scoped namespaces, such as `namespace ePermitsApp.Controllers;` in `Controllers/AdminEmailNotificationConfigsController.cs`.
- Braces are typically on new lines for block-scoped files. Short guard clauses often omit braces for single-line returns, especially in controllers and services.
- Nullable reference types are enabled in `ePermitsApp.csproj`, and nullability is expressed with `?` in signatures and DTO properties, such as `Task<Application?>` in `Repositories/Interfaces/IApplicationRepository.cs` and `string? AssignedReviewerName` in `DTOs/ApplicationDtos.cs`.

**Linting:**
- No explicit linting or style enforcement package/config was detected in `ePermitsApp.csproj` or the repository root.
- Conventions are enforced informally through repeated patterns in `Controllers/`, `Services/`, `Repositories/`, `DTOs/`, and `Mappings/`.

## Import Organization

**Order:**
1. Project namespaces first, often grouped by layer or feature, such as `ePermitsApp.DTOs`, `ePermitsApp.Services.Interfaces`, and `ePermitsApp.Helpers`.
2. Microsoft namespaces next, such as `Microsoft.AspNetCore.Mvc`, `Microsoft.EntityFrameworkCore`, and `Microsoft.Extensions.Options`.
3. `System` namespaces last in many block-scoped files, such as `System.Text` and `System.Security.Cryptography` in `Services/AuthService.cs`.

**Path Aliases:**
- No path alias mechanism applies. C# namespace imports are used directly.
- Namespaces are not fully uniform. Both `ePermitsApp.*` and `ePermits.*` roots appear, such as `using ePermits.DTOs;` in `Controllers/AuthController.cs` and `using ePermitsApp.DTOs;` in `Controllers/ApplicationsController.cs`. Follow the namespace already established by the surrounding files when adding code.

## Error Handling

**Patterns:**
- Controllers typically validate `ModelState` explicitly and return `BadRequest` early, as shown in `Controllers/AuthController.cs` and `Controllers/ApplicationsController.cs`.
- Services commonly signal business-rule failures with `null` or tuple results instead of throwing, such as `Task<AuthResponseDto?> RegisterAsync` in `Services/AuthService.cs` and `(bool Success, string Message, ApplicationDepartmentReviewDto? Review)` in `Services/ApplicationService.cs`.
- Controllers catch expected exceptions from services and translate them into HTTP responses, such as `catch (ArgumentException ex)` in `Controllers/AdminEmailNotificationConfigsController.cs` and `catch (InvalidOperationException ex)` in `Controllers/ApplicationsController.cs`.
- Infrastructure and background work log exceptions instead of propagating them, such as `_logger.LogError(...)` in `Services/ApplicationService.cs` and `Services/BackgroundEmailSender.cs`.
- Some older service code still uses `Console.WriteLine` for exception and success logging in `Services/AuthService.cs`; newer services use `ILogger<T>`.

## Logging

**Framework:** `ILogger<T>` from `Microsoft.Extensions.Logging`

**Patterns:**
- Prefer structured logging with message templates, as in `Extensions/DatabaseExtensions.cs`, `Services/EmailService.cs`, `Services/ApplicationService.cs`, and `Services/BackgroundEmailSender.cs`.
- Use logging mainly for operational concerns: background email dispatch, database migration state, and notification failures.
- Do not rely on `Console.WriteLine` for new code. Existing console logging in `Services/AuthService.cs` is inconsistent with the newer logging pattern.

## Comments

**When to Comment:**
- Comments are sparse and used mainly to mark sections or intent, such as `// SignalR`, `// Add services to the container.`, and `// Run modular seeders` in `Program.cs` and `Data/ApplicationDbContext.cs`.
- Use a short comment only when a block performs non-obvious orchestration, such as mapping file metadata or seeding review assignments.

**JSDoc/TSDoc:**
- XML documentation comments are rare but present on some controller actions, such as `/// Register a new user` in `Controllers/AuthController.cs`.
- The dominant pattern is not to document every method. Add XML docs only when the endpoint or service contract benefits from generated API documentation clarity.

## Function Design

**Size:** 
- Controllers stay thin and usually delegate directly to services, as in `Controllers/ApplicationsController.cs` and `Controllers/AdminEmailNotificationConfigsController.cs`.
- Services range from small orchestration classes, such as `Services/EmailService.cs`, to large feature services with mixed responsibilities, such as `Services/BuildingPermitService.cs` and `Services/ApplicationService.cs`.
- Private helper methods are used to isolate authorization checks, data shaping, hashing, and email-notification logic inside services.

**Parameters:** 
- Use constructor injection for dependencies throughout `Controllers/`, `Services/`, `Repositories/`, and `Extensions/DatabaseExtensions.cs`.
- Use DTO objects for complex request bodies, such as `AssignApplicationReviewerDto` and `UpdateApplicationOverallStatusDto` in `Controllers/ApplicationsController.cs`.
- Use primitive route/query parameters only for identifiers and small filters, such as `applicationId`, `departmentId`, and `userId`.

**Return Values:** 
- Controllers generally return `ActionResult<T>` or `IActionResult` and wrap success payloads with `Ok(...)`.
- Service methods return entities, DTOs, nullable DTOs, paged results, or tuple status objects depending on failure mode, such as `Task<PagedResult<BuildingPermit>>` in `Services/BuildingPermitService.cs`.
- Repositories return entities and collections directly, with `SaveChangesAsync` returning `bool` in CRUD-style repositories such as `Repositories/ProvinceRepository.cs`.

## Module Design

**Exports:** 
- Keep one primary public type per file, with DTO files as the main exception. `DTOs/ApplicationDtos.cs` groups several related DTO types in a single file.
- Separate layer contracts from implementations with `Interfaces/` directories under `Services/` and `Repositories/`.
- Centralize EF Core model configuration in `Data/ApplicationDbContext.cs` rather than splitting into per-entity configuration classes.

**Barrel Files:** 
- Not used. Imports target concrete namespaces and files indirectly through namespaces.

## Validation Patterns

- Request validation is primarily handled with data-annotation attributes on DTOs, such as `[Required]`, `[StringLength]`, `[EmailAddress]`, `[Phone]`, and `[Compare]` in `DTOs/RegisterDto.cs`, `DTOs/BuildingPermitDtos.cs`, `DTOs/CoOAppDtos.cs`, and `DTOs/ApplicationStatusDtos.cs`.
- Controllers check `ModelState.IsValid` manually before calling services instead of relying on a global validation response format, as shown in `Controllers/AuthController.cs` and `Controllers/ApplicationsController.cs`.
- Entity constraints are duplicated at the persistence layer with EF Core fluent configuration in `Data/ApplicationDbContext.cs`, such as `IsRequired()`, `HasMaxLength(...)`, indexes, and query filters.

## Data Access Patterns

- Repositories encapsulate `ApplicationDbContext` usage and keep EF Core queries in `Repositories/`, such as `Repositories/ApplicationRepository.cs` and `Repositories/ProvinceRepository.cs`.
- Read queries frequently use `AsNoTracking()` for list endpoints, as in `Repositories/ProvinceRepository.cs`.
- Rich aggregate reads use chained `Include` and `ThenInclude` calls in repositories, not controllers, as in `Repositories/ApplicationRepository.cs`.
- Shared query helpers live in extension methods, such as `ApplySorting` and `Paginate` in `Extensions/QueryableExtensions.cs`.

## Prescriptive Guidance

- Add new endpoints in `Controllers/` as thin HTTP adapters that validate input, call a single service method, and translate domain failures into HTTP responses.
- Add business rules and orchestration in `Services/`, returning nullable results or explicit success/message tuples when the controller needs to distinguish expected failures.
- Add EF Core queries and persistence logic in `Repositories/` or `Data/ApplicationDbContext.cs`, not in controllers.
- Match the existing file naming and suffix patterns exactly.
- Prefer `ILogger<T>` for any new operational logging and avoid introducing more `Console.WriteLine` usage.
- Follow the existing nullability style and initialize DTO string/list properties defensively with `string.Empty` and `new()`.

---

*Convention analysis: 2026-04-02*
