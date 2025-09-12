<template>
  <div class="dashboard-page">
    <!-- Header -->
    <header class="header">
      <h1>📊 Dashboard</h1>
      <p class="subtitle">Tổng quan hệ thống & thống kê</p>
    </header>

    <!-- Dashboard Stats -->
    <section class="stats">
      <div class="stat-card highlight">
        <h2>12.500.000₫</h2>
        <p>Tổng doanh thu</p>
      </div>
      <div class="stat-card">
        <h2>1,245</h2>
        <p>Tổng số User</p>
      </div>
      <div class="stat-card">
        <h2>356</h2>
        <p>Check-in hôm nay</p>
      </div>
      <div class="stat-card">
        <h2>42</h2>
        <p>Landmark chờ duyệt</p>
      </div>
      <div class="stat-card">
        <h2>128</h2>
        <p>Mission đã hoàn thành</p>
      </div>
    </section>

    <!-- Charts -->
    <section class="charts">
      <!-- Doanh thu -->
      <div class="chart-box full-width">
        <div class="chart-header">
          <h3>Doanh thu</h3>
          <select v-model="revenueMode" class="chart-filter">
            <option value="daily">Theo ngày</option>
            <option value="monthly">Theo tháng</option>
            <option value="yearly">Theo năm</option>
          </select>
        </div>
        <div class="chart-container">
          <Line :data="currentRevenueData" :options="chartOptions" />
        </div>
      </div>

      <!-- User mới -->
      <div class="chart-box">
        <h3>User mới theo tháng</h3>
        <div class="chart-container">
          <Bar :data="barData" :options="chartOptions" />
        </div>
      </div>

      <!-- Tỉ lệ role -->
      <div class="chart-box">
        <h3>Tỉ lệ Role</h3>
        <div class="chart-container">
          <Pie :data="pieData" :options="chartOptions" />
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale,
  ArcElement,
  Filler,
} from "chart.js";
import { Bar, Pie, Line } from "vue-chartjs";

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale,
  ArcElement,
  Filler
);

// Data cho User mới (Bar chart)
const barData = {
  labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5"],
  datasets: [
    {
      label: "User mới",
      backgroundColor: "#4caf50",
      data: [50, 80, 40, 100, 75],
    },
  ],
};

// Data cho Role (Pie chart)
const pieData = {
  labels: ["Admin", "User", "Moderator"],
  datasets: [
    {
      backgroundColor: ["#34a853", "#5c9ded", "#fbbc05"],
      data: [5, 1200, 40],
    },
  ],
};

// Dropdown mode
const revenueMode = ref("monthly");

// Data cho Doanh thu (Line chart)
const revenueDatasets = {
  daily: {
    labels: ["1", "2", "3", "4", "5", "6", "7"],
    datasets: [
      {
        label: "Doanh thu (VNĐ)",
        borderColor: "#ff7043",
        backgroundColor: "rgba(255,112,67,0.2)",
        fill: true,
        tension: 0.3,
        data: [500000, 700000, 800000, 600000, 900000, 750000, 650000],
      },
    ],
  },
  monthly: {
    labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5"],
    datasets: [
      {
        label: "Doanh thu (VNĐ)",
        borderColor: "#ff7043",
        backgroundColor: "rgba(255,112,67,0.2)",
        fill: true,
        tension: 0.3,
        data: [3000000, 4500000, 2800000, 6000000, 5000000],
      },
    ],
  },
  yearly: {
    labels: ["2021", "2022", "2023", "2024", "2025"],
    datasets: [
      {
        label: "Doanh thu (VNĐ)",
        borderColor: "#ff7043",
        backgroundColor: "rgba(255,112,67,0.2)",
        fill: true,
        tension: 0.3,
        data: [32000000, 45000000, 60000000, 80000000, 70000000],
      },
    ],
  },
};

const currentRevenueData = computed(() => revenueDatasets[revenueMode.value]);

// Options chung cho chart
const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
};
</script>

<style scoped>
.dashboard-page {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 20px;
}

/* Header */
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
  min-height: 350px;
}
.chart-box.full-width {
  grid-column: span 2;
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
.chart-container {
  flex: 1;
  position: relative;
}
</style>
