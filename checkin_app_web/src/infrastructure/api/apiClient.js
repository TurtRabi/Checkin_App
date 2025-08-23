import axios from 'axios';
import { handleApiCall } from './apiHandler'; // Import a handler mới

const instance = axios.create({
  baseURL: 'http://localhost:5027/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

instance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor response cũ có thể được đơn giản hóa hoặc loại bỏ
// vì logic xử lý lỗi chi tiết đã được chuyển vào apiHandler.
instance.interceptors.response.use(
  (response) => response, // Chỉ trả về response cho các trường hợp thành công
  (error) => {
    // Xử lý lỗi 401 vẫn có thể hữu ích ở đây như một fallback cuối cùng
    if (error.response && error.response.status === 401) {
      console.error('UNAUTHORIZED ACCESS - 401. Redirecting to login.');
      localStorage.removeItem('authToken');
      // window.location.href = '/login';
    }
    // Vẫn reject để handleApiCall có thể bắt được
    return Promise.reject(error);
  }
);

/**
 * Xuất ra một phiên bản của apiClient đã được "bọc" bởi handler.
 * Mọi cuộc gọi qua apiClient giờ đây sẽ tự động có logic xử lý chuẩn.
 */
const apiClient = {
  async get(url, config) {
    return handleApiCall(instance.get(url, { ...config, validateStatus: () => true }));
  },
  async post(url, data, config) {
    return handleApiCall(instance.post(url, data, { ...config, validateStatus: () => true }));
  },
  async put(url, data, config) {
    return handleApiCall(instance.put(url, data, { ...config, validateStatus: () => true }));
  },
  async delete(url, config) {
    return handleApiCall(instance.delete(url, { ...config, validateStatus: () => true }));
  },
  async patch(url, data, config) {
    return handleApiCall(instance.patch(url, data, { ...config, validateStatus: () => true }));
  },
};

export default apiClient;

