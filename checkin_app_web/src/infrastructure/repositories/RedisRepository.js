import IRedisRepository from '@/domain/repositories/IRedisRepository';
import apiClient from '@/infrastructure/api/apiClient';
export default class RedisRepository extends IRedisRepository {
    /**
     * Lấy giá trị từ Redis theo key.
     * @param {string} key - Key cần lấy giá trị.
     * @returns {Promise<string|null>} Giá trị tương ứng với key, hoặc null nếu không tìm thấy.
     */
    async getValue(key) {
        try {
        const response = await apiClient.get("/Redis/" + key);
        return response.data;
        } catch (error) {
        console.error("Lỗi khi lấy giá trị từ Redis:", error);
        throw error;
        }
    }
    /**
     * Lưu giá trị vào Redis với key và thời gian hết hạn.
     * @param {string} key - Key để lưu giá trị.
     * @param {string} value - Giá trị cần lưu.
     * @param {number} expireInSeconds - Thời gian hết hạn tính bằng giây.
     * @returns {Promise<void>}
     */
    async setValue(key, value, expireInSeconds) {
        try {
            await apiClient.post("/Redis", { key, value, expireInSeconds });
        } catch (error) {
            console.error("Lỗi khi lưu giá trị vào Redis:", error);
            throw error;
        }
    }
}