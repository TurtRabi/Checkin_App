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
            <div class="otp-input-container">
              <input
                v-for="(digit, index) in otpDigits"
                :key="index"
                type="text"
                maxlength="1"
                @input="handleOtpInput(index, $event)"
                @keydown.backspace="handleBackspace(index, $event)"
                :ref="el => { if (el) otpInputs[index] = el }"
                class="otp-input form-input"
                required
              />
            </div>
          </div>
          <div class="modal-actions">
            <button type="button" @click="closeOtpModal" class="btn-secondary">Hủy</button>
            <button type="submit" class="btn-primary">Xác nhận</button>
          </div>
        </form>
        <div class="resend-otp">
          <button @click="handleResendOtp" :disabled="resendCooldown > 0" class="btn-link">
            Gửi lại OTP {{ resendCooldown > 0 ? `(${resendCooldown}s)` : '' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from "@/application/stores/auth";
import { GoogleLogin } from "vue3-google-login";
import { ref } from "vue";
import { useToast } from "vue-toastification";

export default {
  name: "LoginPage",
  components: {
    GoogleLogin,
  },
  setup() {
    const toast = useToast();
    return { toast };
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
      otpDigits: ['', '', '', '', '', ''],
      otpInputs: [],
      resendCooldown: 0,
      cooldownInterval: null,
    };
  },
  methods: {
    async handleLogin() {
      const authStore = useAuthStore();
      const rememberMe = document.getElementById('remember')?.checked || false;

      if (!this.username || !this.password) {
        this.toast.error("Vui lòng điền đầy đủ thông tin đăng nhập.");
        return;
      }

      try {
        const role = await authStore.handleEmailPasswordLogin(this.username, this.password, rememberMe);

        if (role) {
          this.toast.success("Đăng nhập thành công!");
          if (role === 'Admin') {
            this.$router.push({ name: 'AdminDashboard' });
          } else if (role === 'User') {
            this.$router.push({ name: 'Home' });
          } else {
            this.toast.info("Quyền của bạn không được hỗ trợ.");
            this.$router.push({ name: 'Introduce' });
          }
        } else {
          this.toast.error(authStore.error || "Đăng nhập thất bại. Vui lòng kiểm tra lại.");
        }
      } catch (error) {
        console.error("Lỗi trong quá trình đăng nhập:", error);
        this.toast.error(authStore.error || "Đã xảy ra lỗi. Vui lòng thử lại.");
      }
    },
    async handleCredentialResponse(response) {
      const authStore = useAuthStore();
      if (!response.credential) {
        console.error("Google response did not contain a credential.");
        this.toast.error("Không nhận được thông tin xác thực từ Google.");
        return;
      }

      try {
        const role = await authStore.handleGoogleLogin(response.credential);
        if (role) {
          this.toast.success("Đăng nhập bằng Google thành công!");
          if (role === 'Admin') {
            this.$router.push({ name: 'AdminDashboard' });
          } else if (role === 'User') {
            this.$router.push({ name: 'User' });
          } else {
            this.toast.info("Quyền của bạn không được hỗ trợ.");
            this.$router.push({ name: 'Introduce' });
          }
        } else {
          this.toast.error(authStore.error || "Xác thực với máy chủ thất bại.");
        }
      } catch (error) {
        console.error("Lỗi trong quá trình đăng nhập bằng Google:", error);
        this.toast.error(authStore.error || "Đăng nhập bằng Google thất bại.");
      }
    },
    openForgotPasswordModal() {
      this.showForgotPasswordModal = true;
    },
    closeForgotPasswordModal() {
      this.showForgotPasswordModal = false;
      this.showOtpModal = false;
    },
    async handleForgotPassword() {
      const authStore = useAuthStore();
      if (this.forgotPasswordEmail && this.forgotPasswordUserName) {
        try {
          const success = await authStore.forgotPassword(this.forgotPasswordEmail, this.forgotPasswordUserName);
          if (success) {
            this.closeForgotPasswordModal();
            this.showOtpModal = true;
            this.toast.success("Mã OTP đã được gửi đến email của bạn.");
            this.startResendCooldown();
          } else {
            this.toast.error(authStore.error || "Yêu cầu gửi OTP thất bại. Vui lòng thử lại.");
          }
        } catch (error) {
          console.error("Lỗi khi gửi OTP:", error);
          this.toast.error("Không thể gửi mã OTP. Vui lòng thử lại.");
        }
      } else {
        this.toast.error("Vui lòng nhập đầy đủ email và tên đăng nhập.");
      }
    },
    closeOtpModal() {
      this.showOtpModal = false;
      this.otp = "";
      this.otpDigits = ['', '', '', '', '', '']; // Clear otpDigits
      clearInterval(this.cooldownInterval);
      this.resendCooldown = 0;
    },
    handleOtpInput(index, event) {
      const value = event.target.value;
      if (value.length > 1) {
        this.otpDigits[index] = value.charAt(0); // Take only the first character
      } else {
        this.otpDigits[index] = value;
      }

      // Move focus to the next input if a digit is entered and it's not the last input
      if (value && index < this.otpDigits.length - 1) {
        this.otpInputs[index + 1].focus();
      }
    },
    handleBackspace(index, event) {
      // If the current input is empty and it's not the first input, move focus to the previous input
      if (!this.otpDigits[index] && index > 0) {
        this.otpInputs[index - 1].focus();
      }
    },
    handleVerifyOtp() {
      const authStore = useAuthStore();
      const fullOtp = this.otpDigits.join('');
      if (fullOtp && fullOtp.length === 6) {
        const success = authStore.verifyOtp(this.forgotPasswordEmail, fullOtp);
        if(success) {
          this.toast.success("Xác thực OTP thành công!");
          this.$router.push({ name: 'ResetPassword', params: { email: this.forgotPasswordEmail, username: this.forgotPasswordUserName } });
          this.forgotPasswordEmail = "";
          this.forgotPasswordUserName = "";
          this.otpDigits = ['', '', '', '', '', ''];
          this.closeOtpModal();
          // Here you can add logic to open a reset password modal if you wish
        } else {
          this.toast.error("Xác thực OTP thất bại. Vui lòng kiểm tra lại mã OTP.");
          this.otpDigits = ['', '', '', '', '', ''];
          this.otpInputs[0].focus(); 
        }
      } else {
        this.toast.error("Vui lòng nhập mã OTP gồm 6 chữ số.");
      }
    },
    handleResendOtp() {
      this.handleForgotPassword();
    },
    startResendCooldown() {
      this.resendCooldown = 60;
      this.cooldownInterval = setInterval(() => {
        if (this.resendCooldown > 0) {
          this.resendCooldown -= 1;
        } else {
          clearInterval(this.cooldownInterval);
        }
      }, 1000);
    },
  },
  beforeUnmount() {
    clearInterval(this.cooldownInterval);
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

/* OTP Input Styles */
.otp-input-container {
  display: flex;
  gap: 10px; /* Space between input boxes */
  justify-content: center;
  margin-top: 10px; /* Adjust as needed */
}

.otp-input {
  width: 40px; /* Width of each input box */
  height: 40px; /* Height of each input box */
  text-align: center;
  font-size: 1.2em;
  /* The form-input class already provides border, padding, etc. */
  /* Add any additional styling specific to OTP inputs here */
}

.resend-otp {
  margin-top: 1.5rem;
}

.btn-link {
  background: none;
  border: none;
  color: #4285F4;
  cursor: pointer;
  text-decoration: underline;
  font-size: 0.9rem;
  padding: 0;
}

.btn-link:disabled {
  color: #aaa;
  cursor: not-allowed;
  text-decoration: none;
}
</style>