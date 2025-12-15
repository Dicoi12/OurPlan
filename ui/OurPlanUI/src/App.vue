<template>
  <div id="app" :class="['app-wrapper', { 'authenticated': isAuthenticated }]">
    <NavBar v-if="isAuthenticated"/>
    <router-view class="fadein animation-duration-300"></router-view>
  </div>
</template>

<script setup lang="ts">
import NavBar from "./components/NavBar.vue";
import { useUserStore } from "./stores/userStore";
import { computed, onMounted } from "vue";

const userStore = useUserStore();
const isAuthenticated = computed(() => userStore.isAuthenticated);

// Inițializare automată a utilizatorului la încărcarea aplicației
onMounted(async () => {
  // Dacă există token dar nu avem userData, încărcăm utilizatorul
  if (userStore.isAuthenticated && !userStore.hasUserData) {
    await userStore.loadCurrentUser();
  }
});
</script>
<style scoped>
.app-wrapper.authenticated {
  padding-top: 70px; /* înălțimea navbar-ului pe desktop */
  padding-bottom: 70px; /* înălțimea navbar-ului pe mobile */
}
@media screen and (min-width: 768px) {
  .app-wrapper.authenticated {
    padding-bottom: 0; /* pe desktop navbar-ul e sus, nu jos */
  }
}

@media screen and (max-width: 767px) {
  .app-wrapper.authenticated {
    padding-top: 0; /* pe mobil navbar-ul e jos, nu sus */
  }
}
</style>
