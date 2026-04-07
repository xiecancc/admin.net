import type { Router } from "vue-router";
import { useAuthStore } from "../stores/auth.store";
import { APP } from "../constants/env.constant";
import { ROUTES, WHITE_LIST } from "../constants/app.constant";

export function setupRouterGuard(router: Router) {
  router.beforeEach(async (to, from, next) => {
    const authStore = useAuthStore();
    const { token } = authStore;

    if (token) {
      if (to.path === ROUTES.LOGIN) {
        next({ path: ROUTES.DASHBOARD });
      } else {
        if (authStore.userInfo) {
          next();
        } else {
          try {
            // TODO: 获取用户信息
            // Await authStore.getUserInfo()
            next();
          } catch {
            authStore.logout();
            next({ path: ROUTES.LOGIN, query: { redirect: to.fullPath } });
          }
        }
      }
    } else {
      if (WHITE_LIST.includes(to.path)) {
        next();
      } else {
        next({ path: ROUTES.LOGIN, query: { redirect: to.fullPath } });
      }
    }
  });

  router.afterEach((to) => {
    const title = to.meta?.title as string;
    if (title) {
      document.title = `${title} - ${APP.TITLE}`;
    } else {
      document.title = APP.TITLE;
    }
  });

  router.onError((error) => {
    console.error("路由错误:", error);
  });
}
