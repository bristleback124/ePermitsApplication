# Technology Stack

**Project:** ePermits chat notification enhancement
**Researched:** 2026-04-02

## Recommended Stack

### Core Backend Pattern
| Technology | Version | Purpose | Why |
|------------|---------|---------|-----|
| ASP.NET Core Web API controllers | .NET 8 | New notification endpoints | Existing chat and application APIs already use attribute-routed controllers with `[Authorize]`, direct claim reads, and `Ok(...)` / `BadRequest(...)` responses. New endpoints should extend `ChatController` or a closely related application-facing controller instead of adding a new subsystem. |
| Service layer (`IChatService`, `IApplicationService`) | current codebase pattern | Authorization-aware business logic | Current controllers stay thin and delegate access checks plus DTO shaping to services. Notification logic should follow that split so role/assignment rules stay centralized. |
| Repository layer over EF Core | EF Core 8 / SQL Server | Query unread state and persist read markers | Existing persistence is repository-driven. Notification reads and read-state updates should be implemented as repository methods, not ad hoc controller or hub queries. |
| SignalR (`ChatHub`) | ASP.NET Core SignalR | Realtime invalidation after message send/read | Existing chat delivery already uses `application-{id}` groups. Notification work should reuse that hub boundary for badge refresh events rather than introducing a separate transport. |

### Persistence Extension
| Technology | Version | Purpose | Why |
|------------|---------|---------|-----|
| Existing `Message` table/entity extension | current schema family | Store unread state needed for per-user notifications | Current `Message.IsRead` is coarse and only works by sender side. For applicant-vs-reviewer unread notifications, extend chat persistence in the existing EF model and migrations rather than creating a generic notifications table first. |
| EF Core migrations | EF Core 8 | Apply schema changes | The app auto-migrates on startup. Notification persistence should ship as a normal migration aligned with `ApplicationDbContext` conventions. |

### Supporting DTO / Response Shape
| Library / Pattern | Version | Purpose | When to Use |
|-------------------|---------|---------|-------------|
| Hand-written DTO classes in `DTOs/` | current codebase pattern | Notification list/count payloads | Use for chat notification endpoints. Current chat DTOs are manually projected in `ChatService`; follow that for notification DTOs unless the data is already mapped elsewhere. |
| Anonymous `{ success, message, data }` envelopes | current codebase pattern | Mutation endpoints | Use for mark-read style endpoints when returning more than `Ok()`. This matches assignment/status endpoints in `ApplicationsController`. |
| Plain `Ok(dto)` / `Ok(list)` / `Ok(new { UnreadCount = count })` | current codebase pattern | Read endpoints | Use for GET endpoints. Existing controllers do not wrap all reads in a standard envelope. Preserve that inconsistency rather than inventing a new response contract. |

## Implementation Patterns To Follow

### 1. Keep notification work inside the existing application-chat boundary
- Build on `ChatController`, `ChatService`, `MessageRepository`, and `ApplicationRepository`.
- Reuse `ApplicationRepository` access data for applicant ownership and reviewer/admin visibility.
- Do not introduce a cross-product notification center, background processor, or separate event bus for this feature.

### 2. Centralize access control in services, not controllers
- Controllers currently read `ClaimTypes.NameIdentifier` and `ClaimTypes.Role`, then pass `userId` and `userRole` into services.
- New notification endpoints should keep that pattern.
- Service methods should enforce:
  - applicant can only see notifications for `Application.UserId == userId`
  - `user` role visibility must be scoped to department/reviewer assignment rules already used by `ApplicationService`
  - `admin` can remain broader, but should still stay application-scoped

### 3. Reuse reviewer scoping rules from application flows
- `ApplicationService.GetReviewerDashboardAsync()` filters department users to applications with matching `DepartmentReviews`.
- `CanUpdateDepartmentReview(...)` is stricter: assigned reviewer plus matching department.
- For unread reviewer-side chat notifications, recommend assignment-aware filtering, not only department membership, because the project requirement is "assigned reviewer/admin-side users."
- Practically: add repository/service queries that join message unread state with `ApplicationDepartmentReviews` and `AssignedReviewerId`.

### 4. Follow existing DTO conventions: explicit, flat, frontend-ready
- Current dashboard/detail DTOs are purpose-built and avoid exposing EF entities.
- New notification DTOs should include only fields the frontend needs directly:
  - `ApplicationId`
  - `FormattedId`
  - `ProjectTitle`
  - `UnreadCount`
  - `LastMessageAt`
  - `LastMessagePreview` if needed
  - sender-side summary such as `LastSenderType`
- Keep names explicit and match existing casing/style.

### 5. Prefer repository-level projections and targeted includes
- Existing repositories use eager `Include(...)` graphs for detail reads and ordered list queries.
- For notification queries, do not load full application detail graphs.
- Add focused repository methods that:
  - filter unread messages in SQL
  - join only the application fields needed for badge/list DTOs
  - order by latest unread or latest message timestamp
  - return counts grouped by application
- `AsNoTracking()` should be used for read-only notification lists/counts even though current repositories do this inconsistently; it is the correct fit for this query type.

### 6. Treat read-state updates as bulk repository operations
- `MessageRepository.MarkAsReadAsync(...)` already performs a filtered bulk-style load/update/save cycle.
- Extend that pattern for recipient-aware unread tracking instead of updating messages one-by-one from the controller.
- Keep mark-read semantics tied to "messages from the opposite side relevant to this viewer."

### 7. Use SignalR only as an integration boundary, not as source of truth
- Persist first via repository.
- Return the persisted DTO from the service.
- Then publish realtime events via `ChatHub` group membership, as `ChatController.SendMessage(...)` already does.
- Notification counts/lists must remain queryable by REST after reconnect or page reload; SignalR should only prompt refresh.

## Recommended Data Model Direction

### Recommended approach
Extend message persistence to support viewer-specific unread semantics for:
- applicant unread messages from government-side senders
- reviewer/admin unread messages from applicant senders, scoped to assigned reviewer/admin-side visibility

### Why the current model is insufficient
- `Message.IsRead` is a single flag shared for everyone.
- `GetUnreadCountAsync(applicationId, senderType)` counts by sender side only.
- That is acceptable for one applicant and one government bucket, but it is not precise enough if multiple reviewer-side users can access the same application with different assignment scopes.

### Opinionated recommendation
Use the existing `Message` domain, but move unread tracking toward recipient-aware persistence rather than adding more controller-side filtering around the current `IsRead` boolean.

The least disruptive implementation in this codebase is:
1. Keep `Message` as the canonical chat record.
2. Add notification/read-tracking data that can distinguish who has read which opposite-side messages.
3. Query that state through `MessageRepository` plus assignment-aware application joins.

If only one effective reviewer-side audience per application must be tracked, a simpler extension may be enough. If unread state must differ per assigned reviewer/admin user, a separate read-tracking table is the safer model. Current code suggests the second case is more defensible for future correctness.

## Endpoint / Contract Recommendations

### Read endpoints
Use `GET` endpoints returning plain DTOs or simple count objects:
- `GET api/chat/notifications/header`
- `GET api/chat/notifications/applications`
- `GET api/chat/{applicationId}/unread-count`

Recommended shapes:
- header summary: `Ok(new { unreadApplications, unreadMessages })` or a dedicated DTO
- application list: `Ok(IEnumerable<ApplicationChatNotificationDto>)`
- per-application count: keep the current `Ok(new { UnreadCount = count })` pattern for compatibility

### Mutation endpoints
Use `PUT` or `POST` for mark-read semantics:
- `PUT api/chat/{applicationId}/mark-read`
- optionally return `Ok(new { success = true, message = \"Messages marked as read\" })` if frontend benefits from a stable mutation envelope

## Alternatives Considered

| Category | Recommended | Alternative | Why Not |
|----------|-------------|-------------|---------|
| API placement | Extend existing chat/application APIs | New standalone notifications controller/subsystem | Too much surface-area churn for a narrow brownfield enhancement. |
| DTO mapping | Manual service projection for chat notification DTOs | AutoMapper-first design | Current chat service already hand-maps messages; matching that is lower-friction. |
| Read-state storage | Extend message persistence with recipient-aware unread tracking | Reuse single `Message.IsRead` only | Too coarse for assignment-aware reviewer notifications. |
| Realtime integration | Reuse `ChatHub` groups | Separate notification hub | Duplicate transport and authorization logic with little benefit. |

## Integration Boundaries

### Chat boundary
- `ChatController` owns REST chat entrypoints.
- `ChatService` should own notification use cases closely tied to messages.
- `ChatHub` remains the realtime fan-out mechanism.

### Application boundary
- `ApplicationRepository` remains the authority for application ownership, department review, and assignment context.
- Notification queries should depend on that boundary rather than reimplementing access logic from scratch.

### Frontend contract boundary
- Response shapes should stay explicit and stable.
- Include navigation-ready identifiers in every notification DTO so the frontend can route directly to the relevant application chat without extra lookup calls.

## Practical Build Recommendation

Implement new chat notification work with:
- new chat notification DTOs under `DTOs/`
- new `IChatService` methods for header summary, application notification list, and recipient-aware mark-read/count operations
- new `IMessageRepository` methods for grouped unread counts and read-state updates
- selective `IApplicationRepository` support only where assignment/visibility joins are needed
- one EF migration for persistence changes
- optional SignalR event emission after send/read to refresh badges

This matches the current stack instead of fighting it.

## Sources

- Local codebase: [`.planning/PROJECT.md`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/.planning/PROJECT.md)
- Local codebase: [`.planning/codebase/STACK.md`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/.planning/codebase/STACK.md)
- Local codebase: [`.planning/codebase/ARCHITECTURE.md`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/.planning/codebase/ARCHITECTURE.md)
- Local codebase: [`Controllers/ChatController.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Controllers/ChatController.cs)
- Local codebase: [`Services/ChatService.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Services/ChatService.cs)
- Local codebase: [`Repositories/MessageRepository.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Repositories/MessageRepository.cs)
- Local codebase: [`Entities/Message.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Entities/Message.cs)
- Local codebase: [`Controllers/ApplicationsController.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Controllers/ApplicationsController.cs)
- Local codebase: [`Services/ApplicationService.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Services/ApplicationService.cs)
- Local codebase: [`Repositories/ApplicationRepository.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Repositories/ApplicationRepository.cs)
- Local codebase: [`DTOs/ApplicationDtos.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/DTOs/ApplicationDtos.cs)
- Local codebase: [`Program.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Program.cs)
- Local codebase: [`Hubs/ChatHub.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Hubs/ChatHub.cs)
- Local codebase: [`DTOs/MessageDto.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/DTOs/MessageDto.cs)
- Local codebase: [`DTOs/SendMessageDto.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/DTOs/SendMessageDto.cs)
- Local codebase: [`Services/Interfaces/IChatService.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Services/Interfaces/IChatService.cs)
- Local codebase: [`Repositories/Interfaces/IMessageRepository.cs`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/Repositories/Interfaces/IMessageRepository.cs)
