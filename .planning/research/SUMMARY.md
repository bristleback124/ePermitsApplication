# Project Research Summary

**Project:** Chat Notifications for Application Review
**Domain:** Brownfield ASP.NET Core backend enhancement for unread application-chat notifications
**Researched:** 2026-04-02
**Confidence:** MEDIUM

## Executive Summary

This work is not a new notification product. It is a backend extension to the existing per-application chat, reviewer assignment, and application access model in the ePermits API. The strongest research signal across stack, feature, architecture, and pitfalls is to keep unread notifications inside the current `ChatController -> ChatService -> Repository -> EF Core` flow, reuse application/reviewer visibility rules, and expose explicit REST payloads the frontend can consume directly for header badges, reviewer dashboard badges, and applicant application-list badges.

The recommended implementation direction is to harden access/scoping first, then add grouped unread query primitives, then expose stable unread-summary and unread-count contracts. For v1, the backend should deliver current-user unread aggregation, per-application unread counts, dashboard/list badge fields, and deterministic mark-read behavior. That keeps the work aligned with the existing codebase and avoids the highest-risk mistake: introducing a parallel notification subsystem that duplicates chat and application authorization rules.

The main risks are authorization drift, ambiguous reviewer scope, and overloading the current global `Message.IsRead` flag beyond what it can safely represent. The plan should explicitly decide whether reviewer-side unread is shared office state or truly per-reviewer state. If the product expectation is "my unread" for individually assigned reviewers, recipient-aware read tracking should be designed before the API contract is finalized. Performance is the other major concern: header and dashboard badges must come from grouped SQL queries over authorized application IDs, not N+1 unread-count calls layered onto heavy dashboard queries.

## Key Findings

### Recommended Stack

The backend stack should stay exactly on the project's existing architecture: ASP.NET Core Web API controllers on .NET 8, service-layer orchestration, repository-driven EF Core queries against SQL Server, and SignalR only for realtime refresh hints after send/read events. Research strongly rejects a new notification service, hub, or table as the starting point for this feature.

Unread behavior should be implemented as chat-domain persistence plus read models. New DTOs should remain hand-shaped and frontend-ready, using the same controller/service/repository conventions already present in chat and application flows. Schema changes should ship through normal EF Core migrations and preserve `Message` as the canonical chat record.

**Core technologies:**
- ASP.NET Core Web API controllers: expose unread endpoints in the existing chat/application API surface rather than a new subsystem.
- `ChatService` and related service-layer helpers: centralize role resolution, reviewer scoping, access checks, and DTO shaping.
- Repository layer over EF Core 8 / SQL Server: execute grouped unread queries and mark-read updates efficiently in SQL.
- Existing `Message` persistence plus EF migrations: extend unread tracking inside the chat domain instead of creating a generic notifications store.
- SignalR `ChatHub`: optional realtime badge refresh trigger after persistence, not the source of truth.

### Expected Features

V1 backend scope is straightforward: unread chat has to be visible wherever the frontend already renders application workflow state. That means a current-user unread summary for the header, per-application unread counts, applicant list badges, reviewer dashboard badges, and predictable mark-read behavior. The feature set should stay application-chat-specific and avoid generic notification-center ambitions.

The research is also clear about what to defer. Per-message recipient receipts, department-wide shared inbox semantics, notification preferences, and a standalone notification system all add policy or model complexity without being required for the initial unread badge experience.

**Must have (table stakes):**
- Current-user unread summary endpoint for header badge and dropdown context.
- Per-application unread count with access-scoped, opposite-side-only counting.
- `UnreadChatCount` / `HasUnreadChat` on applicant application list DTOs.
- `UnreadChatCount` / `HasUnreadChat` on reviewer dashboard DTOs.
- Deterministic `mark-read` semantics tied to the current actor opening the chat.

**Should have (competitive):**
- Header summary items with navigation-ready metadata such as `ApplicationId`, formatted ID, title/counterparty, and last unread timestamp.
- Optional SignalR refresh events after send/read so badges update without reload, while REST remains authoritative.

**Defer (v2+):**
- Generic notification center or cross-feature notification platform.
- Department-shared unread inboxes without an explicit product rule.
- Per-recipient read receipts beyond what v1 actually requires.
- Notification preferences, mute controls, or richer notification policies.

### Architecture Approach

The architectural fit is strong as long as the unread feature is treated as a projection over existing chat and application data. `ChatController` should stay thin, `ChatService` should own actor-side and access-side decisions, `ApplicationRepository` should provide the authoritative set of visible/relevant applications, and `MessageRepository` should provide grouped unread aggregation and mark-read updates. `ApplicationService` should consume unread summaries or counts for dashboard shaping rather than reimplementing chat logic. The key pattern is service-layer scoping plus repository-level grouped projections.

**Major components:**
1. `ChatController` - auth/claim extraction, explicit unread endpoints, HTTP response mapping, optional hub notifications.
2. `ChatService` - central unread orchestration, sender-side resolution, access enforcement, reviewer assignment narrowing, DTO shaping.
3. `ApplicationRepository` / application access helper - authoritative application visibility and assignment context for applicants, reviewers, and admins.
4. `MessageRepository` - grouped unread counts, unread summary projections, and read-state updates.
5. DTO layer - stable chat-focused summary/count contracts and minimal unread fields added to existing list/dashboard DTOs.

### Critical Pitfalls

1. **Using the current broad non-applicant chat access rule for notifications** - replace it with one shared authorized-application scope that matches reviewer assignment and application visibility rules before exposing unread endpoints.
2. **Treating `Message.IsRead` as if it were always per-user unread state** - decide early whether unread is shared government-side state or truly per-reviewer; if it is per-reviewer, design recipient-aware tracking before finalizing APIs.
3. **Leaving reviewer scope ambiguous between department-wide and assigned-reviewer-only** - choose one explicit unread audience rule for `user` and `admin` roles so header, dashboard, and per-application counts agree.
4. **Building header/dashboard badges with N+1 unread queries** - add grouped repository queries over authorized application IDs and avoid looping through heavy dashboard/application results.
5. **Adding unread fields ad hoc across DTOs and endpoints** - define explicit unread summary contracts and update list/dashboard DTOs intentionally so frontend integration stays stable.

## Implications for Roadmap

Based on research, suggested phase structure:

### Phase 1: Access and Unread Semantics
**Rationale:** Every later API and DTO depends on a single, explicit definition of who can see unread chat and what "read" means on the reviewer side.
**Delivers:** Shared access/scoping rules in the service layer, actor-to-sender-side resolution, and a written decision on assignment-only vs shared-office unread semantics.
**Addresses:** Access-scoped unread visibility, role-aware sender-side counting, predictable read semantics.
**Avoids:** Authorization leaks, header/dashboard disagreement, and accidental overuse of the current broad chat access rule.

### Phase 2: Repository and Persistence Primitives
**Rationale:** Once scope is fixed, the backend needs efficient data access before it can safely expose new endpoints or enrich existing DTOs.
**Delivers:** Grouped unread summary queries, scoped unread count queries, mark-read updates aligned to the chosen unread model, and any EF migration needed for recipient-aware tracking.
**Uses:** EF Core 8, existing `Message` persistence, repository projections, SQL grouping.
**Implements:** `MessageRepository` unread aggregation plus `ApplicationRepository` notification-scope application queries.

### Phase 3: Chat Notification APIs
**Rationale:** Explicit REST contracts should be added only after scope and query primitives are stable, so the frontend is not built against provisional semantics.
**Delivers:** Header unread summary endpoint, hardened per-application unread count endpoint, and stable mutation response for mark-read.
**Addresses:** Header badge, notification dropdown context, direct application-chat navigation, authoritative REST unread state.
**Avoids:** Anonymous/shifting payloads and controller-level business logic drift.

### Phase 4: Existing Surface Integration
**Rationale:** Once unread APIs exist, the backend can enrich the existing application surfaces without duplicate query logic.
**Delivers:** `UnreadChatCount` / `HasUnreadChat` on applicant list DTOs and reviewer dashboard DTOs, composed from shared unread summaries/counts.
**Addresses:** Table-stakes v1 badges on applicant application lists and reviewer dashboards.
**Avoids:** N+1 per-row unread calls and divergence between dashboard/list and header counts.

### Phase 5: Realtime Refresh and Verification
**Rationale:** Realtime polish should come after persisted REST correctness so cross-device and refresh behavior stays trustworthy.
**Delivers:** Optional SignalR invalidation events after send/read, backend verification for role/access edge cases, and performance validation on grouped unread paths.
**Addresses:** Fresh badge UX without changing the source of truth.
**Avoids:** Treating SignalR as unread persistence and shipping latent performance regressions.

### Phase Ordering Rationale

- Access rules come first because every unread endpoint, count, and badge depends on the same authorized application set.
- Repository primitives come before API work because the feature must aggregate unread data in SQL, not through controller/service loops.
- Chat-specific unread endpoints should stabilize before enriching application DTOs so existing frontend surfaces consume one authoritative unread path.
- Realtime behavior belongs last because it is an optimization over persisted unread state, not a substitute for it.

### Research Flags

Phases likely needing deeper research during planning:
- **Phase 1:** Decide whether reviewer unread is assigned-reviewer-specific or shared government-side state; this is the largest semantic and schema decision.
- **Phase 2:** If per-reviewer unread is required, research and design the recipient-aware read-tracking model carefully before migration and API contract work.
- **Phase 5:** Validate performance and concurrency behavior if mark-read updates and summary aggregation interact with high-volume dashboard loads.

Phases with standard patterns (skip research-phase):
- **Phase 3:** Adding explicit ASP.NET Core controller/service/repository endpoints is a standard extension pattern in this codebase.
- **Phase 4:** DTO enrichment of existing application list/dashboard responses is straightforward once unread aggregation is authoritative.

## Confidence Assessment

| Area | Confidence | Notes |
|------|------------|-------|
| Stack | HIGH | Strongly grounded in the existing .NET 8 codebase structure and local implementation patterns. |
| Features | MEDIUM | User-facing needs are clear, but reviewer-side unread semantics still need an explicit product rule. |
| Architecture | HIGH | The recommended fit with current controller/service/repository boundaries is clear and internally consistent. |
| Pitfalls | HIGH | The main failure modes are concrete, codebase-specific, and repeatedly supported across the research outputs. |

**Overall confidence:** MEDIUM

### Gaps to Address

- Reviewer unread ownership: planning must lock down whether unread is shared across government viewers or isolated to assigned reviewers.
- Admin scope: research assumes admins keep broad reviewer-side visibility, but planning should confirm whether admin unread should mirror reviewer dashboards or span all accessible applications.
- Data model sufficiency: if product language implies true "my unread" for multiple government users, `Message.IsRead` alone is not sufficient and schema work should be pulled forward.
- Existing chat access alignment: current chat access rules are broader than the proposed unread scope and should be reconciled during implementation, not worked around endpoint by endpoint.

## Sources

### Primary (HIGH confidence)
- [`.planning/PROJECT.md`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/.planning/PROJECT.md) - project scope, constraints, and required backend outcomes.
- [`.planning/research/STACK.md`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/.planning/research/STACK.md) - recommended stack and persistence direction grounded in local code patterns.
- [`.planning/research/ARCHITECTURE.md`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/.planning/research/ARCHITECTURE.md) - component boundaries, access-control placement, and build order.
- [`.planning/research/PITFALLS.md`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/.planning/research/PITFALLS.md) - codebase-specific failure modes and mitigations.

### Secondary (MEDIUM confidence)
- [`.planning/research/FEATURES.md`](/C:/Users/Harvie/Documents/Coding/ePermit/ePermitsApplication/.planning/research/FEATURES.md) - table-stakes v1 backend capabilities and deferrals.

### Tertiary (LOW confidence)
- None. Open questions are product-scope decisions rather than missing source material.

---
*Research completed: 2026-04-02*
*Ready for roadmap: yes*
