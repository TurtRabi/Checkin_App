# CheckinAppBE.API

This is the backend API for the Checkin App.

## API Endpoints

### 1. AuthController (`/api/Auth`)

*   **`POST /api/Auth/login`**
    *   **Chức năng:** Đăng nhập người dùng bằng tên người dùng và mật khẩu. Trả về Access Token và Refresh Token nếu đăng nhập thành công.
*   **`POST /api/Auth/register`**
    *   **Chức năng:** Đăng ký người dùng mới. Tạo tài khoản người dùng và thông tin xác thực cục bộ, sau đó gán vai trò "User" mặc định. Trả về Access Token và Refresh Token.
*   **`POST /api/Auth/refresh-token`**
    *   **Chức năng:** Làm mới Access Token bằng cách sử dụng Refresh Token đã có. Thu hồi Refresh Token cũ và cấp một cặp Access Token/Refresh Token mới.
*   **`POST /api/Auth/send-otp`**
    *   **Chức năng:** Gửi mã OTP (One-Time Password) đến email của người dùng để xác minh.
*   **`POST /api/Auth/verify-otp`**
    *   **Chức năng:** Xác minh mã OTP mà người dùng đã nhận được.
*   **`POST /api/Auth/social-login`**
    *   **Chức năng:** Đăng nhập hoặc đăng ký người dùng thông qua tài khoản mạng xã hội (ví dụ: Google, Facebook) bằng cách sử dụng token từ nhà cung cấp.
*   **`POST /api/Auth/link-social`** (Yêu cầu xác thực)
    *   **Chức năng:** Liên kết tài khoản mạng xã hội hiện có với tài khoản người dùng đã đăng nhập.
*   **`POST /api/Auth/unlink-social`** (Yêu cầu xác thực)
    *   **Chức năng:** Hủy liên kết tài khoản mạng xã hội khỏi tài khoản người dùng đã đăng nhập.

### 2. BadgeController (`/api/Badge`)

*   **`GET /api/Badge`**
    *   **Chức năng:** Lấy tất cả các huy hiệu có trong hệ thống.
*   **`GET /api/Badge/{id}`**
    *   **Chức năng:** Lấy thông tin chi tiết của một huy hiệu cụ thể bằng ID.
*   **`GET /api/Badge/user/{userId}`**
    *   **Chức năng:** Lấy tất cả các huy hiệu mà một người dùng cụ thể đã đạt được.
*   **`POST /api/Badge/award`**
    *   **Chức năng:** Gán một huy hiệu cho một người dùng. (Thường dùng cho admin hoặc quy trình nội bộ).
*   **`POST /api/Badge`**
    *   **Chức năng:** Tạo một huy hiệu mới.
*   **`PUT /api/Badge`**
    *   **Chức năng:** Cập nhật thông tin của một huy hiệu hiện có.
*   **`DELETE /api/Badge/{id}`**
    *   **Chức năng:** Xóa một huy hiệu khỏi hệ thống.

### 3. LandmarkController (`/api/Landmark`)

*   **`GET /api/Landmark`**
    *   **Chức năng:** Lấy tất cả các địa danh (landmark) có trong hệ thống.
*   **`GET /api/Landmark/{id}`**
    *   **Chức năng:** Lấy thông tin chi tiết của một địa danh cụ thể bằng ID.
*   **`POST /api/Landmark`**
    *   **Chức năng:** Tạo một địa danh mới.
*   **`PUT /api/Landmark/{id}`**
    *   **Chức năng:** Cập nhật thông tin của một địa danh hiện có.
*   **`DELETE /api/Landmark/{id}`**
    *   **Chức năng:** Xóa một địa danh khỏi hệ thống.

### 4. LandmarkVisitController (`/api/LandmarkVisit`)

*   **`POST /api/LandmarkVisit`**
    *   **Chức năng:** Thực hiện check-in tại một địa danh. API này cũng chứa logic để tự động cấp huy hiệu cho người dùng dựa trên số lần check-in.
*   **`GET /api/LandmarkVisit/user/{userId}`**
    *   **Chức năng:** Lấy danh sách tất cả các lần check-in của một người dùng cụ thể.

### 5. MissionController (`/api/Mission`)

*   **`GET /api/Mission`**
    *   **Chức năng:** Lấy tất cả các nhiệm vụ có trong hệ thống.
*   **`GET /api/Mission/{id}`**
    *   **Chức năng:** Lấy thông tin chi tiết của một nhiệm vụ cụ thể bằng ID.
*   **`GET /api/Mission/user/{userId}`**
    *   **Chức năng:** Lấy tất cả các nhiệm vụ của một người dùng cụ thể.
*   **`POST /api/Mission/assign`**
    *   **Chức năng:** Gán một nhiệm vụ cho một người dùng.
*   **`POST /api/Mission/complete/{userMissionId}`**
    *   **Chức năng:** Đánh dấu một nhiệm vụ của người dùng là đã hoàn thành.
*   **`POST /api/Mission`**
    *   **Chức năng:** Tạo một nhiệm vụ mới.
*   **`PUT /api/Mission`**
    *   **Chức năng:** Cập nhật thông tin của một nhiệm vụ hiện có.
*   **`DELETE /api/Mission/{id}`**
    *   **Chức năng:** Xóa một nhiệm vụ khỏi hệ thống.

### 6. RoleController (`/api/Role`)

*   **`GET /api/Role`**
    *   **Chức năng:** Lấy tất cả các vai trò (role) có trong hệ thống, có thể lọc theo `RoleFilterRequestDto`.
*   **`GET /api/Role/{roleId}`**
    *   **Chức năng:** Lấy thông tin chi tiết của một vai trò cụ thể bằng ID.
*   **`POST /api/Role`**
    *   **Chức năng:** Tạo một vai trò mới.
*   **`PUT /api/Role/{roleId}`**
    *   **Chức năng:** Cập nhật thông tin của một vai trò hiện có.
*   **`DELETE /api/Role/{roleId}`**
    *   **Chức năng:** Xóa một vai trò khỏi hệ thống.

### 7. SessionController (`/api/Session`)

*   **`GET /api/Session/me`** (Yêu cầu xác thực)
    *   **Chức năng:** Lấy tất cả các phiên đăng nhập hiện tại của người dùng đã xác thực.
*   **`DELETE /api/Session/{sessionId}`** (Yêu cầu xác thực)
    *   **Chức năng:** Thu hồi một phiên đăng nhập cụ thể của người dùng đã xác thực bằng ID phiên.
*   **`DELETE /api/Session/me/all-except-current`** (Yêu cầu xác thực)
    *   **Chức năng:** Thu hồi tất cả các phiên đăng nhập của người dùng đã xác thực, ngoại trừ phiên hiện tại.

### 8. UserController (`/api/User`)

*   **`GET /api/User`** (Chỉ Admin)
    *   **Chức năng:** Lấy tất cả người dùng trong hệ thống, có thể lọc theo `UserFilterRequestDto`.
*   **`GET /api/User/{userId}`** (Chỉ Admin)
    *   **Chức năng:** Lấy thông tin chi tiết của một người dùng cụ thể bằng ID.
*   **`PUT /api/User/{userId}`** (Chỉ Admin)
    *   **Chức năng:** Cập nhật thông tin của một người dùng hiện có.
*   **`DELETE /api/User/{userId}`** (Chỉ Admin)
    *   **Chức năng:** Xóa một người dùng khỏi hệ thống.
*   **`POST /api/User/{userId}/change-password`** (Chỉ Admin)
    *   **Chức năng:** Admin thay đổi mật khẩu của một người dùng bất kỳ.
*   **`POST /api/User/{userId}/assign-roles`** (Chỉ Admin)
    *   **Chức năng:** Gán hoặc cập nhật các vai trò cho một người dùng cụ thể.
*   **`POST /api/User/me/change-password`** (Yêu cầu xác thực)
    *   **Chức năng:** Người dùng đã đăng nhập tự thay đổi mật khẩu của chính họ.
