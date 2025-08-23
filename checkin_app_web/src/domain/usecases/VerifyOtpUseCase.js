export default class VerifyOtpUseCase{
    constructor(authRepository) {
        this.authRepository = authRepository;
    }
    /**
     * Xác thực mã OTP.
     * @param {string} email - Địa chỉ email để xác thực mã OTP.
     * @param {string} otp - Mã OTP cần xác thực.
     * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là thông tin người dùng hoặc token).
     */
    async execute(email, otpCode) {
        console.log('Executing verifyOtpUseCase with email:', email, 'and otp:', otpCode);
        if (!email || !otpCode) {
            throw new Error("Email and OTP are required");
        }
        console.log('[UseCase] Attempting to call authRepository.verifyOtp');
        return await this.authRepository.verifyOtp(email, otpCode);
    }
}