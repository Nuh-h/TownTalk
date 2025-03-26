# User Story: Create Post

**Title:** Create Post

**Description:**
As a **logged-in user**, I want to **create a new post** so that **I can share news, events, or alerts that are relevant to my community**.

**Acceptance Criteria:**
- Users can enter a title, body text, and optionally upload multimedia (images/videos).
- The post must have a category (e.g., News, Events, Alerts) selected from a dropdown.
- Users can preview the post before publishing.
- After submitting, the post is displayed on the homepage and is visible to all users.
- A success message is shown after the post is published.

**Estimated Time:** 6 hours

**Priority:** High

---

# Tasks for Create Post
- [x] **Create Post Creation View**
  - **Considerations**: Consider using a WYSIWYG editor for rich content.
  - **Cross-Functional**: Work with frontend and design teams for UI consistency.
  - **Estimate**: 3 hours
  - **Definition of Done**: The view is functional and visually appealing. [TODO tinymce integration]

- [x] **Implement Client-Side Validation**
  - **Considerations**: Validate title and content length.
  - **Estimate**: 2 hours
  - **Definition of Done**: Errors display correctly without submission.

- [x] **Set Up Server-Side Validation**
  - **Considerations**: Align validations with business rules.
  - **Cross-Functional**: Collaborate with backend developers on validation logic.
  - **Estimate**: 2 hours
  - **Definition of Done**: Validation functions correctly on post submission.

- [ ] **Add Multimedia Upload Functionality**
  - **Considerations**: Handle file types and sizes appropriately.
  - **Cross-Functional**: Coordinate with backend for file handling.
  - **Estimate**: 4 hours
  - **Definition of Done**: Users can upload multimedia without issues.

- [x] **Implement Logic to Save Posts**
  - **Considerations**: Ensure proper database handling.
  - **Cross-Functional**: Work with backend team on database interactions.
  - **Estimate**: 3 hours
  - **Definition of Done**: Posts are saved and retrievable from the database.

- [x] **Redirect After Post Creation**
  - **Considerations**: Ensure seamless navigation.
  - **Estimate**: 1 hour
  - **Definition of Done**: Users are redirected to the correct post page.
