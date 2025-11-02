import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import PrimeVue from 'primevue/config';
import Aura from '@primeuix/themes/aura';
import Button from 'primevue/button';
import { createPinia } from 'pinia';
import router from './router';
import ToastService from 'primevue/toastservice';


const app = createApp(App);
app.use(PrimeVue, {
    theme: {
        preset: Aura
    }
});
app.use(router);
app.use(createPinia());
app.use(ToastService);
app.component('Button', Button);

app.mount('#app');
