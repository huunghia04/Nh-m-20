# Báo Cáo Rà Soát Lỗi Tiềm Ẩn (Hidden Bugs Audit Report) - HRManagement

**Thời gian:** 2026-06-13
**Mục tiêu:** Rà soát mã nguồn Backend và Frontend để phát hiện các lỗi tiềm ẩn (chỉ đọc, không chỉnh sửa mã nguồn).

Sau quá trình rà soát chi tiết toàn bộ Controllers, Services, Repositories, Models và Views, chúng tôi đã phát hiện **5 lỗi tiềm ẩn** có thể ảnh hưởng đến logic nghiệp vụ, tính ổn định và trải nghiệm người dùng của hệ thống.

---

## 1. Backend: Lỗi thiếu kiểm tra null (Missing null checks)
- **Tên file:** `Controllers/AttendanceController.cs`
- **Vị trí dòng code:** Dòng 224-226 / Phương thức `ExportAllToExcel`
- **Mô tả lỗi:** Phương thức gọi `await _attendanceService.GetAllAsync()` để lấy danh sách điểm danh, sau đó truy cập thẳng vào thuộc tính `.Data` và gọi LINQ `.Where(...)`. Nếu dịch vụ trả về kết quả lỗi (ví dụ: gián đoạn kết nối CSDL), thuộc tính `.Data` sẽ bị `null`. Việc không kiểm tra giá trị null sẽ dẫn đến lỗi `ArgumentNullException`, làm hệ thống bị sập (HTTP 500).
- **Mức độ nghiêm trọng:** Cao (High)
- **Đề xuất khắc phục:** Cần kiểm tra trạng thái Success hoặc null trước khi sử dụng.
```csharp
var allAttendancesResult = await _attendanceService.GetAllAsync();
if (!allAttendancesResult.Success || allAttendancesResult.Data == null)
{
    TempData["Error"] = "Lấy dữ liệu chấm công thất bại!";
    return RedirectToAction("Index");
}
var allAttendances = allAttendancesResult.Data;
var attendancesInRange = allAttendances
    .Where(a => a.AttendanceDate >= start && a.AttendanceDate <= end)
    .ToList();
```

---

## 2. Backend: Rủi ro đa luồng (Concurrency risks)
- **Tên file:** `Services/PayrollService.cs`
- **Vị trí dòng code:** Dòng 63-71 & 155-163 / Phương thức `CalculatePayrollAsync`
- **Mô tả lỗi:** Khi hệ thống kiểm tra sự tồn tại của bảng lương qua `existingPayroll = await _payrollRepo.GetByEmployeeAndMonthAsync(...)`. Nếu `null`, nó sẽ tạo mới bảng lương. Nếu có 2 luồng yêu cầu tính lương diễn ra đồng thời cho cùng một nhân viên, cả 2 sẽ thấy `null` và tạo ra 2 bản ghi trùng lặp. Đây là lỗi "Time-of-Check to Time-of-Use" (TOCTOU) kinh điển, gây sai lệch báo cáo kế toán.
- **Mức độ nghiêm trọng:** Cao (High)
- **Đề xuất khắc phục:** Bắt lỗi ngoại lệ từ database (nếu có unique constraint) khi lưu.
```csharp
if (existingPayroll == null)
{
    payroll.Status = "Draft";
    await _payrollRepo.AddAsync(payroll);
    try 
    {
        await _payrollRepo.SaveChangesAsync();
    } 
    catch (Microsoft.EntityFrameworkCore.DbUpdateException) 
    {
        return ServiceResult<Payroll?>.ErrorResult("Bảng lương của nhân viên này trong tháng đã được tạo bởi một tiến trình khác.");
    }
}
else
{
    _payrollRepo.Update(payroll);
    await _payrollRepo.SaveChangesAsync();
}
```

---

## 3. Backend: Hổng logic nghiệp vụ (Business logic flaws)
- **Tên file:** `Services/PayrollService.cs`
- **Vị trí dòng code:** Dòng 125-131 / Phương thức `CalculatePayrollAsync`
- **Mô tả lỗi:** Khi tính lại lương, mã nguồn gán cứng `payroll.Deduction = latePenalty + 0` để tránh cộng dồn lỗi tiền phạt đi muộn. Tuy nhiên, trường `Deduction` còn được sử dụng để chứa các khoản khấu trừ thủ công do phòng nhân sự cập nhật. Việc gán cứng này vô tình ghi đè và xóa mất các số liệu khấu trừ thủ công đã được nhập trước đó.
- **Mức độ nghiêm trọng:** Trung bình (Medium)
- **Đề xuất khắc phục:** Cần bóc tách khoản phạt đi muộn cũ ra khỏi tổng tiền khấu trừ trước khi cộng thêm khoản phạt mới.
```csharp
decimal latePenalty = payroll.LateDays * LATE_PENALTY;
if (existingPayroll != null)
{
    // Giữ lại khoản khấu trừ thủ công
    decimal oldLatePenalty = existingPayroll.LateDays * LATE_PENALTY;
    decimal manualDeduction = existingPayroll.Deduction - oldLatePenalty;
    payroll.Deduction = manualDeduction + latePenalty;
}
else
{
    payroll.Deduction = latePenalty;
}
```

---

## 4. Frontend: Lỗi render sai trạng thái (Incorrect state rendering)
- **Tên file:** `Views/Payroll/Index.cshtml`
- **Vị trí dòng code:** Dòng 183
- **Mô tả lỗi:** Form đánh dấu đã thanh toán lương gọi đến `asp-action="MarkAsPaid"` nhưng thiếu các tham số `month` và `year` theo yêu cầu của Controller. Hậu quả là Controller nhận `month=0`, `year=0`. Giao diện sau khi thanh toán xong sẽ tải lại bảng lương của tháng 0/0 và hiển thị màn hình trống không có dữ liệu, gây hoang mang cho người dùng.
- **Mức độ nghiêm trọng:** Trung bình (Medium)
- **Đề xuất khắc phục:** Bổ sung thuộc tính `asp-route-month` và `asp-route-year`.
```html
<form method="post" asp-action="MarkAsPaid" asp-route-id="@item.PayrollId" asp-route-month="@month" asp-route-year="@year" style="display:inline;">
```

---

## 5. Frontend: Gọi Action không tồn tại (Calls to non-existent API/Action)
- **Tên file:** `Views/Payroll/Details.cshtml`
- **Vị trí dòng code:** Dòng 158
- **Mô tả lỗi:** Form "Điều chỉnh bảng lương" được khai báo với `asp-action="UpdatePayroll"`. Tuy nhiên, trong `PayrollController` không hề định nghĩa Action nào mang tên này. Action thực sự quản lý logic này là `UpdateAdjustments`. Hơn nữa, form thiếu tham số `allowance` nên Request gửi đi sẽ bị lỗi và không thể cập nhật được thông tin.
- **Mức độ nghiêm trọng:** Cao (High)
- **Đề xuất khắc phục:** Đổi tên action thành `UpdateAdjustments` và cung cấp tham số `allowance` bằng thẻ `<input hidden>`.
```html
<form method="post" asp-action="UpdateAdjustments">
    <input type="hidden" name="payrollId" value="@Model.PayrollId" />
    <input type="hidden" name="allowance" value="0" />
    <!-- Các phần còn lại của form -->
</form>
```

---
*Báo cáo được khởi tạo tự động. Quá trình kiểm tra đảm bảo 100% Read-Only, không sửa đổi bất kỳ mã nguồn gốc nào.*
