/**
 * Lớp này hoạt động như một Interface (bản thiết kế) cho tất cả các AuthRepository.
 * Nó định nghĩa các phương thức bắt buộc mà mọi lớp repository xác thực phải triển khai.
 * Nếu một lớp kế thừa từ IAuthRepository mà không triển khai các phương thức này,
 * một lỗi sẽ được ném ra khi phương thức đó được gọi.
 */
export default class IAuthRepository {
  /**
   * @param {string} googleToken The token received from Google Sign-In.
   * @returns {Promise<any>} A promise that resolves with user data and system token.
   */
  loginWithGoogle(googleToken) {
    throw new Error("AuthRepository must implement loginWithGoogle()");
  }

  /**
   * @param {string} email The user's email.
   * @param {string} password The user's password.
   * @returns {Promise<any>} A promise that resolves with user data and system token.
   */
  loginWithUsernameAndPassword(username, password) {
    throw new Error("AuthRepository must implement loginWithUsernameAndPassword()");
  }
   /**
   * @param {string} refreshToken The refresh token.
   * @returns {Promise<any>} A promise that resolves with new token data.
   */
  refreshToken(refreshToken) {
    throw new Error("AuthRepository must implement refreshToken()");
  }
  /**
   * @param {string} username The user's username.
   * @param {string} password The user's password.
   * @param {string} displayName The user's display name.
   * @returns {Promise<any>} A promise that resolves with user data and system token.
   */
  registerWithUsernameAndPassword(username, password, displayName) {
    throw new Error("AuthRepository must implement registerWithUsernameAndPassword()");
  }
  /**
   * @param {string} email The user's email.
   * @returns {Promise<void>} A promise that resolves when the OTP verification email is sent.
   */
  senOtpVerificationEmail(email) {
    throw new Error("AuthRepository must implement senOtpVerificationEmail()");
  }
  /**
   * @param {string} otp The one-time password to verify.
   * @returns {Promise<any>} A promise that resolves with verification result.
   */
  verifyOtp(email,otp) {
    throw new Error("AuthRepository must implement verifyOtp()");
  }

  forgotPassword(email,user) {
    throw new Error("AuthRepository must implement forgotPassword()");
  }
  linkSocialAccount(provider,token) {
    throw new Error("AuthRepository must implement linkSocialAccount()");
  }

}