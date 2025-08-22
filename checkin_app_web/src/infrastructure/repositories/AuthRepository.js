import IAuthRepository from "@/domain/repositories/IAuthRepository";
import apiClient from "@/infrastructure/api/apiClient";

export default class AuthRepository extends IAuthRepository {
  /**
   * Gửi Google token lên backend để xác thực và lấy về thông tin người dùng cùng JWT token của hệ thống.
   * @param {string} googleToken - Token nhận được từ Google.
   * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là user info và token).
   */
  async loginWithGoogle(googleToken) {
    try {
      const response = await apiClient.post("/Auth/social-login", { provider: 'Google',devide:'Web',token: googleToken });
      console.log(response.data);
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.loginWithGoogle:",
        error.response?.data || error.message
      );
      // Ném lỗi để các tầng cao hơn (use case, store) có thể bắt và xử lý (vd: hiển thị thông báo cho người dùng)
      throw error;
    }
  }
  /**
   * Gửi thông tin đăng nhập (username và password) lên backend để xác thực.
   * @param {string} username - Tên đăng nhập của người dùng.
   * @param {string} password - Mật khẩu của người dùng.
   * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là user info và token).
   */
  async loginWithUsernameAndPassword(username, password) {
    try{

      const response = await apiClient.post("/Auth/login", {userName: username,password: password,deviceName: 'Web'});
      console.log(response.data);
      return response.data;
    }catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.loginWithUsernameAndPassword:",
        error.response?.data || error.message
      );
      // Ném lỗi để các tầng cao hơn (use case, store) có thể bắt và xử lý (vd: hiển thị thông báo cho người dùng)
      throw error;
    }
  }
  /**
   * Làm mới token xác thực bằng refresh token.
   * @param {string} refreshToken - Refresh token để lấy token mới.
   * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là token mới).
   */
  async refreshToken(refreshToken) {
    try {
      const response = await apiClient.post("Auth/refresh-token", { refreshToken });
      console.log(response.data);
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.refreshToken:",
        error.response?.data || error.message
      );
      // Ném lỗi để các tầng cao hơn (use case, store) có thể bắt và xử lý (vd: hiển thị thông báo cho người dùng)
      throw error;
    }
  }
  /**
   * Đăng ký người dùng mới với tên đăng nhập và mật khẩu.
   * @param {string} username - Tên đăng nhập của người dùng.
   * @param {string} password - Mật khẩu của người dùng.
   * @param {string} displayName - Tên hiển thị của người dùng.
   * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là user info và token).
   */
  async registerWithUsernameAndPassword(username, password, displayName) {
    try {
      const response = await apiClient.post("/Auth/register", {
        userName: username,
        password: password,
        displayName: displayName
      });
      console.log(response.data);
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.registerWithUsernameAndPassword:",
        error.response?.data || error.message
      );
      // Ném lỗi để các tầng cao hơn (use case, store) có thể bắt và xử lý (vd: hiển thị thông báo cho người dùng)
      throw error;
    }
  }
  /**
   * Gửi email xác thực OTP đến người dùng.
   * @param {string} email - Email của người dùng để gửi OTP.
   * @returns {Promise<void>} Promise khi gửi email thành công.
   */
  async sendOtpVerificationEmail(email) {
    try {
      const response = await apiClient.post("/Auth/send-otp", { email:email });
      console.log(response.data);
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.senOtpVerificationEmail:",
        error.response?.data || error.message
      );
      throw error;
    }
  }
 
  /**
   * Xác thực OTP do người dùng cung cấp.
   * @param {string} otp - Mã OTP do người dùng cung cấp.
   * @param {string} email - Email của người dùng để xác thực OTP.
   * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là kết quả xác thực).
   */
  async verifyOtp( email, otpCode) {
    try {
      const response = await apiClient.post("/Auth/verify-otp", { email: email, otpCode: otpCode });
      console.log(response.data);
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.verifyOtp:",
        error.response?.data || error.message
      );
      throw error;
    }
  }
  async forgotPassword(email,user) {
    try {
      const response = await apiClient.post("/Auth/forgot-password", { userName: user,email: email  });
      console.log(response.data);
      return response.data;
    } catch (error) {
      console.error(
        "Lỗi cụ thể tại AuthRepository.forgotPassword:",
        error.response?.data || error.message
      );
      throw error;
    }
  }
}
