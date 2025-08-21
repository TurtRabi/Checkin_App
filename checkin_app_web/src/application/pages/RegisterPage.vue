<template>
  <div class="login-container">
    <div class="login-card">
      <h2 class="login-title">Tạo tài khoản mới</h2>
      <p class="login-subtitle">Chào mừng bạn! Vui lòng điền thông tin để bắt đầu.</p>

      <form @submit.prevent="handleRegister" class="login-form">
        <div class="form-group">
          <label for="userName" class="form-label">Tên đăng nhập</label>
          <input
            type="text"
            id="userName"
            v-model="userName"
            class="form-input"
            required
            autocomplete="username"
          />
        </div>

        <div class="form-group">
          <label for="displayName" class="form-label">Tên hiển thị</label>
          <input
            type="text"
            id="displayName"
            v-model="displayName"
            class="form-input"
            required
            autocomplete="name"
          />
        </div>

        <div class="form-group">
          <label for="password" class="form-label">Mật khẩu</label>
          <input
            type="password"
            id="password"
            v-model="password"
            class="form-input"
            required
            autocomplete="new-password"
          />
        </div>

        <div class="form-group">
          <label for="repassword" class="form-label">Nhập lại mật khẩu</label>
          <input
            type="password"
            id="repassword"
            v-model="rePassword"
            class="form-input"
            required
            autocomplete="new-password"
          />
        </div>

        <button type="submit" class="login-button">Đăng ký</button>
      </form>

      <div class="divider">
        <span class="divider-text">Hoặc</span>
      </div>

      <div class="google-login-wrapper">
        <GoogleLogin :callback="handleCredentialResponse" />
      </div>

      <div class="login-footer">
        <p>Bạn đã có tài khoản? <router-link to="/login" class="register-link">Đăng nhập ngay</router-link></p>
      </div>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from "@/application/stores/auth";
import { GoogleLogin } from "vue3-google-login";

export default {
  name: "RegisterPage",
  components: {
    GoogleLogin,
  },
  data(){
    return {
      userName: '',
      displayName: '',
      password: '',
      rePassword: '',
    };
  },
  methods: {
    async handleRegister() {
      if (this.password !== this.rePassword) {
        alert("Mật khẩu không khớp!");
        return;
      }

      const authStore = useAuthStore();
      try {
        await authStore.registerWithUsernameAndPassword(this.userName, this.displayName, this.password);
        this.$router.push('/login');
      } catch (error) {
        console.error("Đăng ký thất bại:", error);
        alert("Đăng ký không thành công. Vui lòng thử lại.");
      }
    },
    async handleCredentialResponse(response) {
      const authStore = useAuthStore(); 
      if (response.credential) {
        try {
          await authStore.handleGoogleLogin(response.credential);
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
      } else {
        console.error("Google response did not contain a credential.");
        alert("Không nhận được thông tin xác thực từ Google. Vui lòng thử lại.");
      }
    },
  },
};
</script>

<style scoped>
/* Styles are copied from LoginPage.vue for consistency */
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap');

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
  border-top: 5px solid #4285F4;
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
  margin-top: 1rem; /* Add some space above the button */
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

.google-login-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 1rem; 
  width: 100%;
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
</style>
