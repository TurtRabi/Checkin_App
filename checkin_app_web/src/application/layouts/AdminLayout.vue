<template>
  <div class="admin-layout">
    <!-- Sidebar -->
    <aside class="sidebar">
      <h2 class="logo">Google Check-in</h2>
      <nav>
        <ul>
          <li><router-link to="/admin/dashboard">📊 Dashboard</router-link></li>
          <li><router-link to="/admin/users">👤 Users</router-link></li>
          <li><router-link to="/admin/landmarks">📍 Landmarks</router-link></li>
          <li><router-link to="/admin/missions">🎯 Missions & Rewards</router-link></li>
          <li><router-link to="/admin/checkins">📌 Check-ins</router-link></li>
          <li><router-link to="/admin/notifications">🔔 Notifications</router-link></li>

          <li>
            <details>
              <summary>📑 Reports</summary>
              <ul>
                <li><router-link to="/admin/reports/revenue">💰 Revenue</router-link></li>
                <li><router-link to="/admin/reports/users">👤 User Report</router-link></li>
                <li><router-link to="/admin/reports/checkins">📌 Check-ins</router-link></li>
                <li><router-link to="/admin/reports/landmarks">📍 Landmarks</router-link></li>
                <li><router-link to="/admin/reports/missions">🎯 Missions</router-link></li>
              </ul>
            </details>
          </li>

          <li><router-link to="/admin/settings">⚙ Settings</router-link></li>
        </ul>
      </nav>
    </aside>

    <!-- Main content -->
    <div class="main">
      <!-- Page content -->
      <main class="content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
// Không cần logic phức tạp ở layout, chỉ là khung
</script>

<style scoped>
.admin-layout {
  display: flex;
  min-height: 100vh;
  background: #f3f3f3;
}

/* Sidebar */
.sidebar {
  width: 240px;
  background: #00c46a;
  color: #fff;
  padding: 1rem;
}

.sidebar .logo {
  margin-bottom: 1.5rem;
  font-size: 1.4rem;
  font-weight: bold;
}

.sidebar ul {
  list-style: none;
  padding: 0;
}

.sidebar li {
  margin: 0.75rem 0;
}

.sidebar a {
  color: #fff;
  text-decoration: none;
  font-weight: 500;
}

.sidebar a.router-link-exact-active {
  text-decoration: underline;
}

/* Main area */
.main {
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* Topbar */
.topbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #00c46a;
  padding: 0.75rem 1.5rem;
  color: #fff;
}

.topbar .title {
  font-weight: bold;
  font-size: 1.2rem;
}

.topbar .topnav a {
  margin: 0 0.75rem;
  color: #fff;
  font-weight: 500;
  text-decoration: none;
}

.topbar .profile {
  background: rgba(255, 255, 255, 0.2);
  padding: 0.4rem 0.8rem;
  border-radius: 6px;
  text-align: right;
  font-size: 0.85rem;
}

.topbar .badge {
  display: block;
  font-size: 0.75rem;
  color: #ffd700;
}

/* Content */
.content {
  flex: 1;
  padding: 1.5rem;
}
</style>
