# Phase 1: Access-Scoped Unread Rules - Context

**Gathered:** 2026-04-02
**Status:** Ready for planning

<domain>
## Phase Boundary

Define who can see unread application-chat activity and who can clear unread state for an application, based on role, current access, assignment context, and actor-specific read behavior. This phase sets unread visibility and mark-read semantics only; it does not define header payloads, dashboard contracts, or a separate notification subsystem.

</domain>

<decisions>
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

### the agent's Discretion
- The exact internal schema and query shape used to support per-user unread state can be chosen during research and planning, as long as it preserves the locked recipient, access, and actor-specific read semantics above.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Planning and requirements
- `.planning/PROJECT.md` - Project scope, constraints, and brownfield chat-notification context
- `.planning/REQUIREMENTS.md` - Phase 1 requirement mapping for `CHAT-01`, `CHAT-02`, and `CHAT-03`
- `.planning/ROADMAP.md` - Phase 1 goal, success criteria, and dependency position
- `.planning/STATE.md` - Current project focus and phase sequencing context

### Existing unread and chat behavior
- `Controllers/ChatController.cs` - Current unread-count and mark-read endpoints, plus sender-type role split
- `Services/ChatService.cs` - Current application access validation and chat unread orchestration
- `Repositories/MessageRepository.cs` - Current coarse unread counting and global mark-read implementation
- `Entities/Message.cs` - Existing message persistence model with message-level `IsRead`

### Existing application access and assignment behavior
- `Services/ApplicationService.cs` - Current reviewer dashboard filtering, assignment flow, and access/update rules
- `Repositories/ApplicationRepository.cs` - Application and department-review data-loading paths used by access checks
- `Entities/ApplicationDepartmentReview.cs` - Assigned reviewer relationship and review-office assignment state
- `DTOs/ApplicationDtos.cs` - Existing reviewer dashboard and application review-office DTO shapes

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `Services/ChatService.cs`: Existing service entry points for `GetMessagesAsync`, `GetUnreadCountAsync`, and `MarkAsReadAsync` provide the natural place to centralize unread-scope semantics.
- `Repositories/MessageRepository.cs`: Existing unread query methods can be extended or replaced to support actor-specific unread state while keeping repository ownership of EF queries.
- `Services/ApplicationService.cs`: Existing `CanAccessApplication`, `CanUpdateDepartmentReview`, dashboard filtering, and assignment flows define the current reviewer/admin/applicant access model to reuse.
- `Repositories/ApplicationRepository.cs`: Existing `GetDepartmentReviewAsync` and detailed application queries already load assignment and review-office context needed for unread scope decisions.

### Established Patterns
- Controller -> Service -> Repository layering is already in place and should remain the integration path for unread behavior changes.
- Applicant access is tied directly to `Application.UserId`, while reviewer-side access today is derived from review-office and assignment context rather than a separate permission system.
- DTO and response contracts are explicit and feature-specific; later unread APIs should follow that same pattern instead of inferred payloads.
- Current unread implementation is coarse and message-level (`Entities/Message.cs` plus `MessageRepository.cs`), so the data model must evolve to support per-user unread state without breaking the existing chat flow.

### Integration Points
- `Controllers/ChatController.cs` is the current API surface for per-application unread count and mark-read behavior.
- `Services/ChatService.cs` is the canonical place to enforce unread recipient, count, and clear semantics before later phases expose grouped summaries or badge data.
- `Services/ApplicationService.cs` and related review-office assignment data are the source of truth for which reviewer-side users are relevant to an application.
- Future phases for header summaries and dashboard badges should depend on the same Phase 1 unread-scope rule rather than introduce parallel authorization logic.

</code_context>

<specifics>
## Specific Ideas

- Applicant messages should create unread state for all assigned reviewers on that application, such as OBO and BFP reviewers assigned to the same permit.
- Admin users should be able to see unread under existing admin oversight access, but their read state should remain user-specific rather than clearing unread for everyone else.
- The behavior should feel like each eligible user has their own unread state for the same application chat, rather than a single global read flag.

</specifics>

<deferred>
## Deferred Ideas

None - discussion stayed within phase scope

</deferred>

---

*Phase: 01-access-scoped-unread-rules*
*Context gathered: 2026-04-02*
