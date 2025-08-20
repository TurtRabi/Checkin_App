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

  // Trong tương lai, bạn có thể thêm các phương thức khác ở đây
  // ví dụ: logout(), loginWithEmailAndPassword(), refreshToken()
}