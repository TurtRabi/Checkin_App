import IAuthRepository from "@/domain/repositories/IAuthRepository";
// Bước 1: Xóa import axios và thay bằng apiClient đã cấu hình
import apiClient from "@/infrastructure/api/apiClient";

export default class AuthRepository extends IAuthRepository {
  /**
   * Gửi Google token lên backend để xác thực và lấy về thông tin người dùng cùng JWT token của hệ thống.
   * @param {string} googleToken - Token nhận được từ Google.
   * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là user info và token).
   */
  async loginWithGoogle(googleToken) {
    try {
      // Bước 2: Sử dụng apiClient thay vì axios.
      // Bạn chỉ cần cung cấp endpoint, không cần lo về URL gốc hay headers.
      // Endpoint ví dụ: /auth/google-signin
      const response = await apiClient.post("/social-login", { provider: 'Google',token: googleToken });
      console.log(response.data);

      // Trả về toàn bộ dữ liệu từ backend để store xử lý
      return response.data;
    } catch (error) {
      // Interceptor đã xử lý các lỗi chung (như 401).
      // Ở đây ta có thể log thêm thông tin cụ thể cho endpoint này.
      console.error(
        "Lỗi cụ thể tại AuthRepository.loginWithGoogle:",
        error.response?.data || error.message
      );
      // Ném lỗi để các tầng cao hơn (use case, store) có thể bắt và xử lý (vd: hiển thị thông báo cho người dùng)
      throw error;
    }
  }
}
