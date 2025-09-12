<template>
  <div class="user-menu" @click="toggleMenu">
    <!-- Avatar + Tên User -->
    <div class="user-info">
      <img
        :src="authStore.user?.profilePictureUrl || defaultAvatar"
        alt="avatar"
        class="avatar"
      />
      <div class="user-text">
        <span class="name">
          {{ authStore.user?.displayName || authStore.user?.fullName || 'Guest' }}
        </span>
        <small class="rank">{{ userRank }}</small>
      </div>
    </div>

    <!-- Dropdown -->
    <div v-if="isOpen" class="dropdown">
      <ul>
        <!-- Chung cho cả Admin & User -->
        <li>
          <router-link to="/profile" @click="toggleMenu">Chỉnh sửa hồ sơ</router-link>
        </li>
        <li>
          <router-link to="/reset-password" @click="toggleMenu">Đổi mật khẩu</router-link>
        </li>

        <!-- Chỉ dành cho User -->
        <li v-if="!isAdmin">
          <router-link to="/achievements" @click="toggleMenu">Xem thành tích</router-link>
        </li>
        <li v-if="!isAdmin">
          <router-link to="/social-links" @click="toggleMenu">Liên kết tài khoản mạng xã hội</router-link>
        </li>

        <!-- Chỉ dành cho Admin -->
        <li v-if="isAdmin" class="admin-label">⚡ Quản trị viên</li>
        <li v-if="isAdmin">
          <router-link to="/admin/dashboard" @click="toggleMenu">Bảng điều khiển</router-link>
        </li>
        <li v-if="isAdmin">
          <router-link to="/admin/reports" @click="toggleMenu">Xem báo cáo</router-link>
        </li>

        <!-- Logout -->
        <li @click="logoutAndCloseMenu" class="logout">Đăng xuất</li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useAuthStore } from '@/application/stores/auth';
import defaultAvatar from '@/assets/logo.png'; // fallback avatar
import { useRouter } from 'vue-router';

const router = useRouter();
const authStore = useAuthStore();
const isOpen = ref(false);

const isAdmin = computed(() => authStore.role === 'Admin');
const userRank = computed(() => authStore.user?.rank || 'Bronze Priority');

function toggleMenu() {
  isOpen.value = !isOpen.value;
}

function logoutAndCloseMenu() {
  router.push('/introduce');
  authStore.logout();
  isOpen.value = false;
}
</script>

<style scoped>
.user-menu {
  position: relative;
  cursor: pointer;
  color: white;
}
.user-info {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  background: rgba(255, 255, 255, 0.1);
  padding: 0.4rem 0.8rem;
  border-radius: 8px;
}
.avatar {
  width: 34px;
  height: 34px;
  border-radius: 50%;
  background: #ddd;
  object-fit: cover;
}
.user-text {
  display: flex;
  flex-direction: column;
  line-height: 1.2;
}
.user-text .name {
  font-weight: bold;
  font-size: 0.95rem;
}
.user-text .rank {
  font-size: 0.75rem;
  color: gold;
}
.dropdown {
  position: absolute;
  right: 0;
  background: white;
  color: black;
  border-radius: 6px;
  box-shadow: 0 4px 10px rgba(0,0,0,0.15);
  margin-top: 0.5rem;
  min-width: 230px;
  z-index: 1000;
  overflow: hidden;
}
.dropdown ul {
  list-style: none;
  margin: 0;
  padding: 0;
}
.dropdown li {
  padding: 0.9rem 1rem;
  border-bottom: 1px solid #eee;
  transition: background 0.2s;
}
.dropdown li:hover {
  background: #f9f9f9;
}
.dropdown .logout {
  color: red;
  cursor: pointer;
}
.admin-label {
  font-weight: bold;
  background: #f5f5f5;
  padding: 0.6rem 1rem;
  font-size: 0.85rem;
  color: #333;
}
</style>
