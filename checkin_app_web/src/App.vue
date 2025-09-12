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

    // onMounted là hook được gọi sau khi component đã được render lần đầu
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

<style scoped>
#app{
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
}
.chart-box.full-width {
  grid-column: span 2;
  height: 400px;
  overflow: hidden;  /* chặn canvas tràn */
}
</style>
