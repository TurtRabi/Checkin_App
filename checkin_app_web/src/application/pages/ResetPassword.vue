<template>
  <div class="reset-password-container">
    <div class="reset-password-card">
      <h2 class="reset-password-title">Tạo mật khẩu mới</h2>
      <p class="reset-password-subtitle">
        Tạo mật khẩu mới cho tài khoản: <strong>{{ username }}</strong>
      </p>

      <form @submit.prevent="handleResetPassword" class="reset-password-form">
        <div class="form-group">
          <label for="new-password" class="form-label">Mật khẩu mới</label>
          <input
            type="password"
            id="new-password"
            v-model="newPassword"
            class="form-input"
            required
          />
        </div>

        <div class="form-group">
          <label for="confirm-password" class="form-label">Xác nhận mật khẩu mới</label>
          <input
            type="password"
            id="confirm-password"
            v-model="confirmPassword"
            class="form-input"
            required
          />
        </div>

        <button type="submit" class="reset-password-button" :disabled="isLoading">
          <span v-if="isLoading">Đang xử lý...</span>
          <span v-else>Lưu mật khẩu</span>
        </button>
      </form>
    </div>
  </div>
</template>

<script>
import { useUserStore } from "../stores/user";
import { useToast } from "vue-toastification";

export default {
  name: "ResetPasswordPage",
  props: ["email", "username"],
  data() {
    return {
      newPassword: "",
      confirmPassword: "",
    };
  },
  computed: {
    isLoading() {
      const store = useUserStore();
      return store.isLoading;
    },
  },
  methods: {
    async handleResetPassword() {
      const userStore = useUserStore();
      const toast = useToast();

      if (this.newPassword !== this.confirmPassword) {
        toast.error("Mật khẩu mới và xác nhận mật khẩu không khớp.");
        return;
      }
      if (this.newPassword.length < 6) {
        toast.error("Mật khẩu mới phải có ít nhất 6 ký tự.");
        return;
      }
      const success = await userStore.handleChangePassword(
        this.email,
        this.username,
        this.newPassword
      );

      if (success) {
        toast.success("Mật khẩu đã được thay đổi thành công! Vui lòng đăng nhập lại.");
        this.$router.push({ name: "Login" });
      } else {
        console.error("Password reset failed:", userStore.error);
        toast.error(userStore.error || "Không thể đổi mật khẩu. Vui lòng thử lại.");
      }
    },
  },
  created() {
    const toast = useToast();
    console.log("ResetPasswordPage created with email:", this.email, "and username:", this.username);
    if (!this.email || !this.username) {
      toast.error("Đường dẫn không hợp lệ. Vui lòng thử lại từ đầu.");
      this.$router.push({ name: "Login" });
    }
  },
};
</script>


<style scoped>
.reset-password-container {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  min-height: 100vh;
  padding: 20px;
  background-color: #f0f2f5;
}

.reset-password-card {
  width: 100%;
  max-width: 420px;
  background-color: #fff;
  padding: 40px;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.reset-password-title {
  font-size: 2rem;
  margin-bottom: 1rem;
}

.reset-password-subtitle {
  margin-bottom: 2rem;
  color: #666;
}

.reset-password-form {
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
  font-weight: 500;
}

.form-input {
  width: 100%;
  padding: 12px 15px;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.reset-password-button {
  padding: 12px;
  background-color: #4285F4;
  color: #fff;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.reset-password-button:hover {
  background-color: #357ae8;
}

.reset-password-button:disabled {
  background-color: #aaa;
  cursor: not-allowed;
}
</style>
