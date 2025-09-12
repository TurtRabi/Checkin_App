<template>
  <div class="home-layout">
    <!-- Sidebar -->
    <aside class="sidebar">
      <h2 class="logo">Google Check-in</h2>
      <nav>
        <ul>
          <li><router-link to="/admin/dashboard">📊 Dashboard</router-link></li>
          <li><router-link to="/admin/users">👤 Users</router-link></li>
          <li><router-link to="/admin/landmarks">📍 Landmarks</router-link></li>
          <li><router-link to="/admin/missions">🎯 Missions & Rewards</router-link></li>

          <li class="submenu">
            <div class="submenu-header" @click="isReportsOpen = !isReportsOpen">
              <span>📑 Reports</span>
              <span>{{ isReportsOpen ? "▲" : "▼" }}</span>
            </div>
            <ul v-show="isReportsOpen">
              <li><router-link to="/admin/reports/revenue">💰 Báo cáo Doanh thu</router-link></li>
              <li><router-link to="/admin/reports/users">👤 Báo cáo User</router-link></li>
              <li><router-link to="/admin/reports/checkins">✅ Báo cáo Check-in</router-link></li>
              <li><router-link to="/admin/reports/landmarks">📍 Báo cáo Landmark</router-link></li>
              <li><router-link to="/admin/reports/missions">🎯 Báo cáo Mission</router-link></li>
            </ul>
          </li>

          <li><router-link to="/admin/settings">⚙ Settings</router-link></li>
        </ul>
      </nav>
    </aside>

    <!-- Main content của từng trang admin -->
    <main class="main-content">
      <router-view /> 
    </main>
  </div>
</template>

<script setup>
import { ref } from "vue";
const isReportsOpen = ref(false);
</script>

<style scoped>
.home-layout {
  display: flex;
  min-height: 100vh;
  background: #f5f6fa;
  font-family: "Segoe UI", sans-serif;
}

/* Sidebar */
.sidebar {
  width: 240px;
  background: #00ca6c;
  color: white;
  padding: 20px;
}
.sidebar .logo {
  margin-bottom: 25px;
  font-size: 22px;
  font-weight: bold;
}
.sidebar ul {
  list-style: none;
  padding: 0;
}
.sidebar li {
  margin-bottom: 18px;
}
.sidebar a {
  color: white;
  text-decoration: none;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 15px;
}
.sidebar a:hover {
  text-decoration: underline;
}

/* Submenu Reports */
.submenu {
  margin-bottom: 10px;
}
.submenu-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  font-size: 15px;
  color: white;
  font-weight: 500;
  padding: 5px 0;
}
.submenu ul {
  list-style: none;
  padding-left: 20px;
  margin-top: 8px;
}
.submenu ul li {
  margin-bottom: 8px;
}
.submenu ul li a {
  font-size: 14px;
  color: #f1f1f1;
  text-decoration: none;
}
.submenu ul li a:hover {
  text-decoration: underline;
}

/* Main Content */
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 20px;
}
.header {
  margin-bottom: 20px;
}
.header h1 {
  margin: 0;
  font-size: 28px;
  font-weight: bold;
  color: #333;
}
.header .subtitle {
  margin-top: 4px;
  font-size: 14px;
  color: #666;
}

/* Stats cards */
.stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}
.stat-card {
  background: white;
  padding: 20px;
  border-radius: 12px;
  text-align: center;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.08);
}
.stat-card h2 {
  margin: 0;
  font-size: 22px;
  color: #4285f4;
}
.stat-card p {
  margin: 5px 0 0;
  font-size: 14px;
  color: #555;
}
.stat-card.highlight {
  border: 2px solid #ff7043;
}

/* Charts */
.charts {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}
.chart-box {
  background: white;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  display: flex;
  flex-direction: column;
  height: auto;
  min-height: 350px;
}
.chart-box.full-width {
  grid-column: span 2;
  height: auto;
}
.chart-box h3 {
  margin-bottom: 10px;
  font-size: 16px;
  font-weight: 600;
  color: #444;
}
.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.chart-filter {
  padding: 4px 8px;
  border-radius: 6px;
  border: 1px solid #ccc;
}

/* Responsive */
@media (max-width: 900px) {
  .charts {
    grid-template-columns: 1fr;
  }
  .chart-box.full-width {
    grid-column: span 1;
  }
}
</style>
