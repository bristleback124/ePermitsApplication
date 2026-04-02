# Codebase Concerns

**Analysis Date:** 2026-04-02

## Tech Debt

**Monolithic maintenance service:**
- Issue: `Services/AdminMaintenanceService.cs` concentrates lookup queries, status changes, reference counting, Excel import/export, cascade enable/disable logic, and entity creation in a single 1,392-line class.
- Files: `Services/AdminMaintenanceService.cs`, `Controllers/AdminMaintenanceController.cs`
- Impact: Changes to one maintenance area carry regression risk across unrelated areas, and the switch-heavy design makes it difficult to add or verify new entity types safely.
- Fix approach: Split by concern into smaller services per maintenance domain and isolate import/export logic behind dedicated components with focused tests.

**Business logic duplicated across service layers:**
- Issue: file persistence, applicant notification flow, permission checks, and create/update orchestration are duplicated between `Services/BuildingPermitService.cs` and `Services/CoOAppService.cs`.
- Files: `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`
- Impact: Bug fixes and validation changes must be repeated in two places, increasing drift risk between permit types.
- Fix approach: Extract shared application workflow, file-storage, and notification helpers behind reusable services.

**Repository pattern still saves per call:**
- Issue: repository methods such as `Repositories/ApplicationRepository.cs`, `Repositories/UserRepository.cs`, and `Repositories/MessageRepository.cs` call `SaveChangesAsync()` internally instead of participating in a unit-of-work boundary.
- Files: `Repositories/ApplicationRepository.cs`, `Repositories/UserRepository.cs`, `Repositories/MessageRepository.cs`, `Repositories/BuildingPermitRepository.cs`, `Repositories/CoOAppRepository.cs`
- Impact: multi-step operations are harder to keep atomic, error recovery is inconsistent, and cross-entity workflows require extra save calls.
- Fix approach: move transaction ownership to application services and keep repositories focused on query/update tracking.

**Custom file metadata serialization is brittle:**
- Issue: file metadata is packed into delimited strings using `name::size::path` and `|` separators.
- Files: `Helpers/FilePathHelper.cs`, `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`
- Impact: unusual filenames can corrupt parsing, schema evolution is difficult, and storage remains tightly coupled to Windows-style absolute paths.
- Fix approach: store file metadata as structured child entities or JSON columns with normalized relative paths.

## Known Bugs

**Any authenticated user can fetch another applicant's application list:**
- Symptoms: `GET /api/applications/user/{userId}` returns applications for the supplied `userId` without verifying that the caller owns them or is an admin.
- Files: `Controllers/ApplicationsController.cs`, `Services/ApplicationService.cs`, `Repositories/ApplicationRepository.cs`
- Trigger: log in as any applicant and request another user's numeric id.
- Workaround: none in code. Access relies on frontend restraint.

**Any authenticated user can list all permit records:**
- Symptoms: `GET /api/building-permits` and `GET /api/coo-apps` expose all records to any authenticated account.
- Files: `Controllers/BuildingPermitsController.cs`, `Controllers/CoOAppsController.cs`, `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`
- Trigger: sign in as an applicant and call the list endpoints.
- Workaround: none in code. Restriction is not enforced server-side.

**Chat hub group membership is not authorized per application:**
- Symptoms: any authenticated socket can join `application-{id}` groups and receive broadcasts for that application.
- Files: `Hubs/ChatHub.cs`, `Controllers/ChatController.cs`
- Trigger: connect to `/chatHub` with a valid token and call `JoinApplicationGroup` for another application's id.
- Workaround: none in code. The hub does not validate access before joining groups.

**Unread message state is shared globally instead of per recipient:**
- Symptoms: when one government reviewer marks applicant messages as read, they become read for all other government users because `IsRead` is tracked on the message row, not per user.
- Files: `Repositories/MessageRepository.cs`, `Services/ChatService.cs`
- Trigger: multiple reviewers access the same application chat and one reviewer calls mark-read.
- Workaround: none. Current model cannot represent per-user read state.

**Registration can leave orphaned users or profiles:**
- Symptoms: registration creates the `User`, then separately creates `UserProfile`, then separately updates `UserProfileId`; a failure in the later steps returns `null` after partial persistence.
- Files: `Services/AuthService.cs`, `Repositories/UserRepository.cs`, `Repositories/UserProfileRepository.cs`
- Trigger: any exception after the initial user save, including uniqueness conflicts or database interruptions.
- Workaround: manual cleanup in the database.

**Permit creation can persist incomplete records if file save fails:**
- Symptoms: building permit and COO creation save database rows first, then write files, then save again. File-system failures can leave created applications with missing or empty document references.
- Files: `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`
- Trigger: disk permission issues, invalid storage path, or interrupted upload during create/update.
- Workaround: manual admin repair or resubmission.

**Email subject text is already mojibaked in source:**
- Symptoms: applicant submission emails use `â€”` in the subject instead of a proper dash.
- Files: `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`
- Trigger: any submission email sent from the application.
- Workaround: none. The incorrect literal is embedded in source.

## Security Considerations

**Password storage is not suitable for production:**
- Risk: passwords are hashed with unsalted SHA-256, which is fast and vulnerable to offline cracking.
- Files: `Services/AuthService.cs`, `Services/ApplicationService.cs`, `Data/Seeders/AdminSeeder.cs`
- Current mitigation: passwords are hashed rather than stored in plain text.
- Recommendations: replace with a slow password hasher such as ASP.NET Core Identity `PasswordHasher<TUser>` or Argon2/bcrypt, then migrate existing records.

**Default credentials are hardcoded in seed paths:**
- Risk: predictable passwords such as `"password123"` are created for seeded admin and dummy reviewer accounts.
- Files: `Data/Seeders/AdminSeeder.cs`, `Services/ApplicationService.cs`
- Current mitigation: none beyond hashing.
- Recommendations: remove default passwords from runtime code, seed only through environment-specific bootstrap flows, and force password reset on first use.

**File access is exposed by absolute paths and an unauthenticated file endpoint:**
- Risk: the API returns absolute storage paths in DTOs and exposes `GET /api/file?path=...` without `[Authorize]`.
- Files: `Controllers/FileController.cs`, `DTOs/BuildingPermitDtos.cs`, `DTOs/CoOAppDtos.cs`, `Helpers/FilePathHelper.cs`
- Current mitigation: `FileController` checks that the requested path starts with `FileStorage:BasePath`.
- Recommendations: require authorization, stop returning filesystem paths to clients, use opaque file ids or signed download URLs, and store relative paths only.

**CORS policy allows credentials from any origin:**
- Risk: `SetIsOriginAllowed(_ => true)` combined with `AllowCredentials()` enables any origin to make credentialed browser requests.
- Files: `Program.cs`
- Current mitigation: none. The policy is globally applied.
- Recommendations: replace with an explicit allowlist from configuration and keep environment-specific frontend origins separate.

**Anonymous email relay endpoint exists:**
- Risk: `POST /api/email/send-test` has no authorization and can enqueue outbound email to arbitrary recipients.
- Files: `Controllers/EmailController.cs`, `Services/EmailService.cs`, `Services/BackgroundEmailSender.cs`
- Current mitigation: none visible in controller code.
- Recommendations: remove the endpoint from production, or restrict it to admins and non-production environments.

## Performance Bottlenecks

**Dashboard and detail queries eagerly load large object graphs:**
- Problem: application queries pull multiple `Include`/`ThenInclude` chains for users, permit data, department reviews, reviewer profiles, and related entities.
- Files: `Repositories/ApplicationRepository.cs`, `Services/ApplicationService.cs`
- Cause: all dashboard and detail reads are built as large eager-loading queries with no projection and no pagination for reviewer dashboard results.
- Improvement path: project directly to DTOs, paginate reviewer views, and load only fields required by each endpoint.

**Chat read operations scan and update whole message sets:**
- Problem: unread count and mark-read operations work on all matching rows for an application and sender type.
- Files: `Repositories/MessageRepository.cs`
- Cause: message reads are modeled as bulk row updates without recipient scoping or indexing strategy visible in code.
- Improvement path: add targeted indexes and redesign read receipts as per-user state to avoid repeated full-set updates.

**Email queue is unbounded:**
- Problem: the email pipeline uses `Channel.CreateUnbounded<EmailMessage>()`.
- Files: `Program.cs`, `Services/EmailService.cs`, `Services/BackgroundEmailSender.cs`
- Cause: enqueue operations never backpressure callers and the background sender processes messages serially.
- Improvement path: use a bounded channel, retry policy, and metrics so spikes do not convert into unbounded memory growth.

**Uploaded files have no size or content validation on main workflows:**
- Problem: building permit and COO uploads accept `IFormFile` and `IFormFileCollection` without server-side extension, MIME, or size checks.
- Files: `DTOs/BuildingPermitDtos.cs`, `DTOs/CoOAppDtos.cs`, `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`
- Cause: upload code writes directly to disk using the original filename as part of the stored name.
- Improvement path: enforce request limits, validate allowed content types, sanitize filenames, and reject oversized uploads early.

## Fragile Areas

**Access control is distributed and inconsistent:**
- Files: `Controllers/ApplicationsController.cs`, `Services/ApplicationService.cs`, `Controllers/BuildingPermitsController.cs`, `Controllers/CoOAppsController.cs`, `Controllers/FileController.cs`, `Hubs/ChatHub.cs`
- Why fragile: some endpoints validate ownership in services, some expose cross-user data directly, and the hub/file paths bypass the same authorization model entirely.
- Safe modification: centralize authorization policies and make all application/file/chat access go through a shared ownership check.
- Test coverage: no automated tests detected for endpoint authorization flows.

**File storage depends on local absolute paths persisted in relational fields:**
- Files: `Helpers/FilePathHelper.cs`, `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`, `Services/DocumentDownloadService.cs`
- Why fragile: changing storage root, moving environments, or renaming files invalidates persisted references and can break downloads retroactively.
- Safe modification: migrate to relative storage keys and isolate the file backend behind a dedicated abstraction before changing storage layout.
- Test coverage: no automated tests detected for upload, edit, or download path behavior.

**Application creation/update spans database, filesystem, and email without a single consistency boundary:**
- Files: `Services/AuthService.cs`, `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`, `Services/ApplicationService.cs`
- Why fragile: the code mixes save calls, file writes, and email enqueueing across multiple steps, so partial failure leaves inconsistent state.
- Safe modification: wrap persistence in transactions where possible and use an outbox/event pattern for post-commit notifications.
- Test coverage: no automated tests detected for failure-path recovery.

## Scaling Limits

**Local filesystem document storage:**
- Current capacity: documents are stored under a single configured base path and indexed by permit id folders.
- Limit: horizontal scaling, containerized deployment, and multi-node hosting break unless all nodes share the same disk.
- Scaling path: move to object storage and store opaque file keys instead of local absolute paths.

**Reviewer dashboard and maintenance lookups load in-process collections:**
- Current capacity: dashboard filtering and some maintenance projections operate in memory after pulling data from EF Core.
- Limit: response time and memory usage will degrade as applications, reviewers, and maintenance records increase.
- Scaling path: keep filtering/projection in SQL, paginate large views, and add query-specific indexes.

## Dependencies at Risk

**Not detected:**
- Risk: no dependency-specific concern was validated from source alone beyond normal upgrade maintenance.
- Impact: not applicable.
- Migration plan: not applicable.

## Missing Critical Features

**Automated test suite is absent:**
- Problem: no unit, integration, or end-to-end test files were detected in the repository.
- Blocks: safe refactoring of auth, file handling, maintenance import, and application workflow logic.

**Centralized audit/security controls are missing:**
- Problem: sensitive flows do not show rate limiting, audit logging, or policy-based authorization around file access, email test sending, and cross-user application reads.
- Blocks: production hardening and reliable incident tracing.

## Test Coverage Gaps

**Authorization and tenancy boundaries:**
- What's not tested: applicant isolation, admin-only endpoints, file download authorization, and SignalR application-group access.
- Files: `Controllers/ApplicationsController.cs`, `Controllers/FileController.cs`, `Hubs/ChatHub.cs`, `Services/ApplicationService.cs`
- Risk: data exposure bugs can ship unnoticed.
- Priority: High

**Registration and application failure paths:**
- What's not tested: partial persistence handling when user/profile creation, file saves, or email enqueueing fail.
- Files: `Services/AuthService.cs`, `Services/BuildingPermitService.cs`, `Services/CoOAppService.cs`
- Risk: orphaned records and broken applications remain in production data.
- Priority: High

**Chat concurrency and read-state behavior:**
- What's not tested: multiple reviewers on one application, message read semantics, and unauthorized group subscriptions.
- Files: `Services/ChatService.cs`, `Repositories/MessageRepository.cs`, `Hubs/ChatHub.cs`
- Risk: incorrect unread counts and data leakage across reviewers.
- Priority: Medium

---

*Concerns audit: 2026-04-02*
