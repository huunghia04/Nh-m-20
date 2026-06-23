# Báo Cáo Kiểm Toán Mã Nguồn Backend - HRManagement

**Thời gian:** 2026-06-13
**Mục tiêu:** Thực hiện audit (read-only) mã nguồn ASP.NET Core backend tập trung vào hai khía cạnh: Performance & Database và Architecture & Logic.

Dưới đây là 5 vấn đề cụ thể được phát hiện trong mã nguồn, bao gồm nguyên nhân và đề xuất khắc phục.

---

## 1. Architecture & Logic: Fat Controller & Hardcode Logic (Controller chứa Business Logic)
**File:** `Controllers/EmployeeController.cs` (Dòng 77-110)

**Nguyên nhân:**
Phương thức `EnableContractNV002` inject trực tiếp `AppDbContext` và thực hiện trực tiếp các thao tác nghiệp vụ phức tạp (khởi tạo, cập nhật hợp đồng, lưu vào database, sau đó gọi `CalculatePayrollAsync`). Ngoài ra, mã nhân viên `"NV002"` và các thông tin hợp đồng được hardcode trực tiếp vào Controller. Điều này vi phạm nghiêm trọng nguyên tắc SRP (Single Responsibility Principle) và tạo ra "Fat Controller". Controller chỉ nên làm nhiệm vụ nhận Request và trả về Response, còn logic nghiệp vụ phải được đặt ở Service layer.

**Code hiện tại (Before):**
```csharp
    [AllowAnonymous]
    public async Task<IActionResult> EnableContractNV002([FromServices] HRManagement.Data.AppDbContext context, [FromServices] HRManagement.Services.Interfaces.IPayrollService payrollService)
    {
        var emp = context.Employees.FirstOrDefault(e => e.EmployeeCode == "NV002");
        if (emp != null)
        {
            var contract = context.Contracts.FirstOrDefault(c => c.EmployeeId == emp.EmployeeId);
            // ... Logic tạo mới Contract và SaveChanges ...
            await payrollService.CalculatePayrollAsync(emp.EmployeeId, 6, 2026);
            return Content("Đã bật hợp đồng và tính lương tháng 6/2026 cho NV002...");
        }
        return Content("Không tìm thấy NV002");
    }
```

**Đề xuất khắc phục (After):**
Chuyển logic này sang `IEmployeeService` hoặc `IContractService` và truyền tham số động thay vì hardcode.
```csharp
    [HttpPost("EnableContract")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EnableContract(string employeeCode, int month, int year)
    {
        // Controller chỉ gọi Service để thực hiện Business Logic
        var result = await _contractService.EnableContractAndCalculatePayrollAsync(employeeCode, month, year);
        if (result.Success)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }
```

---

## 2. Architecture & Logic: Vi phạm SOLID (Open/Closed Principle) & Hardcode
**File:** `Services/PayrollService.cs` (Dòng 110-138)

**Nguyên nhân:**
Trong phương thức `CalculatePayrollAsync`, các quy tắc tính phụ cấp (`positionAllowance`), thưởng (`positionBonus`) và hệ số tăng ca (`overtimeRate`) đang được hardcode trực tiếp vào code bằng hàng loạt câu lệnh `if-else` dựa trên chuỗi tên chức vụ (ví dụ: `"giám đốc"`, `"trưởng phòng"`). Điều này vi phạm nguyên tắc Open/Closed (OCP). Bất cứ khi nào công ty có chức vụ mới hoặc thay đổi mức phụ cấp, lập trình viên đều phải mở file code này ra để sửa và re-build dự án.

**Code hiện tại (Before):**
```csharp
        if (employee.Position != null)
        {
            string posName = employee.Position.PositionName.ToLower();
            if (posName.Contains("giám đốc")) {
                positionAllowance = 5_000_000m;
                positionBonus = 10_000_000m;
                overtimeRate = 0m; // Không tính tăng ca
            }
            else if (posName.Contains("trưởng phòng") || posName.Contains("quản lý")) {
//...
```

**Đề xuất khắc phục (After):**
Thêm các trường `Allowance`, `Bonus`, `OvertimeRate` vào bảng/entity `Position`. Service chỉ việc lấy dữ liệu này ra tính toán.
```csharp
        if (employee.Position != null)
        {
            // Lấy trực tiếp từ thuộc tính của đối tượng Position, không hardcode string
            positionAllowance = employee.Position.Allowance ?? 0m;
            positionBonus = employee.Position.Bonus ?? 0m;
            overtimeRate = employee.Position.OvertimeRate ?? OVERTIME_RATE;
        }
```

---

## 3. Performance & DB: Load tất cả dữ liệu vào bộ nhớ (Load all tables)
**File:** `Controllers/AttendanceController.cs` (Dòng 27-44)

**Nguyên nhân:**
Trong phương thức `Today`, để lấy danh sách điểm danh của ngày hôm nay, ứng dụng đã gọi `await _attendanceService.GetAllAsync()` để kéo TOÀN BỘ dữ liệu điểm danh của tất cả nhân viên từ trước đến nay từ database lên RAM (client-side), sau đó mới sử dụng LINQ `.Where(...)` để lọc ra dữ liệu của ngày hôm nay. Với lượng dữ liệu lớn, việc này sẽ gây sập bộ nhớ (Out of Memory) và làm chậm hệ thống nghiêm trọng.

**Code hiện tại (Before):**
```csharp
            var result = await _attendanceService.GetAllAsync();
            attendances = result.Data;
        }
        var today = DateTime.Now.Date;
        var todays = attendances
            .Where(a => a.AttendanceDate.Date == today && a.CheckInTime.HasValue)
            .ToList();
```

**Đề xuất khắc phục (After):**
Viết một truy vấn riêng trong Repository để lọc dữ liệu trực tiếp ở mức Database thông qua câu lệnh SQL (chỉ lấy những bản ghi của ngày hôm nay).
```csharp
        // Controller gọi phương thức đã được tối ưu từ Service
        var result = await _attendanceService.GetTodayAttendancesAsync();
        attendances = result.Data; // Dữ liệu trả về đã được lọc sẵn từ DB
        
        var todays = attendances.ToList();
```

---

## 4. Performance & DB: Lỗi truy vấn N+1 (N+1 Query Issue)
**File:** `Controllers/LeaveController.cs` (Dòng 102-117)

**Nguyên nhân:**
Trong phương thức `Balance`, hệ thống lặp qua danh sách toàn bộ nhân viên đang hoạt động bằng vòng lặp `foreach`. Ứng với mỗi nhân viên, hệ thống thực hiện một truy vấn `await _leaveService.GetBalanceAsync(...)` để lấy số dư phép. Nếu có 1000 nhân viên, sẽ có 1 truy vấn lấy danh sách + 1000 truy vấn lấy số dư phép = 1001 truy vấn đến cơ sở dữ liệu. Đây là lỗi truy vấn N+1 kinh điển làm suy giảm hiệu năng cơ sở dữ liệu.

**Code hiện tại (Before):**
```csharp
        foreach (var emp in activeEmployees)
        {
            var balanceResult = await _leaveService.GetBalanceAsync(emp.EmployeeId, currentYear);
            var balance = balanceResult.Data;
            if (balance == null)
            {
//...
```

**Đề xuất khắc phục (After):**
Lấy toàn bộ số dư phép của các nhân viên trong năm hiện tại chỉ bằng MỘT truy vấn duy nhất (sử dụng `.Where(b => b.Year == currentYear)` trong DB), đưa vào Dictionary, sau đó map trong vòng lặp.
```csharp
        // Lấy tất cả balance của năm hiện tại trong 1 lần truy vấn duy nhất
        var allBalancesResult = await _leaveService.GetAllBalancesByYearAsync(currentYear);
        var balancesDict = allBalancesResult.Data.ToDictionary(b => b.EmployeeId);

        foreach (var emp in activeEmployees)
        {
            balancesDict.TryGetValue(emp.EmployeeId, out var balance);
            // ... logic kiểm tra và gán balance
```

---

## 5. Performance & DB: Thiếu AsNoTracking cho các truy vấn Read-Only
**File:** `Repositories/EmployeeRepository.cs` (Dòng 24-30)

**Nguyên nhân:**
Phương thức `SearchAsync` dùng để tìm kiếm và trả về danh sách nhân viên phục vụ việc hiển thị trên giao diện (Read-Only). Tuy nhiên, Entity Framework mặc định sẽ Tracking (theo dõi trạng thái) của tất cả các entity được tải lên. Việc này tốn thêm bộ nhớ RAM và CPU để duy trì snapshot, trong khi dữ liệu này không hề được cập nhật (`SaveChanges`) trong request này.

**Code hiện tại (Before):**
```csharp
    public async Task<PaginatedList<Employee>> SearchAsync(EmployeeSearchVM search)
    {
        var query = _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .AsQueryable();
//...
```

**Đề xuất khắc phục (After):**
Thêm phương thức `.AsNoTracking()` vào chuỗi truy vấn để tắt tính năng theo dõi, qua đó tối ưu hóa tốc độ truy vấn và tiết kiệm bộ nhớ.
```csharp
    public async Task<PaginatedList<Employee>> SearchAsync(EmployeeSearchVM search)
    {
        var query = _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .AsNoTracking() // Tối ưu hóa hiệu suất cho truy vấn chỉ đọc
            .AsQueryable();
//...
```

---
**Phương pháp xác minh (Verification Method):**
- **Issue 1, 3, 4:** Tìm kiếm các đoạn mã lỗi trong Visual Studio / VS Code để xác thực file và dòng bị lỗi.
- **Issue 2:** Có thể tìm chuỗi "giám đốc" trong file `PayrollService.cs`.
- **Issue 5:** Kiểm tra hàm `SearchAsync` trong `EmployeeRepository.cs` xem đã có lệnh `AsNoTracking()` hay chưa.
- **Để kiểm tra hiệu năng (Performance):** Chạy project với Entity Framework Core Logger bật tính năng hiển thị câu lệnh SQL (`EnableSensitiveDataLogging`), có thể thấy rõ các truy vấn SELECT N+1 khi vào trang `Leave/Balance` và truy vấn SELECT toàn bộ bảng khi vào `Attendance/Today`.
