# 📋 ĐẶC TẢ YÊU CẦU DỰ ÁN HRM
## Human Resource Management System

---

## 1. GIỚI THIỆU

### Tên Dự Án
**Hệ Thống Quản Lý Nhân Sự (HRM - Human Resource Management)**

### Mục Tiêu
- ✅ Quản lý thông tin nhân viên
- ✅ Quản lý phòng ban và chức vụ
- ✅ Theo dõi chấm công (Check-in/Check-out)
- ✅ Quản lý hợp đồng lao động
- ✅ Quản lý resign/nghỉ việc
- ✅ Báo cáo thống kê

### Khách Hàng
- Các doanh nghiệp vừa và nhỏ (SMEs)

---

## 2. FEATURES CHÍNH

### Feature 1: Quản Lý Nhân Viên
**Chức năng:**
- Thêm nhân viên mới (Employee Code, Full Name, DOB, Gender, Email, Phone, Address, Department, Position, HireDate, Avatar)
- Sửa thông tin nhân viên
- Xóa nhân viên
- Xem chi tiết nhân viên
- Tìm kiếm & Lọc (theo tên, mã, phòng ban, chức vụ)
- Phân trang (10, 20, 50 items/page)

**Validation:**
- Mã nhân viên: bắt buộc, unique, max 50 ký tự
- Họ tên: bắt buộc, max 100 ký tự
- Email: định dạng email
- Ngày sinh: < ngày hiện tại
- Avatar: chỉ ảnh (.jpg, .png, .gif), max 5MB

### Feature 2: Quản Lý Phòng Ban
**Chức năng:**
- Thêm phòng ban (Tên, Mô tả)
- Sửa phòng ban
- Xóa phòng ban (kiểm tra FK constraint)
- Xem danh sách nhân viên trong phòng ban

**Constraint:**
- ❌ Không thể xóa phòng ban nếu có nhân viên

### Feature 3: Quản Lý Chức Vụ
**Chức năng:**
- Thêm chức vụ (Tên, Mô tả, Lương cơ bản)
- Sửa chức vụ
- Xóa chức vụ

### Feature 4: Quản Lý Chấm Công
**Chức năng:**
- Check-in: Nhân viên quét mã hoặc chọn từ danh sách → Lưu thời gian (DateTime.Now)
- Check-out: Tính tổng giờ làm = (CheckOutTime - CheckInTime) / 3600 giây
- Xem lịch sử check-in/out (có date picker)
- Export Excel báo cáo chấm công (1 nhân viên hoặc toàn công ty)
- Thống kê hôm nay (bao nhiêu người check-in)

**Ràng buộc:**
- ✅ Chỉ nhập được 1 check-in/check-out mỗi ngày
- ✅ TotalHours = (CheckOutTime - CheckInTime) / 3600

### Feature 5: Quản Lý Hợp Đồng
**Chức năng:**
- Thêm hợp đồng (Mã HĐ, Tên nhân viên, Loại HĐ, Ngày bắt đầu, Ngày kết thúc)
- Sửa hợp đồng
- Xóa hợp đồng
- Cảnh báo HĐ sắp hết hạn (trong 30 ngày)

### Feature 6: Quản Lý Resign
**Chức năng:**
- Tạo yêu cầu resign (Ngày resign, Lý do)
- Xác nhận resign (Admin)
- Xem danh sách nhân viên đã resign
- Khôi phục nhân viên (Restore)

### Feature 7: Dashboard
**Hiển thị:**
- Tổng nhân viên
- Nhân viên đang làm việc
- Số phòng ban
- HĐ sắp hết hạn
- Truy cập nhanh (Quick links)

### Feature 8: Xác Thực & Phân Quyền
**Roles:**
- Admin: Toàn quyền quản lý hệ thống
- HR: Quản lý nhân viên, hợp đồng, chấm công
- Employee: Xem thông tin cá nhân, check-in/out

---

## 3. CÔNG NGHỆ SỬ DỤNG

| Lớp | Công Nghệ | Phiên Bản |
|-----|-----------|----------|
| **Frontend** | Razor Pages | .NET 10 |
|  | HTML5, CSS3, Bootstrap 5.3 | - |
|  | Vanilla JavaScript | ES6+ |
| **Backend** | ASP.NET Core | .NET 10 |
|  | C# | 14.0 |
|  | Entity Framework Core | Latest |
| **Database** | SQL Server | 2019+ |
| **IDE** | Visual Studio Community | 2026 |

---

## 4. DATABASE SCHEMA

### Tables (8)
1. **Departments** - Phòng Ban
2. **Positions** - Chức Vụ
3. **Employees** - Nhân Viên
4. **Attendances** - Chấm Công
5. **Contracts** - Hợp Đồng
6. **Resignations** - Resign
7. **Roles** - Vai Trò
8. **Users** - Người Dùng

### Relationships
- Department 1:N Employee (RESTRICT)
- Position 1:N Employee (RESTRICT)
- Employee 1:N Attendance (CASCADE)
- Employee 1:N Contract (CASCADE)
- Employee 1:1 Resignation (CASCADE)
- Role 1:N User (RESTRICT)

### Key Constraints
- EmployeeCode: UNIQUE
- DepartmentName: UNIQUE
- PositionName: UNIQUE
- Username: UNIQUE
- ContractCode: UNIQUE

---

## 5. ARCHITECTURE

### Pattern: Repository + Service + Controller

```
Request 
  ↓
Controller (HTTP handlers)
  ↓
Service (Business logic)
  ↓
Repository (Data access)
  ↓
DbContext (EF Core)
  ↓
SQL Server
```

### Layers
- **Presentation**: Razor Views
- **Controller**: HTTP request handling
- **Service**: Business logic
- **Repository**: Data access & LINQ
- **Entity**: Database models
- **DbContext**: EF Core context

---

## 6. DESIGN PATTERNS

### Repository Pattern
```csharp
public interface IGenericRepository<T>
{
	Task<IEnumerable<T>> GetAllAsync();
	Task<T?> GetByIdAsync(int id);
	Task AddAsync(T entity);
	Task SaveChangesAsync();
}
```

### Service Layer Pattern
```csharp
public class EmployeeService : IEmployeeService
{
	private readonly IEmployeeRepository _repo;

	public async Task<bool> CreateAsync(EmployeeDto dto)
	{
		// Validate
		// Map DTO to Entity
		// Save to database
		return true;
	}
}
```

### Dependency Injection
```csharp
services.AddScoped<IEmployeeService, EmployeeService>();
services.AddScoped<IEmployeeRepository, EmployeeRepository>();
```

---

## 7. UI/UX DESIGN

### Color Palette (Dark Theme)
- Primary: #6366f1 (Indigo) - Main actions
- Success: #10b981 (Green) - Check-in, active
- Danger: #ef4444 (Red) - Errors
- Warning: #f59e0b (Amber) - Expiring
- Blue: #3b82f6 - Information

### Key Pages
1. **Login/Register** - Authentication
2. **Dashboard** - Stat cards + Quick links
3. **Employee List** - CRUD + Search + Filter
4. **Employee Form** - Modal add/edit
5. **Check-in Page** - Simple attendance interface
6. **Attendance History** - Reports with export
7. **Department Management** - CRUD
8. **Contract Management** - CRUD + alerts

### Responsive Design
- Mobile: < 576px (1 column)
- Tablet: 576-992px (2 columns)
- Desktop: > 992px (4 columns)

---

## 8. TESTING REQUIREMENTS

### Functional Testing
- ✅ CRUD operations work correctly
- ✅ Search & Filter functionality
- ✅ FK constraint validation
- ✅ Check-in/out logic
- ✅ Excel export
- ✅ Role-based access

### Security Testing
- ✅ SQL Injection prevention
- ✅ XSS prevention
- ✅ Password hashing
- ✅ CSRF protection
- ✅ Authorization checks

### Performance Testing
- ✅ Page load time < 2s
- ✅ Search results < 1s
- ✅ Excel export < 5s

---

## 9. DEPLOYMENT

### Steps
1. Build: `dotnet build`
2. Run migrations: `Update-Database`
3. Publish: `dotnet publish -c Release`
4. Deploy to server
5. Configure connection string
6. Start application

---

## 10. ACCEPTANCE CRITERIA

### Functional
- ✅ All CRUD operations complete
- ✅ Search & Filter working
- ✅ Excel export functional
- ✅ Role-based access working
- ✅ Check-in/out logic correct
- ✅ FK constraints working

### Non-Functional
- ✅ Responsive design (Mobile/Tablet/Desktop)
- ✅ Dark theme UI consistent
- ✅ Page load < 2s
- ✅ No SQL injection vulnerabilities
- ✅ Proper error handling

---

**Document Version**: 1.0  
**Created**: May 2026  
**Status**: ✅ Complete
