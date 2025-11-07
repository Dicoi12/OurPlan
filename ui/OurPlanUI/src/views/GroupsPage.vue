<template>
  <div>groups page</div>
  {{ groupsStore.group }}
</template>
<script lang="ts" setup>
import { onBeforeMount } from 'vue';
import { useGroupsStore } from '../stores/groupsStore';
const groupsStore = useGroupsStore();
onBeforeMount(async() => {
  await groupsStore.getUserGroup();
});
</script>
