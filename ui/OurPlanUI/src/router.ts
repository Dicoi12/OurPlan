import { createWebHistory, createRouter } from "vue-router";
import { useUserStore } from "./stores/userStore";
import { defineAsyncComponent } from "vue";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      component: defineAsyncComponent(() => import("./views/Main.vue")),
      meta: { requiresAuth: true },
    },

    {
      path: "/login",
      component: defineAsyncComponent(() => import("./views/LoginPage.vue")),
    },
    {
      path: "/groups",
      component: defineAsyncComponent(() => import("./views/GroupsPage.vue")),
      meta: { requiresAuth: true },
    },
    {
      path: "/events",
      component: defineAsyncComponent(() => import("./views/EventsPage.vue")),
      meta: { requiresAuth: true },
    },
  ],
});
router.beforeEach(async (to, _from, next) => {
  const authStore = useUserStore();
  authStore.syncTokenFromCookie();
  
  if (to.meta.requiresAuth) {
    if (authStore.isAuthenticated === false) {
      next("/login");
      return;
    }
    
    // Dacă utilizatorul este autentificat dar nu are date, le încărcăm
    if (authStore.isAuthenticated && !authStore.hasUserData) {
      await authStore.loadCurrentUser();
    }
  }
  
  next();
});
export default router;
