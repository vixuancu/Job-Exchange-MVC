# 🚀 Job Exchange MVC - Sàn Giao Dịch Việc Làm Trực Tuyến# 🚀 Job Exchange MVC - Sàn Giao Dịch Việc Làm Trực Tuyến

## 📋 Mục lục## 📋 Mục lục

- [Giới thiệu](#-giới-thiệu)- [Giới thiệu](#-giới-thiệu)

- [Công nghệ sử dụng](#️-công-nghệ-sử-dụng)- [Công nghệ sử dụng](#️-công-nghệ-sử-dụng)

- [Tính năng chính](#-tính-năng-chính)- [Tính năng chính](#-tính-năng-chính)

- [Cài đặt và Chạy dự án](#-cài-đặt-và-chạy-dự-án)- [Cài đặt và Chạy dự án](#-cài-đặt-và-chạy-dự-án)

- [Tài khoản Demo](#-tài-khoản-demo)- [Tài khoản Demo](#-tài-khoản-demo)

- [Troubleshooting](#-troubleshooting)

---

---

## 🎯 Giới thiệu

## 🎯 Giới thiệu

**Job Exchange MVC** là một hệ thống sàn giao dịch việc làm trực tuyến được xây dựng bằng **ASP.NET Core MVC (.NET 9)**, hỗ trợ đầy đủ chức năng Front-End (Razor Views) và Back-End. Hệ thống được thiết kế theo chuẩn **Responsive**, **SEO-friendly**, có **phân quyền người dùng**, **xác thực JWT + Cookie**, xử lý **AJAX**, **Pagination**, và đảm bảo **an toàn bảo mật**.

**Job Exchange MVC** là một hệ thống sàn giao dịch việc làm trực tuyến được xây dựng bằng **ASP.NET Core MVC (.NET 9)**, hỗ trợ đầy đủ chức năng Front-End (Razor Views) và Back-End. Hệ thống được thiết kế theo chuẩn **Responsive**, **SEO-friendly**, có **phân quyền người dùng**, **xác thực JWT + Cookie**, xử lý **AJAX**, **Pagination**, và đảm bảo **an toàn bảo mật**.

### 🎯 Mục đích dự án

### 🎯 Mục đích dự án- Kết nối giữa nhà tuyển dụng và ứng viên

- Kết nối giữa nhà tuyển dụng và ứng viên- Quản lý tin tuyển dụng và đơn ứng tuyển hiệu quả

- Quản lý tin tuyển dụng và đơn ứng tuyển hiệu quả- Cung cấp nền tảng tìm kiếm việc làm thông minh

- Cung cấp nền tảng tìm kiếm việc làm thông minh- Hệ thống quản trị tập trung cho Admin

- Hệ thống quản trị tập trung cho Admin

---

---

## ⚙️ Công nghệ sử dụng

## ⚙️ Công nghệ sử dụng

### Backend

### Backend- **Framework:** ASP.NET Core MVC 9.0

- **Framework:** ASP.NET Core MVC 9.0- **Database:** MySQL 8.0+

- **Database:** MySQL 8.0+- **ORM:** Entity Framework Core 9.0 (Code-First)

- **ORM:** Entity Framework Core 9.0 (Code-First)- **Authentication:** JWT Bearer Token + Cookie Authentication

- **Authentication:** JWT Bearer Token + Cookie Authentication- **Password Hashing:** BCrypt.Net-Next 4.0.3

- **Password Hashing:** BCrypt.Net-Next 4.0.3- **Validation:** FluentValidation 11.3.1

- **Validation:** FluentValidation 11.3.1- **Design Pattern:** Repository Pattern, Service Layer, DTO Pattern

- **Design Pattern:** Repository Pattern, Service Layer, DTO Pattern

### Frontend

### Frontend- **View Engine:** Razor Pages

- **View Engine:** Razor Pages- **CSS Framework:** Bootstrap 5.3

- **CSS Framework:** Bootstrap 5.3- **Icons:** Font Awesome 6.4

- **Icons:** Font Awesome 6.4- **JavaScript:** jQuery 3.7 + AJAX

- **JavaScript:** jQuery 3.7 + AJAX- **Responsive:** Mobile-First Design

- **Responsive:** Mobile-First Design

### DevOps

### DevOps- **Version Control:** Git

- **Version Control:** Git- **Package Manager:** NuGet

- **Package Manager:** NuGet- **Build Tool:** .NET CLI

- **Build Tool:** .NET CLI- **Migration:** EF Core Migrations

- **Migration:** EF Core Migrations

---

---

## ✨ Tính năng chính

## ✨ Tính năng chính

### 👥 Cho Ứng viên (Applicant)

### 👥 Cho Ứng viên (Applicant)- ✅ Đăng ký, đăng nhập tài khoản

- ✅ Đăng ký, đăng nhập tài khoản- ✅ Tìm kiếm việc làm (theo từ khóa, danh mục, địa điểm)

- ✅ Tìm kiếm việc làm (theo từ khóa, danh mục, địa điểm)- ✅ Xem chi tiết tin tuyển dụng

- ✅ Xem chi tiết tin tuyển dụng- ✅ Nộp đơn ứng tuyển (CV + thư xin việc)

- ✅ Nộp đơn ứng tuyển (CV + thư xin việc)- ✅ Quản lý đơn ứng tuyển của mình

- ✅ Quản lý đơn ứng tuyển của mình- ✅ Theo dõi trạng thái đơn ứng tuyển

- ✅ Theo dõi trạng thái đơn ứng tuyển- ✅ Cập nhật hồ sơ cá nhân

- ✅ Cập nhật hồ sơ cá nhân

### 🏢 Cho Nhà tuyển dụng (Employer)

### 🏢 Cho Nhà tuyển dụng (Employer)- ✅ Đăng ký, đăng nhập tài khoản

- ✅ Đăng ký, đăng nhập tài khoản- ✅ Tạo và quản lý hồ sơ công ty

- ✅ Tạo và quản lý hồ sơ công ty- ✅ Đăng tin tuyển dụng

- ✅ Đăng tin tuyển dụng- ✅ Quản lý tin tuyển dụng (Tạo, Sửa, Xóa)

- ✅ Quản lý tin tuyển dụng (Tạo, Sửa, Xóa)- ✅ Xem danh sách ứng viên theo từng tin

- ✅ Xem danh sách ứng viên theo từng tin- ✅ Duyệt/Từ chối hồ sơ ứng viên

- ✅ Duyệt/Từ chối hồ sơ ứng viên- ✅ Quản lý quy trình phỏng vấn (Pending → Approved → Interviewing → Accepted/Rejected)

- ✅ Quản lý quy trình phỏng vấn (Pending → Approved → Interviewing → Accepted/Rejected)- ✅ Thống kê số lượng ứng viên

- ✅ Thống kê số lượng ứng viên

### 👨‍💼 Cho Admin

### 👨‍💼 Cho Admin- ✅ Dashboard thống kê tổng quan

- ✅ Dashboard thống kê tổng quan- ✅ Quản lý người dùng (Khóa/Mở khóa tài khoản)

- ✅ Quản lý người dùng (Khóa/Mở khóa tài khoản)- ✅ Quản lý tin tuyển dụng (Duyệt/Từ chối/Xóa)

- ✅ Quản lý tin tuyển dụng (Duyệt/Từ chối/Xóa)- ✅ Quản lý danh mục nghề nghiệp

- ✅ Quản lý danh mục nghề nghiệp- ✅ Xem báo cáo thống kê chi tiết

- ✅ Xem báo cáo thống kê chi tiết- ✅ Đóng tin tuyển dụng quá hạn tự động

- ✅ Đóng tin tuyển dụng quá hạn tự động

### 🚀 Tính năng kỹ thuật

### 🚀 Tính năng kỹ thuật- ✅ **Phân trang (Pagination):** Tối ưu hiệu suất, tránh load toàn bộ data

- ✅ **Phân trang (Pagination):** Tối ưu hiệu suất, tránh load toàn bộ data- ✅ **AJAX:** Cập nhật trạng thái không reload trang

- ✅ **AJAX:** Cập nhật trạng thái không reload trang- ✅ **Soft Delete:** Job và User không bị xóa vật lý

- ✅ **Soft Delete:** Job và User không bị xóa vật lý- ✅ **Job Status Workflow:** Pending → Approved → Expired → Closed

- ✅ **Job Status Workflow:** Pending → Approved → Expired → Closed- ✅ **Application Status Workflow:** 6 trạng thái rõ ràng

- ✅ **Application Status Workflow:** 6 trạng thái rõ ràng- ✅ **Job View Tracking:** Theo dõi lượt xem tin tuyển dụng

- ✅ **Job View Tracking:** Theo dõi lượt xem tin tuyển dụng- ✅ **Auto Expire Jobs:** Tự động đóng tin quá hạn

- ✅ **Auto Expire Jobs:** Tự động đóng tin quá hạn- ✅ **Responsive Design:** Tương thích mọi thiết bị

- ✅ **Responsive Design:** Tương thích mọi thiết bị

---

---

## 🔧 Cài đặt và Chạy dự án

## 🔧 Cài đặt và Chạy dự án

### 📋 Yêu cầu hệ thống

### 📋 Yêu cầu hệ thống

````bash

```bash- .NET SDK 9.0 trở lên

- .NET SDK 9.0 trở lên- MySQL 8.0 trở lên

- MySQL 8.0 trở lên- Visual Studio 2022 (hoặc VS Code)

- Visual Studio 2022 (hoặc VS Code)- Git

- Git```

````

### 📥 Bước 1: Clone dự án

### 📥 Bước 1: Clone dự án

````bash

```bashgit clone https://github.com/vixuancu/Job-Exchange-MVC.git

git clone https://github.com/vixuancu/Job-Exchange-MVC.gitcd Job-Exchange-MVC

cd JobExchangeMvc```

````

### 🗄️ Bước 2: Cài đặt MySQL

### 🗄️ Bước 2: Cài đặt MySQL

1. **Cài đặt MySQL Server 8.0+**

1. **Cài đặt MySQL Server 8.0+** - Download tại: https://dev.mysql.com/downloads/mysql/

   - Download tại: https://dev.mysql.com/downloads/mysql/ - Hoặc dùng XAMPP/WAMP

   - Hoặc dùng XAMPP/WAMP

1. **Tạo Database**

1. **Tạo Database** ```sql

   ````sql CREATE DATABASE JobExchangeDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

   CREATE DATABASE JobExchangeDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;   ```

   ````

1. **Tạo User (Optional)**

1. **Tạo User (Optional)** ```sql

   ````sql CREATE USER 'jobexchange_user'@'localhost' IDENTIFIED BY 'YourSecurePassword123!';

   CREATE USER 'jobexchange_user'@'localhost' IDENTIFIED BY 'YourSecurePassword123!';   GRANT ALL PRIVILEGES ON JobExchangeDb.* TO 'jobexchange_user'@'localhost';

   GRANT ALL PRIVILEGES ON JobExchangeDb.* TO 'jobexchange_user'@'localhost';   FLUSH PRIVILEGES;

   FLUSH PRIVILEGES;   ```

   ````

### ⚙️ Bước 3: Cấu hình appsettings.json

### ⚙️ Bước 3: Cấu hình appsettings.json

Mở file `appsettings.json` và cập nhật connection string:

Mở file `appsettings.json` và cập nhật connection string:

````json

```json{

{  "ConnectionStrings": {

  "ConnectionStrings": {    "DefaultConnection": "Server=localhost;Port=3306;Database=JobExchangeDb;User=root;Password=123456;"

    "DefaultConnection": "Server=localhost;Port=3306;Database=JobExchangeDb;User=root;Password=123456;"  },

  },  "JwtSettings": {

  "JwtSettings": {    "Secret": "YourSuperSecureSecretKey2025!@#$%^&*()_+JobExchangeMvc",

    "Secret": "YourSuperSecureSecretKey2025!@#$%^&*()_+JobExchangeMvc",    "Issuer": "https://localhost:5001",

    "Issuer": "https://localhost:5001",    "Audience": "https://localhost:5001",

    "Audience": "https://localhost:5001",    "AccessTokenExpirationMinutes": 60,

    "AccessTokenExpirationMinutes": 60,    "RefreshTokenExpirationDays": 30

    "RefreshTokenExpirationDays": 30  }

  }}

}```

````

**Lưu ý:** Thay đổi `User` và `Password` phù hợp với MySQL của bạn.

**Lưu ý:** Thay đổi `User` và `Password` phù hợp với MySQL của bạn.

### 📦 Bước 4: Restore Dependencies

### 📦 Bước 4: Restore Dependencies

````bash

```bashdotnet restore

dotnet restore```

````

### 🗃️ Bước 5: Chạy Migration (Tạo Database Schema)

### 🗃️ Bước 5: Chạy Migration (Tạo Database Schema)

````bash

```bash# Tạo migration (nếu chưa có)

# Cài đặt EF Core tools (nếu chưa có)dotnet ef migrations add InitialCreate

dotnet tool install --global dotnet-ef

# Apply migration vào database

# Apply migration vào databasedotnet ef database update

dotnet ef database update```

````

**Kết quả:** Database `JobExchangeDb` sẽ được tạo với các bảng:

**Kết quả:** Database `JobExchangeDb` sẽ được tạo với các bảng:- Users

- Users- Companies

- Companies- Categories

- Categories- Jobs

- Jobs- Applications

- Applications- RefreshTokens

- RefreshTokens- JobViews

- JobViews

### 🌱 Bước 6: Seed Data (Dữ liệu mẫu)

### 🌱 Bước 6: Seed Data (Dữ liệu mẫu)

Dữ liệu mẫu sẽ tự động được tạo khi chạy ứng dụng lần đầu (xem `Data/DbInitializer.cs`).

Dữ liệu mẫu sẽ tự động được tạo khi chạy ứng dụng lần đầu (xem `Data/DbInitializer.cs`).

Bao gồm:

Bao gồm:- 1 Admin

- 1 Admin- 2 Employer (có công ty)

- 2 Employer (có công ty)- 2 Applicant

- 2 Applicant- 8 Categories

- 8 Categories- 3 Jobs mẫu

- 3 Jobs mẫu

### ▶️ Bước 7: Chạy ứng dụng

### ▶️ Bước 7: Chạy ứng dụng

````bash

```bashdotnet run

dotnet run```

````

Hoặc trong Visual Studio: nhấn `F5` hoặc `Ctrl+F5`

Hoặc trong Visual Studio: nhấn `F5` hoặc `Ctrl+F5`

### 🌐 Bước 8: Truy cập ứng dụng

### 🌐 Bước 8: Truy cập ứng dụng

- **URL:** https://localhost:5001 hoặc http://localhost:5205

- **URL:** https://localhost:5001 hoặc http://localhost:5205- **Swagger (API docs):** Không có (MVC thuần, không dùng API controllers)

- Trang chủ sẽ hiển thị danh sách việc làm

---

## 👤 Tài khoản Demo

Đối với production, thiết lập các environment variables:

Sau khi chạy ứng dụng lần đầu, database sẽ có các tài khoản demo:

````bash

### Adminexport ConnectionStrings__DefaultConnection="Server=production-server;..."

- **Email:** admin@jobexchange.comexport JwtSettings__Secret="ProductionSecretKey..."

- **Password:** Admin@123```

- **Quyền:** Toàn quyền quản trị

## 🛠️ Troubleshooting

### Employer 1

- **Email:** employer1@company.com### Lỗi kết nối database

- **Password:** Employer@123

- **Công ty:** Công ty TNHH Công nghệ ABC```bash

# Kiểm tra MySQL service

### Employer 2mysqladmin -u root -p status

- **Email:** employer2@company.com

- **Password:** Employer@123# Test connection string

- **Công ty:** Công ty Cổ phần XYZmysql -h localhost -P 3306 -u root -p

````

### Applicant 1

- **Email:** applicant1@example.com### Lỗi migration

- **Password:** Applicant@123

- **Skills:** C#, ASP.NET Core, React, MySQL```bash

# Drop database và tạo lại

### Applicant 2dotnet ef database drop

- **Email:** applicant2@example.comdotnet ef migrations remove

- **Password:** Applicant@123dotnet ef migrations add InitialCreate

- **Skills:** React, Vue.js, TypeScript, HTML/CSSdotnet ef database update

````

---

### Lỗi JWT

## 🐛 Troubleshooting

- Kiểm tra Secret key có đủ độ dài (>= 32 chars)

### Lỗi kết nối MySQL- Kiểm tra Issuer và Audience trong appsettings.json

```- Xóa cookies và login lại

Error: Unable to connect to database

```## 📚 Documentation

**Giải pháp:**

1. Kiểm tra MySQL service đang chạy### API Endpoints (nếu cần)

   ```bash

   # Windows- POST `/api/auth/register` - Đăng ký

   net start MySQL80- POST `/api/auth/login` - Đăng nhập

   - POST `/api/auth/refresh` - Refresh token

   # Linux- GET `/api/jobs` - Lấy danh sách việc làm

   sudo systemctl start mysql- POST `/api/applications` - Nộp đơn ứng tuyển

````

2. Xác nhận connection string trong `appsettings.json`### View Routes

3. Kiểm tra user/password MySQL

4. Test connection:- `/` - Trang chủ

   ```bash- `/Account/Login` - Đăng nhập

   mysql -h localhost -u root -p- `/Account/Register` - Đăng ký

   ```- `/Jobs` - Danh sách việc làm

- `/Jobs/Details/{id}` - Chi tiết việc làm

### Lỗi Migration- `/Applications/MyApplications` - Đơn ứng tuyển của tôi

```- `/Admin/Dashboard` - Trang quản trị

Error: A connection was successfully established, but then an error occurred

````## 🤝 Contributing

**Giải pháp:**

```bashContributions are welcome! Please feel free to submit a Pull Request.

# Xóa database cũ

dotnet ef database drop## 📄 License



# Tạo lại từ migrationThis project is licensed under the MIT License.

dotnet ef database update

```## 👨‍💻 Author



### Lỗi JWT Token- **Your Name**

```- **Email:** your.email@example.com

Error: IDX10223: Unable to validate signature- **GitHub:** https://github.com/yourusername

````

**Giải pháp:**## 🙏 Acknowledgments

- Kiểm tra `JwtSettings:Secret` trong `appsettings.json`

- Secret phải dài >= 32 ký tự- ASP.NET Core Documentation

- Entity Framework Core Documentation

### Lỗi Port đã được sử dụng- Bootstrap Documentation

```bash- Font Awesome Icons

# Thay đổi port trong Properties/launchSettings.json

"applicationUrl": "https://localhost:5002;http://localhost:5003"---

```

**Note:** Đây là project demo cho mục đích học tập. Đối với production, cần bổ sung thêm các tính năng như email confirmation, forgot password, advanced logging, monitoring, etc.

---

## 📚 Tài liệu tham khảo

- **ASP.NET Core:** https://learn.microsoft.com/aspnet/core/
- **Entity Framework Core:** https://learn.microsoft.com/ef/core/
- **MySQL:** https://dev.mysql.com/doc/
- **Bootstrap 5:** https://getbootstrap.com/docs/5.3/
- **jQuery:** https://api.jquery.com/

---

## 📝 License

This project is licensed under the MIT License.

---

## 👨‍💻 Author

**Vi Xuân Cử**

- GitHub: [@vixuancu](https://github.com/vixuancu)
- Repository: [Job-Exchange-MVC](https://github.com/vixuancu/Job-Exchange-MVC)

---

## 🙏 Acknowledgments

- ASP.NET Core Team
- Entity Framework Team
- Bootstrap Team
- Font Awesome Team
- Open Source Community

---

**⭐ Nếu bạn thấy dự án hữu ích, hãy cho một Star nhé! ⭐**
