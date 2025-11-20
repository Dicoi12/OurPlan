<template>
  <nav class="navbar-container">
    <!-- Desktop Navigation -->
    <div class="desktop-nav">
      <div class="nav-brand">
        <router-link to="/" class="brand-link">
          <div class="brand-icon">
            <i class="pi pi-calendar-check"></i>
          </div>
          <span class="brand-text">OurPlan</span>
        </router-link>
      </div>

      <div class="nav-center">
        <router-link 
          v-for="item in navItems" 
          :key="item.path"
          :to="item.path" 
          class="nav-item"
          active-class="active"
        >
          <i :class="item.icon"></i>
          <span class="nav-label">{{ item.label }}</span>
          <span class="nav-indicator"></span>
        </router-link>
      </div>

      <div class="nav-right">
        <router-link to="/groups" class="nav-item groups-link" active-class="active">
          <i class="pi pi-users"></i>
          <span class="nav-label">Groups</span>
        </router-link>
        
        <div class="user-menu-wrapper" @click.stop="toggleUserMenu">
          <div class="user-avatar">
            <span class="avatar-initial">{{ ownerInitial }}</span>
          </div>
          <div class="user-info">
            <span class="user-name">{{ owner }}</span>
            <i class="pi pi-angle-down" :class="{ 'rotated': showUserMenu }"></i>
          </div>
          
          <transition name="dropdown">
            <div v-if="showUserMenu" class="user-dropdown" @click.stop>
              <div class="dropdown-header">
                <div class="dropdown-avatar">
                  <span class="avatar-initial">{{ ownerInitial }}</span>
                </div>
                <div class="dropdown-user-info">
                  <span class="dropdown-name">{{ owner }}</span>
                  <span class="dropdown-email">{{ ownerEmail }}</span>
                </div>
              </div>
              <div class="dropdown-divider"></div>
              <div class="dropdown-item" @click="handleLogout">
                <i class="pi pi-sign-out"></i>
                <span>Logout</span>
              </div>
            </div>
          </transition>
        </div>
      </div>
    </div>

    <!-- Mobile Navigation -->
    <div class="mobile-nav">
      <router-link 
        v-for="item in mobileNavItems" 
        :key="item.path"
        :to="item.path" 
        class="mobile-nav-item"
        active-class="active"
      >
        <i :class="item.icon"></i>
        <span class="mobile-label">{{ item.label }}</span>
      </router-link>
      
      <div class="mobile-nav-item more-menu" @click="toggleMoreMenu">
        <i class="pi pi-ellipsis-h"></i>
        <span class="mobile-label">More</span>
        
        <transition name="dropdown">
          <div v-if="showMoreMenu" class="more-dropdown" @click.stop>
            <router-link 
              v-for="item in moreNavItems" 
              :key="item.path"
              :to="item.path" 
              class="dropdown-item"
              active-class="active"
              @click="closeMoreMenu"
            >
              <i :class="item.icon"></i>
              <span>{{ item.label }}</span>
            </router-link>
          </div>
        </transition>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import router from '../router'

const owner = ref('')
const ownerEmail = ref('')
const showUserMenu = ref(false)
const showMoreMenu = ref(false)

const navItems = [
  { path: '/dashboard', icon: 'pi pi-calendar', label: 'Calendar' },
  { path: '/cheltuieli', icon: 'pi pi-wallet', label: 'Expenses' },
  { path: '/tasks', icon: 'pi pi-check-square', label: 'Tasks' },
  { path: '/shopping', icon: 'pi pi-shopping-cart', label: 'Shopping' },
  { path: '/obiective', icon: 'pi pi-chart-pie', label: 'Goals' },
]

const mobileNavItems = [
  { path: '/dashboard', icon: 'pi pi-calendar', label: 'Calendar' },
  { path: '/cheltuieli', icon: 'pi pi-wallet', label: 'Expenses' },
  { path: '/tasks', icon: 'pi pi-check-square', label: 'Tasks' },
]

const moreNavItems = [
  { path: '/shopping', icon: 'pi pi-shopping-cart', label: 'Shopping' },
  { path: '/obiective', icon: 'pi pi-chart-pie', label: 'Goals' },
  { path: '/groups', icon: 'pi pi-users', label: 'Groups' },
]

const ownerInitial = computed(() => {
  if (!owner.value) return 'U'
  return owner.value.charAt(0).toUpperCase()
})

onMounted(() => {
  owner.value = localStorage.getItem('owner') || 'User'
  ownerEmail.value = localStorage.getItem('email') || ''
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

const handleClickOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement
  if (!target.closest('.user-menu-wrapper')) {
    showUserMenu.value = false
  }
  if (!target.closest('.more-menu')) {
    showMoreMenu.value = false
  }
}

const toggleUserMenu = () => {
  showUserMenu.value = !showUserMenu.value
  showMoreMenu.value = false
}

const toggleMoreMenu = () => {
  showMoreMenu.value = !showMoreMenu.value
  showUserMenu.value = false
}

const closeMoreMenu = () => {
  showMoreMenu.value = false
}

const handleLogout = () => {
  localStorage.removeItem('owner')
  localStorage.removeItem('email')
  localStorage.removeItem('token')
  showUserMenu.value = false
  router.push('/login')
}
</script>

<style scoped>
.navbar-container {
  width: 100%;
  background: var(--color-card);
  backdrop-filter: blur(10px);
  box-shadow: 0 2px 20px rgba(0, 0, 0, 0.08);
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
}

/* Desktop Navigation */
.desktop-nav {
  display: none;
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 2rem;
  height: 70px;
  align-items: center;
  justify-content: space-between;
  gap: 2rem;
}

.nav-brand {
  flex-shrink: 0;
}

.brand-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  color: var(--color-text-dark);
  transition: all 0.3s ease;
}

.brand-link:hover {
  transform: translateY(-2px);
}

.brand-icon {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--color-primary), var(--color-accent-blue));
  border-radius: 10px;
  color: white;
  font-size: 1.25rem;
  box-shadow: 0 4px 12px rgba(45, 125, 210, 0.3);
}

.brand-text {
  font-size: 1.25rem;
  font-weight: 700;
  background: linear-gradient(135deg, var(--color-primary), var(--color-accent-blue));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.nav-center {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
  justify-content: center;
}

.nav-item {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  color: var(--color-text-muted);
  text-decoration: none;
  border-radius: 10px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  font-weight: 500;
  font-size: 0.9375rem;
}

.nav-item i {
  font-size: 1.125rem;
  transition: transform 0.3s ease;
}

.nav-item:hover {
  color: var(--color-primary);
  background: rgba(45, 125, 210, 0.08);
  transform: translateY(-1px);
}

.nav-item:hover i {
  transform: scale(1.1);
}

.nav-item.active {
  color: var(--color-primary);
  background: rgba(45, 125, 210, 0.12);
  font-weight: 600;
}

.nav-item.active .nav-indicator {
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 4px;
  height: 4px;
  background: var(--color-primary);
  border-radius: 50%;
}

.nav-label {
  white-space: nowrap;
}

.nav-right {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-shrink: 0;
}

.groups-link {
  background: linear-gradient(135deg, rgba(45, 125, 210, 0.1), rgba(53, 167, 255, 0.1));
  border: 1px solid rgba(45, 125, 210, 0.2);
}

.groups-link:hover {
  background: linear-gradient(135deg, rgba(45, 125, 210, 0.15), rgba(53, 167, 255, 0.15));
  border-color: var(--color-primary);
}

.user-menu-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 1rem;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
  background: var(--color-background);
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.user-menu-wrapper:hover {
  background: rgba(45, 125, 210, 0.08);
  border-color: rgba(45, 125, 210, 0.2);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(45, 125, 210, 0.15);
}

.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: linear-gradient(135deg, var(--color-primary), var(--color-accent-blue));
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 0.875rem;
  box-shadow: 0 2px 8px rgba(45, 125, 210, 0.3);
}

.avatar-initial {
  line-height: 1;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.user-name {
  font-weight: 600;
  color: var(--color-text-dark);
  font-size: 0.9375rem;
}

.user-info i {
  color: var(--color-text-muted);
  font-size: 0.75rem;
  transition: transform 0.3s ease;
}

.user-info i.rotated {
  transform: rotate(180deg);
}

.user-dropdown {
  position: absolute;
  top: calc(100% + 0.75rem);
  right: 0;
  background: var(--color-card);
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  min-width: 240px;
  overflow: hidden;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.dropdown-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: var(--color-background);
}

.dropdown-avatar {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  background: linear-gradient(135deg, var(--color-primary), var(--color-accent-blue));
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 1.125rem;
  box-shadow: 0 4px 12px rgba(45, 125, 210, 0.3);
}

.dropdown-user-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  flex: 1;
}

.dropdown-name {
  font-weight: 600;
  color: var(--color-text-dark);
  font-size: 0.9375rem;
}

.dropdown-email {
  font-size: 0.8125rem;
  color: var(--color-text-muted);
}

.dropdown-divider {
  height: 1px;
  background: rgba(0, 0, 0, 0.05);
  margin: 0.5rem 0;
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.875rem 1rem;
  color: var(--color-text-muted);
  cursor: pointer;
  transition: all 0.2s ease;
  text-decoration: none;
  font-size: 0.9375rem;
}

.dropdown-item i {
  font-size: 1rem;
  width: 20px;
  text-align: center;
}

.dropdown-item:hover {
  background: var(--color-background);
  color: var(--color-primary);
}

.dropdown-item.active {
  background: rgba(45, 125, 210, 0.1);
  color: var(--color-primary);
  font-weight: 600;
}

/* Mobile Navigation */
.mobile-nav {
  display: flex;
  justify-content: space-around;
  align-items: center;
  padding: 0.75rem 0.5rem;
  background: var(--color-card);
  border-top: 1px solid rgba(0, 0, 0, 0.05);
}

.mobile-nav-item {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
  padding: 0.5rem 0.75rem;
  color: var(--color-text-muted);
  text-decoration: none;
  border-radius: 10px;
  transition: all 0.3s ease;
  min-width: 60px;
}

.mobile-nav-item i {
  font-size: 1.25rem;
  transition: transform 0.3s ease;
}

.mobile-nav-item:hover {
  color: var(--color-primary);
  background: rgba(45, 125, 210, 0.08);
}

.mobile-nav-item.active {
  color: var(--color-primary);
  background: rgba(45, 125, 210, 0.12);
}

.mobile-nav-item.active::before {
  content: '';
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 30px;
  height: 3px;
  background: var(--color-primary);
  border-radius: 0 0 3px 3px;
}

.mobile-label {
  font-size: 0.6875rem;
  font-weight: 500;
  white-space: nowrap;
}

.more-menu {
  cursor: pointer;
}

.more-dropdown {
  position: absolute;
  bottom: calc(100% + 0.75rem);
  left: 50%;
  transform: translateX(-50%);
  background: var(--color-card);
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  min-width: 180px;
  overflow: hidden;
  border: 1px solid rgba(0, 0, 0, 0.05);
  padding: 0.5rem 0;
}

.more-dropdown .dropdown-item {
  padding: 0.75rem 1rem;
}

/* Transitions */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.dropdown-enter-from {
  opacity: 0;
  transform: translateY(-10px) scale(0.95);
}

.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px) scale(0.95);
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
}

/* Mobile styles */
@media screen and (max-width: 767px) {
  .navbar-container {
    top: auto;
    bottom: 0;
    border-top: 1px solid rgba(0, 0, 0, 0.05);
    border-bottom: none;
  }

  .mobile-nav {
    display: flex;
  }

  .desktop-nav {
    display: none;
  }
}

/* Tablet adjustments */
@media screen and (min-width: 768px) and (max-width: 1024px) {
  .desktop-nav {
    padding: 0 1.5rem;
  }

  .nav-item .nav-label {
    display: none;
  }

  .nav-item {
    padding: 0.625rem;
    justify-content: center;
  }
}
</style>
