# Checkin App Project

Đây là một dự án ứng dụng Checkin đa nền tảng, bao gồm một ứng dụng di động được phát triển bằng Flutter và một hệ thống API backend được xây dựng bằng .NET, cùng với một dịch vụ thông báo riêng biệt bằng Go.

## Tổng quan về ứng dụng

Ứng dụng Checkin cho phép người dùng tương tác với các địa điểm (landmarks), hoàn thành nhiệm vụ (missions), thu thập huy hiệu (badges) và mở kho báu (treasures). Nó cung cấp một trải nghiệm gamification cho việc khám phá và tương tác với thế giới xung quanh.

## Công nghệ sử dụng

### 1. Ứng dụng di động (Frontend)

*   **Nền tảng:** Flutter (Dart)
*   **Mô tả:** Giao diện người dùng thân thiện, hoạt ảnh mượt mà, và khả năng chạy trên cả Android và iOS từ một codebase duy nhất.

### 2. API Backend

*   **Nền tảng:** .NET (C#)
*   **Cơ sở dữ liệu:** SQL Server (sử dụng Entity Framework Core)
*   **Xác thực:** JWT (JSON Web Tokens), hỗ trợ đăng nhập xã hội (Google).
*   **Quản lý phiên:** Redis (cho refresh token và OTP).
*   **Kiến trúc:** Được tổ chức theo các tầng (Controllers, Services, Repositories) với các mẫu thiết kế như Repository, Unit of Work, và DTOs.
*   **Chức năng chính:** Quản lý người dùng, vai trò, huy hiệu, địa điểm, lượt check-in, nhiệm vụ, kho báu, và log căng thẳng.

### 3. Dịch vụ Thông báo (Go Notification Service)

*   **Nền tảng:** Go
*   **Mô tả:** Một microservice nhẹ, hiệu suất cao được phát triển bằng Go để xử lý việc gửi thông báo thời gian thực thông qua Redis Pub/Sub. Dịch vụ này được tách biệt để tối ưu hóa khả năng xử lý đồng thời và mở rộng.

## Cách sử dụng / Chạy ứng dụng

Để chạy toàn bộ hệ thống, bạn cần khởi động dịch vụ thông báo Go, API backend .NET và ứng dụng Flutter.

### 1. Khởi động Dịch vụ Thông báo Go

1.  **Di chuyển đến thư mục:** `E:\AppGoogleMap\GoNotificationService`
2.  **Cài đặt dependencies:**
    ```bash
    go mod tidy
    ```
3.  **Đặt biến môi trường cho Redis Cloud (nếu sử dụng):**
    *   **PowerShell:**
        ```powershell
        $env:REDIS_ADDR="your_redis_host:your_redis_port"
        $env:REDIS_PASSWORD="your_redis_password"
        ```
    *   **Command Prompt:**
        ```bash
        set REDIS_ADDR=your_redis_host:your_redis_port
        set REDIS_PASSWORD=your_redis_password
        ```
    *(Thay thế `your_redis_host:your_redis_port` và `your_redis_password` bằng thông tin Redis Cloud của bạn.)*
4.  **Chạy dịch vụ:**
    ```bash
    go run ./cmd/gonotificationservice
    ```
    Dịch vụ sẽ lắng nghe trên cổng `8080` (mặc định).

### 2. Khởi động API Backend .NET

1.  **Di chuyển đến thư mục:** `E:\AppGoogleMap\Checkin_App_BE\CheckinAppBE.API`
2.  **Build dự án:**
    ```bash
    dotnet build
    ```
3.  **Chạy dự án:**
    ```bash
    dotnet run --project Checkin_App_API
    ```
    API sẽ chạy trên các cổng được cấu hình trong `launchSettings.json` (thường là `https://localhost:70xx` và `http://localhost:5xxx`).

### 3. Chạy Ứng dụng Flutter

1.  **Di chuyển đến thư mục:** `E:\AppGoogleMap\Checkin_app_android\Checkin_App`
2.  **Tải dependencies:**
    ```bash
    flutter pub get
    ```
3.  **Chạy ứng dụng trên thiết bị/emulator:**
    ```bash
    flutter run
    ```
    Đảm bảo bạn đã cấu hình ứng dụng Flutter để trỏ đến URL của API backend .NET.

---

**Lưu ý:** Đảm bảo rằng tất cả các dịch vụ (Go Notification, .NET API) đang chạy trước khi khởi động ứng dụng Flutter để đảm bảo chức năng đầy đủ.
