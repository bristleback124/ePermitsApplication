# Domain Pitfalls

**Domain:** Brownfield unread chat notifications for application chat
**Researched:** 2026-04-02

## Critical Pitfalls

### Pitfall 1: Reusing the current chat access check for reviewer notifications
**What goes wrong:** Reviewer-side unread notifications are exposed to any authenticated government user instead of only the assigned reviewer or otherwise-authorized office viewer.
**Why it happens:** `ChatService.ValidateApplicationAccessAsync` only blocks applicants from other users' applications. For non-applicants it effectively allows access if the application exists. That is already too broad for notification endpoints, especially header-level aggregation across many applications.
**Consequences:** Unauthorized reviewers can see unread counts, recent activity presence, or application ids for applications outside their assignment scope. Header notifications become a new data-leak surface even if chat message bodies stay hidden.
**Prevention:** Do not reuse the current chat access rule as-is. Route notification visibility through the same assignment-aware rules used by reviewer dashboard access, then tighten further for unread semantics. Decide explicitly whether department users can see unread counts for all department applications or only `AssignedReviewerId == currentUser.Id`.
**Detection:** A department user from the correct department but not assigned to a review can still get unread counts for an application, or a government user can enumerate application ids through notification endpoints.

### Pitfall 2: Treating `Message.IsRead` as valid per-user unread state
**What goes wrong:** One reviewer reading applicant messages clears unread state for all reviewers because read state is stored once on the message row.
**Why it happens:** The model only has `Message.IsRead`, and `MessageRepository.MarkAsReadAsync` bulk-updates every unread message for `applicationId + senderType`. There is no recipient identity, role scope, or reviewer scope in read tracking.
**Consequences:** Header badges and dashboard unread counts become wrong as soon as more than one government user can access the same application. The first reader suppresses notifications for everyone else.
**Prevention:** If unread must be reviewer-specific, add recipient-scoped read state now instead of layering more endpoints over `IsRead`. If the product intentionally wants shared office-level unread state, document that explicitly and keep all API names and UI copy aligned to "office unread", not "my unread".
**Detection:** Two reviewer accounts open the same application; one marks read and the other user's header/dashboard count drops to zero without opening chat.

### Pitfall 3: Mixing assignment scope with department scope without a written rule
**What goes wrong:** Reviewer dashboard unread badges and header notifications disagree on who should be notified.
**Why it happens:** `ApplicationService.GetReviewerDashboardAsync` filters department users to applications with a matching department review, but review status updates are restricted more tightly by `AssignedReviewerId`. The codebase already has two different reviewer scopes, and unread notifications will be ambiguous unless the new feature picks one.
**Consequences:** The dashboard may show an unread badge while the header does not, or vice versa. Frontend logic starts compensating for backend inconsistency, which makes future fixes harder.
**Prevention:** Define one unread audience rule per role before implementation:
1. Applicant unread = government-authored unread on their own application.
2. Reviewer unread = either assigned-reviewer-only or department-wide office unread.
3. Admin unread = explicit rule, not implicit carryover from reviewer behavior.
**Detection:** Same reviewer account sees different unread totals between header and dashboard for the same application set.

### Pitfall 4: Adding notification DTO fields ad hoc and drifting from existing contracts
**What goes wrong:** New unread fields are injected into existing DTOs or controller responses inconsistently, forcing frontend conditionals and brittle parsing.
**Why it happens:** Existing APIs mix raw DTO returns (`MessageDto`) with anonymous response wrappers (`new { UnreadCount = count }`). Reviewer dashboard items are AutoMapper-driven, while chat responses are hand-mapped in `ChatService`. That makes it easy to add `UnreadCount`, `HasUnread`, or last-message summaries in only one path.
**Consequences:** The frontend must special-case payloads per endpoint. Property naming drift between PascalCase JSON output and frontend expectations becomes harder to track. AutoMapper updates can silently omit new notification fields if the mapping is not updated everywhere.
**Prevention:** Define explicit unread notification DTOs before coding. Keep one stable shape for header items and one stable shape for dashboard items. Avoid anonymous objects for notification responses. If enriching `ReviewerDashboardItemDto`, update the mapping profile and every endpoint contract intentionally.
**Detection:** Frontend needs fallback checks like `item.unreadCount ?? item.UnreadCount ?? 0`, or one endpoint returns unread state while another still requires an extra count call.

### Pitfall 5: Building header counts with N+1 unread queries
**What goes wrong:** Header notifications become slow because unread counts are computed per application in a loop over already-heavy dashboard/application queries.
**Why it happens:** `ApplicationRepository.GetDashboardDetailedAsync` already loads a large graph. If notification code fetches visible applications first and then calls `MessageRepository.GetUnreadCountAsync` for each one, the feature adds one count query per application on top of eager loading.
**Consequences:** Reviewer dashboard and header rendering latency grows with application volume. The unread feature becomes the reason an already-heavy dashboard query tips into poor performance.
**Prevention:** Aggregate unread counts in SQL with a single grouped query over the authorized application set. Project only the fields needed for notifications; do not reuse full dashboard/detail includes just to calculate badges.
**Detection:** Loading the reviewer dashboard or header causes many repeated message count queries, or response time scales linearly with application count.

## Moderate Pitfalls

### Pitfall 1: Using sender type alone to define unread semantics
**What goes wrong:** Counts include or exclude the wrong messages because the logic assumes only two sides exist: `"Applicant"` and `"Government"`.
**Why it happens:** `ChatController` derives unread direction from role and passes only `senderType` into repository methods. That is sufficient for a single-thread, two-party model, but fragile once reviewer-specific semantics or admin behavior matter.
**Prevention:** Keep "who sent the message" separate from "who still needs to read it". Do not extend unread logic by adding more `senderType` conditionals alone.

### Pitfall 2: Forgetting that message reads are bulk updates with no transaction boundary
**What goes wrong:** A mark-read call clears a large set of rows, but related notification summaries are computed from stale or separately-saved state.
**Why it happens:** Repositories own `SaveChangesAsync()` internally, so notification workflows cannot easily coordinate multi-step updates atomically.
**Prevention:** Keep unread state derivable from a single source of truth, or move multi-entity unread updates into one service-level transaction if the design adds separate notification tables.

### Pitfall 3: Returning unread data for applications the dashboard later trims away
**What goes wrong:** Header counts include applications that the reviewer dashboard does not render, confusing users and the frontend.
**Why it happens:** Dashboard shaping is done in `ApplicationService` after repository loading and AutoMapper projection. A separate notification query can easily miss the same trimming rules.
**Prevention:** Centralize the "visible application ids for current user" query and reuse it for both dashboard data and header notifications.

### Pitfall 4: Assuming SignalR delivery equals unread-state correctness
**What goes wrong:** The UI badge logic is built around realtime events only, but unread counts diverge after refresh, reconnect, or cross-device use.
**Why it happens:** `ChatController.SendMessage` publishes SignalR events to application groups, but the hub already has weak authorization and realtime delivery is not a substitute for persisted unread semantics.
**Prevention:** Treat SignalR as an optimization only. The source of truth for badges must be server-side unread state that survives reconnects and can be queried directly.

## Minor Pitfalls

### Pitfall 1: Naming/enum drift around government sender terminology
**What goes wrong:** Comments and code disagree on whether non-applicant messages are `"Government"` or `"Admin"`, and downstream logic copies the wrong term.
**Prevention:** Normalize constants or enums for sender categories before extending the feature. Do not rely on string literals spread across controller, service, repository, and frontend code.

### Pitfall 2: Silent null/empty sender display issues in notification summaries
**What goes wrong:** Header notification items show blank names if sender profile data is missing or not eagerly loaded in a new projection.
**Prevention:** Decide whether notification summaries need sender display names at all. If they do, project them deliberately instead of assuming the existing message include graph is always present.

### Pitfall 3: Overloading existing dashboard DTOs with chat-specific UI concerns
**What goes wrong:** `ReviewerDashboardItemDto` accumulates unread count, latest message preview, last chat timestamp, and badge flags until it becomes a mixed workflow/chat contract that is hard to evolve.
**Prevention:** Add only the minimum unread fields needed for the dashboard, or use a dedicated notification projection if richer chat metadata is required.

## Phase-Specific Warnings

| Phase Topic | Likely Pitfall | Mitigation |
|-------------|---------------|------------|
| Permission design | Copying `ValidateApplicationAccessAsync` into new notification endpoints | Define a single authorized-application query per role and assignment rule first |
| Data model change | Extending global `IsRead` instead of introducing recipient-aware state | Decide shared-office vs per-reviewer unread semantics before schema/API work |
| Reviewer dashboard badge | Computing unread counts per row after loading dashboard includes | Use one grouped unread query joined to authorized application ids |
| Header notifications | Returning counts/items for applications frontend cannot open | Reuse the same access filter and include application id + navigation-safe metadata only |
| DTO/API work | Adding anonymous unread payloads or scattered extra fields | Create explicit DTOs and update AutoMapper/manual mappings together |
| Frontend hookup | Trusting SignalR event stream as the unread source of truth | Make the REST notification endpoints authoritative; use SignalR only to prompt refresh |

## Sources

- Local project brief: `.planning/PROJECT.md`
- Local concern audit: `.planning/codebase/CONCERNS.md`
- Local source: `Controllers/ChatController.cs`
- Local source: `Services/ChatService.cs`
- Local source: `Repositories/MessageRepository.cs`
- Local source: `Services/ApplicationService.cs`
- Local source: `Repositories/ApplicationRepository.cs`
- Local source: `Entities/Message.cs`
- Local source: `Entities/ApplicationDepartmentReview.cs`
- Local DTO/mapping references: `DTOs/ApplicationDtos.cs`, `DTOs/MessageDto.cs`, `DTOs/SendMessageDto.cs`, `Mappings/ApplicationProfile.cs`
