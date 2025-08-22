export default class forgotPassword{
    constructor(authRepository) {
        this.authRepository = authRepository;
    }
    /**
     * Xử lý quên mật khẩu.
     * @param {string} email - Địa chỉ email của người dùng.
     * @param {string} user - Tên người dùng hoặc thông tin cần thiết khác.
     * @returns {Promise<void>} Trả về một Promise khi việc xử lý quên mật khẩu hoàn tất.
     */
    async execute(email, user) {
        console.log("Executing forgot password with email:", email, "and user:", user);
        if (!email || !user) {
            throw new Error("Email and user information are required");
        }
        console.log("Executing forgot password for:", email, user);
        return await this.authRepository.forgotPassword(email, user);
    }
}