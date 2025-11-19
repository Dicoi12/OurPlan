<template>
  <!-- <div>First template</div> -->
  <NavBar />
  <router-view class="fadein animation-duration-300"> </router-view>
</template>

<script setup lang="ts">
import NavBar from "./components/NavBar.vue";
</script>
<style scoped></style>
