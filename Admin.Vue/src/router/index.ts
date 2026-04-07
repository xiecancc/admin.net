import { createRouter, createWebHistory } from "vue-router";
import type { RouteRecordRaw } from "vue-router";
import { setupRouterGuard } from "./guard";

const routes: RouteRecordRaw[] = [
  {
    children: [
      {
        path: "",
        redirect: "login",
      },
      {
        component: () => import("../views/frontend/login/index.vue"),
        meta: {
          requiresAuth: false,
          title: "登录",
        },
        name: "Login",
        path: "login",
      },
      {
        component: () => import("../views/frontend/register/index.vue"),
        meta: {
          requiresAuth: false,
          title: "注册",
        },
        name: "Register",
        path: "register",
      },
    ],
    component: () => import("../layouts/frontend/index.vue"),
    path: "/",
  },
  {
    children: [
      {
        path: "",
        redirect: "dashboard",
      },
      {
        component: () => import("../views/backend/dashboard/index.vue"),
        meta: {
          icon: "HomeFilled",
          requiresAuth: true,
          title: "仪表盘",
        },
        name: "Dashboard",
        path: "dashboard",
      },
      {
        component: () => import("../views/backend/users/index.vue"),
        meta: {
          icon: "UserFilled",
          permissions: ["users:view"],
          requiresAuth: true,
          title: "用户管理",
        },
        name: "Users",
        path: "users",
      },
      {
        component: () => import("../views/backend/roles/index.vue"),
        meta: {
          icon: "User",
          permissions: ["roles:view"],
          requiresAuth: true,
          title: "角色管理",
        },
        name: "Roles",
        path: "roles",
      },
      {
        component: () => import("../views/backend/menu-permissions/index.vue"),
        meta: {
          icon: "Menu",
          permissions: ["menu-permissions:view"],
          requiresAuth: true,
          title: "菜单权限",
        },
        name: "MenuPermissions",
        path: "menu-permissions",
      },
      {
        component: () => import("../views/backend/button-permissions/index.vue"),
        meta: {
          icon: "Grid",
          permissions: ["button-permissions:view"],
          requiresAuth: true,
          title: "按钮权限",
        },
        name: "ButtonPermissions",
        path: "button-permissions",
      },
      {
        component: () => import("../views/backend/api-permissions/index.vue"),
        meta: {
          icon: "Connection",
          permissions: ["api-permissions:view"],
          requiresAuth: true,
          title: "API 权限",
        },
        name: "ApiPermissions",
        path: "api-permissions",
      },
      {
        component: () => import("../views/backend/settings/index.vue"),
        meta: {
          icon: "Setting",
          permissions: ["settings:view"],
          requiresAuth: true,
          title: "系统设置",
        },
        name: "Settings",
        path: "settings",
      },
    ],
    component: () => import("../layouts/backend/index.vue"),
    meta: {
      requiresAuth: true,
    },
    path: "/admin",
  },
  {
    component: () => import("../views/error/404.vue"),
    meta: {
      hidden: true,
      title: "页面不存在",
    },
    name: "NotFound",
    path: "/:pathMatch(.*)*",
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

setupRouterGuard(router);

export default router;
