# 🔧 Tối ưu hóa Performance API

## ⚠️ Các vấn đề đã phát hiện:

### 1. **Thiếu AsNoTracking() - VẤN ĐỀ NGHIÊM TRỌNG**
- Tất cả query đều tracking entities → tốn memory và chậm
- **Giải pháp**: Thêm `.AsNoTracking()` cho tất cả read-only queries

### 2. **N+1 Query Problem**
- `GetProjectsByUserId()` gọi `GetTeamsByUserId()` trước → 2 queries thay vì 1
- **Giải pháp**: Join trực tiếp trong 1 query

### 3. **Email Service Blocking**
- Email service đang `await` trong request → chậm nếu SMTP server chậm
- **Giải pháp**: Dùng background task hoặc fire-and-forget

### 4. **Multiple FindAsync() thay vì Batch Query**
- Nhiều `FindAsync()` riêng lẻ thay vì 1 query batch
- **Giải pháp**: Dùng `Where().ToListAsync()` cho batch

### 5. **Include quá nhiều Navigation Properties**
- Include tất cả navigation properties khi chỉ cần một vài field
- **Giải pháp**: Dùng `Select()` projection thay vì Include

---

## 🚀 Các file cần sửa:

