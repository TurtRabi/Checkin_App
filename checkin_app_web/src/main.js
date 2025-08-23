import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './application/router'
import { createPinia } from 'pinia'
import vue3GoogleLogin from 'vue3-google-login'
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(Toast)
app.use(vue3GoogleLogin, {
  clientId: '1033058279745-1ifdbvgg295t05amgqoqp42gppavomk9.apps.googleusercontent.com'
})
app.mount('#app')
