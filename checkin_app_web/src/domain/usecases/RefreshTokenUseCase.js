export default class RefreshTokenUseCase {
    constructor(authRepository) {
        this.authRepository = authRepository;
    }
    /**
     * Thực hiện làm mới token xác thực bằng refresh token.
     * @param {string} refreshToken - Refresh token để lấy token mới.
     * @returns {Promise<any>} Dữ liệu trả về từ backend (thường là token mới).
     */
    async execute(refreshToken) {
        if (!refreshToken) {
            throw new Error("Refresh token is required");
        }
        return await this.authRepository.refreshToken(refreshToken);
    }
    async linkSocialAccount(provider, token) {
        if (!provider || !token) {
            throw new Error("Provider and token are required");
        }
        return await this.authRepository.linkSocialAccount(provider, token);
    }
}