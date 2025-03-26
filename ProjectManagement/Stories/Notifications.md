# User Story: Notifications

**Title:** Notifications

**Description:**
As a **user**, I want to **receive notifications for new comments on my posts** so that **I can engage with my audience**.

**Acceptance Criteria:**
- Users receive notifications when someone comments on their posts.
- Notifications are displayed on the user dashboard or as a notification bell icon.
- Users can view a list of notifications and mark them as read.

**Estimated Time:** 6 hours

**Priority:** Medium

---

## Tasks for Notifications

- [x] **Design Notification System Architecture**
   - **Considerations**:
     - Define a database schema for notifications (e.g., notification type, message, user ID, post ID, timestamp, status).
     - Decide whether to use a real-time service (like SignalR) for instant notifications or batch processing.
   - **Cross-Functional**: Collaborate with backend developers for architecture.
   - **Estimate**: 3 hours
   - **Definition of Done**: Notification system design document is completed.

- [x] **Implement Logic to Send Notifications**
   - **Considerations**:
     - Create event listeners to trigger notifications when comments or reactions occur.
     - Ensure notifications are stored in the database and marked as unread by default.
     - Implement background jobs if needed for processing large volumes of notifications.
   - **Cross-Functional**: Work with frontend developers for integration.
   - **Estimate**: 4 hours
   - **Definition of Done**: Notifications are sent out as intended based on user actions.

- [x] **Create Notification View for Users**
   - **Considerations**:
     - Design a user-friendly interface for displaying notifications (e.g., using Bootstrap modals or dropdowns).
     - Ensure that notifications can be filtered (e.g., all, unread, by type).
     - Implement a mark-as-read feature that updates the database.
   - **Estimate**: 3 hours
   - **Definition of Done**: Users can view their notifications in a structured manner.

- [ ] **Set Up Notification Preferences**
   - **Considerations**:
     - Allow users to choose which notifications they want to receive (e.g., comments, likes).
     - Implement a user settings page where preferences can be managed.
     - Ensure changes are reflected in real-time or upon next login.
   - **Estimate**: 3 hours
   - **Definition of Done**: Users can successfully modify their notification preferences.

---

## Additional Suggestions:
- **Testing**: Ensure you allocate time for testing both the backend logic and frontend display.
- **Documentation**: Maintain clear documentation for future reference on how the notification system works.
- **User Feedback**: Consider gathering user feedback on the notification system after implementation to make improvements.


## Notification types and recipients

| Notification Type      | Triggering Event                  | Recipients                            |
|------------------------|-----------------------------------|---------------------------------------|
| **Comment**            | A comment is added to a post      | Original poster, commentors          |
| **Reaction**           | A reaction is added to a post     | Original poster                       |
| **New Post**           | A new post is created             | Followers of the user                 |
| **Tag**                | A user is tagged                  | Tagged user                           |
| **Direct Message**     | A new direct message is sent      | Message recipient                     |
| **Follow**             | A user follows another user       | Followed user                         |
