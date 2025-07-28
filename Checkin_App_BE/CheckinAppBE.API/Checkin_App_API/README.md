# Checkin_App_API

Đây là API backend cho ứng dụng Checkin App, được xây dựng bằng .NET. API này cung cấp các chức năng quản lý người dùng, xác thực, huy hiệu, địa điểm, nhiệm vụ, kho báu và các tính năng liên quan khác.

## Các API chính:

### Auth API (AuthController)

*   **`POST /api/Auth/login`**
    *   **Mô tả:** Đăng nhập người dùng bằng thông tin đăng nhập (username/email và password).
    *   **Request:** `LoginRequestDto`.
    *   **Response:** `LoginResponseDto` (chứa JWT token, refresh token và thông tin người dùng).
*   **`POST /api/Auth/register`**
    *   **Mô tả:** Đăng ký tài khoản người dùng mới.
    *   **Request:** `RegisterRequestDto`.
    *   **Response:** `LoginResponseDto`.
*   **`POST /api/Auth/refresh-token`**
    *   **Mô tả:** Làm mới JWT access token bằng refresh token.
    *   **Request:** `RefreshTokenRequestDto`.
    *   **Response:** `LoginResponseDto`.
*   **`POST /api/Auth/send-otp`**
    *   **Mô tả:** Gửi mã OTP (One-Time Password) đến email hoặc số điện thoại của người dùng để xác minh.
    *   **Request:** `OtpSendRequestDto`.
    *   **Response:** `ServiceResult`.
*   **`POST /api/Auth/verify-otp`**
    *   **Mô tả:** Xác minh mã OTP mà người dùng đã nhận được.
    *   **Request:** `OtpVerifyRequestDto`.
    *   **Response:** `ServiceResult`.
*   **`POST /api/Auth/social-login`**
    *   **Mô tả:** Đăng nhập người dùng thông qua các tài khoản mạng xã hội (ví dụ: Google, Facebook).
    *   **Request:** `SocialLoginRequestDto`.
    *   **Response:** `LoginResponseDto`.
*   **`POST /api/Auth/link-social`** (Yêu cầu xác thực)
    *   **Mô tả:** Liên kết tài khoản mạng xã hội với tài khoản người dùng hiện có.
    *   **Request:** `LinkSocialAccountRequestDto`.
    *   **Response:** `ServiceResult`.
*   **`POST /api/Auth/unlink-social`** (Yêu cầu xác thực)
    *   **Mô tả:** Hủy liên kết tài khoản mạng xã hội khỏi tài khoản người dùng hiện có.
    *   **Request:** `UnlinkSocialAccountRequestDto`.
    *   **Response:** `ServiceResult`.

### User API (UserController)

*   **`GET /api/User`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Lấy danh sách tất cả người dùng, có thể lọc.
    *   **Request:** `UserFilterRequestDto`.
    *   **Response:** `IEnumerable<UserResponseDto>`.
*   **`GET /api/User/{userId}`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Lấy thông tin chi tiết của một người dùng cụ thể bằng ID.
    *   **Response:** `UserResponseDto`.
*   **`PUT /api/User/{userId}`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Cập nhật thông tin của một người dùng cụ thể.
    *   **Request:** `UserUpdateRequestDto`.
    *   **Response:** `ServiceResult`.
*   **`DELETE /api/User/{userId}`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Xóa một người dùng cụ thể.
    *   **Response:** `ServiceResult`.
*   **`POST /api/User/{userId}/change-password`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Admin thay đổi mật khẩu của một người dùng khác.
    *   **Request:** `AdminChangeUserPasswordRequestDto`.
    *   **Response:** `ServiceResult`.
*   **`POST /api/User/{userId}/assign-roles`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Gán các vai trò cho một người dùng cụ thể.
    *   **Request:** `List<Guid>`.
    *   **Response:** `ServiceResult`.
*   **`POST /api/User/me/change-password`** (Yêu cầu xác thực)
    *   **Mô tả:** Người dùng tự thay đổi mật khẩu của mình.
    *   **Request:** `UserChangePasswordRequestDto`.
    *   **Response:** `ServiceResult`.

### Badge API (BadgeController)

*   **`GET /api/Badge`**
    *   **Mô tả:** Lấy tất cả các huy hiệu có sẵn trong hệ thống.
    *   **Response:** `IEnumerable<BadgeResponseDto>`.
*   **`GET /api/Badge/{id}`**
    *   **Mô tả:** Lấy thông tin chi tiết của một huy hiệu cụ thể bằng ID.
    *   **Response:** `BadgeResponseDto`.
*   **`GET /api/Badge/user/{userId}`**
    *   **Mô tả:** Lấy tất cả các huy hiệu mà một người dùng cụ thể đã đạt được.
    *   **Response:** `IEnumerable<UserBadgeResponseDto>`.
*   **`POST /api/Badge/award`**
    *   **Mô tả:** Trao một huy hiệu cho một người dùng cụ thể. (Có thể dùng cho admin hoặc quy trình nội bộ).
    *   **Request:** `userId` (Guid), `badgeId` (Guid).
    *   **Response:** `UserBadgeResponseDto`.
*   **`POST /api/Badge`**
    *   **Mô tả:** Tạo một huy hiệu mới.
    *   **Request:** `BadgeCreateRequestDto`.
    *   **Response:** `BadgeResponseDto`.
*   **`PUT /api/Badge`**
    *   **Mô tả:** Cập nhật thông tin của một huy hiệu hiện có.
    *   **Request:** `BadgeUpdateRequestDto`.
    *   **Response:** `BadgeResponseDto`.
*   **`DELETE /api/Badge/{id}`**
    *   **Mô tả:** Xóa một huy hiệu bằng ID.
    *   **Response:** `bool`.

### Landmark API (LandmarkController)

*   **`GET /api/Landmark`**
    *   **Mô tả:** Lấy tất cả các địa điểm (landmarks) có sẵn.
    *   **Response:** `IEnumerable<LandmarkResponseDto>`.
*   **`GET /api/Landmark/{id}`**
    *   **Mô tả:** Lấy thông tin chi tiết của một địa điểm cụ thể bằng ID.
    *   **Response:** `LandmarkResponseDto`.
*   **`POST /api/Landmark`**
    *   **Mô tả:** Tạo một địa điểm mới.
    *   **Request:** `LandmarkCreateRequestDto`.
    *   **Response:** `LandmarkResponseDto`.
*   **`PUT /api/Landmark/{id}`**
    *   **Mô tả:** Cập nhật thông tin của một địa điểm hiện có.
    *   **Request:** `LandmarkUpdateRequestDto`.
    *   **Response:** `LandmarkResponseDto`.
*   **`DELETE /api/Landmark/{id}`**
    *   **Mô tả:** Xóa một địa điểm bằng ID.
    *   **Response:** `bool`.

### Landmark Visit API (LandmarkVisitController)

*   **`POST /api/LandmarkVisit`**
    *   **Mô tả:** Thực hiện check-in tại một địa điểm. Sau khi check-in thành công, API cũng sẽ cố gắng mở một kho báu đặc biệt liên quan đến check-in đó.
    *   **Request:** `LandmarkVisitCreateRequestDto`.
    *   **Response:** `LandmarkVisitResponseDto`.
*   **`GET /api/LandmarkVisit/user/{userId}`**
    *   **Mô tả:** Lấy danh sách tất cả các lượt check-in của một người dùng cụ thể.
    *   **Response:** `IEnumerable<LandmarkVisitResponseDto>`.

### Mission API (MissionController)

*   **`GET /api/Mission`**
    *   **Mô tả:** Lấy tất cả các nhiệm vụ có sẵn trong hệ thống.
    *   **Response:** `IEnumerable<MissionResponseDto>`.
*   **`GET /api/Mission/{id}`**
    *   **Mô tả:** Lấy thông tin chi tiết của một nhiệm vụ cụ thể bằng ID.
    *   **Response:** `MissionResponseDto`.
*   **`GET /api/Mission/user/{userId}`**
    *   **Mô tả:** Lấy tất cả các nhiệm vụ của một người dùng cụ thể.
    *   **Response:** `IEnumerable<UserMissionResponseDto>`.
*   **`POST /api/Mission/assign`**
    *   **Mô tả:** Gán một nhiệm vụ cho một người dùng.
    *   **Request:** `userId` (Guid), `missionId` (Guid).
    *   **Response:** `UserMissionResponseDto`.
*   **`POST /api/Mission/complete/{userMissionId}`**
    *   **Mô tả:** Đánh dấu một nhiệm vụ của người dùng là đã hoàn thành.
    *   **Request:** `userMissionId` (Guid).
    *   **Response:** `UserMissionResponseDto`.
*   **`POST /api/Mission`**
    *   **Mô tả:** Tạo một nhiệm vụ mới.
    *   **Request:** `MissionCreateRequestDto`.
    *   **Response:** `MissionResponseDto`.
*   **`PUT /api/Mission`**
    *   **Mô tả:** Cập nhật thông tin của một nhiệm vụ hiện có.
    *   **Request:** `MissionUpdateRequestDto`.
    *   **Response:** `MissionResponseDto`.
*   **`DELETE /api/Mission/{id}`**
    *   **Mô tả:** Xóa một nhiệm vụ bằng ID.
    *   **Response:** `bool`.

### Role API (RoleController)

*   **`GET /api/Role`**
    *   **Mô tả:** Lấy danh sách tất cả các vai trò, có thể lọc.
    *   **Request:** `RoleFilterRequestDto`.
    *   **Response:** `IEnumerable<RoleResponseDto>`.
*   **`GET /api/Role/{roleId}`**
    *   **Mô tả:** Lấy thông tin chi tiết của một vai trò cụ thể bằng ID.
    *   **Response:** `RoleResponseDto`.
*   **`POST /api/Role`**
    *   **Mô tả:** Tạo một vai trò mới.
    *   **Request:** `RoleCreateRequestDto`.
    *   **Response:** `ServiceResult`.
*   **`PUT /api/Role/{roleId}`**
    *   **Mô tả:** Cập nhật thông tin của một vai trò cụ thể.
    *   **Request:** `RoleUpdateRequestDto`.
    *   **Response:** `ServiceResult`.
*   **`DELETE /api/Role/{roleId}`**
    *   **Mô tả:** Xóa một vai trò cụ thể.
    *   **Response:** `ServiceResult`.

### Session API (SessionController)

*   **`GET /api/Session/me`** (Yêu cầu xác thực)
    *   **Mô tả:** Lấy danh sách tất cả các phiên đăng nhập hiện tại của người dùng đã xác thực.
    *   **Response:** `IEnumerable<object>`.
*   **`DELETE /api/Session/{sessionId}`** (Yêu cầu xác thực)
    *   **Mô tả:** Thu hồi một phiên đăng nhập cụ thể của người dùng đã xác thực.
    *   **Response:** `ServiceResult`.
*   **`DELETE /api/Session/me/all-except-current`** (Yêu cầu xác thực)
    *   **Mô tả:** Thu hồi tất cả các phiên đăng nhập của người dùng hiện tại, trừ phiên đang sử dụng.
    *   **Response:** `ServiceResult`.

### Stress Logs API (StressLogsController)

*   **`POST /api/StressLogs`** (Yêu cầu xác thực)
    *   **Mô tả:** Tạo một bản ghi log mức độ căng thẳng của người dùng.
    *   **Request:** `StressLogCreateRequestDto`.
    *   **Response:** `ServiceResult`.
*   **`GET /api/StressLogs`** (Yêu cầu xác thực)
    *   **Mô tả:** Lấy danh sách các bản ghi log căng thẳng của người dùng hiện tại, có thể lọc theo thời gian.
    *   **Request:** `StressLogFilterRequestDto`.
    *   **Response:** `IEnumerable<StressLogResponseDto>`.
*   **`GET /api/StressLogs/average-by-period`** (Yêu cầu xác thực)
    *   **Mô tả:** Tính toán mức độ căng thẳng trung bình của người dùng theo các khoảng thời gian (ví dụ: hàng ngày, hàng tuần).
    *   **Request:** `StressLogFilterRequestDto`.
    *   **Response:** `Dictionary<string, double>`.

### Treasure API (TreasureController)

*   **`GET /api/Treasure`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Lấy tất cả các kho báu có sẵn trong hệ thống.
    *   **Response:** `IEnumerable<TreasureResponseDto>`.
*   **`GET /api/Treasure/{id}`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Lấy thông tin chi tiết của một kho báu cụ thể bằng ID.
    *   **Response:** `TreasureResponseDto`.
*   **`POST /api/Treasure`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Tạo một kho báu mới.
    *   **Request:** `TreasureCreateRequestDto`.
    *   **Response:** `TreasureResponseDto`.
*   **`PUT /api/Treasure/{id}`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Cập nhật thông tin của một kho báu hiện có.
    *   **Request:** `TreasureUpdateRequestDto`.
    *   **Response:** `TreasureResponseDto`.
*   **`DELETE /api/Treasure/{id}`** (Yêu cầu xác thực, chỉ Admin)
    *   **Mô tả:** Xóa một kho báu bằng ID.
    *   **Response:** `bool`.
*   **`POST /api/Treasure/open-daily`** (Yêu cầu xác thực)
    *   **Mô tả:** Mở kho báu hàng ngày của người dùng.
    *   **Response:** `OpenTreasureResponseDto`.
*   **`POST /api/Treasure/open-special`** (Yêu cầu xác thực)
    *   **Mô tả:** Mở kho báu đặc biệt liên quan đến một lượt check-in cụ thể.
    *   **Request:** `OpenTreasureRequestDto`.
    *   **Response:** `OpenTreasureResponseDto`.

### Admin API (AdminController)

*   **`POST /api/Admin/user/{userId}/ban`**
    *   **Mô tả:** Cấm (ban) một người dùng.
    *   **Response:** HTTP 200 OK.
*   **`POST /api/Admin/user/{userId}/unban`**
    *   **Mô tả:** Bỏ cấm (unban) một người dùng.
    *   **Response:** HTTP 200 OK.
*   **`POST /api/Admin/user/{userId}/assign-role`**
    *   **Mô tả:** Gán một vai trò cho người dùng.
    *   **Request:** `roleName`.
    *   **Response:** HTTP 200 OK.
*   **`POST /api/Admin/landmark/{landmarkId}/approve`**
    *   **Mô tả:** Phê duyệt một địa điểm.
    *   **Response:** HTTP 200 OK.
*   **`POST /api/Admin/landmark/{landmarkId}/reject`**
    *   **Mô tả:** Từ chối một địa điểm.
    *   **Response:** HTTP 200 OK.

## Cấu hình Dịch vụ Thông báo Go:

API này hiện sử dụng một dịch vụ Go riêng biệt để gửi thông báo qua Redis. Cấu hình cho dịch vụ Go được đặt trong `appsettings.json`:

```json
"GoNotificationService": {
  "BaseUrl": "http://localhost:8080" // URL của dịch vụ Go Notification
}
```

Đảm bảo dịch vụ Go Notification đang chạy và có thể truy cập được tại URL đã cấu hình.