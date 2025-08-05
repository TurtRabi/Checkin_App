import HomePage from '@/application/pages/HomePage.vue'
import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: HomePage,
    // nếu cần bảo vệ route thì thêm meta.requiresAuth
    // meta: { requiresAuth: true }
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, from, next) => {
  // const isLoggedIn = checkAuth()
  if (to.meta.requiresAuth /* && !isLoggedIn */) {
    next('/login')
  } else {
    next()
  }
})

export default router
