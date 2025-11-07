import { createWebHistory, createRouter } from "vue-router";
import { useUserStore } from "./stores/userStore";
import { defineAsyncComponent } from "vue";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    // { path: "/", component: defineAsyncComponent(()=>import("./views/MainPage.vue")) },
    {
      path: "/login",
      component: defineAsyncComponent(() => import("./views/LoginPage.vue")),
    },
    {
      path:"/groups",
      component: defineAsyncComponent(() => import("./views/GroupsPage.vue")),
      meta: { requiresAuth: true },
    }
  ],
});
router.beforeEach((to, from, next) => {
  const authStore = useUserStore();
  
  if (to.meta.requiresAuth && !authStore.token) {
    next('/login'); 
  } else {
    next();
  }
});
export default router;