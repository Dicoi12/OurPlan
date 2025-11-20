<template>
  <div id="app" class="app-wrapper">
    <NavBar />
    <router-view class="fadein animation-duration-300"></router-view>
  </div>
</template>

<script setup lang="ts">
import NavBar from "./components/NavBar.vue";
</script>
<style scoped>
.app-wrapper {
  padding-top: 70px; /* înălțimea navbar-ului pe desktop */
  padding-bottom: 70px; /* înălțimea navbar-ului pe mobile */
}
@media screen and (min-width: 768px) {
  .app-wrapper {
    padding-bottom: 0; /* pe desktop navbar-ul e sus, nu jos */
  }
}

@media screen and (max-width: 767px) {
  .app-wrapper {
    padding-top: 0; /* pe mobil navbar-ul e jos, nu sus */
  }
}
</style>
