using JobExchangeMvc.Models;
using JobExchangeMvc.Helpers;

namespace JobExchangeMvc.Data;

/// <summary>
/// Khởi tạo dữ liệu mẫu cho database
/// </summary>
public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (context.Users.Any())
        {
            return; // DB has been seeded
        }

        // Create Categories
        var categories = new[]
        {
            new Category { Name = "Công nghệ thông tin", Description = "Lập trình, phát triển phần mềm, IT", IsActive = true },
            new Category { Name = "Kinh doanh", Description = "Bán hàng, marketing, quản lý", IsActive = true },
            new Category { Name = "Kế toán - Tài chính", Description = "Kế toán, kiểm toán, tài chính", IsActive = true },
            new Category { Name = "Giáo dục - Đào tạo", Description = "Giảng dạy, đào tạo", IsActive = true },
            new Category { Name = "Y tế - Chăm sóc sức khỏe", Description = "Bác sĩ, y tá, dược sĩ", IsActive = true },
            new Category { Name = "Xây dựng", Description = "Kiến trúc, xây dựng, nội thất", IsActive = true },
            new Category { Name = "Du lịch - Khách sạn", Description = "Du lịch, nhà hàng, khách sạn", IsActive = true },
            new Category { Name = "Dịch vụ khách hàng", Description = "Chăm sóc khách hàng, tư vấn", IsActive = true }
        };
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Create Admin User
        var adminUser = new User
        {
            Email = "admin@jobexchange.com",
            PasswordHash = PasswordHasher.HashPassword("Admin@123"),
            FullName = "Quản trị viên",
            PhoneNumber = "0900000000",
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        // Create Employer Users
        var employer1 = new User
        {
            Email = "employer1@company.com",
            PasswordHash = PasswordHasher.HashPassword("Employer@123"),
            FullName = "Nguyễn Văn A",
            PhoneNumber = "0901111111",
            Role = "Employer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var employer2 = new User
        {
            Email = "employer2@company.com",
            PasswordHash = PasswordHasher.HashPassword("Employer@123"),
            FullName = "Trần Thị B",
            PhoneNumber = "0902222222",
            Role = "Employer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddRangeAsync(employer1, employer2);
        await context.SaveChangesAsync();

        // Create Companies
        var company1 = new Company
        {
            Name = "Công ty TNHH Công nghệ ABC",
            Description = "Chuyên phát triển phần mềm và giải pháp CNTT",
            Website = "https://abc-tech.com",
            Address = "123 Nguyễn Huệ",
            City = "Hồ Chí Minh",
            EmployerId = employer1.Id,
            CreatedAt = DateTime.UtcNow
        };

        var company2 = new Company
        {
            Name = "Công ty Cổ phần XYZ",
            Description = "Chuyên về tư vấn và đào tạo doanh nghiệp",
            Website = "https://xyz-corp.com",
            Address = "456 Lê Lợi",
            City = "Hà Nội",
            EmployerId = employer2.Id,
            CreatedAt = DateTime.UtcNow
        };

        await context.Companies.AddRangeAsync(company1, company2);
        await context.SaveChangesAsync();

        // Create Jobs
        var jobs = new[]
        {
            new Job
            {
                Title = "Full Stack Developer (.NET + React)",
                Description = "Chúng tôi đang tìm kiếm Full Stack Developer có kinh nghiệm với .NET Core và React để phát triển các ứng dụng web quy mô lớn.",
                Requirements = "- 2+ năm kinh nghiệm với ASP.NET Core\n- Thành thạo React, TypeScript\n- Kinh nghiệm với MySQL/PostgreSQL\n- Kiến thức về Docker, CI/CD",
                Benefits = "- Lương: 20-30 triệu VNĐ\n- Thưởng theo dự án\n- Bảo hiểm đầy đủ\n- Làm việc với công nghệ mới nhất",
                SalaryRange = "20-30 triệu VNĐ",
                Location = "Hồ Chí Minh",
                JobType = "Full-time",
                NumberOfPositions = 3,
                ApplicationDeadline = DateTime.UtcNow.AddMonths(1),
                Status = "Approved",
                CompanyId = company1.Id,
                CategoryId = categories[0].Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Job
            {
                Title = "Frontend Developer (React/Vue)",
                Description = "Tuyển dụng Frontend Developer tham gia dự án phát triển nền tảng thương mại điện tử.",
                Requirements = "- 1+ năm kinh nghiệm với React hoặc Vue\n- Thành thạo HTML5, CSS3, JavaScript ES6+\n- Hiểu biết về Responsive Design\n- Có kinh nghiệm với Git",
                Benefits = "- Lương: 15-25 triệu VNĐ\n- Review lương 6 tháng/lần\n- Team building, du lịch hàng năm",
                SalaryRange = "15-25 triệu VNĐ",
                Location = "Hà Nội",
                JobType = "Full-time",
                NumberOfPositions = 2,
                ApplicationDeadline = DateTime.UtcNow.AddMonths(1),
                Status = "Approved",
                CompanyId = company2.Id,
                CategoryId = categories[0].Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Job
            {
                Title = "Business Analyst",
                Description = "Tuyển dụng Business Analyst phân tích yêu cầu và tư vấn giải pháp cho khách hàng.",
                Requirements = "- Tốt nghiệp Đại học chuyên ngành liên quan\n- Kỹ năng phân tích, tổng hợp thông tin tốt\n- Tiếng Anh giao tiếp",
                Benefits = "- Lương: 12-18 triệu VNĐ\n- Được đào tạo về phân tích nghiệp vụ\n- Cơ hội thăng tiến",
                SalaryRange = "12-18 triệu VNĐ",
                Location = "Hồ Chí Minh",
                JobType = "Full-time",
                NumberOfPositions = 1,
                ApplicationDeadline = DateTime.UtcNow.AddDays(20),
                Status = "Approved",
                CompanyId = company1.Id,
                CategoryId = categories[1].Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Jobs.AddRangeAsync(jobs);
        await context.SaveChangesAsync();

        // Create Applicant Users
        var applicant1 = new User
        {
            Email = "applicant1@example.com",
            PasswordHash = PasswordHasher.HashPassword("Applicant@123"),
            FullName = "Lê Văn C",
            PhoneNumber = "0903333333",
            Skills = "C#, ASP.NET Core, React, MySQL",
            Bio = "Full Stack Developer với 3 năm kinh nghiệm",
            Role = "Applicant",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var applicant2 = new User
        {
            Email = "applicant2@example.com",
            PasswordHash = PasswordHasher.HashPassword("Applicant@123"),
            FullName = "Phạm Thị D",
            PhoneNumber = "0904444444",
            Skills = "React, Vue.js, TypeScript, HTML/CSS",
            Bio = "Frontend Developer yêu thích công nghệ",
            Role = "Applicant",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddRangeAsync(applicant1, applicant2);
        await context.SaveChangesAsync();

        Console.WriteLine("Database seeded successfully!");
    }
}
