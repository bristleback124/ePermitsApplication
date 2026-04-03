## Project

**Chat Notifications for Application Review**

This project extends the existing ePermits application workflow with unread chat notification support for applicants and assigned reviewer-side users. It builds on the current per-application chat, reviewer assignment, and application access model so the frontend can surface header and per-application badges without introducing a separate notification architecture.

**Core Value:** Users with legitimate access to an application can immediately see when the other side has sent unread chat activity and navigate directly to the correct application chat.

### Constraints

- Reuse the existing controller, service, repository, DTO, and response conventions already used in the backend.
- Respect current application-access rules, reviewer assignment behavior, and role semantics for `applicant`, `user`, and `admin`.
- Extend current chat/message persistence patterns carefully instead of introducing a generic notification subsystem.
- Keep API contracts explicit and stable for frontend integration.
- Keep scope limited to unread notifications for the existing application chat domain.

## Workflow

Before making code changes, start work through a GSD command so planning artifacts and execution context stay in sync.

Use these entry points:
- `/gsd:quick` for small fixes and ad-hoc updates
- `/gsd:debug` for investigation and bug fixing
- `/gsd:execute-phase` for planned phase work

Do not make direct repo edits outside a GSD workflow unless the user explicitly asks to bypass it.

## Planning Artifacts

- Project context: `.planning/PROJECT.md`
- Requirements: `.planning/REQUIREMENTS.md`
- Roadmap: `.planning/ROADMAP.md`
- State: `.planning/STATE.md`
- Codebase map: `.planning/codebase/`
- Research: `.planning/research/`
