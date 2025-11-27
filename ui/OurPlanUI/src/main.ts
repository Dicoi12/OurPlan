import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import PrimeVue from 'primevue/config';
import Aura from '@primeuix/themes/aura';
import Button from 'primevue/button';
import { createPinia } from 'pinia';
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate';
import router from './router';
import ToastService from 'primevue/toastservice';
import { registerSW } from 'virtual:pwa-register';
import 'primeicons/primeicons.css'


const app = createApp(App);
app.use(PrimeVue, {
    theme: {
        preset: Aura
    }
});
app.use(router);
const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);
app.use(pinia);
app.use(ToastService);
app.component('Button', Button);

// Înregistrare PWA Service Worker
if ('serviceWorker' in navigator) {
    registerSW({
        onNeedRefresh() {
            // Poți adăuga aici o notificare către utilizator pentru actualizare
            console.log('Actualizare disponibilă pentru aplicație');
        },
        onOfflineReady() {
            console.log('Aplicația este gata pentru utilizare offline');
        },
    });
}

app.mount('#app');
