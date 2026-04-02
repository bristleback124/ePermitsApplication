# Architecture Patterns

**Domain:** Brownfield unread chat notification enhancement for application chat
**Researched:** 2026-04-02
**Confidence:** HIGH for current codebase fit, MEDIUM for exact unread state semantics if multi-reviewer read tracking is needed later

## Recommended Architecture

Unread chat notifications should remain a chat concern inside the existing `ChatController -> ChatService -> MessageRepository -> EF Core` flow, with application access and reviewer assignment reused from the existing application domain instead of introducing a separate notification subsystem.

The practical shape is:

1. `ChatController` exposes explicit unread endpoints and keeps doing only auth/claim extraction plus HTTP response mapping.
2. `ChatService` becomes the orchestration layer for:
   - resolving the current actor (`applicant`, `user`, `admin`)
   - enforcing application access
   - deciding which messages count as unread for that actor
   - requesting aggregated unread data from repositories
3. `ApplicationRepository` remains the source of truth for which applications a user can see, especially reviewer-assignment and department scoping.
4. `MessageRepository` remains the source of truth for message persistence and unread aggregation queries.
5. EF Core continues to persist unread state on `Message`; do not add a parallel notification aggregate or generic notification center.

Recommended boundary:

```text
ChatController
  -> ChatService
       -> Application access policy/helper
       -> ApplicationRepository
       -> MessageRepository
  -> DTO response
```

## Component Boundaries

| Component | Responsibility | Communicates With |
|-----------|---------------|-------------------|
| `ChatController` | Route handling, claim extraction, endpoint auth, HTTP status mapping | `IChatService`, SignalR hub |
| `ChatService` | Unread use cases, access enforcement, recipient/sender scoping, DTO shaping | `IApplicationRepository`, `IMessageRepository`, optional shared access helper |
| `ApplicationRepository` | Load applications and assignment/access data needed to decide visibility | EF Core `ApplicationDbContext` |
| `MessageRepository` | Message CRUD, unread counts, unread summary queries, mark-read updates | EF Core `ApplicationDbContext` |
| `ApplicationService` | Existing reviewer dashboard shaping; should consume unread summaries rather than reimplementing chat logic | `IApplicationRepository`, `IChatService` or chat summary DTOs |
| `Message` entity | Canonical unread state storage for application chat | `Application`, `User` |
| `Application` / `ApplicationDepartmentReview` | Canonical ownership, department visibility, and reviewer assignment graph | `Message`, reviewer dashboard flows |

## Where Access Control Should Live

Access control should live in the service layer, not in controllers and not in repositories.

Recommended rule split:

- `Controller`: authenticate and pass `userId` and `role` to the service.
- `Service`: decide whether this actor can access the application and whether they are allowed to see unread items for it.
- `Repository`: accept already-scoped IDs and execute database queries without embedding role logic.

For this codebase, the current `ChatService.ValidateApplicationAccessAsync` is too weak because it only blocks applicants from other users' applications. It does not mirror the richer reviewer-side access rules already present in `ApplicationService`.

Recommended refinement:

- Extract a shared application access policy in the service layer.
- Use the same policy for:
  - `GetMessagesAsync`
  - `GetUnreadCountAsync`
  - unread summary/list endpoints
  - `MarkAsReadAsync`
- Keep department and assignment decisions based on `Application.DepartmentReviews` and `ApplicationDepartmentReview.AssignedReviewerId`.

Opinionated rule set:

- `applicant`: may access only `Application.UserId == currentUserId`
- `user` department reviewer: may access only applications that contain a department review for their department; unread notifications should be further narrowed to items assigned to them when an `AssignedReviewerId` exists
- `admin`: may access all applications, but unread summaries should still be derived from the same application/review graph instead of bypassing it

## Where Recipient Scoping Should Live

Recipient scoping should also live in `ChatService`, with repository support for query predicates.

Reason:

- sender/recipient semantics are business rules, not persistence rules
- the repository can count/filter, but it should not decide what "relevant to this actor" means
- the current design already infers chat sides from role by mapping to `SenderType`

Recommended unread scoping model:

- Applicant unread = unread messages on applications they own where `SenderType == "Government"`
- Reviewer-side unread = unread messages on applications they are allowed to act on where `SenderType == "Applicant"`
- Reviewer dashboard unread = same reviewer-side unread rule, but projected per application
- Header unread summary = same rule, but aggregated across all accessible applications

Repository methods should therefore accept scoped application IDs plus sender type rather than user roles. Example shape:

```csharp
Task<int> GetUnreadCountAsync(int applicationId, string senderType);
Task<IReadOnlyList<ApplicationUnreadSummary>> GetUnreadSummariesAsync(
    IReadOnlyCollection<int> applicationIds,
    string senderType);
Task MarkAsReadAsync(int applicationId, string senderType);
```

The service should compute `applicationIds` and `senderType`.

## Data Flow

### Per-Application Unread Count

1. `ChatController` receives `GET /api/chat/{applicationId}/unread-count`
2. Controller extracts `userId` and `role`
3. `ChatService` validates access using the shared application access policy
4. `ChatService` resolves the unread sender side:
   - applicant reads `Government`
   - reviewer/admin reads `Applicant`
5. `MessageRepository` returns the count for that application and sender side
6. Controller returns a stable unread-count DTO

### Header Unread Summary

1. `ChatController` receives a new endpoint such as `GET /api/chat/unread-summary`
2. `ChatService` resolves the caller's accessible application IDs from `ApplicationRepository`
3. `ChatService` narrows those IDs for reviewer users:
   - department-visible applications only
   - if assignment exists, prefer assigned applications only for notification relevance
4. `ChatService` resolves sender side to count against
5. `MessageRepository` groups unread messages by `ApplicationId`
6. `ChatService` enriches grouped results with lightweight application metadata already available from the application aggregate
7. Controller returns:
   - total unread count
   - per-application unread counts
   - minimal application navigation fields

### Mark As Read

1. Client opens an application chat
2. `ChatController` calls `MarkAsRead`
3. `ChatService` validates access using the same policy as read/list flows
4. `ChatService` resolves the opposite sender side for the current actor
5. `MessageRepository` updates unread messages for that application and sender side
6. Optional: controller emits a SignalR event so connected clients refresh badges

## Recommended Build Shape

Do not build a separate `NotificationService`, `NotificationRepository`, or notification table for this feature. The unread notification is a projection over existing chat and application data.

Preferred additions:

- `ChatService`
  - add `GetUnreadSummaryAsync(userId, role)`
  - centralize actor-to-sender-side resolution
  - centralize application access and reviewer scoping
- `MessageRepository`
  - add grouped unread summary query by application
  - keep mark-read and per-application count queries here
- `ApplicationRepository`
  - add a read model/query that returns applications visible to the current actor for notification purposes
  - include department review assignment data needed for reviewer narrowing
- DTOs
  - add a dedicated unread summary DTO; keep it chat-focused, not generic notification-shaped

## Anti-Patterns To Avoid

### Anti-Pattern 1: Parallel Notification Subsystem
**What:** Adding separate notification tables, services, and controllers that duplicate chat ownership and review assignment rules.
**Why bad:** It creates a second source of truth for who should be notified and will drift from `Application` and `ApplicationDepartmentReview`.
**Instead:** Keep unread as a derived chat read model backed by `Message` and application access rules.

### Anti-Pattern 2: Controller-Level Authorization Logic
**What:** Encoding role and assignment decisions directly in controller actions.
**Why bad:** Message list, unread count, mark-read, and future summary endpoints will diverge.
**Instead:** Put all authorization and actor scoping in `ChatService` or a shared service-layer access helper.

### Anti-Pattern 3: Repository-Level Role Semantics
**What:** Passing `userRole` into repositories and letting repositories decide which applications are relevant.
**Why bad:** Repositories should execute predicates, not own business rules.
**Instead:** Service computes scope; repository runs scoped queries.

### Anti-Pattern 4: Reimplementing Reviewer Access Separately For Chat
**What:** Creating new chat-specific reviewer visibility rules disconnected from `ApplicationService` patterns.
**Why bad:** Reviewer dashboard and chat badges will disagree.
**Instead:** Reuse the same department/assignment model already used by application review flows.

## Known Constraint In Current Model

Current unread state is a single `Message.IsRead` flag. That fits a two-sided chat model if unread is tracked at the audience side level:

- applicant side
- government side

It is acceptable for this feature if "reviewer-side unread" is treated as a shared government-side unread state for the relevant scoped reviewer audience.

It becomes insufficient if the product later requires independent unread state for multiple government users on the same application at the same time. If that requirement appears, evolve the chat aggregate carefully with per-user read receipts inside the chat domain, not as a generic notification subsystem.

## Recommended Build Order

1. **Unify access/scoping in `ChatService`**
   - Introduce a shared helper for application visibility and actor-side resolution.
   - Align chat access with the existing application/reviewer rules before adding new unread endpoints.

2. **Add repository query primitives**
   - Add grouped unread summary queries in `MessageRepository`.
   - Add notification-scope application query support in `ApplicationRepository`.

3. **Add chat unread DTOs and endpoints**
   - Add header unread summary endpoint.
   - Keep per-application unread count endpoint, but route it through the new shared scoping path.

4. **Integrate reviewer dashboard badges**
   - Feed per-application unread counts into reviewer dashboard DTO shaping.
   - Do this by composition from chat unread summaries, not by duplicating unread queries inside `ApplicationService`.

5. **Tighten mark-read behavior**
   - Ensure mark-read uses the same application visibility rules and actor-side sender resolution.
   - Optionally emit SignalR refresh events for badge updates.

## Sources

- Local project context: `.planning/PROJECT.md`
- Existing codebase architecture: `.planning/codebase/ARCHITECTURE.md`
- Chat entrypoints: `Controllers/ChatController.cs`, `Services/ChatService.cs`
- Persistence: `Repositories/MessageRepository.cs`, `Repositories/ApplicationRepository.cs`
- Application access patterns: `Services/ApplicationService.cs`
- Domain entities: `Entities/Application.cs`, `Entities/ApplicationDepartmentReview.cs`, `Entities/Message.cs`
