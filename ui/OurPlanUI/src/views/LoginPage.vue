<template>
  <div class="card flex justify-center">
    <div class="flex flex-col gap-1">
      <InputText name="username" type="text" placeholder="Username" fluid />
    </div>
    <div class="flex flex-col gap-1">
      <InputText name="email" type="text" placeholder="Email" fluid />
    </div>
    <Button type="submit" severity="secondary" label="Submit" />
  </div>
</template>

<script setup lang="ts">
import { useToast } from "primevue/usetoast";
import { InputText } from "primevue";

const toast = useToast();

</script>
