export default class IRedisRepository {
    /**
     * @param {string} key The key to set in Redis.
     * @param {any} value The value to associate with the key.
     * @returns {Promise<void>} A promise that resolves when the value is set.
     */
    set(key, value) {
        throw new Error("IRedisRepository must implement set()");
    }
    /**
     * @param {string} key The key to retrieve from Redis.
     * @returns {Promise<any>} A promise that resolves with the value associated with the key.
     */
    get(key) {
        throw new Error("IRedisRepository must implement get()");
    }
    /**
     * @param {string} key The key to delete from Redis.
     * @returns {Promise<void>} A promise that resolves when the key is deleted.
     */
    delete(key) {
        throw new Error("IRedisRepository must implement delete()");
    }
    
}