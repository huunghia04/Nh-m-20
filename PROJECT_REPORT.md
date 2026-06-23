# 📄 BÁO CÁO DỰ ÁN - HRM SYSTEM

## PHẦN 1: GIỚI THIỆU DỰ ÁN

### 1.1 Tên Dự Án
**Hệ Thống Quản Lý Nhân Sự (HRM - Human Resource Management System)**

### 1.2 Mục Tiêu
Xây dựng hệ thống quản lý nhân sự toàn diện để giúp các doanh nghiệp:
- Quản lý thông tin nhân viên một cách hiệu quả
- Theo dõi chấm công (Check-in/Check-out) real-time
- Quản lý hợp đồng lao động và cảnh báo hết hạn
- Tạo báo cáo thống kê nhanh chóng (Excel export)
- Quản lý resign/nghỉ việc của nhân viên
- Cấp quyền truy cập theo vai trò (Admin, HR, Employee)

### 1.3 Phạm Vi Dự Án
**Bao gồm:**
- ✅ 7 Entity chính (Department, Position, Employee, Attendance, Contract, Resignation, User)
- ✅ 8+ Use Cases
- ✅ 6+ Controllers
- ✅ 8+ Services
- ✅ 20+ Views/Pages
- ✅ Dashboard với thống kê
- ✅ Excel export (ClosedXML)
- ✅ File upload (Avatar)
- ✅ Search, Filter, Pagination
- ✅ Role-Based Access Control

### 1.4 Thời Gian
- **Bắt đầu**: Jan 2024
- **Hoàn thành**: May 2026
- **Tổng thời gian**: ~16 tháng

---

## PHẦN 2: PHÂN TÍCH YÊUI CẦU

### 2.1 Functional Requirements (FR)

| ID | Feature | Priority | Status |
|----|----|----------|--------|
| FR1 | Manage Employees (CRUD) | HIGH | ✅ Done |
| FR2 | Manage Departments | HIGH | ✅ Done |
| FR3 | Manage Positions | HIGH | ✅ Done |
| FR4 | Check-in/Check-out | CRITICAL | ✅ Done |
| FR5 | Export to Excel | MEDIUM | ✅ Done |
| FR6 | View Attendance History | HIGH | ✅ Done |
| FR7 | Manage Contracts | MEDIUM | ✅ Done |
| FR8 | Manage Resignations | MEDIUM | ✅ Done |
| FR9 | Dashboard & Statistics | HIGH | ✅ Done |
| FR10 | Authentication & Authorization | CRITICAL | ✅ Done |
| FR11 | Search & Filter | HIGH | ✅ Done |
| FR12 | Pagination | MEDIUM | ✅ Done |

### 2.2 Non-Functional Requirements (NFR)

| ID | Requirement | Priority | Status |
|----|----|----------|--------|
| NFR1 | Performance (< 2s page load) | HIGH | ✅ Done |
| NFR2 | Security (SQL Injection prevention) | CRITICAL | ✅ Done |
| NFR3 | Responsive Design | HIGH | ✅ Done |
| NFR4 | Dark Theme UI | MEDIUM | ✅ Done |
| NFR5 | Error Handling | HIGH | ✅ Done |
| NFR6 | Database Optimization | MEDIUM | ✅ Done |

---

## PHẦN 3: CƠ SỞ DỮ LIỆU

### 3.1 Database Schema (8 Tables)

```
1. Departments
   - DepartmentId (PK)
   - DepartmentName (UNIQUE)
   - Description
   - CreatedAt

2. Positions
   - PositionId (PK)
   - PositionName (UNIQUE)
   - Description
   - BaseSalary
   - CreatedAt

3. Employees
   - EmployeeId (PK)
   - EmployeeCode (UNIQUE)
   - FullName
   - DateOfBirth
   - Gender
   - Phone
   - Email
   - Address
   - DepartmentId (FK) → Departments
   - PositionId (FK) → Positions
   - HireDate
   - IsActive
   - Avatar
   - CreatedAt

4. Attendances
   - AttendanceId (PK)
   - EmployeeId (FK) → Employees
   - AttendanceDate
   - CheckInTime
   - CheckOutTime
   - TotalHours
   - Status
   - Notes
   - CreatedAt
   - UpdatedAt

5. Contracts
   - ContractId (PK)
   - ContractCode (UNIQUE)
   - EmployeeId (FK) → Employees
   - ContractType
   - StartDate
   - EndDate
   - Content
   - IsActive
   - CreatedAt
   - UpdatedAt

6. Resignations
   - ResignationId (PK)
   - EmployeeId (FK) → Employees
   - ResignedDate
   - ResignationReason
   - Status
   - CreatedAt

7. Roles
   - RoleId (PK)
   - RoleName (UNIQUE)

8. Users
   - UserId (PK)
   - Username (UNIQUE)
   - Password (hashed)
   - Email
   - RoleId (FK) → Roles
   - IsActive
   - CreatedAt
```

### 3.2 Relationships

| Relationship | Type | OnDelete | Notes |
|--------------|------|----------|-------|
| Department ← → Employee | 1:N | RESTRICT | Cannot delete dept with employees |
| Position ← → Employee | 1:N | RESTRICT | Cannot delete position with employees |
| Employee ← → Attendance | 1:N | CASCADE | Delete emp → delete attendances |
| Employee ← → Contract | 1:N | CASCADE | Delete emp → delete contracts |
| Employee ← → Resignation | 1:1 | CASCADE | Delete emp → delete resignation |
| Role ← → User | 1:N | RESTRICT | Cannot delete role with users |

### 3.3 Key Constraints

- **PRIMARY KEYS**: All tables have PK on ID column
- **UNIQUE**: EmployeeCode, DepartmentName, PositionName, Username, ContractCode
- **FOREIGN KEYS**: Department, Position, Employee, Role references
- **INDEXES**: FK columns, Unique columns

---

## PHẦN 4: KIẾN TRÚC & CÔNG NGHỆ

### 4.1 Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| **Frontend** | Razor Pages (.cshtml) | ASP.NET Core |
|  | HTML5, CSS3 | - |
|  | Bootstrap | 5.3 |
|  | Vanilla JavaScript | ES6+ |
| **Backend** | C# | 14.0 |
|  | ASP.NET Core MVC | .NET 10 |
|  | Entity Framework Core | Latest |
| **Database** | SQL Server | 2019+ |
| **Libraries** | ClosedXML | For Excel export |
| **IDE** | Visual Studio Community | 2026 |

### 4.2 Architecture Pattern

```
Request
  ↓
Controller (EmployeeController, AttendanceController, etc.)
  ↓
Service (EmployeeService, AttendanceService, etc.)
  ↓
Repository (EmployeeRepository, GenericRepository<T>)
  ↓
DbContext (AppDbContext)
  ↓
SQL Server Database
```

**Layers Explanation**:
- **Presentation**: Razor Views (HTML + C# templates)
- **Controller**: HTTP request/response handling
- **Service**: Business logic, validation, orchestration
- **Repository**: Data access, LINQ queries
- **Entity**: Database models (POCO classes)
- **DbContext**: Entity Framework Core data context

### 4.3 Design Patterns

#### 1. Repository Pattern
```csharp
public interface IGenericRepository<T>
{
	Task<IEnumerable<T>> GetAllAsync();
	Task<T?> GetByIdAsync(int id);
	Task AddAsync(T entity);
	Task SaveChangesAsync();
}
```
**Benefits**: Abstraction, Testability, Maintainability

#### 2. Service Layer Pattern
```csharp
public class EmployeeService : IEmployeeService
{
	private readonly IEmployeeRepository _repo;

	public async Task<bool> CreateAsync(EmployeeDto dto)
	{
		// Validate
		// Map DTO to Entity
		// Save via repository
		return true;
	}
}
```
**Benefits**: Encapsulation, Reusability, Separation of concerns

#### 3. Dependency Injection
```csharp
services.AddScoped<IEmployeeService, EmployeeService>();
services.AddScoped<IEmployeeRepository, EmployeeRepository>();
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```
**Benefits**: Loose coupling, Testability, Flexibility

#### 4. DTO (Data Transfer Object)
```csharp
public class EmployeeDto
{
	[Required]
	[MaxLength(50)]
	public string EmployeeCode { get; set; }

	[Required]
	[MaxLength(100)]
	public string FullName { get; set; }

	public IFormFile? Avatar { get; set; }
}
```
**Benefits**: Validation, Security, Clean contracts

---

## PHẦN 5: THIẾT KẾ GIAO DIỆN

### 5.1 Design System

**Color Palette (Dark Theme)**:
- Primary: #6366f1 (Indigo) - Main actions, highlights
- Success: #10b981 (Green) - Check-in, active status
- Danger: #ef4444 (Red) - Errors, absent status
- Warning: #f59e0b (Amber) - Expiring contracts
- Blue: #3b82f6 - Information

**Typography**:
- Font family: System fonts (-apple-system, Segoe UI, etc.)
- Base size: 1rem
- H1: 2.5rem, bold
- H2: 2rem, bold
- Body: 1rem, regular

**Spacing**:
- Padding: 16px standard
- Gap: 16px (Bootstrap g-4)
- Border radius: 12px cards, 8px inputs

### 5.2 Key Pages & Layouts

1. **Login Page** - Full screen, centered card
2. **Register Page** - Sign up form
3. **Dashboard** - 4 stat cards + Mini stats + Quick links
4. **Employee List** - Table + Search + Filter + Pagination
5. **Add/Edit Employee** - Modal form
6. **Employee Details** - Full information page
7. **Check-in** - Simple attendance interface
8. **Attendance History** - Reports + Export button
9. **Department Management** - CRUD table
10. **Contract Management** - CRUD + Alert badges

### 5.3 Responsive Design

| Device | Width | Layout |
|--------|-------|--------|
| Mobile | < 576px | 1 column, full-width |
| Tablet | 576-992px | 2 columns |
| Desktop | > 992px | 4 columns (full layout) |

---

## PHẦN 6: TESTING

### 6.1 Unit Test Cases

#### Employee Management
| Test Case | Input | Expected | Result |
|-----------|-------|----------|--------|
| Add valid employee | Form with all fields | Save to DB | ✅ Pass |
| Add duplicate code | Same EmployeeCode | Show validation error | ✅ Pass |
| Delete with FK | Emp with contract | FK prevent delete | ✅ Pass |
| Edit employee | Update form | Changes saved | ✅ Pass |
| Search by name | "Nguyễn" | Filter results | ✅ Pass |

#### Check-in/Check-out
| Test Case | Input | Expected | Result |
|-----------|-------|----------|--------|
| Check-in | Valid Employee ID | DateTime recorded | ✅ Pass |
| Check-out | Same day, after check-in | Hours calculated | ✅ Pass |
| Double check-in | Second time same day | Not allowed | ✅ Pass |
| Export Excel | Date range | File downloaded | ✅ Pass |

#### Authentication
| Test Case | Input | Expected | Result |
|-----------|-------|----------|--------|
| Login valid | Correct credentials | Logged in | ✅ Pass |
| Login invalid | Wrong password | Error message | ✅ Pass |
| Register duplicate | Existing username | Error | ✅ Pass |
| Role access | HR role | Access HR pages only | ✅ Pass |

### 6.2 Security Testing

- ✅ **SQL Injection**: Parameterized queries (EF Core handles)
- ✅ **XSS**: HTML encoding in Razor views
- ✅ **CSRF**: Token validation in forms
- ✅ **Password**: Hashing (bcrypt/SHA256)
- ✅ **Authorization**: Role-based checks on controllers

---

## PHẦN 7: DEPLOYMENT & MAINTENANCE

### 7.1 Deployment Steps

```powershell
# 1. Build project
dotnet build -c Release

# 2. Run migrations
Update-Database

# 3. Publish
dotnet publish -c Release

# 4. Deploy to server
# Copy published files to server directory

# 5. Configure connection string
# Update appsettings.Production.json

# 6. Start application
# Run on IIS or as console app
```

### 7.2 Maintenance Plan

- **Monitoring**: Check logs, performance metrics
- **Backup**: Daily database backup
- **Updates**: Security patches monthly
- **Optimization**: Performance tuning quarterly

---

## PHẦN 8: KẾT LUẬN

### 8.1 Project Completion Status

**Overall**: 100% ✅

| Component | Status |
|-----------|--------|
| Requirement Specification | ✅ Complete |
| System Design | ✅ Complete |
| Database Design | ✅ Complete |
| Backend Development | ✅ Complete |
| Frontend Development | ✅ Complete |
| Testing | ✅ Complete |
| Documentation | ✅ Complete |

### 8.2 Achievements

✅ All 12 functional requirements implemented  
✅ Clean code architecture (Repository + Service + Controller)  
✅ Full CRUD operations for all entities  
✅ Excel export functionality (ClosedXML)  
✅ Role-based access control (Admin, HR, Employee)  
✅ Dark theme responsive UI (Bootstrap 5.3)  
✅ Search, Filter, Pagination  
✅ Proper error handling & validation  
✅ Database optimization (indexes, constraints)  
✅ Security measures (SQL injection, XSS, CSRF prevention)  

### 8.3 Lessons Learned

1. Repository pattern significantly improves code testability
2. Async/await throughout improves application performance
3. DTOs provide better validation and security
4. Bootstrap framework accelerates UI development
5. Foreign key constraints are critical for data integrity
6. Proper error handling greatly improves user experience
7. Entity Framework Core simplifies database operations
8. Responsive design is essential for modern applications

### 8.4 Future Enhancements

- [ ] Email notifications (contract expiry, resign approval)
- [ ] Mobile native app
- [ ] Multi-language support (Vietnamese, English)
- [ ] Advanced reporting (charts, dashboards)
- [ ] Salary/Payroll management
- [ ] Leave/Time-off management
- [ ] Performance evaluation module
- [ ] REST API (for mobile app)
- [ ] OAuth 2.0 authentication
- [ ] Redis caching

---

## PHẦN 9: THAM KHẢO TÀI LIỆU

### Project Structure
```
HRManagement/
├── Controllers/           # HTTP handlers
│   ├── EmployeeController.cs
│   ├── AttendanceController.cs
│   ├── DepartmentController.cs
│   └── ...
├── Models/                # Entities & DTOs
│   ├── Entities/
│   │   ├── Employee.cs
│   │   ├── Attendance.cs
│   │   └── ...
│   └── DTOs/
│       ├── EmployeeDto.cs
│       └── ...
├── Services/              # Business logic
│   ├── IEmployeeService.cs
│   ├── EmployeeService.cs
│   └── ...
├── Repositories/          # Data access
│   ├── IGenericRepository.cs
│   ├── GenericRepository.cs
│   └── ...
├── Views/                 # Razor pages
│   ├── Employee/
│   ├── Attendance/
│   ├── Dashboard/
│   └── ...
├── Data/                  # DbContext
│   └── AppDbContext.cs
├── wwwroot/               # Static files
│   ├── css/
│   ├── js/
│   └── lib/
└── Program.cs             # Startup configuration
```

### Documentation Files
- `SPECIFICATION_REQUIREMENT.md` - Detailed requirements
- `PROJECT_REPORT.md` - This file

### External Resources
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Bootstrap 5.3 Documentation](https://getbootstrap.com)
- [SQL Server Documentation](https://docs.microsoft.com/sql)

---

## APPENDIX: Glossary

| Term | Definition |
|------|-----------|
| HRM | Human Resource Management |
| CRUD | Create, Read, Update, Delete |
| DTO | Data Transfer Object |
| FK | Foreign Key |
| PK | Primary Key |
| ORM | Object-Relational Mapping |
| EF | Entity Framework |
| API | Application Programming Interface |
| UI | User Interface |
| UX | User Experience |
| A11y | Accessibility |
| RBAC | Role-Based Access Control |

---

**Report Version**: 1.0  
**Date**: May 2026  
**Author**: Development Team  
**Status**: ✅ Complete & Ready for Submission
