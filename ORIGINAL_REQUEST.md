# Original User Request

## Initial Request — 2026-06-04T14:59:02Z

# Teamwork Project Prompt

Review the entire HRManagement ASP.NET Core project, identify bugs, code smells, architectural flaws, or UX/UI issues, and propose concrete solutions to fix them.

Working directory: C:\Users\Admin\Documents\HRManagement\HRManagement
Integrity mode: development

## Requirements

### R1. Comprehensive Codebase Audit
Perform a thorough review of the Controllers, Services, Repositories, Views, and Models to identify bad practices, security vulnerabilities, or performance bottlenecks.

### R2. Read-Only Review
Do NOT modify any source code files. Your task is strictly to analyze the codebase and generate a report.

### R3. Actionable Proposals
For every issue found, provide a concrete, actionable solution or refactoring proposal (e.g., code snippets, architecture diagrams) rather than just pointing out the flaw.

## Acceptance Criteria

### Audit Report
- [ ] The team produces a detailed markdown report (`audit_report.md` in the workspace root) categorizing issues by severity and module.
- [ ] Each issue includes a code snippet showing the problem and a proposed code change to fix it.
- [ ] The report covers at least 3 distinct areas (e.g., Security, Performance, Code Quality).

## Follow-up — 2026-06-04T14:59:32Z

Please write the audit report (`audit_report.md`) entirely in Vietnamese. (Vui lòng viết toàn bộ nội dung báo cáo bằng tiếng Việt).

## Follow-up — 

# Teamwork Project Prompt — Draft

> Status: Launched
> Goal: Team AI is currently auditing the backend codebase.

Thực hiện rà soát (review) toàn diện mã nguồn Backend của dự án HRManagement để phát hiện các vấn đề tiềm ẩn và xuất báo cáo. Yêu cầu chỉ đọc, KHÔNG sửa code.

Working directory: C:\Users\Admin\Documents\HRManagement\HRManagement
Integrity mode: development

## Requirements

### R1. Read-Only Backend Audit
Tiến hành rà soát toàn bộ các file .cs thuộc về Backend bao gồm Controllers/, Services/, Repositories/, Models/, và cấu hình Program.cs. Tuyệt đối KHÔNG được chỉnh sửa, ghi đè hay xóa bất kỳ file mã nguồn nào.

### R2. Comprehensive Focus Areas
Tập trung tìm kiếm các lỗi thuộc về hai khía cạnh:
- Hiệu năng & Cơ sở dữ liệu: Lỗi truy vấn N+1, thiếu AsNoTracking khi đọc dữ liệu, lạm dụng tải toàn bộ bảng (Load all).
- Kiến trúc & Logic: Các vi phạm nguyên tắc SOLID, logic rườm rà ở Controller thay vì Service, thiếu xử lý ngoại lệ (Exception Handling), hoặc mã cứng (hardcode).

### R3. Actionable Report Generation
Tạo ra một file báo cáo tên là ackend_audit_report.md lưu tại thư mục gốc của dự án. Trong báo cáo, liệt kê rõ các file bị lỗi, mô tả chi tiết nguyên nhân, và cung cấp đoạn mã C# đề xuất để sửa (dạng Before/After).

## Acceptance Criteria

### Verification
- [ ] File ackend_audit_report.md được tạo thành công ở thư mục C:\Users\Admin\Documents\HRManagement\HRManagement.
- [ ] Báo cáo phải chỉ ra được ít nhất 5 vấn đề cụ thể về Backend kèm theo mã nguồn C# (code snippet) đề xuất sửa chữa.
- [ ] Lệnh git status (nếu có) không phát hiện bất kỳ file .cs nào bị thay đổi. (Đảm bảo tính chất "Chỉ đọc").

## Follow-up — 2026-06-13T15:50:00Z

# Teamwork Project Prompt — Draft

> Status: Launched
> Goal: Team AI is currently auditing the backend codebase.

Thực hiện rà soát (review) toàn diện mã nguồn Backend của dự án HRManagement để phát hiện các vấn đề tiềm ẩn và xuất báo cáo. Yêu cầu chỉ đọc, KHÔNG sửa code.

Working directory: C:\Users\Admin\Documents\HRManagement\HRManagement
Integrity mode: development

## Requirements

### R1. Read-Only Backend Audit
Tiến hành rà soát toàn bộ các file .cs thuộc về Backend bao gồm Controllers/, Services/, Repositories/, Models/, và cấu hình Program.cs. Tuyệt đối KHÔNG được chỉnh sửa, ghi đè hay xóa bất kỳ file mã nguồn nào.

### R2. Comprehensive Focus Areas
Tập trung tìm kiếm các lỗi thuộc về hai khía cạnh:
- Hiệu năng & Cơ sở dữ liệu: Lỗi truy vấn N+1, thiếu AsNoTracking khi đọc dữ liệu, lạm dụng tải toàn bộ bảng (Load all).
- Kiến trúc & Logic: Các vi phạm nguyên tắc SOLID, logic rườm rà ở Controller thay vì Service, thiếu xử lý ngoại lệ (Exception Handling), hoặc mã cứng (hardcode).

### R3. Actionable Report Generation
Tạo ra một file báo cáo tên là ackend_audit_report.md lưu tại thư mục gốc của dự án. Trong báo cáo, liệt kê rõ các file bị lỗi, mô tả chi tiết nguyên nhân, và cung cấp đoạn mã C# đề xuất để sửa (dạng Before/After).

## Acceptance Criteria

### Verification
- [ ] File ackend_audit_report.md được tạo thành công ở thư mục C:\Users\Admin\Documents\HRManagement\HRManagement.
- [ ] Báo cáo phải chỉ ra được ít nhất 5 vấn đề cụ thể về Backend kèm theo mã nguồn C# (code snippet) đề xuất sửa chữa.
- [ ] Lệnh git status (nếu có) không phát hiện bất kỳ file .cs nào bị thay đổi. (Đảm bảo tính chất "Chỉ đọc").

## Follow-up — 2026-06-13T16:16:31Z

# Teamwork Project Prompt — Draft

> Status: Launched
> Goal: Craft prompt → get user approval → delegate to teamwork_preview

Thực hiện rà soát toàn bộ dự án HRManagement (cả Frontend và Backend) để phát hiện các lỗi tiềm ẩn (hidden bugs) và lập một bản báo cáo chi tiết. Tuyệt đối KHÔNG sửa đổi mã nguồn.

Working directory: C:\Users\Admin\Documents\HRManagement\HRManagement
Integrity mode: development

## Requirements

### R1. Rà soát Backend
Phân tích toàn diện Controllers, Services, Repositories và Models để tìm kiếm các lỗi tiềm ẩn như: thiếu kiểm tra null, rủi ro đa luồng (concurrency), hổng logic nghiệp vụ, hoặc lỗi xử lý ngoại lệ.

### R2. Rà soát Frontend
Kiểm tra hệ thống Views (Razor/HTML), CSS và JavaScript để phát hiện các lỗi như: biến UI không được xử lý, lỗi render sai trạng thái, thẻ HTML/JS hỏng, hoặc gọi API/Action không tồn tại.

### R3. Báo cáo đánh giá (Report)
Tạo ra một bản báo cáo Markdown liệt kê toàn bộ các lỗi tiềm ẩn đã phát hiện. Báo cáo cần bao gồm: Tên file, vị trí dòng code, mô tả lỗi, mức độ nghiêm trọng và đề xuất cách khắc phục. Tuyệt đối không thực hiện bất kỳ thay đổi nào lên mã nguồn (Read-only).

## Acceptance Criteria

### Chất lượng Báo cáo
- [ ] Báo cáo phải chỉ ra được ít nhất 3-5 lỗi tiềm ẩn cụ thể (nếu có) trong dự án.
- [ ] Mỗi lỗi được báo cáo phải trỏ chính xác đến Tên file và Phương thức / Dòng code cụ thể.
- [ ] Lỗi phải là vấn đề logic/nghiệp vụ thực sự, không phải chỉ là cảnh báo (warning) lặt vặt của code convention.

### Tuân thủ quy định
- [ ] Tuyệt đối KHÔNG có bất kỳ file mã nguồn `.cs`, `.cshtml`, `.js`, `.css` nào bị chỉnh sửa, ghi đè hay xóa bỏ trong quá trình thực hiện.
