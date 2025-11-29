# Hướng dẫn Test Chức năng Quên Mật khẩu (Forgot Password)

## 📋 Tổng quan Flow

1. **Bước 1**: User gửi email để yêu cầu reset password
2. **Bước 2**: Hệ thống tạo token và gửi email chứa link reset
3. **Bước 3**: User click link trong email (hoặc copy token)
4. **Bước 4**: User gửi token + mật khẩu mới để reset

---

## 🔗 Endpoints

### 1. Forgot Password (Gửi email reset)
```
POST /api/auth/forgot-password
Content-Type: application/json

Body:
{
  "email": "user@example.com"
}
```

**Response Success:**
```json
{
  "success": true,
  "message": "If the email exists, a password reset link has been sent to your email.",
  "data": null,
  "statusCode": 200
}
```

**Lưu ý**: API luôn trả về success message (không tiết lộ email có tồn tại hay không) để bảo mật.

---

### 2. Reset Password (Đặt lại mật khẩu)
```
POST /api/auth/reset-password
Content-Type: application/json

Body:
{
  "token": "guid-token-from-email",
  "newPassword": "newpassword123"
}
```

**Response Success:**
```json
{
  "success": true,
  "message": "Password has been reset successfully",
  "data": null,
  "statusCode": 200
}
```

**Response Error:**
```json
{
  "success": false,
  "message": "Invalid or expired password reset token.",
  "data": null,
  "statusCode": 400
}
```

---

## 🧪 Test Cases

### Test Case 1: Gửi yêu cầu reset password thành công

**Request:**
```bash
POST /api/auth/forgot-password
{
  "email": "test@example.com"
}
```

**Expected:**
- Status: 200 OK
- Message: "If the email exists, a password reset link has been sent to your email."
- Email được gửi đến địa chỉ test@example.com
- Token được tạo trong bảng `password_reset_tokens`
- Token có `expires_at` = thời gian hiện tại + 1 giờ

---

### Test Case 2: Email không tồn tại

**Request:**
```bash
POST /api/auth/forgot-password
{
  "email": "notexist@example.com"
}
```

**Expected:**
- Status: 200 OK (vẫn trả về success để bảo mật)
- Message: "If the email exists, a password reset link has been sent to your email."
- Không có token nào được tạo

---

### Test Case 3: Email là Google OAuth account (không có password)

**Request:**
```bash
POST /api/auth/forgot-password
{
  "email": "googleuser@gmail.com"  // User đăng ký bằng Google, không có password
}
```

**Expected:**
- Status: 200 OK
- Message: "This account uses Google authentication. Please use Google sign-in."
- Không có token nào được tạo

---

### Test Case 4: Reset password với token hợp lệ

**Bước 1**: Lấy token từ database sau khi gửi forgot-password:
```sql
SELECT token, expires_at, used_at, user_id 
FROM password_reset_tokens 
WHERE user_id = (SELECT id FROM users WHERE email = 'test@example.com')
ORDER BY created_at DESC 
LIMIT 1;
```

**Bước 2**: Reset password:
```bash
POST /api/auth/reset-password
{
  "token": "token-from-database",
  "newPassword": "newpassword123"
}
```

**Expected:**
- Status: 200 OK
- Message: "Password has been reset successfully"
- Token được đánh dấu `used_at` = thời gian hiện tại
- Password của user được cập nhật

---

### Test Case 5: Reset password với token đã hết hạn

**Request:**
```bash
POST /api/auth/reset-password
{
  "token": "expired-token",
  "newPassword": "newpassword123"
}
```

**Expected:**
- Status: 400 Bad Request
- Message: "Token has expired. Please request a new password reset link."

---

### Test Case 6: Reset password với token đã được sử dụng

**Request:**
```bash
POST /api/auth/reset-password
{
  "token": "already-used-token",
  "newPassword": "newpassword123"
}
```

**Expected:**
- Status: 400 Bad Request
- Message: "This token has already been used. Please request a new password reset link."

---

### Test Case 7: Reset password với mật khẩu quá ngắn

**Request:**
```bash
POST /api/auth/reset-password
{
  "token": "valid-token",
  "newPassword": "12345"  // < 6 ký tự
}
```

**Expected:**
- Status: 400 Bad Request
- Message: "Password must be at least 6 characters long"

---

## 🔍 Cách lấy Token từ Database để Test

### Option 1: Query trực tiếp từ MySQL

```sql
-- Lấy token mới nhất của user
SELECT 
    prt.token,
    prt.expires_at,
    prt.used_at,
    prt.created_at,
    u.email,
    u.name
FROM password_reset_tokens prt
INNER JOIN users u ON prt.user_id = u.id
WHERE u.email = 'test@example.com'
  AND prt.used_at IS NULL
  AND prt.expires_at > NOW()
ORDER BY prt.created_at DESC
LIMIT 1;
```

### Option 2: Kiểm tra email đã gửi

Nếu email service được cấu hình, kiểm tra inbox của email test để lấy link:
```
Link format: {ClientURL}/reset-password?token={GUID_TOKEN}
```

---

## 📝 Postman Collection

### Request 1: Forgot Password
```json
{
  "name": "Forgot Password",
  "request": {
    "method": "POST",
    "header": [
      {
        "key": "Content-Type",
        "value": "application/json"
      }
    ],
    "body": {
      "mode": "raw",
      "raw": "{\n  \"email\": \"test@example.com\"\n}"
    },
    "url": {
      "raw": "{{base_url}}/api/auth/forgot-password",
      "host": ["{{base_url}}"],
      "path": ["api", "auth", "forgot-password"]
    }
  }
}
```

### Request 2: Reset Password
```json
{
  "name": "Reset Password",
  "request": {
    "method": "POST",
    "header": [
      {
        "key": "Content-Type",
        "value": "application/json"
      }
    ],
    "body": {
      "mode": "raw",
      "raw": "{\n  \"token\": \"{{reset_token}}\",\n  \"newPassword\": \"newpassword123\"\n}"
    },
    "url": {
      "raw": "{{base_url}}/api/auth/reset-password",
      "host": ["{{base_url}}"],
      "path": ["api", "auth", "reset-password"]
    }
  }
}
```

---

## ⚠️ Lưu ý khi Test

1. **Token Expiration**: Token chỉ có hiệu lực trong **1 giờ** (theo FR-003)
2. **Token One-time Use**: Mỗi token chỉ được sử dụng 1 lần
3. **Email Security**: API không tiết lộ email có tồn tại hay không
4. **Google OAuth Users**: Users đăng ký bằng Google không thể reset password (phải dùng Google login)
5. **Password Validation**: Mật khẩu mới phải có ít nhất 6 ký tự

---

## 🐛 Debug Tips

### Kiểm tra token trong database:
```sql
-- Xem tất cả tokens của user
SELECT * FROM password_reset_tokens WHERE user_id = 1;

-- Xem tokens chưa sử dụng và chưa hết hạn
SELECT * FROM password_reset_tokens 
WHERE used_at IS NULL 
  AND expires_at > NOW();
```

### Kiểm tra email service:
- Xem logs của email service
- Kiểm tra cấu hình SMTP trong `appsettings.json`
- Xác nhận template `email-reset-password.html` tồn tại

### Kiểm tra ClientURL:
- Đảm bảo `ClientURL` trong `appsettings.json` đúng
- Link reset sẽ là: `{ClientURL}/reset-password?token={token}`

---

## ✅ Checklist Test

- [ ] Gửi forgot-password với email hợp lệ → nhận được success message
- [ ] Kiểm tra token được tạo trong database
- [ ] Kiểm tra email được gửi (nếu email service hoạt động)
- [ ] Reset password với token hợp lệ → thành công
- [ ] Reset password với token đã hết hạn → lỗi
- [ ] Reset password với token đã dùng → lỗi
- [ ] Reset password với mật khẩu < 6 ký tự → lỗi
- [ ] Gửi forgot-password với email Google OAuth → thông báo phù hợp
- [ ] Gửi forgot-password với email không tồn tại → vẫn trả success (bảo mật)

