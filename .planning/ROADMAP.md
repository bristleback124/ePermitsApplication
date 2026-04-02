# Roadmap: Chat Notifications for Application Review

## Overview

This roadmap delivers unread chat notifications as a brownfield backend enhancement inside the existing ePermits chat and application-review flow. The phases follow the actual dependency chain in the current system: first make unread visibility and mark-read behavior assignment-aware, then harden persistence and grouped query primitives, then expose stable header APIs, then wire the same unread semantics into reviewer dashboard badges, and finally lock the work down with backend verification focused on authorization, consistency, and frontend-safe contracts.

## Phases

**Phase Numbering:**
- Integer phases (1, 2, 3): Planned milestone work
- Decimal phases (2.1, 2.2): Urgent insertions (marked with INSERTED)

Decimal phases appear between their surrounding integers in numeric order.

- [ ] **Phase 1: Access-Scoped Unread Rules** - Define who can see unread chat and how mark-read behaves for applicants and assigned reviewers.
- [ ] **Phase 2: Unread Persistence Foundations** - Reuse current chat and repository patterns so new unread activity is stored and queried consistently.
- [ ] **Phase 3: Header Unread Summary APIs** - Expose stable header unread summary contracts with navigation-ready application data.
- [ ] **Phase 4: Reviewer Dashboard Badge Integration** - Surface per-application unread counts on reviewer-facing application rows using the same unread rules.
- [ ] **Phase 5: Verification and Contract Stability** - Prove the backend behavior is correct, scoped, and safe for frontend integration.

## Phase Details

### Phase 1: Access-Scoped Unread Rules
**Goal**: Users can only see unread chat activity that is relevant to their role, application access, and assignment context, and read actions only clear the correct opposite-side messages.
**Depends on**: Nothing (first phase)
**Requirements**: CHAT-01, CHAT-02, CHAT-03
**Success Criteria** (what must be TRUE):
  1. An applicant can only retrieve unread chat activity for their own applications, and only when the unread messages were sent by reviewer-side users.
  2. An assigned reviewer-side user can only retrieve unread applicant chat activity for applications they are authorized to review under existing assignment rules.
  3. Opening or marking one application chat as read clears only that actor's opposite-side unread state for that application and leaves unrelated applications unchanged.
  4. Unauthorized users cannot observe or clear unread chat state for applications outside their existing access scope.
**Plans**: 3 plans

Plans:
- [ ] 01-01-PLAN.md - Add the Phase 1 backend test project, fixture host, and failing unread-scope tests
- [ ] 01-02-PLAN.md - Add recipient-state persistence, migration, and recipient-aware repository contracts
- [ ] 01-03-PLAN.md - Implement canonical unread scope in ChatService and keep controller contracts stable

### Phase 2: Unread Persistence Foundations
**Goal**: New chat messages immediately create the correct unread state for the opposite side through the existing chat, assignment, authorization, DTO, and repository patterns.
**Depends on**: Phase 1
**Requirements**: INTE-01, INTE-02
**Success Criteria** (what must be TRUE):
  1. After an applicant sends a chat message, the relevant assigned reviewer-side recipients can see new unread state without a separate notification subsystem.
  2. After a reviewer-side user sends a chat message, the owning applicant can see new unread state for that application.
  3. Unread persistence and grouped query behavior follow the backend's current controller, service, repository, and DTO conventions so downstream endpoints stay explicit and stable.
**Plans**: TBD

### Phase 3: Header Unread Summary APIs
**Goal**: Logged-in users can request a stable unread summary for the header that groups relevant unread chat by application and includes direct navigation metadata.
**Depends on**: Phase 2
**Requirements**: NOTF-01, NOTF-02, NOTF-03
**Success Criteria** (what must be TRUE):
  1. A logged-in user can request a header unread summary that includes all and only the unread application-chat activity relevant to that user.
  2. Each unread summary item includes stable application identifiers and related metadata the frontend can use to navigate directly to the correct chat.
  3. Header unread responses use explicit DTOs and response shapes that do not require the frontend to infer payload structure.
**Plans**: TBD

### Phase 4: Reviewer Dashboard Badge Integration
**Goal**: Reviewer-facing application rows expose per-application unread chat badges that stay consistent with the header summary and the underlying assignment-aware unread rules.
**Depends on**: Phase 3
**Requirements**: BADG-01, BADG-02
**Success Criteria** (what must be TRUE):
  1. A reviewer-side user can retrieve unread chat count per relevant application row for the reviewer dashboard.
  2. Reviewer dashboard unread counts use the same access rules and opposite-side unread semantics as the header summary for the same user.
  3. Per-application unread badge data is available through explicit backend contracts suitable for stable dashboard integration.
**Plans**: TBD
**UI hint**: yes

### Phase 5: Verification and Contract Stability
**Goal**: The unread chat notification enhancement is verified against role scope, assignment scope, unauthorized access boundaries, grouped summaries, and badge consistency before rollout.
**Depends on**: Phase 4
**Requirements**: INTE-03
**Success Criteria** (what must be TRUE):
  1. Backend tests prove applicant unread visibility only includes reviewer-side messages on the applicant's own applications.
  2. Backend tests prove reviewer-side unread visibility only includes applicant messages for relevant assigned applications and rejects unauthorized access.
  3. Backend tests prove grouped header summaries and per-application badge counts stay consistent for the same unread state.
  4. Backend tests prove mark-read behavior clears only the intended application and actor-specific opposite-side unread state.
**Plans**: TBD

## Progress

**Execution Order:**
Phases execute in numeric order: 1 -> 2 -> 3 -> 4 -> 5

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Access-Scoped Unread Rules | 0/TBD | Not started | - |
| 2. Unread Persistence Foundations | 0/TBD | Not started | - |
| 3. Header Unread Summary APIs | 0/TBD | Not started | - |
| 4. Reviewer Dashboard Badge Integration | 0/TBD | Not started | - |
| 5. Verification and Contract Stability | 0/TBD | Not started | - |
