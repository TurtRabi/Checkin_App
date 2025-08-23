export default class IRedisRepository {
    /**
     * Lấy giá trị từ Redis theo key.
     * @param {string} key - Key cần lấy giá trị.
     * @returns {Promise<any>}
     */
    getValue(key) {
        throw new Error("IRedisRepository must implement getValue()");
    }

    /**
     * Lưu giá trị vào Redis.
     * @param {string} key - Key để lưu.
     * @param {any} value - Giá trị để lưu.
     * @param {number} expireInSeconds - Thời gian hết hạn (giây).
     * @returns {Promise<void>}
     */
    setValue(key, value, expireInSeconds) {
        throw new Error("IRedisRepository must implement setValue()");
    }

    /**
     * Xóa một key khỏi Redis.
     * @param {string} key - Key cần xóa.
     * @returns {Promise<void>}
     */
    deleteValue(key) {
        throw new Error("IRedisRepository must implement deleteValue()");
    }
}
