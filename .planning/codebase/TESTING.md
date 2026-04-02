# Testing Patterns

**Analysis Date:** 2026-04-02

## Test Framework

**Runner:**
- Not detected.
- Config: Not detected. No `*.Tests.csproj`, `xunit`, `NUnit`, `MSTest`, `Microsoft.NET.Test.Sdk`, `jest`, `vitest`, or `playwright` configuration files were found in `C:\Users\Harvie\Documents\Coding\ePermit\ePermitsApplication`.

**Assertion Library:**
- Not detected.

**Run Commands:**
```bash
dotnet restore          # Restore application dependencies
dotnet build            # Build current project or solution
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish  # Current CI build command from `.github/workflows/build-backend.yml`
```

## Test File Organization

**Location:**
- Not detected. There is no dedicated test directory under the repository root, and no co-located `*.test.*` or `*.spec.*` files were found.

**Naming:**
- Not detected. No test files or test projects exist to establish a naming rule.

**Structure:**
```text
Current state:
- `Controllers/`
- `Services/`
- `Repositories/`
- `Data/`
- `DTOs/`
- `Mappings/`

Missing:
- `tests/`
- `*.Tests/`
- `*.IntegrationTests/`
- any `*.test.cs` or `*.spec.cs`
```

## Test Structure

**Suite Organization:**
```csharp
Not applicable. No test suites were found in the repository.
```

**Patterns:**
- Setup pattern: Not detected.
- Teardown pattern: Not detected.
- Assertion pattern: Not detected.

## Mocking

**Framework:** Not detected.

**Patterns:**
```csharp
Not applicable. No mocking library or mock-based tests were found.
```

**What to Mock:**
- Current code structure implies that unit tests should mock service dependencies in `Controllers/`, repository dependencies in `Services/`, and infrastructure collaborators such as `IEmailService`, `ICurrentUserService`, `ILogger<T>`, `IMapper`, and `IOptions<T>`.
- Likely mock boundaries based on constructor injection:
  - `Controllers/ApplicationsController.cs` should mock `IApplicationService` and `IApplicationPdfService`.
  - `Services/ApplicationService.cs` should mock `IApplicationRepository`, `IUserRepository`, `ICurrentUserService`, `IMapper`, and `IEmailService`.
  - `Services/BuildingPermitService.cs` should mock `IBuildingPermitRepository`, `IUserRepository`, `ICurrentUserService`, `IEmailService`, `IAdminEmailNotificationConfigService`, and `IMapper`.

**What NOT to Mock:**
- If integration tests are added, do not mock EF Core behavior that lives in `Repositories/ApplicationRepository.cs`, `Repositories/ProvinceRepository.cs`, and `Data/ApplicationDbContext.cs`; use a real test database or a close provider instead.
- Do not mock AutoMapper profiles when validating mapping configuration in `Mappings/ApplicationProfile.cs` and related profile files.

## Fixtures and Factories

**Test Data:**
```csharp
Not detected in tests.

Existing production-side data setup patterns that tests could reuse as references:
- Seeder classes in `Data/Seeders/`, such as `Data/Seeders/AdminSeeder.cs` and `Data/Seeders/RequirementSeeder.cs`
- Domain helper constants in `Helpers/ApplicationWorkflowDefinitions.cs`
- DTO defaults with initialized strings/lists in `DTOs/ApplicationDtos.cs`
```

**Location:**
- No fixtures, factories, builders, or sample-data helpers were found in a test project.
- Existing reusable seed data lives in `Data/Seeders/`.

## Coverage

**Requirements:** None enforced.

**View Coverage:**
```bash
Not available in the current repository. No coverage tool or report configuration was found.
```

## Test Types

**Unit Tests:**
- Not used in the current repository.
- The most natural unit-test targets are:
  - Controller response shaping in `Controllers/AuthController.cs`, `Controllers/ApplicationsController.cs`, and `Controllers/AdminEmailNotificationConfigsController.cs`
  - Business-rule helpers and tuple-return flows in `Services/ApplicationService.cs`
  - File and workflow orchestration in `Services/BuildingPermitService.cs`
  - Validation-centric DTO behavior through ASP.NET model binding using `DTOs/RegisterDto.cs`, `DTOs/BuildingPermitDtos.cs`, and `DTOs/CoOAppDtos.cs`

**Integration Tests:**
- Not used in the current repository.
- The most valuable integration coverage would target:
  - EF Core query shape and includes in `Repositories/ApplicationRepository.cs`
  - Model constraints and query filters in `Data/ApplicationDbContext.cs`
  - Startup wiring in `Program.cs`, including authentication, AutoMapper registration, SignalR, and hosted services
  - Migration execution path in `Extensions/DatabaseExtensions.cs`

**E2E Tests:**
- Not used.
- No browser, API smoke-test, or workflow automation framework was detected.

## Common Patterns

**Async Testing:**
```csharp
Not detected.

Async-heavy code that would require async test methods includes:
- `Services/ApplicationService.cs`
- `Services/BuildingPermitService.cs`
- `Services/AuthService.cs`
- `Repositories/ApplicationRepository.cs`
- `Extensions/DatabaseExtensions.cs`
```

**Error Testing:**
```csharp
Not detected.

Important future error cases to cover based on current code:
- `Controllers/ApplicationsController.cs` returning `BadRequest` for invalid status updates or invalid `type`
- `Controllers/AdminEmailNotificationConfigsController.cs` translating `ArgumentException` to `BadRequest`
- `Services/ApplicationService.cs` returning `(false, message, null)` when reviewers, department reviews, or permissions are invalid
- `Services/AuthService.cs` returning `null` when usernames/emails already exist or related records are missing
```

## Existing Verification Signals

- The only automated verification currently committed is the GitHub Actions publish workflow in `.github/workflows/build-backend.yml`.
- That workflow restores dependencies and publishes a release artifact, but it does not execute `dotnet test`, collect coverage, or run static-analysis gates.
- The repository README in `README.md` describes architectural layering but does not define any testing workflow.

## Prescriptive Guidance

- Treat testing as absent, not partial. New work should not assume any existing test harness.
- If adding the first test project, keep naming aligned with the solution and create a sibling project such as `ePermitsApp.Tests/` or `tests/ePermitsApp.Tests/`.
- Start with controller and service tests because constructor injection already creates clean seams around `Controllers/` and `Services/`.
- Add repository integration tests separately from service unit tests because repositories in `Repositories/` contain substantial EF Core query behavior that mocks will not validate.
- Add CI test execution to `.github/workflows/build-backend.yml` before or alongside the first test project so tests become part of the repository’s default verification path.

---

*Testing analysis: 2026-04-02*
