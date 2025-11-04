<template>
  <div class="login-page">
    <div class="login-card">
      <h2 class="login-title">
        {{ isRegistering ? "Creează un cont nou" : "Conectează-te" }}
      </h2>
      <form @submit.prevent="isRegistering ? handleRegister() : handleLogin()">
        <div v-if="!isRegistering">
          <div class="input-group">
            <label for="email">Email</label>
            <InputText
              v-model="email"
              placeholder="Introdu utilizatorul tău"
            />
          </div>
          <div class="input-group">
            <label for="password">Parolă</label>
            <InputText
              v-model="password"
              type="password"
              required
              placeholder="Introdu parola:"
            />
          </div>
          <button type="submit" class="login-button">Login</button>
          <label
            class="my-2 text-white font-xl cursor-pointer"
            @click="toggleForm"
          >
            Crează un cont nou...
          </label>
        </div>

        <div v-else>
          <div class="input-group">
            <label for="username">Utilizator</label>
            <InputText
              v-model="newUsername"
              required
              placeholder="Alege un nume de utilizator"
            />
          </div>
          <div class="input-group">
            <label for="email">Email</label>
            <InputText
              v-model="email"
              type="email"
              required
              placeholder="Introdu email-ul tău"
            />
          </div>
          <div class="input-group">
            <label for="password">Parolă</label>
            <InputText
              v-model="newPassword"
              type="password"
              required
              placeholder="Alege o parolă"
            />
          </div>
          <button type="submit" class="login-button" >Înregistrează-te</button>
          <label
            class="my-2 text-white font-xl cursor-pointer"
            @click="toggleForm"
          >
            Ai deja un cont? Conectează-te aici.
          </label>
        </div>
      </form>
    </div>
    <Toast group="general"/>
  </div>
</template>

<script setup lang="ts">
import InputText from "primevue/inputtext";
import { ref } from "vue";
import { useUserStore } from "../stores/userStore";
import router from "../router"; 
import { useToast } from "primevue/usetoast";
import Toast from "primevue/toast";

const username = ref("");
const password = ref("");
const email = ref("");
const newUsername = ref("");
const newPassword = ref("");
const isRegistering = ref(false);

const userStore = useUserStore();
const toast = useToast();

async function handleLogin() {
  console.log("Logging in with:", username.value, password.value);
  if (await userStore.login(email.value, password.value)) {
    router.push("/objectives");
    toast.add({
      severity: "success",
      summary: "Autentificare cu succes",
      detail: "Te-ai autentificat cu succes",
      life: 3000,
      group: "general"
    });
  } else {
    toast.add({
      severity: "error",
      summary: "Eroare de autentificare",
      detail: "Numele de utilizator sau parola sunt incorecte",
      life: 3000,
      group: "general"
    });
  }
}

async function handleRegister() {
  let resp = await userStore.signup(
    newUsername.value,
    newPassword.value,
    email.value
  );
  if (resp) {
    isRegistering.value = false;
  }
  toast.add({
    severity: "success",
    summary: "Înregistrare cu succes",
    detail: "Contul a fost înregistrat cu succes",
    life: 3000,
    group: "general"
  });
  console.log(resp);
}

function toggleForm() {
  isRegistering.value = !isRegistering.value;
}
</script>

<style scoped>
.login-page {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #2f6dd1;
  background-position: center;
  background-repeat: no-repeat;
  overflow: hidden;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}

.login-card {
  background-color: rgba(255, 255, 255, 0.1);
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0px 4px 15px rgba(0, 0, 0, 0.2);
  backdrop-filter: blur(10px);
  width: 350px;
  max-width: 90vh;
  max-height: 90vh;
  overflow-y: auto;
}

.login-title {
  text-align: center;
  color: white;
  margin-bottom: 1.5rem;
}

.input-group {
  margin-bottom: 1rem;
}

.input-group label {
  display: block;
  color: white;
  margin-bottom: 0.5rem;
  font-weight: bold;
}

.input-group input {
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 8px;
  background-color: rgba(255, 255, 255, 0.9);
  color: #333;
}

.input-group input::placeholder {
  color: #888;
}

.login-button {
  width: 100%;
  padding: 0.75rem;
  background-color: #4caf50;
  color: white;
  font-size: 1rem;
  font-weight: bold;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.login-button:hover {
  background-color: #45a049;
}

.my-2 {
  margin-top: 10px;
  margin-bottom: 10px;
}

.cursor-pointer {
  cursor: pointer;
}
</style>