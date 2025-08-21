export default class RedisUseCase{
    constructor(redisRepository) {
        this.redisRepository = redisRepository;
    }
    /**
     * Lấy giá trị từ Redis theo key.
     * @param {string} key - Key để lấy giá trị từ Redis.
     * @returns {Promise<any>} Giá trị tương ứng với key trong Redis.
     */
    async getValue(key) {
        if (!key) {
            throw new Error("Key is required");
        }
        return await this.redisRepository.getValue(key);
    }
    /**
     * Lưu giá trị vào Redis với key và thời gian hết hạn.
     * @param {string} key - Key để lưu giá trị vào Redis.
     * @param {string} value - Giá trị cần lưu.
     * @param {number} expireInSeconds - Thời gian hết hạn tính bằng giây.
     * @returns {Promise<void>}
     */
    async setValue(key, value, expireInSeconds) {
        if (!key || !value || typeof expireInSeconds !== 'number') {
            throw new Error("Key, value and expireInSeconds are required");
        }
        return await this.redisRepository.setValue(key, value, expireInSeconds);
    }
    /**
     * Xóa giá trị khỏi Redis theo key.
     * @param {string} key - Key để xóa giá trị khỏi Redis.
     * @returns {Promise<void>}
     */
    async deleteValue(key) {
        if (!key) {
            throw new Error("Key is required");
        }
        try {
            await this.redisRepository.setValue(key, null, 0); // Set value to null with 0 expiration to delete it
        } catch (error) {
            console.error("Error deleting value from Redis:", error);
            throw error;
        }
    }
}