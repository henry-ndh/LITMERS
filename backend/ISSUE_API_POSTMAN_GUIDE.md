# 📋 Issue API - Postman Guide

Hướng dẫn sử dụng Postman để test Issue & Kanban API.

## 🚀 Cài đặt

### 1. Import Collection
1. Mở Postman
2. Click **Import** (góc trên bên trái)
3. Chọn file `Issue_API_Postman_Collection.json`
4. Click **Import**

### 2. Import Environment
1. Click **Environments** (sidebar trái)
2. Click **Import**
3. Chọn file `Issue_API_Postman_Environment.json`
4. Click **Import**
5. Chọn environment **"Issue API - Development"** ở dropdown góc trên bên phải

## 🔐 Authentication

### Bước 1: Login để lấy token
1. Mở request **Auth > Login**
2. Body mặc định:
```json
{
  "email": "admin@gmail.com",
  "password": "123456"
}
```
3. Click **Send**
4. Token sẽ tự động được lưu vào biến `access_token` (xem trong Console)

### Bước 2: Kiểm tra token
- Token được tự động set trong collection variables
- Tất cả requests khác sẽ tự động sử dụng token này (Bearer Token)

## 📝 Các Endpoints

### 🏗️ Issue Status Management (Kanban Columns)

#### 1️⃣ **Tạo Issue Status (Column)**
```
POST /api/issue/status?projectId={projectId}
Body:
{
  "name": "To Do",
  "color": "#3B82F6",
  "position": 0,
  "isDefault": false,
  "wipLimit": null
}
```
✅ User phải có quyền truy cập project

#### 2️⃣ **Cập nhật Issue Status**
```
PUT /api/issue/status/{statusId}?projectId={projectId}
Body:
{
  "name": "In Progress",
  "color": "#F59E0B",
  "position": 1,
  "isDefault": false,
  "wipLimit": 5
}
```

#### 3️⃣ **Xóa Issue Status**
```
DELETE /api/issue/status/{statusId}?projectId={projectId}
```
⚠️ Không thể xóa nếu status có issues

#### 4️⃣ **Lấy Issue Status theo ID**
```
GET /api/issue/status/{statusId}
```

#### 5️⃣ **Lấy tất cả Statuses của Project**
```
GET /api/issue/status/project/{projectId}
```
📊 Trả về danh sách statuses đã sắp xếp theo position

#### 6️⃣ **Sắp xếp lại Statuses**
```
PUT /api/issue/status/reorder?projectId={projectId}
Body: [1, 2, 3, 4]
```
🔄 Cung cấp mảng ID statuses theo thứ tự mong muốn

### 📋 Issue Management

#### 1️⃣ **Tạo Issue mới**
```
POST /api/issue
Body:
{
  "projectId": 1,
  "statusId": 1,
  "title": "Fix login bug",
  "description": "User cannot login with Google account",
  "assigneeId": null,
  "dueDate": "2024-12-31",
  "priority": 0,
  "position": 0,
  "labelIds": [1, 2]
}
```
📌 Priority: 0=HIGH, 1=MEDIUM, 2=LOW

#### 2️⃣ **Cập nhật Issue**
```
PUT /api/issue/{issueId}
Body:
{
  "statusId": 2,
  "title": "Updated title",
  "assigneeId": 3,
  "priority": 0,
  "labelIds": [1, 3]
}
```
📝 Tất cả các thay đổi được ghi lại trong history

#### 3️⃣ **Xóa Issue**
```
DELETE /api/issue/{issueId}
```
⚠️ Chỉ issue owner mới có thể xóa

#### 4️⃣ **Lấy Issue theo ID**
```
GET /api/issue/{issueId}
```

#### 5️⃣ **Lấy chi tiết Issue**
```
GET /api/issue/{issueId}/detail
```
📊 Bao gồm subtasks và labels

#### 6️⃣ **Lấy Issues theo Project**
```
GET /api/issue/project/{projectId}
```
📋 Sắp xếp theo status position và issue position

#### 7️⃣ **Lấy Issues theo Status (Column)**
```
GET /api/issue/status/{statusId}/issues
```
📋 Sắp xếp theo position trong column

#### 8️⃣ **Di chuyển Issue (Drag & Drop)**
```
PUT /api/issue/{issueId}/move
Body:
{
  "statusId": 2,
  "position": 0
}
```
🔄 Dùng cho drag-and-drop trong Kanban board

### 🏷️ Project Label Management

#### 1️⃣ **Tạo Label**
```
POST /api/issue/label?projectId={projectId}
Body:
{
  "name": "Bug",
  "color": "#EF4444"
}
```

#### 2️⃣ **Cập nhật Label**
```
PUT /api/issue/label/{labelId}?projectId={projectId}
Body:
{
  "name": "Critical Bug",
  "color": "#DC2626"
}
```

#### 3️⃣ **Xóa Label**
```
DELETE /api/issue/label/{labelId}?projectId={projectId}
```
⚠️ Sẽ xóa label khỏi tất cả issues

#### 4️⃣ **Lấy Label theo ID**
```
GET /api/issue/label/{labelId}
```

#### 5️⃣ **Lấy tất cả Labels của Project**
```
GET /api/issue/label/project/{projectId}
```

### 🏷️ Issue Label Management

#### 1️⃣ **Thêm Label vào Issue**
```
POST /api/issue/{issueId}/label/{labelId}
```

#### 2️⃣ **Xóa Label khỏi Issue**
```
DELETE /api/issue/{issueId}/label/{labelId}
```

#### 3️⃣ **Cập nhật tất cả Labels của Issue**
```
PUT /api/issue/{issueId}/labels
Body: [1, 2, 3]
```
🔄 Cung cấp mảng label IDs

### ✅ Subtask Management

#### 1️⃣ **Tạo Subtask**
```
POST /api/issue/{issueId}/subtask
Body:
{
  "title": "Check user authentication",
  "position": 0
}
```

#### 2️⃣ **Cập nhật Subtask**
```
PUT /api/issue/subtask/{subtaskId}?issueId={issueId}
Body:
{
  "title": "Updated title",
  "isDone": true,
  "position": 1
}
```

#### 3️⃣ **Xóa Subtask**
```
DELETE /api/issue/subtask/{subtaskId}?issueId={issueId}
```

#### 4️⃣ **Lấy Subtask theo ID**
```
GET /api/issue/subtask/{subtaskId}
```

#### 5️⃣ **Lấy tất cả Subtasks của Issue**
```
GET /api/issue/{issueId}/subtasks
```

#### 6️⃣ **Sắp xếp lại Subtasks**
```
PUT /api/issue/{issueId}/subtasks/reorder
Body: [1, 2, 3, 4]
```

### 📜 Issue History

#### 1️⃣ **Lấy Lịch sử Issue**
```
GET /api/issue/{issueId}/history?limit=50
```
📊 Hiển thị tất cả thay đổi: status, assignee, priority, title, due_date

## 🔧 Variables

Collection có các biến sau (có thể cập nhật trong Collection Variables):

| Variable | Mô tả | Mặc định |
|----------|-------|----------|
| `baseUrl` | Base URL của API | `http://localhost:5000` |
| `access_token` | JWT token (tự động lưu sau login) | - |
| `projectId` | ID của project | `1` |
| `statusId` | ID của issue status | `1` |
| `issueId` | ID của issue | `1` |
| `labelId` | ID của label | `1` |
| `subtaskId` | ID của subtask | `1` |

## 📋 Workflow Test

### Test Flow Cơ bản cho Kanban Board:

1. **Login** → Lấy token
2. **Create Issue Status** → Tạo các columns (To Do, In Progress, Done)
3. **Get Issue Statuses** → Xem danh sách columns
4. **Create Issue** → Tạo issue mới trong column đầu tiên
5. **Get Issues By Project** → Xem tất cả issues
6. **Move Issue** → Di chuyển issue sang column khác (drag & drop)
7. **Create Label** → Tạo label cho project
8. **Add Label To Issue** → Gán label cho issue
9. **Create Subtask** → Tạo subtask cho issue
10. **Update Subtask** → Đánh dấu subtask hoàn thành
11. **Get Issue History** → Xem lịch sử thay đổi

## ⚠️ Lưu ý

1. **Permission**: 
   - User phải có quyền truy cập project mới có thể thao tác với issues
   - Chỉ issue owner mới có thể xóa issue

2. **Status Deletion**: 
   - Không thể xóa status nếu status có issues
   - Cần di chuyển hoặc xóa issues trước

3. **History Tracking**: 
   - Tự động ghi lại mọi thay đổi: status, assignee, priority, title, due_date
   - History không thể chỉnh sửa hoặc xóa

4. **WIP Limit**: 
   - Có thể set WIP limit cho status
   - null = unlimited
   - Frontend nên validate WIP limit khi move issue

5. **Priority Values**: 
   - 0 = HIGH
   - 1 = MEDIUM (default)
   - 2 = LOW

## 🐛 Troubleshooting

### Lỗi 401 Unauthorized
- Token đã hết hạn → Login lại
- Token không đúng → Kiểm tra lại login

### Lỗi 403 Forbidden
- User không có quyền truy cập project
- Không có quyền thực hiện action

### Lỗi 404 Not Found
- Issue/Status/Label không tồn tại
- Kiểm tra lại IDs

### Lỗi 400 Bad Request
- Thiếu required fields
- Dữ liệu không hợp lệ (ví dụ: name quá dài, color không đúng format)
- Status có issues nên không thể xóa

## 📚 Response Format

Tất cả responses đều theo format:
```json
{
  "success": true,
  "data": { ... },
  "message": "Lưu thành công.",
  "statusCode": 200
}
```

## 🎯 Use Cases

### Kanban Board Setup:
1. Tạo project
2. Tạo các statuses (columns): To Do, In Progress, Review, Done
3. Set WIP limit cho các columns nếu cần
4. Tạo issues và di chuyển giữa các columns

### Issue Management:
1. Tạo issue với title, description, assignee
2. Gán labels cho issue
3. Tạo subtasks để break down công việc
4. Theo dõi progress qua subtasks completion
5. Xem history để biết ai đã thay đổi gì

## 🔗 Liên kết

- Project API Collection: `Project_API_Postman_Collection.json`
- Project API Guide: `PROJECT_API_POSTMAN_GUIDE.md`
- Team API Collection: `Team_API_Postman_Collection.json`

