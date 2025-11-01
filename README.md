# 🚀 Job Exchange MVC - Sàn Giao Dịch Việc Làm Trực Tuyến# 🚀 Job Exchange MVC - Sàn Giao Dịch Việc Làm Trực Tuyến

## 📋 Mục lục## 📋 Mục lục

- [Giới thiệu](#-giới-thiệu)- [Giới thiệu](#-giới-thiệu)

- [Công nghệ sử dụng](#️-công-nghệ-sử-dụng)- [Công nghệ sử dụng](#️-công-nghệ-sử-dụng)

- [Tính năng chính](#-tính-năng-chính)- [Tính năng chính](#-tính-năng-chính)

- [Cài đặt và Chạy dự án](#-cài-đặt-và-chạy-dự-án)- [Cài đặt và Chạy dự án](#-cài-đặt-và-chạy-dự-án)

- [Cấu trúc dự án](#-cấu-trúc-dự-án)- [Cấu trúc dự án](#-cấu-trúc-dự-án)

- [Luồng nghiệp vụ](#-luồng-nghiệp-vụ)- [Luồng nghiệp vụ](#-luồng-nghiệp-vụ)

- [Routes & Endpoints](#-routes--endpoints)- [API Endpoints](#-api-endpoints)

- [Database Schema](#️-database-schema)- [Database Schema](#-database-schema)

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

---

## 📁 Cấu trúc dự án

## 📁 Cấu trúc dự án

````

```JobExchangeMvc/

JobExchangeMvc/├── Controllers/             # MVC Controllers

├── Controllers/             # MVC Controllers│   ├── AccountController.cs    # Đăng ký, Đăng nhập, Profile

│   ├── AccountController.cs    # Đăng ký, Đăng nhập, Profile│   ├── JobsController.cs       # Quản lý tin tuyển dụng

│   ├── JobsController.cs       # Quản lý tin tuyển dụng│   ├── ApplicationsController.cs # Quản lý đơn ứng tuyển

│   ├── ApplicationsController.cs # Quản lý đơn ứng tuyển│   ├── AdminController.cs      # Quản trị hệ thống

│   ├── AdminController.cs      # Quản trị hệ thống│   └── HomeController.cs       # Trang chủ

│   └── HomeController.cs       # Trang chủ│

│├── Models/                  # Domain Models (Entities)

├── Models/                  # Domain Models (Entities)│   ├── User.cs                 # Người dùng (Admin/Employer/Applicant)

│   ├── User.cs                 # Người dùng (Admin/Employer/Applicant)│   ├── Company.cs              # Công ty (của Employer)

│   ├── Company.cs              # Công ty (của Employer)│   ├── Job.cs                  # Tin tuyển dụng

│   ├── Job.cs                  # Tin tuyển dụng│   ├── Application.cs          # Đơn ứng tuyển

│   ├── Application.cs          # Đơn ứng tuyển│   ├── Category.cs             # Danh mục nghề nghiệp

│   ├── Category.cs             # Danh mục nghề nghiệp│   ├── RefreshToken.cs         # JWT Refresh Token

│   ├── RefreshToken.cs         # JWT Refresh Token│   └── JobView.cs              # Lượt xem tin tuyển dụng

│   └── JobView.cs              # Lượt xem tin tuyển dụng│

│├── DTOs/                    # Data Transfer Objects

├── DTOs/                    # Data Transfer Objects│   ├── LoginDto.cs

│   ├── LoginDto.cs│   ├── RegisterDto.cs

│   ├── RegisterDto.cs│   ├── ProfileDto.cs

│   ├── ProfileDto.cs│   ├── JobDto.cs

│   ├── JobDto.cs│   ├── ApplicationDto.cs

│   ├── ApplicationDto.cs│   ├── PagedResultDto.cs       # Pagination wrapper

│   ├── PagedResultDto.cs       # Pagination wrapper│   └── AdminDashboardStatsDto.cs

│   └── AdminDashboardStatsDto.cs│

│├── Services/                # Business Logic Layer

├── Services/                # Business Logic Layer│   ├── Interfaces/

│   ├── Interfaces/│   │   ├── IAuthService.cs

│   │   ├── IAuthService.cs│   │   ├── IUserService.cs

│   │   ├── IUserService.cs│   │   ├── IJobService.cs

│   │   ├── IJobService.cs│   │   └── IApplicationService.cs

│   │   └── IApplicationService.cs│   └── Implementations/

│   └── Implementations/│       ├── AuthService.cs

│       ├── AuthService.cs│       ├── UserService.cs

│       ├── UserService.cs│       ├── JobService.cs

│       ├── JobService.cs│       └── ApplicationService.cs

│       └── ApplicationService.cs│

│├── Data/                    # Database Context & Migrations

├── Data/                    # Database Context & Migrations│   ├── ApplicationDbContext.cs

│   ├── ApplicationDbContext.cs│   ├── DbInitializer.cs        # Seed data

│   ├── DbInitializer.cs        # Seed data│   └── Migrations/

│   └── Migrations/│       ├── 20251005090836_InitialCreate.cs

│       ├── 20251005090836_InitialCreate.cs│       └── 20251007091426_AddJobViewTracking.cs

│       └── 20251007091426_AddJobViewTracking.cs│

│├── Helpers/                 # Utility Classes

├── Helpers/                 # Utility Classes│   ├── JwtTokenHelper.cs       # JWT token generation

│   ├── JwtTokenHelper.cs       # JWT token generation│   ├── PasswordHasher.cs       # BCrypt password hashing

│   ├── PasswordHasher.cs       # BCrypt password hashing│   └── ServiceCollectionExtensions.cs # DI extensions

│   └── ServiceCollectionExtensions.cs # DI extensions│

│├── Views/                   # Razor Views (HTML)

├── Views/                   # Razor Views (HTML)│   ├── Account/                # Login, Register, Profile

│   ├── Account/                # Login, Register, Profile│   ├── Jobs/                   # Index, Details, Create, Edit, MyJobs, Applicants

│   ├── Jobs/                   # Index, Details, Create, Edit, MyJobs, Applicants│   ├── Applications/           # MyApplications, Apply, Details

│   ├── Applications/           # MyApplications, Apply, Details│   ├── Admin/                  # Dashboard, Users, Jobs, Categories, Statistics

│   ├── Admin/                  # Dashboard, Users, Jobs, Categories, Statistics│   ├── Home/                   # Index, About, Contact, Privacy

│   ├── Home/                   # Index, About, Contact, Privacy│   └── Shared/

│   └── Shared/│       ├── _Layout.cshtml      # Master layout

│       ├── _Layout.cshtml      # Master layout│       └── _PaginationPartial.cshtml # Pagination component

│       └── _PaginationPartial.cshtml # Pagination component│

│├── wwwroot/                 # Static files

├── wwwroot/                 # Static files│   ├── css/

│   ├── css/│   │   └── site.css

│   │   └── site.css│   ├── js/

│   ├── js/│   │   ├── site.js

│   │   ├── site.js│   │   └── app.js              # Custom JavaScript + AJAX

│   │   └── app.js              # Custom JavaScript + AJAX│   ├── lib/                    # Bootstrap, jQuery, Font Awesome

│   ├── lib/                    # Bootstrap, jQuery, Font Awesome│   └── uploads/                # User uploads (CV, avatars)

│   └── uploads/                # User uploads (CV, avatars)│

│├── appsettings.json         # Configuration

├── appsettings.json         # Configuration├── Program.cs               # Application entry point

├── Program.cs               # Application entry point└── JobExchangeMvc.csproj    # Project file

└── JobExchangeMvc.csproj    # Project file│   │   └── IAuthService.cs

```│   └── Implementations/

│       ├── UserService.cs

---│       ├── JobService.cs

│       ├── ApplicationService.cs

## 🔄 Luồng nghiệp vụ│       └── AuthService.cs

│

### 1️⃣ Quy trình Đăng ký & Đăng nhập├── Data/                    # Database context và migrations

│   ├── ApplicationDbContext.cs

```│   ├── DbInitializer.cs

User → Chọn vai trò (Applicant/Employer)│   └── Migrations/

     ↓│

Đăng ký tài khoản├── DTOs/                    # Data Transfer Objects

     ↓│   ├── RegisterDto.cs

- Applicant: Tạo User record│   ├── LoginDto.cs

- Employer: Tạo User + Company record│   ├── JobDto.cs

     ↓│   ├── ApplicationDto.cs

Đăng nhập → JWT Token + Cookie│   └── ProfileDto.cs

     ↓│

Truy cập hệ thống theo role├── Helpers/                 # Helper classes

```│   ├── JwtTokenHelper.cs

│   ├── PasswordHasher.cs

**Chi tiết:**│   └── ServiceCollectionExtensions.cs

1. User chọn role (Applicant/Employer) khi đăng ký│

2. Server hash password bằng BCrypt├── Views/                   # Razor views

3. Tạo User record trong database│   ├── Shared/

4. Employer tự động tạo Company record (1-1 relationship)│   ├── Home/

5. Login thành công → Server tạo JWT token + Refresh token│   ├── Account/

6. Cookie authentication cho MVC views│   ├── Jobs/

7. JWT token cho API calls (nếu có)│   ├── Applications/

│   └── Admin/

---│

├── wwwroot/                 # Static files

### 2️⃣ Quy trình Đăng tin tuyển dụng (Employer)│   ├── css/

│   ├── js/

```│   └── uploads/

Employer tạo Job → Status = "Pending"│

                          ↓├── appsettings.json         # Configuration

                    Admin duyệt├── Program.cs               # Application entry point

                          ↓└── README.md

              Status = "Approved" → Hiển thị công khai```

                          ↓

              ApplicationDeadline qua → Auto expire## 🚀 Hướng dẫn cài đặt

                          ↓

              Status = "Expired" hoặc "Closed"### 1. Yêu cầu hệ thống

````

- .NET 9 SDK

**Job Status Workflow:**- MySQL Server 8.0+

- `Pending`: Tin mới đăng, chờ Admin duyệt- Visual Studio 2022 hoặc VS Code

- `Approved`: Admin đã duyệt, tin hiển thị công khai- Git

- `Rejected`: Admin từ chối tin

- `Expired`: Tin quá hạn (ApplicationDeadline)### 2. Clone và cài đặt packages

- `Closed`: Employer tự đóng tin

````bash

**Business Rules:**# Clone project (nếu có git repository)

- Employer phải có Company profile trước khi đăng tingit clone <repository-url>

- Admin phải duyệt tin trước khi hiển thịcd JobExchangeMvc

- Tin quá hạn tự động chuyển sang "Expired" (lazy check hoặc batch job)

- Xóa tin = Soft delete (IsActive = false, Status = "Closed")# Restore packages

dotnet restore

---```



### 3️⃣ Quy trình Ứng tuyển (Applicant)### 3. Cấu hình Database



```**Bước 1:** Tạo database MySQL

Applicant nộp đơn → Status = "Pending"

                           ↓```sql

                  Employer xem xétCREATE DATABASE JobExchangeDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

                           ↓```

        ┌──────────────────┴──────────────────┐

        ↓                                     ↓**Bước 2:** Cập nhật connection string trong `appsettings.json`

Status = "Approved"                   Status = "Rejected"

        ↓                                     ↓```json

Status = "Interviewing"                  [KẾT THÚC]{

        ↓  "ConnectionStrings": {

   ┌────┴────┐    "DefaultConnection": "Server=localhost;Port=3306;Database=JobExchangeDb;User=root;Password=YOUR_PASSWORD;"

   ↓         ↓  }

"Accepted" "Rejected"}

   ↓         ↓```

[TUYỂN]  [KHÔNG TUYỂN]

```**Lưu ý:** Thay `YOUR_PASSWORD` bằng mật khẩu MySQL của bạn.



**Application Status Workflow:**### 4. Chạy Migration

- `Pending`: Đơn mới nộp, chờ Employer xem xét

- `Approved`: Employer duyệt hồ sơ ban đầu```bash

- `Interviewing`: Employer mời phỏng vấn# Cài đặt EF Core tools (nếu chưa có)

- `Accepted`: ✅ Employer tuyển dụng (KẾT QUẢ CUỐI - THÀNH CÔNG)dotnet tool install --global dotnet-ef

- `Rejected`: ❌ Employer không tuyển (KẾT QUẢ CUỐI - THẤT BẠI)

- `Cancelled`: Applicant tự hủy đơn# Tạo migration

dotnet ef migrations add InitialCreate -o Data/Migrations

**Business Rules:**

- Applicant chỉ ứng tuyển 1 lần cho mỗi Job# Cập nhật database

- Không thể ứng tuyển Job đã Expired/Closeddotnet ef database update

- Applicant có thể hủy đơn khi Status = "Pending"```

- Employer có thể reject ở bất kỳ giai đoạn nào

- Cascade: Job xóa → Applications chuyển sang "Rejected" (nếu Pending)### 5. Chạy ứng dụng



---```bash

dotnet run

### 4️⃣ Quy trình Quản trị (Admin)```



```Ứng dụng sẽ chạy tại: `https://localhost:5001` hoặc `http://localhost:5000`

Admin Dashboard

     ↓## 👥 Tài khoản mặc định

┌────┴────┬────────┬──────────┐

↓         ↓        ↓          ↓Sau khi chạy migration, hệ thống tự động tạo các tài khoản mẫu:

Users    Jobs   Categories Statistics

↓         ↓        ↓          ↓### Admin

- Khóa/Mở - Duyệt  - CRUD     - Tổng quan

- Xóa     - Từ chối - Toggle  - Báo cáo- **Email:** admin@jobexchange.com

          - Xóa vĩnh viễn      - Charts- **Password:** Admin@123

```- **Vai trò:** Quản trị viên hệ thống



**Admin Capabilities:**### Employer 1

- **Users:** Lock/Unlock account, Hard delete user

- **Jobs:** Approve/Reject job, Hard delete job, Auto expire batch- **Email:** employer1@company.com

- **Categories:** Create/Update/Toggle status- **Password:** Employer@123

- **Statistics:** User by role, Job by status, Application metrics- **Vai trò:** Nhà tuyển dụng

- **Công ty:** Công ty TNHH Công nghệ ABC

---

### Employer 2

## 🌐 Routes & Endpoints

- **Email:** employer2@company.com

### 🏠 Public Routes (Không cần đăng nhập)- **Password:** Employer@123

- **Vai trò:** Nhà tuyển dụng

| Method | Route | Controller | Action | Mô tả |- **Công ty:** Công ty Cổ phần XYZ

|--------|-------|------------|--------|-------|

| GET | `/` | Home | Index | Trang chủ |### Applicant 1

| GET | `/Jobs` | Jobs | Index | Danh sách việc làm |

| GET | `/Jobs/Details/{id}` | Jobs | Details | Chi tiết tin tuyển dụng |- **Email:** applicant1@example.com

| GET | `/Account/Login` | Account | Login | Trang đăng nhập |- **Password:** Applicant@123

| POST | `/Account/Login` | Account | Login | Xử lý đăng nhập |- **Vai trò:** Ứng viên

| GET | `/Account/Register` | Account | Register | Trang đăng ký |

| POST | `/Account/Register` | Account | Register | Xử lý đăng ký |### Applicant 2



### 👤 Applicant Routes (Role: Applicant)- **Email:** applicant2@example.com

- **Password:** Applicant@123

| Method | Route | Controller | Action | Mô tả |- **Vai trò:** Ứng viên

|--------|-------|------------|--------|-------|

| GET | `/Applications/MyApplications` | Applications | MyApplications | Đơn của tôi (paginated) |## 🎨 Tính năng chính

| GET | `/Applications/Apply/{jobId}` | Applications | Apply | Form ứng tuyển |

| POST | `/Applications/Apply` | Applications | Apply | Nộp đơn ứng tuyển |### 👤 Người dùng (User)

| GET | `/Applications/Details/{id}` | Applications | Details | Chi tiết đơn ứng tuyển |

| POST | `/Applications/Cancel/{id}` | Applications | Cancel | Hủy đơn (AJAX) |#### Applicant (Ứng viên)

| GET | `/Account/Profile` | Account | Profile | Hồ sơ cá nhân |

| POST | `/Account/Profile` | Account | Profile | Cập nhật hồ sơ |- ✅ Đăng ký, đăng nhập, đăng xuất

- ✅ Quản lý hồ sơ cá nhân (họ tên, kỹ năng, ảnh đại diện, CV)

### 🏢 Employer Routes (Role: Employer)- ✅ Tìm kiếm việc làm theo từ khóa, ngành nghề, địa điểm

- ✅ Xem chi tiết công việc

| Method | Route | Controller | Action | Mô tả |- ✅ Nộp đơn ứng tuyển

|--------|-------|------------|--------|-------|- ✅ Xem lịch sử ứng tuyển

| GET | `/Jobs/MyJobs` | Jobs | MyJobs | Tin của tôi (paginated) |- ✅ Hủy đơn ứng tuyển (khi đang pending)

| GET | `/Jobs/Create` | Jobs | Create | Form đăng tin |

| POST | `/Jobs/Create` | Jobs | Create | Tạo tin mới |#### Employer (Nhà tuyển dụng)

| GET | `/Jobs/Edit/{id}` | Jobs | Edit | Form sửa tin |

| POST | `/Jobs/Edit/{id}` | Jobs | Edit | Cập nhật tin |- ✅ Đăng ký, đăng nhập, đăng xuất

| POST | `/Jobs/Delete/{id}` | Jobs | Delete | Xóa tin (AJAX, soft delete) |- ✅ Quản lý thông tin công ty

| GET | `/Jobs/Applicants/{id}` | Jobs | Applicants | Danh sách ứng viên (paginated) |- ✅ Tạo tin tuyển dụng (CRUD)

| POST | `/Applications/UpdateStatus` | Applications | UpdateStatus | Duyệt/Từ chối ứng viên (AJAX) |- ✅ Quản lý danh sách việc làm

- ✅ Xem danh sách ứng viên nộp hồ sơ

### 👨‍💼 Admin Routes (Role: Admin)- ✅ Xác nhận hoặc từ chối ứng viên

- ✅ Ghi chú về ứng viên

| Method | Route | Controller | Action | Mô tả |

|--------|-------|------------|--------|-------|#### Admin (Quản trị viên)

| GET | `/Admin/Dashboard` | Admin | Dashboard | Dashboard tổng quan |

| GET | `/Admin/Users` | Admin | Users | Quản lý users (paginated) |- ✅ Dashboard thống kê tổng quan

| POST | `/Admin/UpdateUserStatus` | Admin | UpdateUserStatus | Khóa/Mở user (AJAX) |- ✅ Quản lý người dùng (kích hoạt/khóa tài khoản, thay đổi vai trò)

| POST | `/Admin/DeleteUser` | Admin | DeleteUser | Xóa user (hard delete) |- ✅ Quản lý việc làm (duyệt/từ chối tin tuyển dụng)

| GET | `/Admin/Jobs` | Admin | Jobs | Quản lý jobs (paginated) |- ✅ Quản lý danh mục ngành nghề

| POST | `/Admin/ApproveJob` | Admin | ApproveJob | Duyệt tin (AJAX) |- ✅ Xem thống kê chi tiết

| POST | `/Admin/RejectJob` | Admin | RejectJob | Từ chối tin (AJAX) |

| POST | `/Admin/HardDeleteJob` | Admin | HardDeleteJob | Xóa vĩnh viễn (AJAX) |### 🔒 Bảo mật

| POST | `/Admin/ExpireJobs` | Admin | ExpireJobs | Đóng tin quá hạn (AJAX) |

| GET | `/Admin/Categories` | Admin | Categories | Quản lý danh mục |- ✅ **JWT Authentication:** Access token ngắn hạn (15 phút), Refresh token dài hạn (30 ngày)

| POST | `/Admin/CreateCategory` | Admin | CreateCategory | Tạo danh mục (AJAX) |- ✅ **Password Hashing:** Sử dụng BCrypt với salt rounds = 12

| POST | `/Admin/UpdateCategory` | Admin | UpdateCategory | Sửa danh mục (AJAX) |- ✅ **Phân quyền:** Role-based authorization (Admin, Employer, Applicant)

| POST | `/Admin/ToggleCategoryStatus` | Admin | ToggleCategoryStatus | Bật/tắt danh mục (AJAX) |- ✅ **CSRF Protection:** AntiForgeryToken trên tất cả POST requests

| GET | `/Admin/Statistics` | Admin | Statistics | Thống kê chi tiết |- ✅ **XSS Protection:** HtmlEncode và Content-Security-Policy

- ✅ **SQL Injection Protection:** Sử dụng LINQ và EF Core parameterized queries

---- ✅ **Secure Cookies:** HttpOnly, Secure, SameSite=Strict



## 🗄️ Database Schema### 🎯 SEO & Performance



### 📊 Entity Relationship Diagram- ✅ **SEO-friendly URLs:** Clean routing

- ✅ **Meta tags:** Dynamic title, description, keywords

```- ✅ **Responsive Design:** Bootstrap 5, mobile-first

Users (1) ──────── (1) Companies- ✅ **Performance:**

  │                     │  - DbContext pooling

  │                     │  - Asynchronous operations

  │ (1)                 │ (1)  - Lazy loading cho navigation properties

  │                     │

  ↓                     ↓## 📊 Database Schema

Applications (N) ─── (1) Jobs (N) ──── (1) Categories

  │                     │### Tables

  │                     │

  └─────────────────────┘1. **Users** - Người dùng (Applicant, Employer, Admin)

  2. **Companies** - Thông tin công ty

RefreshTokens (N) ──── (1) Users3. **Categories** - Danh mục ngành nghề

4. **Jobs** - Tin tuyển dụng

JobViews (N) ──────── (1) Jobs5. **Applications** - Đơn ứng tuyển

         │6. **RefreshTokens** - Refresh tokens

         └──────── (0..1) Users

```### Relationships



### 📋 Table Descriptions- User 1-1 Company (Employer)

- Company 1-N Jobs

#### **Users** (Bảng người dùng)- Category 1-N Jobs

- **Id:** Primary Key- Job 1-N Applications

- **Email:** Unique, Not Null- User 1-N Applications

- **PasswordHash:** Hashed with BCrypt- User 1-N RefreshTokens

- **FullName:** Not Null

- **Role:** Admin, Employer, Applicant## 🔧 Configuration

- **IsActive:** Soft delete flag

- **Skills, Bio, AvatarUrl, CvUrl:** Optional fields### appsettings.json

- **CreatedAt, UpdatedAt:** Timestamps

```json

#### **Companies** (Bảng công ty){

- **Id:** Primary Key  "ConnectionStrings": {

- **Name:** Company name    "DefaultConnection": "Server=localhost;Port=3306;Database=JobExchangeDb;User=root;Password=123456;"

- **Description:** Company description  },

- **EmployerId:** Foreign Key to Users (1-1)  "JwtSettings": {

- **Logo, Website, Address, City:** Optional    "Secret": "YourSuperSecureSecretKey2025!@#$%^&*()_+JobExchangeMvc",

    "Issuer": "https://localhost:5001",

#### **Categories** (Bảng danh mục)    "Audience": "https://localhost:5001",

- **Id:** Primary Key    "AccessTokenExpirationMinutes": 15,

- **Name:** Category name    "RefreshTokenExpirationDays": 30

- **Description:** Optional  },

- **IsActive:** Toggle status  "CorsSettings": {

    "AllowedOrigins": [

#### **Jobs** (Bảng tin tuyển dụng)      "http://localhost:3000",

- **Id:** Primary Key      "http://localhost:5173",

- **Title, Description, Requirements, Benefits:** Job details      "https://localhost:5001"

- **SalaryRange, Location, JobType:** Job specifications    ]

- **Status:** Pending, Approved, Rejected, Expired, Closed  }

- **CompanyId:** Foreign Key to Companies}

- **CategoryId:** Foreign Key to Categories```

- **IsActive:** Soft delete flag

- **ApplicationDeadline:** Expiry date**Lưu ý quan trọng:**



#### **Applications** (Bảng đơn ứng tuyển)- ⚠️ **Secret key** phải có ít nhất 32 ký tự và phức tạp

- **Id:** Primary Key- ⚠️ **Không commit** secret key vào Git trong production

- **CoverLetter, CvUrl:** Application materials- ⚠️ Sử dụng **User Secrets** hoặc **Environment Variables** cho production

- **Status:** Pending, Approved, Interviewing, Accepted, Rejected, Cancelled

- **Note:** Employer's note## 🧪 Testing

- **JobId:** Foreign Key to Jobs

- **ApplicantId:** Foreign Key to Users```bash

- **AppliedAt, ReviewedAt:** Timestamps# Build project

- **UNIQUE (JobId, ApplicantId):** One application per jobdotnet build



#### **RefreshTokens** (Bảng JWT refresh tokens)# Run project

- **Id:** Primary Keydotnet run

- **Token:** Refresh token string

- **UserId:** Foreign Key to Users# Watch mode (auto-reload)

- **ExpiresAt:** Expiry datedotnet watch run

- **RevokedAt:** Revoke timestamp```



#### **JobViews** (Bảng lượt xem)## 📦 Deployment

- **Id:** Primary Key

- **JobId:** Foreign Key to Jobs### Publish cho Production

- **UserId:** Foreign Key to Users (nullable for anonymous)

- **IpAddress, UserAgent:** Tracking info```bash

- **ViewedAt:** Timestampdotnet publish -c Release -o ./publish

````

---

### Environment Variables

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
