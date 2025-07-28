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

## Phân quyền API (API Authorization)

Các API trong hệ thống được phân loại quyền truy cập như sau:

### 1. AuthController (api/Auth)

*   **`POST /api/Auth/register`**: `[AllowAnonymous]` - Đăng ký tài khoản mới.
*   **`POST /api/Auth/login`**: `[AllowAnonymous]` - Đăng nhập.
*   **`POST /api/Auth/refresh-token`**: `[AllowAnonymous]` - Làm mới token (refresh token thường không yêu cầu token truy cập hợp lệ).
*   **`POST /api/Auth/send-otp`**: `[AllowAnonymous]` - Gửi OTP để xác minh.
*   **`POST /api/Auth/verify-otp`**: `[AllowAnonymous]` - Xác minh OTP.
*   **`POST /api/Auth/social-login`**: `[AllowAnonymous]` - Đăng nhập mạng xã hội.
*   **`POST /api/Auth/link-social`**: `[Authorize]` - Liên kết tài khoản mạng xã hội (yêu cầu người dùng đã đăng nhập).
*   **`POST /api/Auth/unlink-social`**: `[Authorize]` - Hủy liên kết tài khoản mạng xã hội (yêu cầu người dùng đã đăng nhập).

### 2. UserController (api/User)

*   **`GET /api/User`**: `[Authorize(Roles = "Admin")]` - Lấy danh sách tất cả người dùng.
*   **`GET /api/User/{userId}`**: `[Authorize(Roles = "Admin")]` - Lấy thông tin người dùng theo ID.
*   **`GET /api/User/{userId}/reward-cards`**: `[Authorize]` - Lấy danh sách thẻ thưởng mà người dùng đã sưu tầm (người dùng có thể xem thẻ của chính họ, admin có thể xem thẻ của bất kỳ ai).
*   **`PUT /api/User/{userId}`**: `[Authorize(Roles = "Admin")]` - Cập nhật thông tin người dùng.
*   **`DELETE /api/User/{userId}`**: `[Authorize(Roles = "Admin")]` - Xóa người dùng.
*   **`POST /api/User/{userId}/assign-roles`**: `[Authorize(Roles = "Admin")]` - Gán vai trò cho người dùng.
*   **`POST /api/User/me/change-password`**: `[Authorize]` - Người dùng tự đổi mật khẩu của mình.

### 3. LandmarkVisitController (api/LandmarkVisit)

*   **`POST /api/LandmarkVisit`**: `[Authorize]` - Check-in địa điểm (yêu cầu người dùng đã đăng nhập).
*   **`GET /api/LandmarkVisit/user/{userId}`**: `[Authorize]` - Lấy lịch sử check-in của người dùng (người dùng có thể xem lịch sử của chính họ, admin có thể xem lịch sử của bất kỳ ai).

### 4. RewardCardController (api/RewardCard)

*   **`GET /api/RewardCard`**: `[Authorize]` - Trả về danh sách tất cả các thẻ có trong hệ thống (có thể cho phép người dùng đã đăng nhập xem).
*   **`POST /api/RewardCard`**: `[Authorize(Roles = "Admin")]` - Thêm thẻ mới.
*   **`PUT /api/RewardCard/{id}`**: `[Authorize(Roles = "Admin")]` - Chỉnh sửa thông tin thẻ.
*   **`DELETE /api/RewardCard/{id}`**: `[Authorize(Roles = "Admin")]` - Xóa thẻ.

### 5. Các Controller khác (ví dụ)

*   **AdminController**: `[Authorize(Roles = "Admin")]` - Toàn bộ controller này chỉ dành cho Admin.
*   **BadgeController**: `[Authorize]` - Các API liên quan đến huy hiệu (có thể cho phép người dùng xem huy hiệu của họ hoặc các huy hiệu có sẵn).
*   **LandmarkController**: `[Authorize]` - Các API liên quan đến địa điểm (có thể cho phép người dùng xem danh sách địa điểm).
*   **MissionController**: `[Authorize]` - Các API liên quan đến nhiệm vụ (có thể cho phép người dùng xem nhiệm vụ của họ hoặc các nhiệm vụ có sẵn).
*   **RoleController**: `[Authorize(Roles = "Admin")]` - Quản lý vai trò (chỉ dành cho Admin).
*   **SessionController**: `[Authorize]` - Quản lý phiên (có thể cho phép người dùng xem/quản lý phiên của họ).
*   **StressLogsController**: `[Authorize]` - Ghi log căng thẳng (có thể cho phép người dùng ghi log của họ).
*   **TreasureController**: `[Authorize]` - Các API liên quan đến kho báu (có thể cho phép người dùng xem kho báu của họ hoặc các kho báu có sẵn).

## Công nghệ sử dụng

Để tích hợp tính năng thẻ thưởng, các thay đổi sau đã được thực hiện:

#### I. Bảng cơ sở dữ liệu mới/chỉnh sửa

1.  **RewardCard** (mới)
    *   Lưu thông tin tất cả các loại thẻ thưởng có trong hệ thống.
    *   **Các trường chính:**
        *   `Id` (Guid): Mã thẻ duy nhất.
        *   `Name` (string): Tên thẻ (ví dụ: "Thẻ Văn Miếu").
        *   `Description` (string): Mô tả chi tiết về thẻ.
        *   `ImageUrl` (string): Đường dẫn ảnh minh họa cho thẻ.
        *   `Rarity` (string): Độ hiếm của thẻ ("Common", "Rare", "Epic", "Legendary").
        *   `DropRate` (double): Xác suất rơi của thẻ (từ 0.0 đến 1.0).
        *   `AnimationVideoUrl` (string, nullable): Đường dẫn video/animation khi thẻ được nhận.
        *   `AnimationType` (string, nullable): Loại animation ("video", "gif", "lottie").

2.  **UserRewardCard** (mới)
    *   Lưu thông tin các thẻ thưởng mà người dùng đã sưu tầm.
    *   **Các trường chính:**
        *   `Id` (Guid): Mã record duy nhất.
        *   `UserId` (Guid): ID của người dùng sở hữu thẻ.
        *   `RewardCardId` (Guid): ID của thẻ thưởng đã nhận.
        *   `CollectedAt` (DateTime): Thời điểm người dùng nhận được thẻ.

#### II. API mới

1.  **POST /api/LandmarkVisit/checkin-random-card**
    *   **Mô tả:** API này được gọi nội bộ sau khi người dùng check-in thành công tại một địa điểm. Nó sẽ dựa trên `DropRate` của các thẻ trong bảng `RewardCard` để xác định xem người dùng có nhận được thẻ ngẫu nhiên nào không. Nếu có, một record mới sẽ được tạo trong `UserRewardCard`.
    *   **Luồng xử lý:**
        *   Được gọi sau khi `POST /api/LandmarkVisit` thành công.
        *   Hệ thống tính toán xác suất rơi thẻ dựa trên `DropRate` của từng thẻ.
        *   Nếu một thẻ rơi, một bản ghi `UserRewardCard` mới sẽ được tạo.
        *   Trả về thông tin thẻ đã rơi (nếu có) cho frontend để hiển thị animation.

2.  **GET /api/User/{userId}/reward-cards**
    *   **Mô tả:** Trả về danh sách tất cả các thẻ thưởng mà một người dùng cụ thể đã sưu tầm.
    *   **Quyền truy cập:** Yêu cầu quyền `Admin`.

3.  **GET /api/RewardCard**
    *   **Mô tả:** Trả về danh sách tất cả các thẻ thưởng có trong hệ thống. API này có thể được sử dụng bởi client hoặc admin.

4.  **POST /api/RewardCard (Admin)**
    *   **Mô tả:** Cho phép admin thêm một thẻ thưởng mới vào hệ thống, bao gồm thông tin về ảnh, URL animation và xác suất rơi.
    *   **Quyền truy cập:** Yêu cầu quyền `Admin`.

5.  **PUT /api/RewardCard/{id} (Admin)**
    *   **Mô tả:** Cho phép admin chỉnh sửa thông tin của một thẻ thưởng hiện có dựa trên ID của thẻ.
    *   **Quyền truy cập:** Yêu cầu quyền `Admin`.

6.  **DELETE /api/RewardCard/{id} (Admin)**
    *   **Mô tả:** Cho phép admin xóa một thẻ thưởng khỏi hệ thống dựa trên ID của thẻ.
    *   **Quyền truy cập:** Yêu cầu quyền `Admin`.

#### III. Cập nhật luồng Check-in

*   **Trước đây:** `POST /api/LandmarkVisit` chỉ xử lý việc check-in và mở kho báu đặc biệt.
*   **Thay đổi:** Sau khi người dùng check-in thành công thông qua `POST /api/LandmarkVisit`, hệ thống sẽ tự động gọi service phụ để thực hiện việc rơi thẻ ngẫu nhiên (`CheckinRandomCard`). Nếu có thẻ rơi, thông tin thẻ sẽ được trả về cho frontend để hiển thị animation tương ứng.

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
