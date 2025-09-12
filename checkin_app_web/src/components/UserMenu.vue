<template>
  <div class="user-menu">
    <!-- Avatar + Tên User -->
    <div class="user-info" @click="toggleMenu">
      <img
        :src="authStore.user?.profilePictureUrl || defaultAvatar"
        alt="avatar"
        class="avatar"
      />
      <div class="user-text">
        <span class="name">
          {{ authStore.user?.displayName || authStore.user?.fullName || "Guest" }}
        </span>
        <small class="rank">{{ userRank }}</small>
      </div>
      <span class="caret">▾</span>
    </div>

    <!-- Dropdown -->
    <transition name="fade">
      <div v-if="isOpen" class="dropdown">
        <div class="dropdown-arrow"></div>
        <ul>
          <!-- Hồ sơ -->
          <li>
            <router-link to="/profile/userProfile" @click="toggleMenu">📝 Chỉnh sửa hồ sơ</router-link>
          </li>
          <li>
            <router-link to="/settings" @click="toggleMenu">⚙️ Cài đặt</router-link>
          </li>

          <hr />

          <!-- User -->
          <li v-if="!isAdmin">
            <router-link to="/achievements" @click="toggleMenu">🏆 Thành tích</router-link>
          </li>

          <!-- Admin -->
          <template v-if="isAdmin">
            <li class="admin-label">⚡ Quản trị viên</li>
            <li>
              <router-link to="/admin/dashboard" @click="toggleMenu">📊 Bảng điều khiển</router-link>
            </li>
            <li>
              <router-link to="/admin/reports" @click="toggleMenu">📑 Xem báo cáo</router-link>
            </li>
          </template>

          <hr />

          <!-- Logout -->
          <li @click="logoutAndCloseMenu" class="logout">🚪 Đăng xuất</li>
        </ul>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import { useAuthStore } from "@/application/stores/auth";
import defaultAvatar from "@/assets/logo.png";
import { useRouter } from "vue-router";

const router = useRouter();
const authStore = useAuthStore();
const isOpen = ref(false);

const isAdmin = computed(() => authStore.role === "Admin");
const userRank = computed(() => authStore.user?.rank || "Bronze Priority");

function toggleMenu() {
  isOpen.value = !isOpen.value;
}

function logoutAndCloseMenu() {
  router.push("/introduce");
  authStore.logout();
  isOpen.value = false;
}
</script>

<style scoped>
.user-menu {
  position: relative;
  color: white;
}

/* Avatar + info */
.user-info {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  background: rgba(255, 255, 255, 0.15);
  padding: 0.4rem 0.8rem;
  border-radius: 8px;
  transition: background 0.2s;
  cursor: pointer;
}
.user-info:hover {
  background: rgba(255, 255, 255, 0.25);
}
.avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  object-fit: cover;
}
.user-text {
  display: flex;
  flex-direction: column;
  line-height: 1.2;
}
.user-text .name {
  font-weight: 600;
  font-size: 0.95rem;
}
.user-text .rank {
  font-size: 0.75rem;
  color: gold;
}
.caret {
  font-size: 0.8rem;
  margin-left: auto;
}

/* Dropdown */
.dropdown {
  position: absolute;
  right: 0;
  margin-top: 0.5rem;
  background: white;
  color: black;
  border-radius: 10px;
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.15);
  min-width: 250px;
  z-index: 1000;
  overflow: hidden;
  padding: 0.4rem 0;
}
.dropdown-arrow {
  position: absolute;
  top: -6px;
  right: 16px;
  width: 12px;
  height: 12px;
  background: white;
  transform: rotate(45deg);
  box-shadow: -2px -2px 2px rgba(0, 0, 0, 0.05);
}
.dropdown ul {
  list-style: none;
  margin: 0;
  padding: 0;
}
.dropdown li {
  padding: 0.8rem 1rem;
  transition: background 0.2s;
  font-size: 0.92rem;
  font-weight: 500;
}
.dropdown li:hover {
  background: #f9f9f9;
}
.dropdown li a {
  text-decoration: none;
  color: inherit;
  display: block;
}
.dropdown .logout {
  color: #e53935;
  font-weight: bold;
  cursor: pointer;
}
.admin-label {
  font-weight: bold;
  background: #f3f3f3;
  padding: 0.6rem 1rem;
  font-size: 0.85rem;
  color: #333;
}
hr {
  border: none;
  border-top: 1px solid #eee;
  margin: 0.3rem 0;
}

/* Animation */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s, transform 0.2s;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(-5px);
}
</style>
