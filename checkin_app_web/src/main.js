import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './application/router'
import { createPinia } from 'pinia'
import vue3GoogleLogin from 'vue3-google-login'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(vue3GoogleLogin, {
  clientId: '1099077344805-9l7r3hcr2u77stl2utm6nh925cfi1505.apps.googleusercontent.com'
})
app.mount('#app')
