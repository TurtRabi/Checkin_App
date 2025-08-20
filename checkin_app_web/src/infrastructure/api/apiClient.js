import axios from 'axios';

/**
 * Đây là API client được cấu hình tập trung để giao tiếp với backend của bạn.
 * Mọi request ra ngoài từ ứng dụng nên sử dụng instance này.
 */
const apiClient = axios.create({
  /**
   * URL gốc của backend API.
   * Sử dụng biến môi trường là một practice tốt nhất, nhưng ở đây ta hardcode để làm ví dụ.
   * Ví dụ: http://localhost:5000/api
   */
  baseURL: 'http://localhost:5027/api/Auth',
  headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * Interceptor cho Request (Yêu cầu gửi đi)
 * Đoạn code này sẽ chạy TRƯỚC KHI một request được gửi đi.
 * Công dụng: Tự động đính kèm JWT token (nếu có) vào header Authorization.
 */
apiClient.interceptors.request.use(
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

/**
 * Interceptor cho Response (Phản hồi nhận về)
 * Đoạn code này sẽ chạy KHI nhận được phản hồi từ server.
 * Công dụng: Xử lý các lỗi chung một cách tập trung, ví dụ lỗi 401 Unauthorized.
 */
apiClient.interceptors.response.use(
  (response) => {
    // Bất kỳ status code nào trong khoảng 2xx sẽ đi vào đây
    return response;
  },
  (error) => {
    // Bất kỳ status code nào ngoài khoảng 2xx sẽ đi vào đây
    if (error.response && error.response.status === 401) {
      console.error('UNAUTHORIZED ACCESS - 401. Redirecting to login.');
      // Xóa token cũ và chuyển hướng người dùng về trang đăng nhập
      localStorage.removeItem('authToken');
      // Có thể dùng window.location hoặc router để chuyển hướng
      // window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
