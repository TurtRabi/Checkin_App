<script>
import { onMounted } from 'vue';
import { useAuthStore } from '@/application/stores/auth';
import Header from './components/Header.vue';
import Footer from './components/Footer.vue';

export default {
  name: 'App',
  components: {
    Header,
    Footer
  },
  setup() {
    const authStore = useAuthStore();

    onMounted(() => {
      authStore.tryAutoLogin();
    });

    return {};
  }
}
</script>

<template>
  <div id="app">
    <Header />
    <main class="main-content">
      <router-view />
    </main>
    <Footer />
  </div>
</template>

<style>
/* reset global margin/padding */
html, body {
  margin: 0;
  padding: 0;
  height: 100%;
}

/* App container */
#app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background: #f3f3f3; /* nền mặc định */
}

/* main content fill toàn bộ */
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
}

/* fix chart box */
.chart-box.full-width {
  grid-column: span 2;
  height: 400px;
  overflow: hidden; /* tránh canvas tràn */
}
</style>
