# 📮 Hướng dẫn sử dụng Postman Collection - Team API

## 📥 Import vào Postman

### Cách 1: Import Collection & Environment
1. Mở Postman
2. Click **Import** (góc trên bên trái)
3. Kéo thả hoặc chọn 2 files:
   - `Team_API_Postman_Collection.json`
   - `Team_API_Postman_Environment.json`
4. Click **Import**

### Cách 2: Import từ URL (nếu files được host)
1. Mở Postman
2. Click **Import** > **Link**
3. Paste URL của file JSON
4. Click **Continue** > **Import**

## 🔧 Cấu hình

### 1. Chọn Environment
Sau khi import, chọn environment **"Team API - Development"** ở góc phải trên.

### 2. Cập nhật Base URL
Nếu API chạy ở port khác, cập nhật biến `baseUrl`:
- Click vào icon **Environment** (mắt) ở góc phải trên
- Edit biến `baseUrl`
- Ví dụ: `https://api.yourdomain.com`

### 3. Lấy Access Token

#### Option A: Tự động (Khuyên dùng)
1. Mở folder **Auth** trong collection
2. Chạy request **Login**
3. Token sẽ **tự động lưu** vào biến `access_token`
4. Tất cả requests sẽ tự động dùng token này

#### Option B: Thủ công
1. Login qua endpoint khác hoặc ứng dụng
2. Copy JWT token
3. Vào **Environment** > Edit biến `access_token`
4. Paste token vào

## 🚀 Sử dụng

### Workflow cơ bản

#### 1️⃣ **Đăng nhập**
```
POST /api/auth/login
Body:
{
  "email": "admin@gmail.com",
  "password": "123456"
}
```
✅ Token tự động lưu vào collection variables

#### 2️⃣ **Tạo team mới**
```
POST /api/team
Body:
{
  "name": "My Awesome Team"
}
```
✅ Response trả về teamId, save lại để dùng

#### 3️⃣ **Cập nhật biến teamId**
- Copy `id` từ response của Create Team
- Vào Environment > Edit `teamId`
- Hoặc thay trực tiếp trong URL

#### 4️⃣ **Mời thành viên**
```
POST /api/team/{{teamId}}/invite
Body:
{
  "email": "member@example.com"
}
```
✅ Email sẽ được gửi với invite token

#### 5️⃣ **Chấp nhận lời mời**
```
POST /api/team/accept-invite
Body:
{
  "token": "token-from-email"
}
```

#### 6️⃣ **Xem danh sách teams**
```
GET /api/team/my-teams
```

#### 7️⃣ **Xem chi tiết team**
```
GET /api/team/{{teamId}}/detail
```

## 📋 Danh sách Endpoints

### 🏢 Team Management
| Method | Endpoint | Mô tả | Permission |
|--------|----------|-------|------------|
| POST | `/api/team` | Tạo team mới | Authenticated |
| PUT | `/api/team/{teamId}` | Cập nhật team | OWNER/ADMIN |
| DELETE | `/api/team/{teamId}` | Xóa team (soft) | OWNER |
| GET | `/api/team/{teamId}` | Lấy thông tin team | Member |
| GET | `/api/team/my-teams` | Danh sách teams của tôi | Authenticated |
| GET | `/api/team/{teamId}/detail` | Chi tiết team + members | Member |

### 👥 Team Members
| Method | Endpoint | Mô tả | Permission |
|--------|----------|-------|------------|
| GET | `/api/team/{teamId}/members` | Danh sách members | Member |
| DELETE | `/api/team/{teamId}/members/{memberId}` | Xóa member | ADMIN/OWNER |
| PUT | `/api/team/{teamId}/members/{memberId}/role` | Thay đổi role | ADMIN/OWNER |
| POST | `/api/team/{teamId}/leave` | Rời team | Member |

### 📧 Team Invites
| Method | Endpoint | Mô tả | Permission |
|--------|----------|-------|------------|
| POST | `/api/team/{teamId}/invite` | Mời member mới | ADMIN/OWNER |
| POST | `/api/team/accept-invite` | Chấp nhận lời mời | Authenticated |
| DELETE | `/api/team/{teamId}/invites/{inviteId}` | Hủy lời mời | ADMIN/OWNER |
| GET | `/api/team/my-invites` | Lời mời của tôi | Authenticated |

### 📊 Activity Logs
| Method | Endpoint | Mô tả | Permission |
|--------|----------|-------|------------|
| GET | `/api/team/{teamId}/activity-logs?limit=50` | Lịch sử hoạt động | Member |

## 🎭 Team Roles

### Permission Levels (từ cao đến thấp)
```
OWNER (0) > ADMIN (1) > MEMBER (2)
```

### Role Values trong API
```json
{
  "OWNER": 0,
  "ADMIN": 1,
  "MEMBER": 2
}
```

### Quyền hạn
| Action | OWNER | ADMIN | MEMBER |
|--------|-------|-------|--------|
| Tạo team | ✅ | ✅ | ✅ |
| Cập nhật team | ✅ | ✅ | ❌ |
| Xóa team | ✅ | ❌ | ❌ |
| Mời member | ✅ | ✅ | ❌ |
| Xóa MEMBER | ✅ | ✅ | ❌ |
| Xóa ADMIN | ✅ | ❌ | ❌ |
| Thay đổi role | ✅ | ✅* | ❌ |
| Xem members | ✅ | ✅ | ✅ |
| Rời team | ✅** | ✅ | ✅ |

*ADMIN không thể thay đổi role của ADMIN khác hoặc OWNER
**OWNER không thể rời team (phải xóa team)

## 🔍 Response Examples

### Success Response
```json
{
  "success": true,
  "message": "Get data successfully",
  "data": {
    "id": 1,
    "name": "My Team",
    "ownerId": 1,
    "ownerName": "Admin",
    "memberCount": 5
  },
  "statusCode": 200
}
```

### Error Response
```json
{
  "success": false,
  "message": "You don't have permission to update this team",
  "data": null,
  "statusCode": 400
}
```

## 🧪 Test Scenarios

### Scenario 1: Tạo và quản lý team
1. Login as Admin
2. Create Team → Save teamId
3. Get Team Detail
4. Invite Member
5. Update Team Name
6. View Activity Logs

### Scenario 2: Member workflow
1. Login as User
2. Get My Invites
3. Accept Invite
4. Get My Teams
5. Get Team Members
6. Leave Team

### Scenario 3: Permission testing
1. Login as MEMBER
2. Try Update Team → Should fail
3. Try Invite Member → Should fail
4. Try Leave Team → Should success

## 📝 Variables

### Collection Variables
- `baseUrl`: API base URL
- `access_token`: JWT token (auto-saved after login)
- `teamId`: Current team ID
- `memberId`: Current member ID
- `inviteId`: Current invite ID

### Environment Variables
Giống collection variables nhưng có thể switch giữa Dev/Staging/Production

## 🐛 Troubleshooting

### ❌ 401 Unauthorized
**Nguyên nhân**: Token không hợp lệ hoặc hết hạn
**Giải pháp**: Chạy lại Login request

### ❌ 400 Bad Request
**Nguyên nhân**: Thiếu field hoặc validation failed
**Giải pháp**: Kiểm tra request body theo mô tả

### ❌ 404 Not Found
**Nguyên nhân**: teamId/memberId không tồn tại
**Giải pháp**: Kiểm tra lại ID trong Environment variables

### ❌ 403 Forbidden
**Nguyên nhân**: Không có quyền thực hiện action
**Giải pháp**: Kiểm tra role của user

## 💡 Tips

### 1. Sử dụng Variables hiệu quả
Thay vì hard-code IDs, dùng `{{teamId}}` để dễ dàng thay đổi

### 2. Pre-request Scripts
Có thể thêm script để tự động generate data:
```javascript
pm.variables.set("timestamp", Date.now());
```

### 3. Tests Scripts
Tự động validate response:
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has data", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.data).to.not.be.null;
});
```

### 4. Save Response Data
Auto-save IDs từ response:
```javascript
var jsonData = pm.response.json();
if (jsonData.data && jsonData.data.id) {
    pm.collectionVariables.set("teamId", jsonData.data.id);
}
```

## 📚 Resources

- [Postman Documentation](https://learning.postman.com/docs)
- [JWT.io](https://jwt.io) - Decode JWT tokens
- [HTTP Status Codes](https://httpstatuses.com)

---

**Chúc bạn test API thành công! 🚀**
