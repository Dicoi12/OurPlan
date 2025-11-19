<template>
  <div class="auth-page">
    <div class="auth-card">
      <h2 class="auth-title">
        {{ isRegistering ? "Create a New Account" : "Welcome Back" }}
      </h2>

      <p class="auth-subtitle">
        {{ isRegistering ? "Register to continue" : "Login to your account" }}
      </p>

      <form
        class="auth-form"
        @submit.prevent="isRegistering ? handleRegister() : handleLogin()"
      >
        <!-- LOGIN FORM -->
        <transition name="fade-slide" mode="out-in">
          <div v-if="!isRegistering" key="login">
            <div class="input-group">
              <label>Email</label>
              <InputText v-model="email" placeholder="Enter your email" />
            </div>

            <div class="input-group">
              <label>Password</label>
              <InputText
                v-model="password"
                type="password"
                placeholder="Enter your password"
              />
            </div>

            <button class="auth-button" type="submit">Login</button>

            <p class="switch-text" @click="toggleForm">
              Don’t have an account?
              <span>Create one</span>
            </p>
          </div>

          <!-- REGISTER FORM -->
          <div v-else key="register">
            <div class="input-group">
              <label>Username</label>
              <InputText
                v-model="newUsername"
                placeholder="Choose a username"
              />
            </div>

            <div class="input-group">
              <label>Email</label>
              <InputText
                v-model="email"
                type="email"
                placeholder="Enter your email"
              />
            </div>

            <div class="input-group">
              <label>Password</label>
              <InputText
                v-model="newPassword"
                type="password"
                placeholder="Create a password"
              />
            </div>

            <button class="auth-button" type="submit">Register</button>

            <p class="switch-text" @click="toggleForm">
              Already have an account?
              <span>Log in</span>
            </p>
          </div>
        </transition>
      </form>
    </div>

    <Toast group="general" />
  </div>
</template>

<script setup lang="ts">
import InputText from "primevue/inputtext";
import { ref } from "vue";
import { useUserStore } from "../stores/userStore";
import router from "../router";
import { useToast } from "primevue/usetoast";
import Toast from "primevue/toast";

const email = ref("");
const password = ref("");
const newUsername = ref("");
const newPassword = ref("");
const isRegistering = ref(false);

const userStore = useUserStore();
const toast = useToast();

async function handleLogin() {
  if (await userStore.login(email.value, password.value)) {
    router.push("/groups");
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Logged in successfully",
      life: 3000,
      group: "general",
    });
  } else {
    toast.add({
      severity: "error",
      summary: "Login failed",
      detail: "Incorrect email or password",
      life: 3000,
      group: "general",
    });
  }
}

async function handleRegister() {
  const resp = await userStore.signup(
    newUsername.value,
    newPassword.value,
    email.value
  );

  if (resp) {
    isRegistering.value = false;
    toast.add({
      severity: "success",
      summary: "Account created",
      detail: "Your account has been created successfully",
      life: 3000,
      group: "general",
    });
  }
}

function toggleForm() {
  isRegistering.value = !isRegistering.value;
}
</script>

<style scoped>
/* Page layout */
.auth-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  padding: 1rem;
  background: linear-gradient(135deg, #2f6dd1, #1d4ca0);
}

/* Card container */
.auth-card {
  width: 100%;
  max-width: 380px;
  background: rgba(255, 255, 255, 0.15);
  padding: 2rem 2.4rem;
  border-radius: 16px;
  box-shadow: 0 12px 35px rgba(0, 0, 0, 0.25);
  backdrop-filter: blur(14px);
  animation: fadeIn 0.5s ease;
}

/* Titles */
.auth-title {
  color: #fff;
  text-align: center;
  font-size: 1.9rem;
  margin-bottom: 0.3rem;
}

.auth-subtitle {
  text-align: center;
  color: #e0e0e0;
  margin-bottom: 2rem;
}

/* Form styling */
.input-group {
  margin-bottom: 1.2rem;
}

.input-group label {
  display: block;
  margin-bottom: 0.4rem;
  color: #fff;
  font-weight: 500;
}

.input-group input {
  width: 100%;
  padding: 0.8rem;
  border-radius: 10px;
}

/* Button */
.auth-button {
  width: 100%;
  padding: 0.9rem;
  background: #4caf50;
  border: none;
  color: white;
  font-weight: 600;
  font-size: 1.1rem;
  border-radius: 10px;
  cursor: pointer;
  transition: 0.3s ease;
  margin-top: 0.5rem;
}

.auth-button:hover {
  background: #43a047;
}

/* Switch text */
.switch-text {
  margin-top: 1.2rem;
  color: #ddd;
  text-align: center;
  font-size: 0.95rem;
  cursor: pointer;
}

.switch-text span {
  color: #fff;
  font-weight: bold;
  text-decoration: underline;
}

.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.3s ease;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(15px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-15px);
}

/* Animations */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(25px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive improvements */
@media (max-width: 480px) {
  .auth-card {
    padding: 1.6rem;
    border-radius: 14px;
  }

  .auth-title {
    font-size: 1.6rem;
  }
}
</style>
