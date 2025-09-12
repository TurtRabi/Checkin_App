import HomePage from '../pages/HomePage.vue'
import { createRouter, createWebHistory } from 'vue-router'
import Introduce from '../pages/Introduce.vue'
import { useAuthStore } from '../stores/auth.js'
import AdminLayout from '../layouts/AdminLayout.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: HomePage,
    meta: { requiresAuth: true }
  },
  {
    path: '/user',
    name: 'User',
    component: () => import('@/application/pages/UserPage.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/introduce',
    name: 'Introduce',
    component: Introduce
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/application/pages/LoginPage.vue')
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/application/pages/RegisterPage.vue')
  },
  {
    path: '/reset-password/:email/:username',
    name: 'ResetPassword',
    component: () => import('@/application/pages/ResetPassword.vue'),
    props: true
  },
  {
    path: '/admin',
    component: AdminLayout,
    meta: { requiresAuth: true, roles: ['Admin'] }, // chỉ cho Admin
    children: [
      { path: '', redirect: '/admin/dashboard' },
      { path: 'dashboard', name: 'AdminDashboard', component: () => import('@/application/pages/admin/DashboardPage.vue') },
      { path: 'notifications', name: 'notifications', component: () => import('@/application/pages/admin/Notifications.vue') },
      { path: 'checkins', name: 'checkins', component: () => import('@/application/pages/admin/CheckinsPage.vue') },
      { path: 'users', name: 'AdminUsers', component: () => import('@/application/pages/admin/AdminUsersPage.vue') },
      { path: 'landmarks', name: 'AdminLandmarks', component: () => import('@/application/pages/admin/LandmarksPage.vue') },
      { path: 'missions', name: 'AdminMissions', component: () => import('@/application/pages/admin/MissionsPage.vue') },
      {
        path: 'reports',
        component: () => import('@/application/pages/admin/ReportsPage.vue'),
        children: [
          { path: 'revenue', name: 'ReportRevenue', component: () => import('@/application/pages/admin/reports/RevenueReport.vue') },
          { path: 'users', name: 'ReportUsers', component: () => import('@/application/pages/admin/reports/UserReport.vue') },
          { path: 'checkins', name: 'ReportCheckins', component: () => import('@/application/pages/admin/reports/CheckinReport.vue') },
          { path: 'landmarks', name: 'ReportLandmarks', component: () => import('@/application/pages/admin/reports/LandmarkReport.vue') },
          { path: 'missions', name: 'ReportMissions', component: () => import('@/application/pages/admin/reports/MissionReport.vue') }
        ]
      },
      { path: 'settings', name: 'AdminSettings', component: () => import('@/application/pages/admin/SettingsPage.vue') }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

// Guard check login + role
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isLoggedIn = authStore.isLoggedIn
  const userRole = authStore.role // 👈 giả sử role được lưu trong authStore.user

  // Chưa login mà vào route cần auth
  if (to.meta.requiresAuth && !isLoggedIn) {
    return next({ name: 'Login' })
  }

  // Đã login nhưng không đủ role
  if (to.meta.roles && (!userRole || !to.meta.roles.includes(userRole))) {
    return next({ name: 'Home' }) // hoặc Introduce tuỳ bạn
  }

  // Nếu đã login mà vẫn vào introduce → redirect về Home
  if (to.name === 'Introduce' && isLoggedIn) {
    return next({ name: 'Home' })
  }

  next()
})

export default router
