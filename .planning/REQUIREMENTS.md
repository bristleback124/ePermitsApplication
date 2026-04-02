# Requirements: Chat Notifications for Application Review

**Defined:** 2026-04-02
**Core Value:** Users with legitimate access to an application can immediately see when the other side has sent unread chat activity and navigate directly to the correct application chat.

## v1 Requirements

### Access and Unread Semantics

- [ ] **CHAT-01**: Applicant unread chat counts only include unread messages on applications owned by the logged-in applicant and sent by reviewer-side users
- [ ] **CHAT-02**: Reviewer-side unread chat counts only include unread applicant messages for applications where the logged-in admin or department user is the relevant assigned reviewer under existing assignment rules
- [ ] **CHAT-03**: Opening or marking an application chat as read clears unread state only for the current actor's opposite-side unread messages without affecting unrelated applications or unauthorized users

### Header Notifications

- [ ] **NOTF-01**: Logged-in users can request a header unread chat summary grouped by application for all unread chat activity relevant to them
- [ ] **NOTF-02**: Each header notification item includes stable navigation-ready data needed by the frontend to open the related application chat directly
- [ ] **NOTF-03**: Header notification responses follow the backend's existing explicit DTO and response structure conventions so frontend integration does not rely on inferred payload shapes

### Application Badges

- [ ] **BADG-01**: Reviewer dashboard consumers can request or receive unread chat count per application for rows relevant to the logged-in reviewer-side user
- [ ] **BADG-02**: Per-application unread chat counts use the same access rules and unread semantics as the header notification summary so counts stay consistent across backend surfaces

### Integration and Stability

- [ ] **INTE-01**: Existing chat send-message behavior persists unread state in a way that immediately makes new unread activity visible to the correct opposite-side recipients
- [ ] **INTE-02**: Notification-related backend changes reuse current chat, reviewer assignment, authorization, DTO, and repository patterns instead of introducing a separate notification architecture
- [ ] **INTE-03**: Backend tests cover applicant unread visibility, assigned-reviewer unread visibility, unauthorized access rejection, grouped header summaries, and per-application badge counts

## v2 Requirements

### Realtime and Extended Notification Behavior

- **REAL-01**: Users receive realtime badge-refresh hints for unread chat summary changes without reloading the page
- **REAL-02**: Header notifications expose richer recent-chat preview metadata such as last unread sender name or content excerpt

### Notification Platform Expansion

- **PLAT-01**: Chat notifications expand into a generic in-app notification center shared with non-chat events
- **PLAT-02**: Users can configure chat notification preferences, mute rules, or delivery channels

## Out of Scope

| Feature | Reason |
|---------|--------|
| Email, SMS, or push delivery for chat notifications | The requested feature is unread in-app notification support for the existing chat flow |
| Generic notification subsystem unrelated to application chat | This project must extend the current chat architecture, not create a parallel platform |
| Shared department-wide unread inbox semantics | The requirement is that only relevant assigned reviewers receive unread applicant chat notifications |
| Frontend implementation details beyond stable backend contracts | This initialization scopes backend behavior so frontend work can integrate against explicit APIs and tests |

## Traceability

Which phases cover which requirements. Updated during roadmap creation.

| Requirement | Phase | Status |
|-------------|-------|--------|
| CHAT-01 | Unmapped | Pending |
| CHAT-02 | Unmapped | Pending |
| CHAT-03 | Unmapped | Pending |
| NOTF-01 | Unmapped | Pending |
| NOTF-02 | Unmapped | Pending |
| NOTF-03 | Unmapped | Pending |
| BADG-01 | Unmapped | Pending |
| BADG-02 | Unmapped | Pending |
| INTE-01 | Unmapped | Pending |
| INTE-02 | Unmapped | Pending |
| INTE-03 | Unmapped | Pending |

**Coverage:**
- v1 requirements: 11 total
- Mapped to phases: 0
- Unmapped: 11

---
*Requirements defined: 2026-04-02*
*Last updated: 2026-04-02 after initial definition*
