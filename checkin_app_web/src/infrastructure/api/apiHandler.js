/**
 * Xử lý một cuộc gọi API chung và chuẩn hóa kết quả trả về.
 * @param {Promise<import('axios').AxiosResponse>} apiCallPromise - Promise từ apiClient (ví dụ: apiClient.post(...)).
 * @param {string} [customErrorMessage='Một lỗi không mong muốn đã xảy ra.'] - Tin nhắn lỗi tùy chỉnh nếu không có tin nhắn từ server.
 * @returns {Promise<{isSuccess: boolean, data: any, message?: string, statusCode?: number}>} - Một đối tượng kết quả được chuẩn hóa.
 */
export async function handleApiCall(apiCallPromise, customErrorMessage = 'Một lỗi không mong muốn đã xảy ra.') {
  try {
    // apiCallPromise nên được gọi với { validateStatus: () => true }
    const res = await apiCallPromise;

    if (res.status >= 200 && res.status < 300) {
      return { isSuccess: true, data: res.data, statusCode: res.status };
    }

    // Cố gắng lấy message lỗi từ nhiều định dạng response khác nhau
    const errorMessage = res.data?.message || res.data?.title || res.data?.errors?.[0]?.msg || customErrorMessage;

    return {
      isSuccess: false,
      message: errorMessage,
      statusCode: res.status,
    };
  } catch (err) {
    // Lỗi mạng, CORS, timeout, hoặc lỗi client-side trước khi request được gửi
    return {
      isSuccess: false,
      message: err.message || 'Lỗi kết nối hoặc sự cố client-side.',
      statusCode: 0, // 0 cho biết lỗi không phải từ server response
    };
  }
}

/**
 * Tạo một instance của apiClient đã được cấu hình sẵn để tự động xử lý validateStatus.
 * Điều này giúp tránh việc phải lặp lại { validateStatus: () => true } ở mỗi lần gọi.
 * @param {import('axios').AxiosInstance} apiClientInstance - Instance của apiClient.
 * @returns {import('axios').AxiosInstance}
 */
export function createApiHandler(apiClientInstance) {
    const handler = {
        get: (target, prop, receiver) => {
            if (['get', 'post', 'put', 'delete', 'patch'].includes(prop)) {
                return (url, data, config) => {
                    const requestConfig = { ...config, validateStatus: () => true };
                    const apiPromise = target[prop](url, data, requestConfig);
                    return handleApiCall(apiPromise);
                };
            }
            return Reflect.get(target, prop, receiver);
        }
    };

    return new Proxy(apiClientInstance, handler);
}
