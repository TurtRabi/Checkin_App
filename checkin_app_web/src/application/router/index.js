import HomePage from '@/application/pages/HomePage.vue'
import { createRouter, createWebHistory } from 'vue-router'
import Introduce from '../pages/Introduce.vue'
import { useAuthStore } from '../stores/auth.js'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: HomePage,
    // nếu cần bảo vệ route thì thêm meta.requiresAuth
    // meta: { requiresAuth: true }
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
