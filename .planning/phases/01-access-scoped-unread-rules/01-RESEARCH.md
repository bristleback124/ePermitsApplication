# Phase 1: Access-Scoped Unread Rules - Research

**Researched:** 2026-04-02
**Domain:** Access-scoped unread chat rules in the existing ASP.NET Core + EF Core backend
**Confidence:** HIGH

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions
## Implementation Decisions

### Reviewer-side unread recipients
- **D-01:** When an applicant sends a message, reviewer-side unread state is created only for explicitly assigned reviewers on that application at send time, across all relevant review offices.
- **D-02:** Normal reviewer-side `user` accounts only receive unread state when they are the assigned reviewer for their review office. Unassigned department users do not receive reviewer-side unread notifications.
- **D-03:** `admin` users may also see reviewer-side unread activity where existing admin application oversight access already allows it.

### Actor-specific read behavior
- **D-04:** Opening or viewing chat clears unread state only for the currently logged-in actor. A reviewer clearing unread does not clear it for other assigned reviewers or for admins.
- **D-05:** Applicant-side unread follows the same actor-specific read rule. Reviewer-side messages clear only for the owning applicant when that applicant views or reads the chat.

### Assignment-change behavior
- **D-06:** Reviewer-side unread recipients are determined as a snapshot at message send time. Reassignment later does not transfer old unread state to newly assigned reviewers.
- **D-07:** If a reviewer later loses access to the application, that reviewer must no longer be able to view chat or clear old unread state that was previously theirs.

### Shared unread rule source
- **D-08:** Phase 1 should define one canonical backend unread-scope rule covering who receives unread, who can count unread, and who can clear unread. Later phases must reuse that rule for persistence, header summaries, and dashboard badges.

### Claude's Discretion
- The exact internal schema and query shape used to support per-user unread state can be chosen during research and planning, as long as it preserves the locked recipient, access, and actor-specific read semantics above.

### Deferred Ideas (OUT OF SCOPE)
## Deferred Ideas

None - discussion stayed within phase scope
</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|------------------|
| CHAT-01 | Applicant unread chat counts only include unread messages on applications owned by the logged-in applicant and sent by reviewer-side users | Reuse `Application.UserId` ownership checks and count unread via recipient rows filtered to the current applicant plus opposite-side sender type |
| CHAT-02 | Reviewer-side unread chat counts only include unread applicant messages for applications where the logged-in admin or department user is the relevant assigned reviewer under existing assignment rules | Resolve reviewer recipients from `ApplicationDepartmentReview.AssignedReviewerId` snapshot-at-send and gate later reads through current `CanAccessApplication`/assignment rules |
| CHAT-03 | Opening or marking an application chat as read clears unread state only for the current actor's opposite-side unread messages without affecting unrelated applications or unauthorized users | Mark recipient rows read by `(applicationId, recipientUserId, oppositeSenderType)` instead of flipping a shared message flag |
</phase_requirements>

## Summary

The current chat implementation cannot satisfy Phase 1 as-is. `ChatService` validates applicant ownership only, while `MessageRepository` treats unread as a single message-level boolean filtered only by `ApplicationId` and `SenderType`. That means any reviewer or applicant read operation would clear unread for every eligible user on that side, which directly violates D-04, D-05, and CHAT-03.

The least disruptive path is to keep the existing controller -> service -> repository layering and add a per-recipient unread persistence model. Messages should remain the source of chat content, but unread should move to a new table keyed by message and recipient user so the backend can snapshot recipients at send time, count only the current actor's unread rows, and clear only that actor's rows while still rechecking live access before returning counts or allowing reads.

**Primary recommendation:** Add a `MessageRecipientState`-style entity/table and make it the only source of truth for unread visibility, counts, and mark-read behavior.

## Standard Stack

### Core
| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| ASP.NET Core Web API | net8.0 target in repo | Existing HTTP/controller surface for chat endpoints | Already hosts the chat API and matches project constraints |
| Entity Framework Core | 8.0.23 | Schema changes, unread queries, migrations | Existing persistence layer and migration workflow |
| SQL Server provider | 8.0.23 | Storage for message and unread-recipient rows | Existing relational backend for chat/application data |

### Supporting
| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| AutoMapper | 16.0.0 | Existing DTO mapping conventions | Only where unread DTOs need explicit mapping later |
| ASP.NET Core auth/JWT | 8.0.23 package set | Current role/user identity source | Required for actor-specific unread scope |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| Per-recipient unread table | Keep `Message.IsRead` and infer per-user state in queries | Cannot represent independent read state for multiple reviewers/admins |
| Per-recipient unread table | Store one unread counter per application/user | Loses message-level traceability and complicates opposite-side clear semantics |
| Existing chat architecture | Generic notification subsystem | Violates project constraints and duplicates authorization logic |

**Installation:**
```bash
dotnet restore
```

**Version verification:** Repo pins `Microsoft.EntityFrameworkCore` and `Microsoft.EntityFrameworkCore.SqlServer` to `8.0.23` in `ePermitsApp.csproj`. No new external package is required for this phase.

## Architecture Patterns

### Recommended Project Structure
```text
Controllers/
  ChatController.cs          # Keep endpoints thin
Services/
  ChatService.cs             # Canonical unread scope and orchestration
Repositories/
  MessageRepository.cs       # Message + unread-recipient queries
  ApplicationRepository.cs   # Assignment/access loading
Entities/
  Message.cs
  MessageRecipientState.cs   # New per-user unread persistence
Data/
  ApplicationDbContext.cs    # DbSet + model config
Migrations/                  # Schema migration for unread-recipient state
```

### Pattern 1: Canonical unread scope service rule
**What:** Put one unread-scope decision path in `ChatService` for send, count, and clear.
**When to use:** Every unread-related operation in this project, including later phases.
**Example:**
```csharp
private async Task<UnreadActorScope> GetUnreadActorScopeAsync(int applicationId, int userId, string userRole)
{
    var application = await _applicationRepository.GetUnreadScopeApplicationAsync(applicationId);
    if (application == null)
        throw new ArgumentException("Application not found.");

    if (string.Equals(userRole, "applicant", StringComparison.OrdinalIgnoreCase))
    {
        if (application.UserId != userId)
            throw new UnauthorizedAccessException("Access denied to this application.");

        return UnreadActorScope.ForApplicant(applicationId, userId);
    }

    var currentUser = await _userRepository.GetByIdAsync(userId)
        ?? throw new UnauthorizedAccessException("User not found.");

    if (!CanAccessApplication(currentUser, application))
        throw new UnauthorizedAccessException("Access denied to this application.");

    return UnreadActorScope.ForReviewer(currentUser, application);
}
```

### Pattern 2: Snapshot recipients at send time
**What:** Resolve opposite-side recipients when a message is created, then persist one unread row per recipient.
**When to use:** Only on send; do not recompute historical recipients after reassignment.
**Example:**
```csharp
var message = await _messageRepository.AddAsync(new Message { ... });
var recipients = await ResolveUnreadRecipientsAsync(message.ApplicationId, senderUserId, senderRole);

await _messageRepository.AddRecipientStatesAsync(
    recipients.Select(recipient => new MessageRecipientState
    {
        MessageId = message.Id,
        RecipientUserId = recipient.UserId,
        RecipientRole = recipient.Role,
        SenderType = message.SenderType,
        IsRead = false
    }));
```

### Pattern 3: Actor-specific clear by recipient row
**What:** Mark only the current actor's opposite-side unread rows as read for one application.
**When to use:** `mark-read` endpoint and any explicit chat-open read action.
**Example:**
```csharp
await _messageRepository.MarkRecipientRowsAsReadAsync(
    applicationId: applicationId,
    recipientUserId: actor.UserId,
    senderType: actor.OppositeSenderType);
```

### Anti-Patterns to Avoid
- **Using `Message.IsRead` as the unread source of truth:** It is global, not actor-specific.
- **Resolving reviewer unread from department membership alone:** Phase 1 requires explicit assignment, not department-wide inbox semantics.
- **Transferring unread on reassignment:** D-06 locks recipients at send time.
- **Skipping a live access check on count/clear:** D-07 requires old unread rows to become non-observable once access is lost.

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Cross-user unread state | Global booleans or ad hoc counters on `Message`/`Application` | A normalized recipient-state table | Supports independent reads, auditability, and precise queries |
| Reviewer access rules | New notification-specific authorization rules | Existing application ownership, department access, and assignment checks | Prevents divergence from the current application model |
| Notification platform | Generic event bus or inbox system | Chat-specific unread persistence in the current chat domain | Keeps scope aligned with Phase 1 and project constraints |

**Key insight:** The hard part is not counting unread messages; it is preserving snapshot recipients while enforcing current access at read time. A dedicated recipient-state table solves that without introducing a parallel notification architecture.

## Common Pitfalls

### Pitfall 1: Clearing unread for all reviewers
**What goes wrong:** One reviewer opens chat and all reviewer unread disappears.
**Why it happens:** Current repository methods update every message row matching `(applicationId, senderType)`.
**How to avoid:** Update rows by recipient user, not just by application and sender side.
**Warning signs:** Counts drop for another assigned reviewer or admin after someone else opens chat.

### Pitfall 2: Treating all department users as recipients
**What goes wrong:** Unassigned users see applicant unread for their department.
**Why it happens:** Existing dashboard filtering is department-based, but Phase 1 reviewer unread is assignment-based.
**How to avoid:** Build reviewer recipients from `ApplicationDepartmentReview.AssignedReviewerId`, with admin handling layered on top.
**Warning signs:** A `user` without assignment sees unread count on an application they can list or inspect.

### Pitfall 3: Recomputing historical recipients after reassignment
**What goes wrong:** New assignees inherit old unread they did not receive.
**Why it happens:** Counts are derived from current assignment instead of send-time recipient rows.
**How to avoid:** Persist recipient rows when the message is sent and never backfill them on reassignment.
**Warning signs:** Reassigning a review office changes unread counts for old messages.

### Pitfall 4: Allowing stale unread visibility after access loss
**What goes wrong:** A reviewer still sees or clears unread tied to rows that already exist after losing access.
**Why it happens:** Query filters use recipient rows but skip current access validation.
**How to avoid:** Apply the same live access validation before listing messages, counting unread, or marking read.
**Warning signs:** Removing assignment or department access does not immediately block unread endpoints.

## Code Examples

Verified repo-aligned patterns:

### Access gating before unread operations
```csharp
await ValidateApplicationAccessAsync(applicationId, userId, userRole);
return await _messageRepository.GetUnreadCountAsync(applicationId, senderType);
```
Source: existing pattern in `Services/ChatService.cs`

### Assignment source of truth
```csharp
if (review.AssignedReviewerId != currentUser.Id)
{
    return false;
}
```
Source: existing pattern in `Services/ApplicationService.cs`

### Department user application visibility
```csharp
return application.DepartmentReviews.Any(r => r.DepartmentId == currentUser.DepartmentId.Value);
```
Source: existing pattern in `Services/ApplicationService.cs`

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| `Messages.IsRead` as one global flag per message | Per-recipient unread rows joined to messages | Phase 1 implementation | Enables actor-specific reads and send-time recipient snapshots |
| Count unread by `(ApplicationId, SenderType)` | Count unread by `(ApplicationId, RecipientUserId, SenderType)` plus live access check | Phase 1 implementation | Prevents cross-user unread leakage |
| Mark read for all opposite-side messages in an application | Mark read only for the current actor's recipient rows in that application | Phase 1 implementation | Satisfies CHAT-03 without touching unrelated applications |

**Deprecated/outdated:**
- `Message.IsRead` as the sole unread authority: insufficient for assigned-reviewer and admin-specific semantics.

## Open Questions

1. **Admin recipient persistence rule**
   - What we know: D-03 says admins may see reviewer-side unread where existing admin oversight access allows it.
   - What's unclear: Whether unread rows should be created for all admin users at send time, or whether admin visibility should be derived at read time from message + access scope.
   - Recommendation: Decide this explicitly in planning. If admin user count is small and stable, persist admin recipient rows; otherwise keep admin counts derived at read time but do not let that path diverge from the canonical unread rule.

2. **When "opening chat" marks as read**
   - What we know: Success criteria allow opening or explicitly marking as read to clear unread.
   - What's unclear: Whether backend `GetMessages` should auto-clear, or frontend should call `mark-read` immediately after loading.
   - Recommendation: Keep `GetMessages` side-effect free and let the frontend call `mark-read` on chat open. It preserves clearer API behavior and matches the current endpoint shape.

## Environment Availability

| Dependency | Required By | Available | Version | Fallback |
|------------|------------|-----------|---------|----------|
| .NET SDK | Build, migrations, future tests | Yes | 9.0.303 | Use installed SDK to target `net8.0` |
| `sqlcmd` CLI | Manual SQL inspection if needed during migration/debugging | Yes | Detected, exact version not captured | EF migrations only if CLI is unnecessary |
| Docker | Local disposable DB/testing option | No | - | Use an existing SQL Server instance configured via connection string |

**Missing dependencies with no fallback:**
- None confirmed during research. SQL Server instance reachability was not verified from this audit.

**Missing dependencies with fallback:**
- Docker is absent; use an existing SQL Server connection for migration and validation work.

## Validation Architecture

### Test Framework
| Property | Value |
|----------|-------|
| Framework | None detected |
| Config file | none - see Wave 0 |
| Quick run command | `dotnet test` after a test project is added |
| Full suite command | `dotnet test` after a test project is added |

### Phase Requirements -> Test Map
| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|-------------|
| CHAT-01 | Applicant only sees reviewer-side unread for owned applications | integration/service | `dotnet test --filter CHAT_01` | No - Wave 0 |
| CHAT-02 | Assigned reviewer/admin unread scope respects assignment and access rules | integration/service | `dotnet test --filter CHAT_02` | No - Wave 0 |
| CHAT-03 | Mark-read clears only current actor unread for one application | integration/service | `dotnet test --filter CHAT_03` | No - Wave 0 |

### Sampling Rate
- **Per task commit:** `dotnet test --filter Phase1`
- **Per wave merge:** `dotnet test`
- **Phase gate:** Full suite green before `/gsd:verify-work`

### Wave 0 Gaps
- [ ] Add a backend test project to `ePermitsApp.sln`
- [ ] Add test database strategy for EF Core integration tests against unread queries
- [ ] Add fixtures/builders for users, applications, department reviews, messages, and unread-recipient rows
- [ ] Add Phase 1 authorization/read-state tests covering applicant, assigned reviewer, admin, unassigned reviewer, and access-loss scenarios

## Sources

### Primary (HIGH confidence)
- `AGENTS.md` - project constraints and required GSD workflow
- `.planning/phases/01-access-scoped-unread-rules/01-CONTEXT.md` - locked decisions and phase boundary
- `.planning/REQUIREMENTS.md` - requirement text for CHAT-01 through CHAT-03
- `Controllers/ChatController.cs` - current unread-count and mark-read API behavior
- `Services/ChatService.cs` - current access validation and unread orchestration
- `Repositories/MessageRepository.cs` - current global unread count and mark-read queries
- `Services/ApplicationService.cs` - current application access and assigned-reviewer update rules
- `Repositories/ApplicationRepository.cs` - current application and department-review loading patterns
- `Entities/Message.cs` - current message-level unread model
- `Entities/ApplicationDepartmentReview.cs` - assigned reviewer persistence shape
- `Data/ApplicationDbContext.cs` - EF model and current DbSet surface
- `Migrations/20260124073649_AddChatTables.cs` - original chat schema showing global `IsRead`

### Secondary (MEDIUM confidence)
- `Repositories/UserRepository.cs` - assignable reviewer query pattern and admin/user role handling
- `ePermitsApp.csproj` - repo-pinned framework and package versions

### Tertiary (LOW confidence)
- None

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH - fully derived from the current repo stack and phase constraints
- Architecture: HIGH - current code clearly shows the mismatch and the lowest-risk extension point
- Pitfalls: HIGH - directly evidenced by existing repository/service behavior against locked decisions

**Research date:** 2026-04-02
**Valid until:** 2026-05-02
