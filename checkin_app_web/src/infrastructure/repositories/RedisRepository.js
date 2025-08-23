import IRedisRepository from '@/domain/repositories/IRedisRepository';
import apiClient from '@/infrastructure/api/apiClient';

/**
 * Repository để tương tác với các endpoint liên quan đến Redis.
 * Logic xử lý lỗi và chuẩn hóa response đã được chuyển vào apiClient.
 */
export default class RedisRepository extends IRedisRepository {
    /**
     * Lấy giá trị từ Redis theo key.
     * @param {string} key - Key cần lấy giá trị.
     * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
     */
    async getValue(key) {
        return apiClient.get(`/Redis/${key}`);
    }

    /**
     * Lưu giá trị vào Redis với key và thời gian hết hạn.
     * @param {string} key - Key để lưu giá trị.
     * @param {string} value - Giá trị cần lưu.
     * @param {number} expireInSeconds - Thời gian hết hạn tính bằng giây.
     * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
     */
    async setValue(key, value, expireInSeconds) {
        return apiClient.post("/Redis", { key, value, expireInSeconds });
    }

    /**
     * Xóa một key khỏi Redis.
     * @param {string} key - Key cần xóa.
     * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>}
     */
    async deleteValue(key) {
        return apiClient.delete(`/Redis/${key}`);
    }
}
