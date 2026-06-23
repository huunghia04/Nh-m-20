# Báo cáo rà soát Frontend (UI/UX) - Dự án HRManagement

Quá trình rà soát read-only (không chỉnh sửa mã nguồn) toàn bộ các file `.cshtml` trong thư mục `Views` và các file CSS/JS tự viết trong `wwwroot` đã được hoàn tất. Không phát hiện rủi ro về mã JavaScript hardcode gây lỗi client-side. Tuy nhiên, đã phát hiện 5 vấn đề về CSS và thiết kế Responsive (Bootstrap 5) cần được khắc phục để cải thiện giao diện và trải nghiệm người dùng (UX).

Dưới đây là danh sách chi tiết các lỗi và đề xuất sửa chữa (Before/After).

---

## 1. Lỗi cấu trúc lưới (Grid) Bootstrap vượt quá 12 cột gây vỡ form
- **File bị lỗi:** `Views/Employee/Index.cshtml`
- **Mô tả lỗi:** Trong Modal "Thêm nhân viên" (khoảng dòng 191) và "Sửa nhân viên" (khoảng dòng 277), các thẻ `div` cột bên trong một thẻ `.row` đang được chia theo tỷ lệ `col-md-6`, `col-md-4`, `col-md-4`, `col-md-4`. Tổng số cột là 18 (lớn hơn giới hạn 12 của Bootstrap). Điều này khiến các dòng thông tin bị rớt cột, tạo khoảng trống và hiển thị không thẳng hàng.
- **Code đề xuất sửa chữa (Before/After):** Tách cột "Ngày vào làm" ra chiếm trọn 1 hàng (`col-md-12`) để 3 cột còn lại (`col-md-4` * 3 = 12) nằm vừa vặn trên hàng tiếp theo.

**Before:**
```html
<div class="col-md-6">
    <label class="form-label">Ngày vào làm</label>...
</div>
<div class="col-md-4">
    <label class="form-label">KC đến CT (km)</label>...
</div>
<div class="col-md-4">
    <label class="form-label">Độ hài lòng (1-4)</label>...
</div>
<div class="col-md-4">
    <div class="form-check mt-4">
        <label class="form-check-label">Có tăng ca</label>...
    </div>
</div>
```

**After:**
```html
<div class="col-md-12">
    <label class="form-label">Ngày vào làm</label>...
</div>
<div class="col-md-4">
    <label class="form-label">KC đến CT (km)</label>...
</div>
<div class="col-md-4">
    <label class="form-label">Độ hài lòng (1-4)</label>...
</div>
<div class="col-md-4">
    <div class="form-check mt-4">
        <label class="form-check-label">Có tăng ca</label>...
    </div>
</div>
```

---

## 2. Menu Navbar bị ẩn hoàn toàn chức năng trên mobile
- **File bị lỗi:** `Views/Shared/_Layout.cshtml`
- **Mô tả lỗi:** Khối chức năng Đăng xuất và thông tin User Profile nằm trong `<div class="collapse navbar-collapse" id="navbarMain">` (dòng 25). Tuy nhiên, trên giao diện mobile không có nút toggle (`navbar-toggler`) nào target đến `#navbarMain` (nút hiện tại chỉ target đến `#sidebarMenu`). Kết quả là trên thiết bị di động, phần chức năng đăng xuất sẽ bị ẩn hoàn toàn và người dùng không thể thao tác được.
- **Code đề xuất sửa chữa (Before/After):** Thay đổi class từ `collapse navbar-collapse` thành `d-flex` để luôn hiển thị các thông tin này (hoặc có thể bổ sung nút toggle riêng). Dưới đây là giải pháp dùng `d-flex` để luôn hiển thị inline.

**Before:**
```html
<div class="collapse navbar-collapse" id="navbarMain">
    <ul class="navbar-nav ms-auto mb-2 mb-lg-0 align-items-center">
```

**After:**
```html
<div class="d-flex align-items-center" id="navbarMain">
    <ul class="navbar-nav flex-row ms-auto mb-0 align-items-center gap-3">
```

---

## 3. Lạm dụng `!important` trong Media Query (CSS)
- **File bị lỗi:** `wwwroot/css/site.css`
- **Mô tả lỗi:** Tại dòng 305-307, trong block `@media (max-width: 767.98px)`, thuộc tính `padding-top: 1rem !important;` được gắn trực tiếp cho thẻ `main`. Sử dụng `!important` phá vỡ luồng specificity tự nhiên của CSS, tạo ra mã CSS rác, gây khó khăn khi cần đè lại padding ở các layout con.
- **Code đề xuất sửa chữa (Before/After):** Sử dụng CSS selector có tính cụ thể cao hơn thay vì dùng `!important`.

**Before:**
```css
    main {
        padding-top: 1rem !important;
    }
```

**After:**
```css
    body > main {
        padding-top: 1rem;
    }
```

---

## 4. Lỗi hiển thị viền bóng (focus state) khi click chuột
- **File bị lỗi:** `wwwroot/css/site.css`
- **Mô tả lỗi:** Các thẻ button và input đang sử dụng pseudo-class `:focus` chung (khoảng dòng 21-24). Điều này dẫn đến việc vòng `box-shadow` màu xanh luôn xuất hiện kể cả khi người dùng dùng chuột để click, gây khó chịu về mặt thị giác (UX không tốt).
- **Code đề xuất sửa chữa (Before/After):** Sử dụng pseudo-class `:focus-visible` để chỉ hiển thị viền bóng khi người dùng thao tác bằng bàn phím (Tab).

**Before:**
```css
.btn:focus, .btn:active:focus, .btn-link.nav-link:focus,
.form-control:focus, .form-check-input:focus, .form-select:focus {
    box-shadow: 0 0 0 0.2rem rgba(59, 130, 246, 0.25);
}
```

**After:**
```css
.btn:focus-visible, .btn-link.nav-link:focus-visible,
.form-control:focus-visible, .form-check-input:focus-visible, .form-select:focus-visible {
    box-shadow: 0 0 0 0.2rem rgba(59, 130, 246, 0.25);
}
```

---

## 5. Màu nền hover của dòng trong bảng gây nhầm lẫn
- **File bị lỗi:** `wwwroot/css/site.css`
- **Mô tả lỗi:** Màu nền hover của các dòng trong bảng `.table-hover > tbody > tr:hover` (dòng 156-158) đang được thiết lập là `#f8fafc`. Màu này trùng khớp với màu nền mặc định của phần Header bảng (thead). Do đó khi người dùng đưa chuột qua bảng, hiệu ứng hover không đủ độ tương phản để nhận biết rõ ràng.
- **Code đề xuất sửa chữa (Before/After):** Đổi màu nền hover sang một sắc độ đậm hơn.

**Before:**
```css
.table-hover > tbody > tr:hover {
    background-color: #f8fafc;
}
```

**After:**
```css
.table-hover > tbody > tr:hover {
    background-color: #f1f5f9;
}
```
