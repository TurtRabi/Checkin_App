<template>
  <div class="login-container">
    <div class="login-card">
      <h2 class="login-title">Đăng nhập tài khoản</h2>
      <p class="login-subtitle">Chào mừng bạn trở lại! Vui lòng điền thông tin để tiếp tục.</p>

      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="username" class="form-label">Tên đăng nhập hoặc Email</label>
          <input
            type="text"
            id="username"
            v-model="username"
            class="form-input"
            required
            autocomplete="username"
          />
        </div>

        <div class="form-group relative">
          <label for="password" class="form-label">Mật khẩu</label>
          <input
            :type="showPassword ? 'text' : 'password'"
            id="password"
            v-model="password"
            class="form-input pr-10"
            required
            autocomplete="current-password"
          />
          
        </div>

        <div class="form-actions">
          <div class="remember-me">
            <input type="checkbox" id="remember" name="remember" class="checkbox-input" />
            <label for="remember" class="checkbox-label">Ghi nhớ tôi</label>
          </div>
          <a href="#" class="forgot-password">Quên mật khẩu?</a>
        </div>

        <button type="submit" class="login-button">Đăng nhập</button>
      </form>

      <div class="divider">
        <span class="divider-text">Hoặc</span>
      </div>

      <button @click="handleGoogleLogin" class="google-login-button">
        <img
          src="https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/Google_%22G%22_logo.svg/100px-Google_%22G%22_logo.svg.png"
          alt="Google logo"
          class="google-logo"
        />
        Đăng nhập bằng Google
      </button>

      <div class="login-footer">
        <p>Bạn chưa có tài khoản? <a href="#" class="register-link">Đăng ký ngay</a></p>
      </div>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from "@/application/stores/auth";
import { useGoogleAuthService } from "@/infrastructure/services/GoogleAuthService";

export default {
  name: "LoginPage",
  data() {
    return {
      username: "",
      password: "",
      showPassword: false,
    };
  },
  methods: {
    handleLogin() {
      // Xử lý đăng nhập thông thường
      console.log("Đăng nhập với:", this.username, this.password);
    },
    async handleGoogleLogin() {
      const authStore = useAuthStore();
      const googleAuthService = useGoogleAuthService();

      try {
        console.log("Bắt đầu quy trình đăng nhập bằng Google...");
        const googleToken = await googleAuthService.login();
        await authStore.handleGoogleLogin(googleToken);
        if (authStore.isLoggedIn) {
          console.log("Đăng nhập bằng Google thành công, đang chuyển hướng đến trang chủ...");
          this.$router.push({ name: 'Home' });
        } else {
          alert("Xác thực với máy chủ thất bại. Vui lòng thử lại.");
        }
      } catch (error) {
        console.error("Lỗi trong quá trình đăng nhập bằng Google:", error);
        alert("Đăng nhập bằng Google thất bại. Vui lòng thử lại.");
      }
    },
  },
};
</script>

<style scoped>
/* Nhập font từ Google Fonts để giao diện chuyên nghiệp hơn */
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap');

/* Reset cơ bản và font chung */
body {
  font-family: 'Roboto', sans-serif;
  background-color: #f0f2f5;
  margin: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
}

.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  min-height: 100vh;
  padding: 20px;
  box-sizing: border-box;
}

.login-card {
  width: 100%;
  max-width: 420px;
  background-color: #fff;
  padding: 40px;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  box-sizing: border-box;
  text-align: center;
  border-top: 5px solid #4285F4; /* Thêm điểm nhấn màu Google */
}

.login-title {
  font-size: 2rem;
  color: #333;
  margin-bottom: 0.5rem;
  font-weight: 700;
}

.login-subtitle {
  font-size: 1rem;
  color: #666;
  margin-bottom: 2rem;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  text-align: left;
}

.form-label {
  display: block;
  margin-bottom: 8px;
  color: #555;
  font-weight: 500;
}

.form-input {
  width: 100%;
  padding: 12px 15px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.3s ease, box-shadow 0.3s ease;
}

.form-input:focus {
  outline: none;
  border-color: #4285F4;
  box-shadow: 0 0 0 3px rgba(66, 133, 244, 0.2);
}

.form-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.9rem;
}

.remember-me {
  display: flex;
  align-items: center;
  gap: 8px;
}

.checkbox-input {
  cursor: pointer;
}

.checkbox-label {
  color: #666;
}

.forgot-password {
  color: #4285F4;
  text-decoration: none;
  transition: color 0.3s ease;
}

.forgot-password:hover {
  text-decoration: underline;
}

.login-button {
  width: 100%;
  padding: 12px;
  background-color: #4285F4;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 1.1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.3s ease, transform 0.1s ease;
}

.login-button:hover {
  background-color: #357ae8;
}

.login-button:active {
  transform: scale(0.99);
}

/* Kiểu cho dải phân cách */
.divider {
  display: flex;
  align-items: center;
  text-align: center;
  margin: 2rem 0 1.5rem;
  color: #aaa;
}

.divider::before,
.divider::after {
  content: '';
  flex: 1;
  border-bottom: 1px solid #ddd;
}

.divider:not(:empty)::before {
  margin-right: .5em;
}

.divider:not(:empty)::after {
  margin-left: .5em;
}

/* Kiểu cho nút đăng nhập bằng Google */
.google-login-button {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background-color: #fff;
  color: #555;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.3s ease, border-color 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.google-login-button:hover {
  background-color: #f0f0f0;
  border-color: #ccc;
}

.google-login-button:active {
  transform: scale(0.99);
}

.google-logo {
  width: 20px;
  height: 20px;
}

.login-footer {
  margin-top: 2rem;
  font-size: 0.9rem;
  color: #666;
}

.register-link {
  color: #4285F4;
  text-decoration: none;
  font-weight: 500;
  transition: color 0.3s ease;
}

.register-link:hover {
  text-decoration: underline;
}

@media (max-width: 480px) {
  .login-card {
    padding: 30px 20px;
  }
}
.eye-icon {
  position: absolute;
  right: 12px;
  top: 68%;
  transform: translateY(-50%);
  cursor: pointer;
  color: #666;
}

.eye-icon:hover {
  color: #4285F4;
}
</style>