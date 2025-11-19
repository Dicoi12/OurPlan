<template>
  <div>groups page</div>
  {{ groupsStore.group }}
</template>
<script lang="ts" setup>
import { onBeforeMount } from "vue";
import { useGroupsStore } from "../stores/groupsStore";
import { useEventStore } from "../stores/eventStore";
const eventStore = useEventStore();
const groupsStore = useGroupsStore();
onBeforeMount(async () => {
  await groupsStore.getUserGroup();
  await eventStore.getEventsForCurrentUser();
});
</script>
