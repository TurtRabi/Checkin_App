<template>
  <div class="profile-layout">
    <!-- Nếu là User thì hiện sidebar -->
    <aside v-if="isUser" class="sidebar">
      <h2 class="title">👤 Cá nhân</h2>
      <ul>
        <li><router-link to="/profile/userProfile">📄 Thông tin</router-link></li>
        <li><router-link to="/profile/achievements">🏆 Thành tích</router-link></li>
        <li><router-link to="/profile/transactions">💳 Giao dịch</router-link></li>
        <li><router-link to="/profile/blogs">✍️ Bài viết</router-link></li>
        <li><router-link to="/profile/news">📰 Tin tức</router-link></li>
      </ul>
    </aside>

    <!-- Nội dung chính -->
    <main class="main">
      <router-view v-slot="{ Component }">
        <Suspense timeout="0">
          <template #default>
            <component :is="Component"></component>
          </template>
          <template #fallback>
            <div>Đang tải trang...</div>
          </template>
        </Suspense>
      </router-view>
    </main>
  </div>
</template>

<script setup>
import { computed } from "vue";
import { useAuthStore } from "@/application/stores/auth";

const authStore = useAuthStore();

// Giả sử roleNames là mảng ["User", "VIP"] hoặc ["Admin"]
const isUser = computed(() => authStore.user?.roleNames.includes("User"));
</script>

<style scoped>
.profile-layout {
  display: flex;
  min-height: 100vh;
  background: #f4f6f9;
}

.sidebar {
  width: 240px;
  background: #fff;
  border-right: 1px solid #eee;
  padding: 1.2rem;
  box-shadow: 2px 0 6px rgba(0, 0, 0, 0.05);
}
.sidebar .title {
  margin-bottom: 1rem;
  font-size: 1.2rem;
  font-weight: bold;
  color: #00c46a;
}
.sidebar ul {
  list-style: none;
  padding: 0;
  margin: 0;
}
.sidebar li {
  margin: 0.8rem 0;
}
.sidebar a {
  text-decoration: none;
  color: #333;
  font-weight: 500;
}
.sidebar a.router-link-active {
  color: #00c46a;
}

.main {
  flex: 1;
  padding: 2rem;
}
</style>
