export default class SendOtpEmailUseCase {
    constructor(authRepository) {
        this.authRepository = authRepository;
    }
    /**
     * Gửi mã OTP qua email.
     * @param {string} email - Địa chỉ email để gửi mã OTP.
     * @returns {Promise<void>} Trả về một Promise khi việc gửi email hoàn tất.
     */
    async execute(email) {
        if (!email) {
            throw new Error("Email is required");
        }
        return await this.authRepository.sendOtpVerificationEmail(email);
    }
}