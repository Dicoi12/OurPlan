<template>
    <div class="navbar-container">
      <div class="desktop-nav">
        <div class="nav-left">
          <router-link to="/dashboard" class="nav-item" active-class="active">
            <i class="pi pi-calendar"></i>
            <span>Calendar</span>
          </router-link>
          <router-link to="/cheltuieli" class="nav-item" active-class="active">
            <i class="pi pi-wallet"></i>
            <span>Cheltuieli</span>
          </router-link>
          <router-link to="/tasks" class="nav-item" active-class="active">
            <i class="pi pi-check-square"></i>
            <span>Task-uri</span>
          </router-link>
          <router-link to="/shopping" class="nav-item" active-class="active">
            <i class="pi pi-shopping-cart"></i>
            <span>Cumpărături</span>
          </router-link>
          <router-link to="/obiective" class="nav-item" active-class="active">
            <i class="pi pi-chart-pie"></i>
            <span>Obiective</span>
          </router-link>
        </div>
        <div class="nav-right">
          <div class="user-menu" @click="toggleUserMenu">
            <span class="user-name">{{ owner }}</span>
            <i class="pi pi-angle-down"></i>
            <div v-if="showUserMenu" class="user-dropdown">
              <div class="dropdown-item" @click="logout">
                <i class="pi pi-sign-out"></i>
                <span>Deconectare</span>
              </div>
            </div>
          </div>
        </div>
      </div>
  
      <div class="mobile-nav">
        <router-link to="/dashboard" class="nav-item" active-class="active">
          <i class="pi pi-calendar"></i>
        </router-link>
        <router-link to="/cheltuieli" class="nav-item" active-class="active">
          <i class="pi pi-wallet"></i>
        </router-link>
        <router-link to="/tasks" class="nav-item" active-class="active">
          <i class="pi pi-check-square"></i>
        </router-link>
        <div class="more-menu" @click="toggleMoreMenu">
          <i class="pi pi-ellipsis-h"></i>
          <div v-if="showMoreMenu" class="more-dropdown">
            <router-link to="/shopping" class="dropdown-item" active-class="active">
              <i class="pi pi-shopping-cart"></i>
              <span>Cumpărături</span>
            </router-link>
            <router-link to="/obiective" class="dropdown-item" active-class="active">
              <i class="pi pi-chart-pie"></i>
              <span>Obiective</span>
            </router-link>
          </div>
        </div>
        <div class="user-menu-mobile" @click="toggleUserMenu">
          <span class="user-name">{{ owner }}</span>
          <i class="pi pi-angle-down"></i>
          <div v-if="showUserMenu" class="user-dropdown-mobile">
            <div class="dropdown-item" @click="logout">
              <i class="pi pi-sign-out"></i>
              <span>Deconectare</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref, onMounted } from 'vue'
  import router from '../router'
  
  const owner = ref('')
  const showUserMenu = ref(false)
  const showMoreMenu = ref(false)
  
  onMounted(() => {
    owner.value = localStorage.getItem('owner') || ''
  })
  
  const toggleUserMenu = () => {
    showUserMenu.value = !showUserMenu.value
  }

  const toggleMoreMenu = () => {
    showMoreMenu.value = !showMoreMenu.value
  }
  
  const logout = () => {
    localStorage.removeItem('owner')
    router.push('/')
  }
  </script>
  
  <style scoped>
  .navbar-container {
    width: 100%;
    background: #242424;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    position: fixed;
    bottom: 0;
    left: 0;
    z-index: 1000;
  }
  
  .desktop-nav {
    display: none;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
  }
  
  .nav-left {
    display: flex;
    gap: 2rem;
  }
  
  .nav-right {
    display: flex;
    align-items: center;
  }
  
  .mobile-nav {
    display: flex;
    justify-content: space-around;
    padding: 1rem;
  }
  
  .nav-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    color: #666;
    text-decoration: none;
    padding: 0.5rem 1rem;
    border-radius: 8px;
    transition: all 0.3s ease;
  }
  
  .nav-item:hover {
    background: #f0f0f0;
  }
  
  .nav-item.active {
    color: #f32121;
    background: #e3f2fd;
  }
  
  .nav-item i {
    font-size: 1.2rem;
  }
  
  .user-menu {
    position: relative;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    color: #666;
    cursor: pointer;
    padding: 0.5rem 1rem;
    border-radius: 8px;
    transition: all 0.3s ease;
  }
  
  .user-menu:hover {
    background: #f0f0f0;
  }
  
  .user-name {
    font-weight: 500;
  }
  
  .user-dropdown {
    position: absolute;
    top: 100%;
    right: 0;
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 12px rgba(0,0,0,0.1);
    min-width: 200px;
    margin-top: 0.5rem;
  }
  
  .dropdown-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.75rem 1rem;
    color: #666;
    cursor: pointer;
    transition: all 0.3s ease;
    text-decoration: none;
  }
  
  .dropdown-item:hover {
    background: #f0f0f0;
  }
  
  .dropdown-item i {
    font-size: 1rem;
  }

  .more-menu {
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #666;
    cursor: pointer;
    padding: 0.5rem;
    border-radius: 8px;
    transition: all 0.3s ease;
  }

  .more-menu:hover {
    background: #f0f0f0;
  }

  .more-dropdown {
    position: absolute;
    bottom: 100%;
    left: 50%;
    transform: translateX(-50%);
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 12px rgba(0,0,0,0.1);
    min-width: 200px;
    margin-bottom: 0.5rem;
  }

  .more-dropdown .dropdown-item {
    justify-content: flex-start;
  }

  .more-dropdown .dropdown-item.active {
    color: #f32121;
    background: #e3f2fd;
  }
  
  .user-menu-mobile {
    position: relative;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    color: #666;
    cursor: pointer;
    padding: 0.5rem;
    border-radius: 8px;
    transition: all 0.3s ease;
  }

  .user-menu-mobile:hover {
    background: #f0f0f0;
  }

  .user-dropdown-mobile {
    position: absolute;
    bottom: 100%;
    right: 0;
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 12px rgba(0,0,0,0.1);
    min-width: 200px;
    margin-bottom: 0.5rem;
  }
  
  .owner-avatar {
    width: 32px;
    height: 32px;
    border-radius: 4px;
    border: 1px solid white;
    object-fit: cover;
  }
  
  /* Desktop styles */
  @media screen and (min-width: 768px) {
    .navbar-container {
      top: 0;
      bottom: auto;
    }
  
    .desktop-nav {
      display: flex;
    }
  
    .mobile-nav {
      display: none;
    }

    .user-menu-mobile {
      display: none;
    }
  }

  /* Mobile styles */
  @media screen and (max-width: 767px) {
    .mobile-nav {
      display: flex;
      justify-content: space-around;
      align-items: center;
      padding: 0.5rem;
    }

    .nav-item {
      padding: 0.5rem;
    }

    .user-menu-mobile {
      display: flex;
    }

    .user-name {
      font-size: 0.9rem;
    }

    .desktop-nav {
      display: none;
    }
  }
  </style>