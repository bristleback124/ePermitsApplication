# Phase 1: Access-Scoped Unread Rules - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md - this log preserves the alternatives considered.

**Date:** 2026-04-02
**Phase:** 01-access-scoped-unread-rules
**Areas discussed:** Reviewer-side access rule, read-clearing rule details, assignment-change behavior, shared unread rule source

---

## Reviewer-side access rule

| Option | Description | Selected |
|--------|-------------|----------|
| Assigned reviewer only for normal reviewer users, broader admin oversight where existing admin access allows | Reviewer-side `user` unread is limited to explicit assigned reviewers; admins can still see unread under existing admin access | Yes |
| Department-wide reviewer visibility | Any department user on the matching review office can see reviewer-side unread | |
| Other | Another reviewer-scope rule chosen by the user | |

**User's choice:** Assigned reviewer only for normal reviewer users, with admin oversight visibility preserved where existing admin access already allows it.
**Notes:** The user clarified that when an applicant sends a message on a building permit, all assigned reviewers on that application, such as OBO and BFP assigned reviewers, should receive unread notification. Admins should also see the unread notification. Unassigned department users should not receive reviewer-side unread state.

---

## Read-clearing rule details

| Option | Description | Selected |
|--------|-------------|----------|
| Per-user clear on view | Opening or viewing chat clears unread only for the currently logged-in user | Yes |
| Per-role clear | One reviewer viewing would clear unread for the whole reviewer-side audience | |
| Manual clear only | Opening chat does not clear unread until an explicit mark-read action | |

**User's choice:** Per-user clear on view.
**Notes:** The user stated that if a specific reviewer views the message, it should mark as read only for that reviewer's own state and not for others. The same per-user behavior should apply to admins. Applicant-side unread should follow the same actor-specific clear rule.

---

## Assignment-change behavior

| Option | Description | Selected |
|--------|-------------|----------|
| Snapshot recipients at send time | Recipients who were assigned when the message was sent keep their unread state even if assignment later changes | Yes |
| Follow current assignment state | Old unread moves to whoever is currently assigned | |
| Hybrid | Old recipients keep unread and new assignees may also inherit old unread | |

**User's choice:** Snapshot recipients at send time.
**Notes:** Reassignment must not transfer old unread state to newly assigned reviewers. If a reviewer later loses access to the application, that reviewer should no longer be able to view chat or clear old unread that was previously theirs.

---

## Shared unread rule source

| Option | Description | Selected |
|--------|-------------|----------|
| Yes, lock one canonical unread-scope rule now | Phase 1 defines the authoritative semantics for receive, count, and clear behavior | Yes |
| Partially | Lock visibility only and defer count/clear wiring | |
| No, defer it | Leave shared unread-rule shape open for later phases | |

**User's choice:** Yes, lock one canonical unread-scope rule now.
**Notes:** The user wants later phases to build on a single unread rule source rather than drift between header, dashboard, and per-application behavior.

---

## the agent's Discretion

- Internal schema and repository query design needed to represent per-user unread state
- Exact enforcement points across service and repository layers, as long as they preserve the locked semantics

## Deferred Ideas

None
