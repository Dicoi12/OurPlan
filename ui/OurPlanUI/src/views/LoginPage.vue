<template>
  <div class="auth-page">
    <!-- Animated background particles -->
    <div class="particles">
      <div class="particle" v-for="i in 20" :key="i" :style="getParticleStyle(i)"></div>
    </div>

    <div class="auth-card">
      <h2 class="auth-title" :key="isRegistering ? 'register' : 'login'">
        <transition name="title-fade" mode="out-in">
          <span :key="isRegistering ? 'register' : 'login'">
            {{ isRegistering ? "Create a New Account" : "Welcome Back" }}
          </span>
        </transition>
      </h2>

      <p class="auth-subtitle">
        <transition name="subtitle-fade" mode="out-in">
          <span :key="isRegistering ? 'register' : 'login'">
            {{ isRegistering ? "Register to continue" : "Login to your account" }}
          </span>
        </transition>
      </p>

      <form
        class="auth-form"
        @submit.prevent="isRegistering ? handleRegister() : handleLogin()"
      >
        <!-- LOGIN FORM -->
        <transition name="fade-slide" mode="out-in">
          <div v-if="!isRegistering" key="login" class="form-content">
            <transition-group name="input-stagger" tag="div">
              <div class="input-group" key="email-login" :style="{ '--delay': '0.1s' }">
                <label>Email</label>
                <div class="input-wrapper">
                  <InputText v-model="email" placeholder="Enter your email" class="animated-input" />
                  <span class="input-highlight"></span>
                </div>
              </div>

              <div class="input-group" key="password-login" :style="{ '--delay': '0.2s' }">
                <label>Password</label>
                <div class="input-wrapper">
                  <InputText
                    v-model="password"
                    type="password"
                    placeholder="Enter your password"
                    class="animated-input"
                  />
                  <span class="input-highlight"></span>
                </div>
              </div>
            </transition-group>

            <button class="auth-button" type="submit" :disabled="isLoading">
              <span class="button-content">
                <span v-if="!isLoading">Login</span>
                <span v-else class="loading-spinner"></span>
              </span>
            </button>

            <p class="switch-text" @click="toggleForm">
              Don't have an account?
              <span class="switch-link">Create one</span>
            </p>
          </div>

          <!-- REGISTER FORM -->
          <div v-else key="register" class="form-content">
            <transition-group name="input-stagger" tag="div">
              <div class="input-group" key="username-register" :style="{ '--delay': '0.1s' }">
                <label>Username</label>
                <div class="input-wrapper">
                  <InputText
                    v-model="newUsername"
                    placeholder="Choose a username"
                    class="animated-input"
                  />
                  <span class="input-highlight"></span>
                </div>
              </div>

              <div class="input-group" key="email-register" :style="{ '--delay': '0.15s' }">
                <label>Email</label>
                <div class="input-wrapper">
                  <InputText
                    v-model="email"
                    type="email"
                    placeholder="Enter your email"
                    class="animated-input"
                  />
                  <span class="input-highlight"></span>
                </div>
              </div>

              <div class="input-group" key="password-register" :style="{ '--delay': '0.2s' }">
                <label>Password</label>
                <div class="input-wrapper">
                  <InputText
                    v-model="newPassword"
                    type="password"
                    placeholder="Create a password"
                    class="animated-input"
                  />
                  <span class="input-highlight"></span>
                </div>
              </div>
            </transition-group>

            <button class="auth-button" type="submit" :disabled="isLoading">
              <span class="button-content">
                <span v-if="!isLoading">Register</span>
                <span v-else class="loading-spinner"></span>
              </span>
            </button>

            <p class="switch-text" @click="toggleForm">
              Already have an account?
              <span class="switch-link">Log in</span>
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
const isLoading = ref(false);

const userStore = useUserStore();
const toast = useToast();

function getParticleStyle(index: number) {
  // Use index to seed random values for consistent particle distribution
  const seed = index * 0.1;
  const size = (Math.sin(seed) * 2 + 3) + 2;
  const left = (Math.cos(seed * 2) * 50 + 50);
  const animationDuration = (Math.sin(seed * 3) * 10 + 20) + 15;
  const animationDelay = (Math.cos(seed * 4) * 2.5 + 2.5);
  
  return {
    width: `${size}px`,
    height: `${size}px`,
    left: `${left}%`,
    animationDuration: `${animationDuration}s`,
    animationDelay: `${animationDelay}s`,
  };
}

async function handleLogin() {
  isLoading.value = true;
  try {
    if (await userStore.login(email.value, password.value)) {
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Logged in successfully",
        life: 3000,
        group: "general",
      });
      setTimeout(() => {
        router.push("/events");
      }, 500);
    } else {
      toast.add({
        severity: "error",
        summary: "Login failed",
        detail: "Incorrect email or password",
        life: 3000,
        group: "general",
      });
    }
  } finally {
    isLoading.value = false;
  }
}

async function handleRegister() {
  isLoading.value = true;
  try {
    const resp = await userStore.signup(
      newUsername.value,
      newPassword.value,
      email.value
    );

    if (resp) {
      toast.add({
        severity: "success",
        summary: "Account created",
        detail: "Your account has been created successfully",
        life: 3000,
        group: "general",
      });
      setTimeout(() => {
        isRegistering.value = false;
      }, 1000);
    }
  } finally {
    isLoading.value = false;
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
  background: linear-gradient(135deg, #2f6dd1, #1d4ca0, #4a90e2);
  background-size: 400% 400%;
  animation: gradientShift 15s ease infinite;
  position: relative;
  overflow: hidden;
}

/* Animated background particles */
.particles {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  overflow: hidden;
  z-index: 0;
}

.particle {
  position: absolute;
  background: rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  animation: float linear infinite;
  pointer-events: none;
}

@keyframes float {
  0% {
    transform: translateY(100vh) rotate(0deg);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100px) rotate(360deg);
    opacity: 0;
  }
}

@keyframes gradientShift {
  0% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0% 50%;
  }
}

/* Card container */
.auth-card {
  width: 100%;
  max-width: 380px;
  background: rgba(255, 255, 255, 0.15);
  padding: 2rem 2.4rem;
  border-radius: 16px;
  box-shadow: 0 12px 35px rgba(0, 0, 0, 0.25), 0 0 60px rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(14px);
  animation: cardEntrance 0.8s cubic-bezier(0.34, 1.56, 0.64, 1);
  position: relative;
  z-index: 1;
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.auth-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 45px rgba(0, 0, 0, 0.3), 0 0 80px rgba(255, 255, 255, 0.15);
}

@keyframes cardEntrance {
  0% {
    opacity: 0;
    transform: translateY(30px) scale(0.9);
  }
  100% {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* Titles */
.auth-title {
  color: #fff;
  text-align: center;
  font-size: 1.9rem;
  margin-bottom: 0.3rem;
  font-weight: 700;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
  position: relative;
  overflow: hidden;
}

.title-fade-enter-active,
.title-fade-leave-active {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.title-fade-enter-from {
  opacity: 0;
  transform: translateY(-20px) scale(0.95);
}

.title-fade-leave-to {
  opacity: 0;
  transform: translateY(20px) scale(0.95);
}

.auth-subtitle {
  text-align: center;
  color: #e0e0e0;
  margin-bottom: 2rem;
  font-size: 0.95rem;
}

.subtitle-fade-enter-active,
.subtitle-fade-leave-active {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.subtitle-fade-enter-from {
  opacity: 0;
  transform: translateX(-10px);
}

.subtitle-fade-leave-to {
  opacity: 0;
  transform: translateX(10px);
}

/* Form styling */
.form-content {
  position: relative;
}

.input-group {
  margin-bottom: 1.2rem;
  position: relative;
}

.input-group label {
  display: block;
  margin-bottom: 0.4rem;
  color: #fff;
  font-weight: 500;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.input-wrapper {
  position: relative;
  overflow: hidden;
  border-radius: 10px;
}

.input-wrapper::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.5s ease;
}

.input-wrapper:hover::before {
  left: 100%;
}

.animated-input {
  width: 100%;
  padding: 0.8rem;
  border-radius: 10px;
  transition: all 0.3s ease;
  background: rgba(255, 255, 255, 0.9);
  border: 2px solid transparent;
}

.animated-input:focus {
  outline: none;
  border-color: rgba(255, 255, 255, 0.5);
  background: rgba(255, 255, 255, 1);
  box-shadow: 0 0 20px rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.input-highlight {
  position: absolute;
  bottom: 0;
  left: 50%;
  width: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, #fff, transparent);
  transition: all 0.3s ease;
  transform: translateX(-50%);
}

.animated-input:focus ~ .input-highlight {
  width: 100%;
}

/* Stagger animation for inputs */
.input-stagger-enter-active {
  transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  transition-delay: var(--delay, 0s);
}

.input-stagger-leave-active {
  transition: all 0.3s ease;
}

.input-stagger-enter-from {
  opacity: 0;
  transform: translateX(-30px) scale(0.95);
}

.input-stagger-leave-to {
  opacity: 0;
  transform: translateX(30px) scale(0.95);
}

/* Button */
.auth-button {
  width: 100%;
  padding: 0.9rem;
  background: linear-gradient(135deg, #4caf50, #43a047);
  border: none;
  color: white;
  font-weight: 600;
  font-size: 1.1rem;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  margin-top: 0.5rem;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(76, 175, 80, 0.4);
}

.auth-button::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.3);
  transform: translate(-50%, -50%);
  transition: width 0.6s, height 0.6s;
}

.auth-button:hover {
  background: linear-gradient(135deg, #43a047, #388e3c);
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(76, 175, 80, 0.5);
}

.auth-button:active {
  transform: translateY(0);
  box-shadow: 0 2px 10px rgba(76, 175, 80, 0.4);
}

.auth-button:active::before {
  width: 300px;
  height: 300px;
}

.auth-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.button-content {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}


/* Switch text */
.switch-text {
  margin-top: 1.2rem;
  color: #ddd;
  text-align: center;
  font-size: 0.95rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.switch-text:hover {
  color: #fff;
}

.switch-link {
  color: #fff;
  font-weight: bold;
  text-decoration: underline;
  transition: all 0.3s ease;
  display: inline-block;
  position: relative;
}

.switch-link::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 0;
  height: 2px;
  background: #fff;
  transition: width 0.3s ease;
}

.switch-text:hover .switch-link::after {
  width: 100%;
}

.switch-text:hover .switch-link {
  transform: translateX(3px);
  text-shadow: 0 0 10px rgba(255, 255, 255, 0.5);
}

/* Form transitions */
.fade-slide-enter-active {
  transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-slide-leave-active {
  transition: all 0.3s ease;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(20px) scale(0.95);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-20px) scale(0.95);
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

  .particles {
    display: none;
  }
}
</style>
