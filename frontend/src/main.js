import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import roteador from './roteador'

const aplicacao = createApp(App)
aplicacao.use(createPinia())
aplicacao.use(roteador)
aplicacao.mount('#app')
