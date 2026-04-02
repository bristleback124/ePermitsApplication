# Chat Notifications for Application Review

## What This Is

This project extends the existing ePermits application workflow with unread chat notification support for applicants and assigned reviewer-side users. It builds on the current per-application chat, reviewer assignment, and application access model so the frontend can surface header and per-application badges without introducing a separate notification architecture.

## Core Value

Users with legitimate access to an application can immediately see when the other side has sent unread chat activity and navigate directly to the correct application chat.

## Requirements

### Validated

- ✓ Applicants can submit and track permit applications through the existing workflow - existing
- ✓ Admin and department users can view reviewer dashboards and assign department reviewers - existing
- ✓ Authorized users can exchange per-application chat messages through REST and SignalR - existing
- ✓ Reviewer-side application access is already scoped by role, department, and assignment-related review data - existing

### Active

- [ ] Add backend support for header-level unread chat notifications relevant to the logged-in user
- [ ] Add backend support for reviewer dashboard unread chat badges per application
- [ ] Ensure unread chat visibility is role-based so applicants only see reviewer/admin activity and reviewer-side users only see applicant activity relevant to their assigned applications
- [ ] Keep chat notification APIs, DTOs, and response shapes explicit and stable for direct frontend integration

### Out of Scope

- Email, SMS, or push notifications for chat activity - the request is for unread in-app/backend notification support only
- Replacing SignalR chat delivery with a different messaging stack - the current chat transport already exists and should be extended, not replaced
- Creating a generic cross-feature notification center unrelated to application chat - this work is intentionally scoped to the existing chat system
- Broad permission model rewrites for application access - existing backend access rules should remain the source of truth

## Context

The codebase is a brownfield ASP.NET Core Web API on .NET 8 with a layered Controller -> Service -> Repository -> EF Core structure. Current chat support already exists in `Controllers/ChatController.cs`, `Services/ChatService.cs`, `Repositories/MessageRepository.cs`, `Entities/Message.cs`, and `Hubs/ChatHub.cs`. Reviewer assignment and reviewer dashboard flows already exist in `Controllers/ApplicationsController.cs`, `Services/ApplicationService.cs`, `Repositories/ApplicationRepository.cs`, `DTOs/ApplicationDtos.cs`, and related workflow helpers.

Unread state exists today only as a simple message-level `IsRead` flag and a per-application unread-count endpoint in `Controllers/ChatController.cs`, but the current implementation is coarse and does not yet provide header-level notification listing or reviewer-assignment-aware scoping across applications. Department-user access already trims visible applications and review-office data, while applicant access is tied to the owning `Application.UserId`. This feature needs to extend those patterns rather than bypass them.

The frontend will consume backend results to navigate the user to the related application and auto-open chat. That means application identity, unread counts, and enough summary data to render stable badges or recent-chat notification items need to be available directly from backend DTOs and controller responses.

## Constraints

- **Architecture**: Reuse the existing controller, service, repository, DTO, and response conventions already used in the backend - avoid introducing a parallel notification subsystem
- **Authorization**: Notification visibility must respect current application-access rules, reviewer assignment behavior, and role semantics for `applicant`, `user`, and `admin`
- **Data model**: Extend current chat/message persistence patterns carefully because unread behavior already depends on `Entities/Message.cs` and repository queries
- **Frontend integration**: APIs must be explicit, stable, and easy for the existing frontend to consume for header badges, per-application badges, and navigation hooks
- **Scope**: Deliver unread chat notifications for the current application chat domain only, not a generalized platform notification feature

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Treat this as a brownfield extension of the existing application chat system | The repository already has submission, review, assignment, dashboard, and chat patterns that should remain authoritative | — Pending |
| Scope unread notifications to application chat only | The request is specific to existing chat behavior and frontend badge needs | — Pending |
| Preserve current DTO and response conventions instead of inventing a new API style | Frontend integration should follow the backend contract patterns already in use | — Pending |
| Use assignment-aware reviewer scoping as a core requirement | Only relevant assigned reviewers should be notified when applicants send messages | — Pending |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition** (via `$gsd-transition`):
1. Requirements invalidated? -> Move to Out of Scope with reason
2. Requirements validated? -> Move to Validated with phase reference
3. New requirements emerged? -> Add to Active
4. Decisions to log? -> Add to Key Decisions
5. "What This Is" still accurate? -> Update if drifted

**After each milestone** (via `$gsd-complete-milestone`):
1. Full review of all sections
2. Core Value check - still the right priority?
3. Audit Out of Scope - reasons still valid?
4. Update Context with current state

---
*Last updated: 2026-04-02 after initialization*
