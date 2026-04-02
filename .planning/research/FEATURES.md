# Feature Landscape

**Domain:** Brownfield unread application-chat notifications for ePermits
**Researched:** 2026-04-02
**Overall confidence:** MEDIUM

## Executive View

This codebase already has the backend primitives for application chat: per-application message history, message send, message read-marking, a message-level `IsRead` flag, and an application/reviewer workflow with department and assignment metadata. That means unread chat notifications should be treated as an extension of the existing application and chat APIs, not as a new cross-product notification platform.

The main table-stakes expectation for users is simple: if the other side sends a chat message on an application the current user is allowed to access, the backend must expose a reliable unread signal in the places the frontend already cares about. For applicants, that is their own applications. For reviewer-side users, that is the reviewer dashboard and application detail views, scoped to the applications they are legitimately responsible for.

The current implementation is not enough for that expectation. `GET /api/chat/{applicationId}/unread-count` only returns a per-application count, and unread/read logic is keyed only by `SenderType`, not by a concrete recipient or reviewer assignment. That is workable for a first release only if reviewer-side notification scope is explicitly limited to assigned reviewer/admin-visible applications and the API shapes make those limits clear.

## Table Stakes

Features users will expect for unread chat notifications in this backend.

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Per-application unread count endpoint | The chat view and application cards need a numeric badge for a single application | Low | Already exists in `ChatController`, but it needs stronger scoping guarantees for reviewer-side users |
| Mark messages as read when the user opens chat | Unread badges must clear predictably after viewing the thread | Low | Already exists, but current read logic marks by `SenderType` only |
| Header-level total unread count for current user | A global badge is the minimum signal users expect before drilling into specific applications | Medium | Missing today; should aggregate only accessible application chats |
| Notification summary list for unread application chats | A header dropdown or notification tray needs more than a number; it needs application identity and recent message context | Medium | Should include application id, formatted id, title/applicant label, unread count, last unread timestamp, sender side |
| Reviewer dashboard per-application unread badge | Reviewer-side workflow is driven from the dashboard, so unread chat must surface there | Medium | Best added as fields on `ReviewerDashboardItemDto` rather than a separate lookup-heavy endpoint |
| Applicant application-list unread badge | Applicants also need unread visibility before opening application detail/chat | Medium | Best added to the applicant application list DTO or a dedicated summary endpoint |
| Role-aware sender-side counting | Users should only see unread messages from the opposite side | Low | Current implementation already infers this using `Applicant` vs `Government` |
| Access-scoped unread visibility | Users must not see unread counts for applications outside their workflow scope | High | This is the most important backend rule for correctness and trust |

## Required Backend Capabilities

### 1. Current-user unread aggregation

The backend needs a current-user unread summary endpoint that returns all unread chat items relevant to the authenticated user. This is the missing backbone for header notifications.

Recommended v1 contract:

```http
GET /api/chat/unread-summary
```

Recommended response shape:

```json
{
  "totalUnreadCount": 5,
  "items": [
    {
      "applicationId": 123,
      "formattedId": "BP-2026-000123",
      "projectTitle": "Warehouse Renovation",
      "counterpartyLabel": "Juan Dela Cruz",
      "unreadCount": 2,
      "lastMessageAt": "2026-04-02T08:15:00Z",
      "lastMessagePreview": "Please check the revised plan set...",
      "senderType": "Applicant"
    }
  ]
}
```

Why this is table stakes:
- A header badge without drilldown context forces the frontend to make multiple extra calls.
- The frontend already wants to navigate directly to an application chat, so the response must identify the application directly.
- Returning only counts is not enough for notification UX.

### 2. Inline unread badges on existing application surfaces

Unread counts should be attachable to the DTOs the frontend already uses, instead of requiring N+1 chat calls.

Recommended v1 additions:
- `ApplicationDtoShort`: `UnreadChatCount`, `HasUnreadChat`
- `ReviewerDashboardItemDto`: `UnreadChatCount`, `HasUnreadChat`, optionally `LastChatAt`

This is the cleanest brownfield fit because:
- applicant list already comes from `GetApplicationsByUserIdAsync`
- reviewer dashboard already comes from `GetReviewerDashboardAsync`
- both endpoints already define the application collections the frontend renders

### 3. Predictable read transition

Unread state must clear when the current user views or explicitly reads the application chat.

v1 expectation:
- keep `PUT /api/chat/{applicationId}/mark-read`
- define it as "mark opposite-side unread messages for this application as read for the current user"
- return either `200 OK` with a small payload or continue with empty `200 OK`, but behavior must be explicit in API notes

Why this matters:
- Without a deterministic read transition, badge counts drift and users stop trusting them.

### 4. Opposite-side only unread logic

The backend should continue counting only messages from the other side:
- applicants count unread `Government` messages
- reviewer/admin-side users count unread `Applicant` messages

This aligns with current behavior and user expectation. A user does not need to be notified about their own outbound messages.

## Scoping Rules

These rules are the real feature boundary for v1.

### Applicant scope

Applicants should see unread chat notifications only for applications where `Application.UserId == current user`.

User-visible outcome:
- header count covers only the applicant's own applications
- application list badges cover only the applicant's own applications
- per-application unread count is allowed only on the applicant's own applications

Confidence: HIGH, because current `ChatService.ValidateApplicationAccessAsync` already enforces applicant ownership.

### Admin scope

Admins currently have broad visibility in both chat access and reviewer workflow access. v1 can treat admins as having reviewer-side notification visibility across applications they can already access.

User-visible outcome:
- admin header count can aggregate unread applicant messages across accessible applications
- admin dashboard badges can surface across all dashboard items

Confidence: MEDIUM, because current chat validation does not narrow admin access beyond non-applicant status.

### Department user / assigned reviewer scope

This is the most important scoping decision. Reviewer-side unread notifications should follow reviewer workflow relevance, not raw department membership alone.

Recommended v1 rule:
- a department `user` can see unread applicant messages only for applications where their department is involved and they are the assigned reviewer for that department review
- if product wants department-shared visibility later, that should be a deliberate follow-up, not default v1 behavior

Why this should be v1:
- `ApplicationDepartmentReview` already has `AssignedReviewerId`
- status updates are already restricted to the assigned reviewer in `ApplicationService.CanUpdateDepartmentReview`
- assignment-aware unread notifications avoid "everyone in the department gets pinged" noise

Important current gap:
- `CanAccessApplication` currently grants department users access to any application with a review in their department, even if they are not the assigned reviewer
- `ChatService.ValidateApplicationAccessAsync` currently does not enforce department or assignment scoping at all for non-applicants

So the feature expectation for v1 is not just "show unread badges"; it is "show unread badges only for the reviewer-side users who are actually accountable for that application."

## API Contract Expectations

These are the backend contract expectations the frontend will rely on.

| Expectation | Why It Matters | v1 Recommendation |
|-------------|----------------|-------------------|
| Stable current-user endpoint | Header notifications should not require passing user ids from the client | Use authenticated-user context, not route user id |
| Explicit unread fields on list DTOs | Frontend should not make one unread-count call per row | Add unread fields to existing application list/dashboard DTOs |
| Application identity in notification items | Frontend needs direct navigation into the correct application/chat | Always include `applicationId` and `formattedId` |
| Summary context in notification items | Notification UI needs something human-readable | Include `projectTitle` or applicant label plus `lastMessageAt` |
| Clear read semantics | Badge-clearing bugs are common and visible | Document that mark-read affects opposite-side unread messages for current user scope |
| Scoping consistency across endpoints | Header badge and row badges must agree | Use one shared service/repository path for unread counting |

## MVP Recommendation

Prioritize these v1 backend capabilities:

1. Add assignment-aware/current-user-aware unread aggregation for application chat.
2. Add `UnreadChatCount` and `HasUnreadChat` to `ReviewerDashboardItemDto`.
3. Add `UnreadChatCount` and `HasUnreadChat` to `ApplicationDtoShort`.
4. Add `GET /api/chat/unread-summary` for header badge + dropdown use.
5. Keep and harden per-application `unread-count` and `mark-read`.

This is enough for a usable first release because it covers:
- header notification badge
- applicant application-list badges
- reviewer dashboard badges
- navigation into the correct application chat
- read-state clearing after viewing

## Later, Not V1

These are reasonable follow-ups, but they should not block the first release.

| Feature | Why Defer | What to Do Instead in V1 |
|---------|-----------|--------------------------|
| Generic platform notification center | Too broad for a chat-specific enhancement | Keep notifications chat-scoped and application-linked |
| Per-message read receipts by recipient | Current model is not recipient-specific | Use aggregate unread counts and per-thread mark-read |
| Department-shared unread inboxes | Changes ownership semantics and creates noise risk | Restrict reviewer-side notifications to assigned reviewers first |
| Real-time unread badge push updates | SignalR delivery already exists for chat messages, but badge sync can be added after stable REST contracts | Let frontend refresh unread summary/list views after send and read actions |
| Notification preferences / mute settings | Adds user settings and policy complexity | Ship default unread behavior first |

## Anti-Features

Features to explicitly avoid in this phase.

| Anti-Feature | Why Avoid | What to Do Instead |
|--------------|-----------|-------------------|
| Separate notification table/system for v1 | Duplicates existing chat domain and increases brownfield risk | Derive unread state from messages and existing application access rules |
| Exposing unread counts for any department-visible application by default | Creates noisy, misleading reviewer notifications | Scope to assigned reviewer relevance |
| Client-computed unread counts from full message lists | Inefficient and inconsistent with dashboard/header usage | Compute unread counts server-side |
| Passing arbitrary user ids into unread APIs | Creates trust and authorization problems | Use authenticated current user context |

## Feature Dependencies

```text
Access-scoped application visibility -> correct unread counting
correct unread counting -> header unread summary
correct unread counting -> applicant list badges
correct unread counting -> reviewer dashboard badges
mark-read semantics -> trustworthy badge clearing
```

## Sources

- `.planning/PROJECT.md`
- `Controllers/ChatController.cs`
- `Services/ChatService.cs`
- `Repositories/MessageRepository.cs`
- `Controllers/ApplicationsController.cs`
- `Services/ApplicationService.cs`
- `Repositories/ApplicationRepository.cs`
- `DTOs/ApplicationDtos.cs`
- `DTOs/MessageDto.cs`
- `DTOs/SendMessageDto.cs`
- `Entities/Message.cs`
- `Entities/ApplicationDepartmentReview.cs`
