import IAuthRepository from "@/domain/repositories/IAuthRepository";
import apiClient from "@/infrastructure/api/apiClient";

/**
 * Repository để xử lý tất cả các tương tác liên quan đến xác thực với API.
 * Mỗi phương thức ở đây tương ứng với một endpoint của backend.
 * Logic xử lý lỗi và chuẩn hóa response đã được chuyển vào apiClient.
 */
export default class AuthRepository extends IAuthRepository {
  /**
   * Gửi Google token lên backend để xác thực.
   * @param {string} googleToken - Token nhận được từ Google.
   * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
   */
  async loginWithGoogle(googleToken) {
    return apiClient.post("/Auth/social-login", { provider: 'Google', device: 'Web', token: googleToken });
  }

  /**
   * Gửi thông tin đăng nhập (username và password) lên backend.
   * @param {string} username - Tên đăng nhập.
   * @param {string} password - Mật khẩu.
   * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
   */
  async loginWithUsernameAndPassword(username, password) {
    return apiClient.post("/Auth/login", { userName: username, password: password, deviceName: 'Web' });
  }

  /**
   * Làm mới token xác thực.
   * @param {string} refreshToken - Refresh token.
   * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
   */
  async refreshToken(refreshToken) {
    return apiClient.post("/Auth/refresh-token", { refreshToken });
  }

  /**
   * Đăng ký người dùng mới.
   * @param {string} username - Tên đăng nhập.
   * @param {string} password - Mật khẩu.
   * @param {string} displayName - Tên hiển thị.
   * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
   */
  async registerWithUsernameAndPassword(username, password, displayName) {
    return apiClient.post("/Auth/register", { userName: username, password: password, displayName: displayName });
  }

  /**
   * Yêu cầu gửi OTP qua email.
   * @param {string} email - Email nhận OTP.
   * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
   */
  async sendOtpVerificationEmail(email) {
    return apiClient.post("/Auth/send-otp", { email: email });
  }

  /**
   * Xác thực mã OTP.
   * @param {string} email - Email đã dùng để gửi OTP.
   * @param {string} otpCode - Mã OTP người dùng nhập.
   * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
   */
  async verifyOtp(email, otpCode) {
    console.log('[Repository] Reached verifyOtp with:', { email, otpCode });
    return apiClient.post("/Auth/verify-otp", { email: email, otpCode: otpCode });
  }

  /**
   * Xử lý yêu cầu quên mật khẩu.
   * @param {string} email - Email của tài khoản.
   * @param {string} username - Tên đăng nhập của tài khoản.
   * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
   */
  async forgotPassword(email, username) {
    return apiClient.post("/auth/forgot-password", { userName: username, email });
  }
  async linkSocialAccount(provider, token) {
    return apiClient.post("/Auth/link-social", { provider, token });
  }
}