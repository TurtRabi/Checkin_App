import HomePage from '../pages/HomePage.vue'
import { createRouter, createWebHistory } from 'vue-router'
import Introduce from '../pages/Introduce.vue'
import { useAuthStore } from '../stores/auth.js'

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
    component: HomePage,
    meta: { requiresAuth: true },
    children: [
      { path: '', redirect: '/admin/dashboard' },
      { path: 'dashboard', name: 'AdminDashboard', component: () => import('@/application/pages/admin/DashboardPage.vue') },
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

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isLoggedIn = authStore.isLoggedIn

  if (to.meta.requiresAuth && !isLoggedIn ) {
    next({ name: 'Introduce'}) 
  } else if(to.name === 'Introduce' && isLoggedIn) {
    next({name: 'Home'})
  }else if(!isLoggedIn && to.path === '/') {
    next({ name: 'Introduce' })
  }else{
    next()
  }
})

export default router
