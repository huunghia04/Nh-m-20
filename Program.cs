using System.Text;
using Microsoft.Extensions.ML;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HRManagement.Data;
using HRManagement.Helpers;
using HRManagement.Models.Entities;
using HRManagement.Repositories;
using HRManagement.Repositories.Interfaces;
using HRManagement.Services;
using HRManagement.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// === Cấu hình Database ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Đăng ký Repositories ===
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

// === Đăng ký Services ===
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<ITurnoverPredictionService, TurnoverPredictionService>();
builder.Services.AddScoped<IPredictionService, PredictionService>();
builder.Services.AddPredictionEnginePool<HRManagement.Models.DTOs.EmployeeTurnoverData, HRManagement.Models.DTOs.TurnoverPrediction>()
    .FromFile(System.IO.Path.Combine(Environment.CurrentDirectory, "TurnoverModel.zip"));

// === Đăng ký JWT Helper ===
builder.Services.AddSingleton<JwtHelper>();

// === Cấu hình JWT Authentication ===
var jwtKey = builder.Configuration["Jwt:SecretKey"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    // Đọc token từ cookie thay vì header (vì dùng Razor Views)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        // Redirect về trang Login khi chưa đăng nhập
        OnChallenge = context =>
        {
            var request = context.Request;
            var isApi = request.Path.StartsWithSegments("/api") || 
                        request.Headers["Accept"].ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
                        request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isApi)
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"success\":false,\"message\":\"Unauthorized\"}");
            }

            context.HandleResponse();
            context.Response.Redirect("/Auth/Login");
            return Task.CompletedTask;
        },
        // Redirect về trang AccessDenied khi không có quyền
        OnForbidden = context =>
        {
            var request = context.Request;
            var isApi = request.Path.StartsWithSegments("/api") || 
                        request.Headers["Accept"].ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
                        request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isApi)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"success\":false,\"message\":\"Forbidden\"}");
            }

            context.Response.Redirect("/Auth/Login?error=forbidden");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// === Cấu hình MVC ===
builder.Services.AddControllersWithViews();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5146);
});

var app = builder.Build();
// ĐOẠN CODE KIỂM TRA DATABASE:
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("✅ KẾT NỐI DATABASE THÀNH CÔNG RỒI TÂM ƠI!");
            Console.WriteLine("==============================================");
            
            // Seed mock data
            // HRManagement.Data.DbSeeder.Seed20Employees(dbContext);
            // HRManagement.Data.DbSeeder.SeedAttendanceData(dbContext);
            // HRManagement.Data.DbSeeder.SeedHighRiskEmployee(dbContext);
            // HRManagement.Data.DbSeeder.SeedPayrollForNewEmployees(dbContext);
            // HRManagement.Data.DbSeeder.FixZeroWorkDays(dbContext);
            // HRManagement.Data.DbSeeder.FixNewEmployeePositions(dbContext);
            // HRManagement.Data.DbSeeder.MakeOnly5HighRisk(dbContext);
            // HRManagement.Data.DbSeeder.MakeRiskyInterns(dbContext);
            HRManagement.Data.DbSeeder.AssignAvatars(dbContext);
            HRManagement.Data.DbSeeder.ReorganizeCompanyStructure(dbContext);
            HRManagement.Data.DbSeeder.SeedLeaveManagementData(dbContext);

            // FIX 5 EMPLOYEES TO HIT EXACTLY ~70%
            var targetCodes = new[] { "NV001", "NV002", "NV003", "NV004", "NV005" };
            var targetEmployees = dbContext.Employees
                .Where(e => targetCodes.Contains(e.EmployeeCode))
                .ToList();

            if (targetEmployees.Any())
            {
                var targetDob = DateTime.Now.AddYears(-24);
                var targetHire = DateTime.Now.AddYears(-1);
                var employeeIds = targetEmployees.Select(e => e.EmployeeId).ToList();

                foreach (var emp in targetEmployees)
                {
                    emp.DateOfBirth = targetDob;
                    emp.HireDate = targetHire;
                }

                var contracts = dbContext.Contracts
                    .Where(c => employeeIds.Contains(c.EmployeeId))
                    .ToList();
                
                foreach (var contract in contracts)
                {
                    contract.Salary = 12000000;
                }

                var payrolls = dbContext.Payrolls
                    .Where(p => employeeIds.Contains(p.EmployeeId))
                    .ToList();
                
                foreach (var payroll in payrolls)
                {
                    payroll.BaseSalary = 12000000;
                    payroll.AbsentDays = 1;
                    payroll.LateDays = 2;
                }

                dbContext.SaveChanges();
            }
            
            var modelPath = System.IO.Path.Combine(Environment.CurrentDirectory, "TurnoverModel.zip");
            // File deletion removed to support PredictionEnginePool
        }
        else
        {
            Console.WriteLine("❌ KẾT NỐI THẤT BẠI: KHÔNG TÌM THẤY DATABASE!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ LỖI RỒI: " + ex.Message);
    }
}
// KẾT THÚC ĐOẠN KIỂM TRA

// === Pipeline ===
app.UseMiddleware<HRManagement.Middlewares.GlobalExceptionMiddleware>();
app.UseStaticFiles();
app.UseRouting();

// Thứ tự quan trọng: Authentication trước Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
