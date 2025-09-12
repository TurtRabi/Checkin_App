<template>   <div class="user-profile">
    <!-- Header -->
    <header class="profile-header">
      <img :src="user.profilePictureUrl || defaultAvatar" alt="Avatar" class="avatar" />
      <div class="info">
        <h2>{{ user.displayName }}</h2>
        <p class="email">{{ user.email }}</p>
        <p class="roles">🎭 {{ user.roleNames.join(', ') }}</p>
      </div>
    </header>

    <!-- Stats -->
    <section class="stats">
      <div class="stat-card">
        <h3>{{ user.coin }}</h3>
        <p>💰 Coins</p>
      </div>
      <div class="stat-card">
        <h3>{{ user.experiencePoints }}</h3>
        <p>⭐ Điểm kinh nghiệm</p>
      </div>
    </section>

    <!-- Meta -->
    <section class="meta">
      <p><strong>🆔 User ID:</strong> {{ user.userId }}</p>
      <p><strong>📅 Ngày tạo:</strong> {{ formatDate(user.createdAt) }}</p>
      <p><strong>🕒 Cập nhật:</strong> {{ formatDate(user.updatedAt) }}</p>
    </section>

    <!-- Actions -->
    <section class="actions">
      <button @click="openUpdateInfo">✏️ Cập nhật thông tin</button>
      <button @click="openChangePassword">🔑 Đổi mật khẩu</button>
      <button @click="openSocialLink">🌐 Liên kết MXH</button>
    </section>

    <!-- Modal Update Info -->
    <div v-if="showUpdateInfo" class="modal">
      <div class="modal-content">
        <h3>Cập nhật thông tin</h3>
        <label>Tên hiển thị</label>
        <input v-model="user.displayName" />
        <label>Email</label>
        <input v-model="user.email" />
        <label>Ảnh đại diện</label>
        <input type="file" @change="uploadAvatar" />
        <div class="modal-actions">
          <button @click="saveInfo">Lưu</button>
          <button @click="closeUpdateInfo">Hủy</button>
        </div>
      </div>
    </div>

    <!-- Modal Change Password -->
    <div v-if="showChangePassword" class="modal">
      <div class="modal-content">
        <h3>Đổi mật khẩu</h3>
        <label>Mật khẩu cũ</label>
        <input type="password" v-model="oldPassword" />
        <label>Mật khẩu mới</label>
        <input type="password" v-model="newPassword" />
        <div class="modal-actions">
          <button @click="changePassword">Đổi</button>
          <button @click="closeChangePassword">Hủy</button>
        </div>
      </div>
    </div>

    <!-- Modal Social Link -->
    <div v-if="showSocialLink" class="modal">
      <div class="modal-content">
        <h3>Liên kết tài khoản MXH</h3>
        <GoogleLogin :callback="handleCredentialResponse" />
        <div class="modal-actions">
          <button @click="closeSocialLink">Đóng</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref,onMounted } from "vue"
import { useUserStore } from "@/application/stores/user"
import { GoogleLogin } from "vue3-google-login"
import { linkSocialAccountUseCase } from "@/dependencies"
import { useToast } from "vue-toastification"

const toast = useToast()
const userStore = useUserStore()
const defaultAvatar = "/assets/default-avatar.png"
const user = ref({
  userId: "",
  displayName: "Guest",
  email: "",
  profilePictureUrl: defaultAvatar,
  createdAt: "",
  updatedAt: "",
  coin: 0,
  experiencePoints: 0,
  roleNames: []
})

onMounted(async () => {
  try {
    const userInfor = await userStore.handleGetMyUserInfo()
    console.log(userInfor)

    user.value = {
      userId: userInfor.data.data.userId,
      displayName: userInfor.data.data.displayName || "Guest",
      email: userInfor.data.data.email,
      profilePictureUrl: defaultAvatar,
      createdAt: userInfor.data.data.createdAt,
      updatedAt: userInfor.data.data.updatedAt,
      coin: userInfor.data.data.coin || 0,
      experiencePoints: userInfor.data.data.experiencePoints || 0,
      roleNames: userInfor.data.data.roleNames || []
    }
  } catch (err) {
    toast.error("Lỗi khi load user:", err)
  }
})

const handleCredentialResponse = async (credential) => {
  try {
    const res = await linkSocialAccountUseCase.execute('google', credential.credential);
    if (res.data.isSuccess) {
      toast.success('Liên kết tài khoản Google thành công!');
      closeSocialLink();
    } else {
      toast.error('Liên kết thất bại: ' + res.data.message);
    }
  } catch (error) {
    console.error('Lỗi khi liên kết tài khoản:', error);
    toast.error('Đã xảy ra lỗi trong quá trình liên kết.');
  }
};

function formatDate(dateStr) {
  if (!dateStr) return "—"
  const d = new Date(dateStr)
  return d.toLocaleString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit"
  })
}

// Modal controls
const showUpdateInfo = ref(false)
const showChangePassword = ref(false)
const showSocialLink = ref(false)

function openUpdateInfo() { showUpdateInfo.value = true }
function closeUpdateInfo() { showUpdateInfo.value = false }
function saveInfo() { toast.success("Thông tin đã cập nhật!"); closeUpdateInfo() }

function uploadAvatar(e) {
  const file = e.target.files[0]
  if (file) {
    user.value.profilePictureUrl = URL.createObjectURL(file)
  }
}

const oldPassword = ref("")
const newPassword = ref("")
function openChangePassword() { showChangePassword.value = true }
function closeChangePassword() { showChangePassword.value = false }
function changePassword() {
  toast.success("Mật khẩu đã đổi thành công!")
  closeChangePassword()
}

function openSocialLink() { showSocialLink.value = true }
function closeSocialLink() { showSocialLink.value = false }
</script>

<style scoped>
.user-profile {
  max-width: 700px;
  margin: 2rem auto;
  padding: 2rem;
  background: white;
  border-radius: 12px;
  box-shadow: 0 6px 16px rgba(0,0,0,0.08);
  font-family: 'Segoe UI', sans-serif;
}

.profile-header {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  border-bottom: 1px solid #eee;
  padding-bottom: 1rem;
  margin-bottom: 1.5rem;
}
.avatar {
  width: 90px;
  height: 90px;
  border-radius: 50%;
  object-fit: cover;
  border: 3px solid #00c46a;
}
.info h2 {
  margin: 0;
  font-size: 1.6rem;
  font-weight: bold;
  color: #222;
}
.info .email {
  margin: 0.2rem 0;
  color: #555;
  font-size: 0.95rem;
}
.roles {
  color: #ff7043;
  font-weight: 500;
}

/* Stats */
.stats {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}
.stat-card {
  flex: 1;
  text-align: center;
  background: #f9f9f9;
  border-radius: 10px;
  padding: 1rem;
  box-shadow: 0 2px 6px rgba(0,0,0,0.05);
}
.stat-card h3 {
  margin: 0;
  font-size: 1.4rem;
  color: #4285f4;
}
.stat-card p {
  margin: 0.2rem 0 0;
  font-size: 0.9rem;
  color: #777;
}

/* Meta */
.meta p {
  margin: 0.4rem 0;
  font-size: 0.95rem;
  color: #444;
}

/* Actions */
.actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
}
.actions button {
  flex: 1;
  padding: 0.6rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
}
.actions button:hover {
  opacity: 0.9;
}
.actions button:nth-child(1) { background: #42b983; color: white; }
.actions button:nth-child(2) { background: #ff7043; color: white; }
.actions button:nth-child(3) { background: #5c9ded; color: white; }

/* Modal */
.modal {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  animation: fadeIn 0.25s ease-in-out; 
}
.modal-content {
  background: #fff;
  padding: 2rem;
  border-radius: 16px;
  width: 420px;
  max-width: 90%;
  box-shadow: 0 8px 24px rgba(0,0,0,0.25); /* nổi bật hơn */
  animation: slideUp 0.3s ease;
  text-align: center;
}
.modal-content h3 {
  margin-bottom: 1.2rem;
  font-size: 1.5rem;
  font-weight: 700;
  color: #222;
  text-align: center;
}
.modal-content label {
  display: block;
  margin: 0.5rem 0 0.3rem;
  font-weight: 500;
  font-size: 0.9rem;
  color: #444;
}
.modal-content input {
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 0.95rem;
  transition: all 0.2s;
}
.modal-content input:focus {
  border-color: #42b983;
  outline: none;
  box-shadow: 0 0 6px rgba(66,185,131,0.4);
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.8rem;
  margin-top: 0.5rem;
}

.modal-actions button {
  min-width: 90px;
  padding: 0.6rem 1.2rem;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: 0.2s;
}

/* Nút Đổi */
.modal-actions button:first-child {
  

  color: white;
}
.modal-actions button:first-child:hover {
  background: #379a6e;
}

/* Nút Hủy */
.modal-act/* Nút liên kết MXH */ {
  background: #f5f5f5;
  color: #333;
}
.modal-actions button:last-child:hover {
  background: #e5e5e5;
}
/* Nút liên kết MXH */
.social {
  width: 100%;
  padding: 0.9rem;
  margin: 0.5rem 0;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease-in-out;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  color: #fff;
}
.social.google {
  background: #db4437;
}
.social.google:hover {
  background: #c23321;
  box-shadow: 0 4px 10px rgba(219,68,55,0.4);
}

.social.facebook {
  background: #3b5998;
}
.social.facebook:hover {
  background: #2d4373;
  box-shadow: 0 4px 10px rgba(59,89,152,0.4);
}


.modal-content input[type="file"]:hover {
  border-color: #42b983;
  background: #f3fff9;
}

.modal-content input[type="file"]:hover {
  border-color: #42b983;
  background: #f3fff9;
}
/* Hiệu ứng */
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from { transform: translateY(30px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}
</style>
