export default class verifyOtp{
    constructor(authRepository) {
        this.authRepository = authRepository;
    }
    /**
     * Xác thực mã OTP.
     * @param {string} email - Địa chỉ email để xác thực mã OTP.
     * @param {string} otp - Mã OTP cần xác thực.
     * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là thông tin người dùng hoặc token).
     */
    async execute(email, otp) {
        if (!email || !otp) {
            throw new Error("Email and OTP are required");
        }
        return await this.authRepository.verifyOtp(email, otp);
    }
}