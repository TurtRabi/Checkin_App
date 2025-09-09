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
        <small class="rank">Bronze Priority</small>
      </div>
    </div>

    <!-- Dropdown -->
    <div v-if="isOpen" class="dropdown">
      <ul>
        <li class="points"><span>0 Điểm</span></li>
        <li><router-link to="/profile">Chỉnh sửa hồ sơ</router-link></li>
        <li><router-link to="/my-cards">Thẻ của tôi</router-link></li>
        <li><router-link to="/transactions">Danh sách giao dịch</router-link></li>
        <li><router-link to="/bookings">Đặt chỗ của tôi</router-link></li>
        <li>
          <router-link to="/refunds">
            Hoàn tiền <span class="new">New!</span>
          </router-link>
        </li>
        <li><router-link to="/notifications">Thông báo giá vé máy bay</router-link></li>
        <li><router-link to="/saved-passengers">Thông tin hành khách đã lưu</router-link></li>
        <li><router-link to="/promotions">Khuyến mãi</router-link></li>
        <li @click="logout" class="logout">Đăng xuất</li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '@/application/stores/auth';
import defaultAvatar from '@/assets/logo.png'; // fallback avatar

const authStore = useAuthStore();
const isOpen = ref(false);

function toggleMenu() {
  isOpen.value = !isOpen.value;
}

function logout() {
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

.dropdown .points {
  font-weight: bold;
  background: #f1f1f1;
}

.dropdown .logout {
  color: red;
  cursor: pointer;
}

.new {
  background: yellow;
  color: red;
  font-size: 0.7rem;
  padding: 2px 5px;
  border-radius: 3px;
  margin-left: 5px;
}
</style>
