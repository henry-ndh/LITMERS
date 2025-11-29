# 📋 Báo cáo đối chiếu PRD với Backend Implementation

## ✅ ĐÃ HOÀN THÀNH

### 3.1 Authentication ✅
- ✅ **FR-001**: Sign Up - `/api/auth/register`
- ✅ **FR-002**: Login/Logout - `/api/auth/login` (token 24h)
- ✅ **FR-003**: Password Recovery/Reset - `/api/auth/forgot-password`, `/api/auth/reset-password` (token 1h)
- ✅ **FR-004**: Google OAuth Login - `/api/auth/login-google`
- ✅ **FR-005**: Profile Management - `/api/auth/get-profile-by-user`, `/api/auth/update-profile`
- ✅ **FR-006**: Password Change - `/api/auth/change-password` (disabled cho Google OAuth users)
- ✅ **FR-007**: Account Deletion - `/api/auth/delete-account` (soft delete, check owned teams)

### 3.2 Team Features ✅
- ✅ **FR-010**: Create Team - `/api/team` (POST)
- ✅ **FR-011**: Update Team - `/api/team/{teamId}` (PUT)
- ✅ **FR-012**: Delete Team - `/api/team/{teamId}` (DELETE, soft delete)
- ✅ **FR-013**: Invite Member - `/api/team/{teamId}/invite` (POST, email sending, 7 days expiration)
- ✅ **FR-014**: View Members - `/api/team/{teamId}/members` (GET)
- ✅ **FR-015**: Kick Member - `/api/team/{teamId}/members/{memberId}` (DELETE)
- ✅ **FR-016**: Leave Team - `/api/team/{teamId}/leave` (POST)
- ✅ **FR-017**: Team Role System - OWNER/ADMIN/MEMBER
- ✅ **FR-018**: Change Role - `/api/team/{teamId}/members/{memberId}/role` (PUT, OWNER only)
- ✅ **FR-019**: Team Activity Log - `/api/team/{teamId}/activity-logs` (GET, pagination)

### 3.3 Project ✅
- ✅ **FR-020**: Create Project - `/api/project` (POST, max 15 projects/team)
- ✅ **FR-021**: View Projects - `/api/project/my-projects` (GET, favorite first, then by date)
- ✅ **FR-022**: Project Detail Page - `/api/project/{projectId}/detail` (GET)
- ✅ **FR-023**: Update Project - `/api/project/{projectId}` (PUT)
- ✅ **FR-024**: Delete Project - `/api/project/{projectId}` (DELETE, soft delete)
- ✅ **FR-025**: Project Description - Max 2000 characters
- ✅ **FR-026**: Archive Project - `/api/project/{projectId}/archive` (PUT)
- ✅ **FR-027**: Favorite Project - `/api/project/{projectId}/favorite` (POST/DELETE)

### 3.4 Issue (Phần cơ bản) ✅
- ✅ **FR-030**: Create Issue - `/api/issue` (POST, max 200 issues/project)
- ✅ **FR-031**: Issue Detail View - `/api/issue/{issueId}/detail` (GET)
- ✅ **FR-032**: Update Issue - `/api/issue/{issueId}` (PUT)
- ✅ **FR-033**: Update Status - `/api/issue/{issueId}/move` (PUT, direct movement allowed)
- ✅ **FR-034**: Assign User - Assignee phải là team member
- ✅ **FR-035**: Delete Issue - `/api/issue/{issueId}` (DELETE, soft delete)
- ✅ **FR-037**: Issue Priority - HIGH/MEDIUM/LOW
- ✅ **FR-038**: Issue Labels/Tags - Max 20 labels/project, max 5 labels/issue
- ✅ **FR-039**: Issue Change History - `/api/issue/{issueId}/history` (GET)
- ✅ **FR-039-2**: Subtasks - Max 20 subtasks/issue, reorder support

### 3.6 Kanban Board ✅
- ✅ **FR-050**: Kanban Board Display - Issues grouped by status
- ✅ **FR-051**: Drag & Drop Movement - `/api/issue/{issueId}/move`
- ✅ **FR-052**: Reorder Within Same Column - Position field
- ✅ **FR-053**: Custom Columns - `/api/issue/status` (POST/PUT/DELETE, max 5 custom + 3 default = 8 total)
- ✅ **FR-054**: WIP Limit - Validation đã thêm vào MoveIssue (cho phép move nhưng warning)

### 3.7 Comments ✅
- ✅ **FR-060**: Create Comment - `/api/comment` (POST, 1-1000 characters)
- ✅ **FR-061**: Comment List - `/api/comment/issue/{issueId}` (GET, chronological order, pagination)
- ✅ **FR-062**: Update Comment - `/api/comment/{commentId}` (PUT, author only)
- ✅ **FR-063**: Delete Comment - `/api/comment/{commentId}` (DELETE, author/owner/admin)

### 3.9 Notifications (Phần cơ bản) ✅
- ✅ **FR-090**: In-App Notification - Có enum ISSUE_DUE_SOON, ISSUE_DUE_TODAY
  - ✅ Issue assign → notify assignee
  - ✅ Comment mới → notify issue owner + assignee
  - ✅ Team invite → notify user
  - ✅ Member role change → notify member
  - ⚠️ **THIẾU**: Due date approaching (1 day before) - chưa có logic tự động
  - ⚠️ **THIẾU**: Due date today - chưa có logic tự động
- ✅ **FR-091**: Mark as Read - `/api/notification/{notificationId}/read`, `/api/notification/mark-all-read`

### 3.10 Permissions/Security ✅
- ✅ **FR-070**: Team Membership Verification - Tất cả endpoints đều check
- ✅ **FR-071**: Soft Delete Implementation - User, Team, Project, Issue, Comment

---

## ⚠️ CÒN THIẾU

### 1. FR-036: Issue Search/Filtering ❌
**Yêu cầu:**
- Search: Title text search
- Filters: By status, assignee, priority, label, has due date, due date range
- Sorting: Creation date, due date, priority, last modified date

**Hiện tại:** Chưa có endpoint search/filter, chỉ có `GetIssuesByProjectId()` và `GetIssuesByStatusId()`

**Cần thêm:**
- Endpoint: `GET /api/issue/search?projectId={id}&title={keyword}&statusId={id}&assigneeId={id}&priority={priority}&labelId={id}&hasDueDate={bool}&dueDateFrom={date}&dueDateTo={date}&sortBy={field}&sortOrder={asc/desc}`

---

### 2. FR-080: Project Dashboard ❌
**Yêu cầu:**
- Issue count by status (pie/bar chart)
- Completion rate (Done / Total)
- Issue count by priority
- Recently created issues (max 5)
- Issues due soon (within 7 days, max 5)

**Hiện tại:** Chưa có endpoint

**Cần thêm:**
- Endpoint: `GET /api/project/{projectId}/dashboard`

---

### 3. FR-081: Personal Dashboard ❌
**Yêu cầu:**
- My assigned issues list (categorized by status)
- Total count of my assigned issues
- Issues due soon (within 7 days)
- Issues due today
- My recent comments (max 5)
- My teams/projects list

**Hiện tại:** Chưa có endpoint

**Cần thêm:**
- Endpoint: `GET /api/dashboard/personal`

---

### 4. FR-082: Team Statistics ❌
**Yêu cầu:**
- Issue creation trend by period (line graph)
- Issue completion trend by period (line graph)
- Assigned issues per member
- Completed issues per member
- Issue status per project
- Period selection: Last 7/30/90 days

**Hiện tại:** Chưa có endpoint

**Cần thêm:**
- Endpoint: `GET /api/team/{teamId}/statistics?period={7|30|90}`

---

### 5. FR-090: Due Date Notifications (Tự động) ⚠️
**Yêu cầu:**
- Due date approaching (1 day before) → notify assignee
- Due date today → notify assignee

**Hiện tại:** 
- Có enum `ISSUE_DUE_SOON` và `ISSUE_DUE_TODAY`
- Chưa có logic tự động tạo notification (cần background job/cron)

**Cần thêm:**
- Background service/job để check và tạo notifications hàng ngày
- Hoặc check khi load issues và tạo notification on-the-fly

---

## 📊 TỔNG KẾT

### Đã hoàn thành: ~85%
- ✅ Authentication: 100%
- ✅ Team Features: 100%
- ✅ Project: 100%
- ✅ Issue (cơ bản): 100%
- ✅ Kanban Board: 100%
- ✅ Comments: 100%
- ✅ Notifications (cơ bản): 80% (thiếu due date auto notifications)
- ✅ Permissions/Security: 100%

### Còn thiếu: ~15%
- ❌ **FR-036**: Issue Search/Filtering
- ❌ **FR-080**: Project Dashboard
- ❌ **FR-081**: Personal Dashboard
- ❌ **FR-082**: Team Statistics
- ⚠️ **FR-090**: Due date auto notifications (cần background job)

---

## 🎯 ĐỘ ƯU TIÊN

### High Priority (Must-have):
1. **FR-036: Issue Search/Filtering** - Rất quan trọng cho UX
2. **FR-090: Due date notifications** - Cần background job

### Medium Priority (Should-have):
3. **FR-080: Project Dashboard** - Hữu ích cho project management
4. **FR-081: Personal Dashboard** - Hữu ích cho user experience

### Low Priority (Nice-to-have):
5. **FR-082: Team Statistics** - Analytics, có thể làm sau

---

## 💡 GỢI Ý

1. **Issue Search/Filtering**: Có thể mở rộng endpoint `GetIssuesByProjectId()` với query parameters
2. **Due Date Notifications**: Có thể tạo background service hoặc check khi user login/load dashboard
3. **Dashboard**: Có thể tạo một DashboardController riêng hoặc thêm vào ProjectController/TeamController

