# 🔄 Hướng dẫn Migration - Fix Nullable Columns

## ⚠️ Vấn đề đã fix

Đã thêm nullable markers (`?`) cho các columns có thể null trong database:

### Models đã sửa:
1. **TeamActivityLogModel** - `TargetType`, `Message`, `Metadata`
2. **ProjectModel** - `Description`
3. **IssueModel** - `Description`, `AiSummary`, `AiSuggestion`, `AiCommentSummary`
4. **IssueStatusModel** - `Color`
5. **IssueHistoryModel** - `OldValue`, `NewValue`
6. **NotificationModel** - `Message`, `Payload`
7. **UserAuthProviderModel** - `Email`

## 🚀 Các bước thực hiện

### Option 1: Tạo migration mới (Khuyên dùng nếu chưa có data)

```bash
# Di chuyển đến thư mục Main.API
cd Main.API

# Xóa migration cũ nếu đã tạo trước đó
dotnet ef migrations remove

# Tạo migration mới
dotnet ef migrations add InitialCreate

# Apply migration xuống database
dotnet ef database update
```

### Option 2: Đã có migration và data trong DB

Nếu bạn đã có migration và data trong database:

#### Cách 1: Drop database và tạo lại (Mất data)
```bash
cd Main.API

# Drop database
dotnet ef database drop

# Tạo migration mới
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

#### Cách 2: Tạo migration mới để alter columns (Giữ data)
```bash
cd Main.API

# Tạo migration mới cho việc sửa đổi
dotnet ef migrations add FixNullableColumns

# Apply migration
dotnet ef database update
```

### Option 3: Manual SQL (Nếu cần tùy chỉnh)

Nếu bạn muốn tự chạy SQL để alter columns:

```sql
-- Team Activity Logs
ALTER TABLE team_activity_logs
MODIFY COLUMN TargetType VARCHAR(50) NULL,
MODIFY COLUMN Message TEXT NULL,
MODIFY COLUMN Metadata TEXT NULL;

-- Projects
ALTER TABLE projects
MODIFY COLUMN Description VARCHAR(2000) NULL;

-- Issues
ALTER TABLE issues
MODIFY COLUMN Description TEXT NULL,
MODIFY COLUMN AiSummary TEXT NULL,
MODIFY COLUMN AiSuggestion TEXT NULL,
MODIFY COLUMN AiCommentSummary TEXT NULL;

-- Issue Statuses
ALTER TABLE issue_statuses
MODIFY COLUMN Color VARCHAR(7) NULL;

-- Issue History
ALTER TABLE issue_history
MODIFY COLUMN OldValue TEXT NULL,
MODIFY COLUMN NewValue TEXT NULL;

-- Notifications
ALTER TABLE notifications
MODIFY COLUMN Message TEXT NULL,
MODIFY COLUMN Payload TEXT NULL;

-- User Auth Providers
ALTER TABLE user_auth_providers
MODIFY COLUMN Email VARCHAR(255) NULL;
```

## 🔍 Kiểm tra Migration

Sau khi chạy migration, kiểm tra file migration được tạo:

```bash
# Xem file migration trong thư mục Migrations
ls -la ./Main.API/Migrations
```

File migration nên có các dòng tương tự:

```csharp
migrationBuilder.AlterColumn<string>(
    name: "Metadata",
    table: "team_activity_logs",
    type: "text",
    nullable: true,  // ← Quan trọng
    oldClrType: typeof(string),
    oldType: "text");
```

## ✅ Test

Sau khi migration thành công, test lại API:

```bash
# Chạy project
dotnet run --project Main.API

# Hoặc
cd Main.API
dotnet run
```

Test endpoint Create Team:
```bash
POST http://localhost:5000/api/team
{
  "name": "Test Team"
}
```

Bây giờ không còn lỗi "Column 'Metadata' cannot be null" nữa! ✅

## 🐛 Troubleshooting

### Lỗi: Migration already exists
```bash
# Xóa migration cũ
dotnet ef migrations remove
```

### Lỗi: Database does not exist
```bash
# Tạo database
dotnet ef database update
```

### Lỗi: Cannot drop database (in use)
```bash
# Đóng tất cả connections đến database
# Hoặc restart MariaDB service
# Sau đó chạy lại drop command
```

### Lỗi: Foreign key constraint
```bash
# Drop database và tạo lại từ đầu
dotnet ef database drop --force
dotnet ef database update
```

## 📝 Notes

- **Backup data** trước khi drop database
- Nếu có data quan trọng, dùng Option 2 - Cách 2
- Check migration file trước khi apply
- Test trên môi trường dev trước khi apply lên production

---

**Happy coding! 🚀**
