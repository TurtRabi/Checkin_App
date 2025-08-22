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
          <a href="#" @click.prevent="openForgotPasswordModal" class="forgot-password">Quên mật khẩu?</a>
        </div>

        <button type="submit" class="login-button">Đăng nhập</button>
      </form>

      <div class="divider">
        <span class="divider-text">Hoặc</span>
      </div>

      <div class="google-login-wrapper">
        <GoogleLogin :callback="handleCredentialResponse" />
      </div>

      <div class="login-footer">
        <p>Bạn chưa có tài khoản? <router-link to="/register" class="register-link">Đăng ký ngay</router-link></p>
      </div>
    </div>

    <!-- Forgot Password Modal -->
    <div v-if="showForgotPasswordModal" class="modal-overlay" @click.self="closeForgotPasswordModal">
      <div class="modal-content">
        <h3 class="modal-title">Quên mật khẩu</h3>
        <p class="modal-description">Vui lòng nhập địa chỉ email của bạn để nhận mã OTP.</p>
        <form @submit.prevent="handleForgotPassword">
          <div class="form-group">
            <label for="forgot-email" class="form-label">Email</label>
            <input type="email" id="forgot-email" v-model="forgotPasswordEmail" class="form-input" required />
            <label for="forgot-email" class="form-label">User Name</label>
            <input type="text" id="forgot-userName" v-model="forgotPasswordUserName" class="form-input" required />
          </div>
          <div class="modal-actions">
            <button type="button" @click="closeForgotPasswordModal" class="btn-secondary">Hủy</button>
            <button type="submit" class="btn-primary">Gửi</button>
          </div>
        </form>
      </div>
    </div>

    <!-- OTP Modal -->
    <div v-if="showOtpModal" class="modal-overlay" @click.self="closeOtpModal">
      <div class="modal-content">
        <h3 class="modal-title">Nhập mã OTP</h3>
        <p class="modal-description">Mã OTP gồm 6 chữ số đã được gửi đến email của bạn.</p>
        <form @submit.prevent="handleVerifyOtp">
          <div class="form-group">
            <label for="otp-input" class="form-label">Mã OTP</label>
            <input type="text" id="otp-input" v-model="otp" class="form-input" required maxlength="6" />
          </div>
          <div class="modal-actions">
            <button type="button" @click="closeOtpModal" class="btn-secondary">Hủy</button>
            <button type="submit" class="btn-primary">Xác nhận</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from "@/application/stores/auth";
import { GoogleLogin } from "vue3-google-login";
import { ref } from "vue";

export default {
  name: "LoginPage",
  components: {
    GoogleLogin,
  },

  data() {
    return {
      username: "",
      password: "",
      showPassword: false,
      showForgotPasswordModal: false,
      forgotPasswordEmail: "",
      forgotPasswordUserName: "",
      showOtpModal: false,
      otp: "",
    };
  },
  methods: {
    handleLogin() {
      const authStore = useAuthStore();
      const rememberMe = ref(false);
      if (this.username && this.password) {
        authStore.handleEmailPasswordLogin(this.username, this.password,rememberMe.value)
          .then(() => {
            if (authStore.isLoggedIn) {
              console.log("Đăng nhập thành công, đang chuyển hướng đến trang chủ...");
              this.$router.push({ name: 'Home' });
            } else {
              alert("Đăng nhập thất bại. Vui lòng kiểm tra thông tin đăng nhập.");
            }
          })
          .catch(error => {
            console.error("Lỗi trong quá trình đăng nhập:", error);
            alert("Đăng nhập thất bại. Vui lòng thử lại.");
          });
      } else {
        alert("Vui lòng điền đầy đủ thông tin đăng nhập.");
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
    openForgotPasswordModal() {
      this.showForgotPasswordModal = true;
    },
    closeForgotPasswordModal() {
      this.showForgotPasswordModal = false;
      this.forgotPasswordEmail = "";
      this.forgotPasswordUserName = "";
      this.showOtpModal = false;
    },
    handleForgotPassword() {
      const authStore = useAuthStore();
      if (this.forgotPasswordEmail) {
        authStore.forgotPassword(this.forgotPasswordEmail,this.forgotPasswordUserName)
          .then(() => {
            this.closeForgotPasswordModal();
            this.showOtpModal = true;
          })
          .catch(error => {
            console.error("Lỗi khi gửi OTP:", error);
            alert("Không thể gửi mã OTP. Vui lòng thử lại.");
          });
      } else {
        alert("Vui lòng nhập địa chỉ email.");
      }
    },
    closeOtpModal() {
      this.showOtpModal = false;
      this.otp = "";
    },
    handleVerifyOtp() {
      const authStore = useAuthStore();
      if (this.otp && this.otp.length === 6) {
        authStore.verifyOtp(this.forgotPasswordEmail, this.otp)
          .then((success) => {
            if (success) {
              alert("Xác thực OTP thành công!");
              this.closeOtpModal();
              // Here you would typically redirect to a password reset page
            } else {
              alert("Mã OTP không đúng. Vui lòng thử lại.");
            }
          })
          .catch(error => {
            console.error("Lỗi khi xác thực OTP:", error);
            alert("Có lỗi xảy ra trong quá trình xác thực OTP.");
          });
      } else {
        alert("Vui lòng nhập mã OTP gồm 6 chữ số.");
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

.google-login-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 1rem; 
  width: 100%;/* Thêm khoảng cách cho dễ nhìn */
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

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: #fff;
  padding: 30px;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
  text-align: center;
}

.modal-title {
  font-size: 1.8rem;
  margin-bottom: 1rem;
  color: #333;
}

.modal-description {
  font-size: 1rem;
  color: #666;
  margin-bottom: 2rem;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}

.btn-primary, .btn-secondary {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.btn-primary {
  background-color: #4285F4;
  color: #fff;
}

.btn-primary:hover {
  background-color: #357ae8;
}

.btn-secondary {
  background-color: #f0f0f0;
  color: #333;
}

.btn-secondary:hover {
  background-color: #e0e0e0;
}
</style>