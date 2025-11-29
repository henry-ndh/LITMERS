# 📋 Project API - Postman Guide

Hướng dẫn sử dụng Postman để test Project & Favorites API.

## 🚀 Cài đặt

### 1. Import Collection
1. Mở Postman
2. Click **Import** (góc trên bên trái)
3. Chọn file `Project_API_Postman_Collection.json`
4. Click **Import**

### 2. Import Environment
1. Click **Environments** (sidebar trái)
2. Click **Import**
3. Chọn file `Project_API_Postman_Environment.json`
4. Click **Import**
5. Chọn environment **"Project API - Development"** ở dropdown góc trên bên phải

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

### 🏗️ Project Management

#### 1️⃣ **Tạo Project mới**
```
POST /api/project
Body:
{
  "teamId": 1,
  "name": "My New Project",
  "description": "Project description here"
}
```
✅ User phải là member của team

#### 2️⃣ **Cập nhật Project**
```
PUT /api/project/{projectId}
Body:
{
  "name": "Updated Project Name",
  "description": "Updated description"
}
```

#### 3️⃣ **Xóa Project (soft delete)**
```
DELETE /api/project/{projectId}
```
⚠️ Chỉ project owner mới có thể xóa

#### 4️⃣ **Lấy Project theo ID**
```
GET /api/project/{projectId}
```

#### 5️⃣ **Lấy chi tiết Project**
```
GET /api/project/{projectId}/detail
```
📊 Bao gồm thông tin issue count

#### 6️⃣ **Lấy Projects theo Team**
```
GET /api/project/team/{teamId}
```

#### 7️⃣ **Lấy tất cả Projects của tôi**
```
GET /api/project/my-projects
```
📋 Lấy tất cả projects từ các teams mà user là member

#### 8️⃣ **Archive Project**
```
PUT /api/project/{projectId}/archive
Body:
{
  "isArchived": true
}
```

#### 9️⃣ **Unarchive Project**
```
PUT /api/project/{projectId}/archive
Body:
{
  "isArchived": false
}
```

### ⭐ Favorite Projects

#### 1️⃣ **Thêm vào Favorites**
```
POST /api/project/{projectId}/favorite
```

#### 2️⃣ **Xóa khỏi Favorites**
```
DELETE /api/project/{projectId}/favorite
```

#### 3️⃣ **Lấy danh sách Favorites**
```
GET /api/project/favorites
```

## 🔧 Variables

Collection có các biến sau (có thể cập nhật trong Collection Variables):

| Variable | Mô tả | Mặc định |
|----------|-------|----------|
| `baseUrl` | Base URL của API | `http://localhost:5000` |
| `access_token` | JWT token (tự động lưu sau login) | - |
| `teamId` | ID của team | `1` |
| `projectId` | ID của project | `1` |

## 📋 Workflow Test

### Test Flow Cơ bản:

1. **Login** → Lấy token
2. **Create Project** → Tạo project mới (cần có teamId)
3. **Get My Projects** → Xem danh sách projects
4. **Get Project Detail** → Xem chi tiết project
5. **Add Favorite** → Thêm vào favorites
6. **Get Favorites** → Xem danh sách favorites
7. **Update Project** → Cập nhật thông tin
8. **Archive Project** → Archive project
9. **Unarchive Project** → Unarchive project
10. **Remove Favorite** → Xóa khỏi favorites
11. **Delete Project** → Xóa project (chỉ owner)

## ⚠️ Lưu ý

1. **Permission**: 
   - User phải là member của team mới có thể truy cập projects
   - Chỉ project owner mới có thể xóa project

2. **Team ID**: 
   - Cần có teamId hợp lệ để tạo project
   - Có thể lấy từ Team API hoặc database

3. **Token Expiry**: 
   - Token có thể hết hạn, cần login lại

4. **Variables**: 
   - Cập nhật `projectId` và `teamId` sau mỗi lần tạo mới để test các endpoints khác

## 🐛 Troubleshooting

### Lỗi 401 Unauthorized
- Token đã hết hạn → Login lại
- Token không đúng → Kiểm tra lại login

### Lỗi 403 Forbidden
- User không phải member của team
- Không có quyền thực hiện action (ví dụ: xóa project)

### Lỗi 404 Not Found
- Project/Team không tồn tại
- Kiểm tra lại `projectId` hoặc `teamId`

### Lỗi 400 Bad Request
- Thiếu required fields
- Dữ liệu không hợp lệ (ví dụ: name quá dài)

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

## 🔗 Liên kết

- Team API Collection: `Team_API_Postman_Collection.json`
- Team API Guide: `POSTMAN_GUIDE.md`

