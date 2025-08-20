<template>
  <div class="home-layout">
    <!-- Sidebar -->
    <aside class="sidebar">
      <h2 class="logo">Google Check In</h2>
      <nav>
        <ul>
          <li><a href="#">Dashboard</a></li>
          <li><a href="#">Users</a></li>
          <li><a href="#">Check-in</a></li>
          <li><a href="#">Reports</a></li>
          <li><a href="#">Settings</a></li>
        </ul>
      </nav>
    </aside>

    <!-- Main Content -->
    <main class="main-content">
      <!-- Header -->
      <header class="header">
        <h1>Welcome to Google Check In</h1>
        <button class="logout-btn">Logout</button>
      </header>

      <!-- Dashboard Blocks -->
      <section class="dashboard">
        <div class="card">User Management</div>
        <div class="card">Check-in Status</div>
        <div class="card">Reports</div>
        <div class="card">Notifications</div>
      </section>
    </main>
  </div>
</template>

<style scoped>
.home-layout {
  display: flex;
  height: 100vh;
  background: #f5f6fa;
}

/* Sidebar */
.sidebar {
  width: 220px;
  background: #4285f4;
  color: white;
  padding: 20px;
}
.sidebar .logo {
  margin-bottom: 20px;
  font-size: 20px;
  font-weight: bold;
}
.sidebar ul {
  list-style: none;
  padding: 0;
}
.sidebar li {
  margin-bottom: 15px;
}
.sidebar a {
  color: white;
  text-decoration: none;
  font-weight: 500;
}
.sidebar a:hover {
  text-decoration: underline;
}

/* Main Content */
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 20px;
}

/* Header */
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
.logout-btn {
  background: #e53935;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
}
.logout-btn:hover {
  background: #c62828;
}

/* Dashboard Blocks */
.dashboard {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 20px;
}
.card {
  background: white;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  text-align: center;
  font-weight: 500;
  color: #333;
}
</style>
