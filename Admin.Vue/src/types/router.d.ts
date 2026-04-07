import type { RouteMeta } from "vue-router";

declare module "vue-router" {
  interface RouteMeta {
    title?: string;
    icon?: string;
    hidden?: boolean;
    requiresAuth?: boolean;
    permissions?: string[];
    roles?: string[];
    keepAlive?: boolean;
    breadcrumb?: boolean;
  }
}

export interface MenuRoute {
  path: string;
  name: string;
  title?: string;
  icon?: string;
  hidden?: boolean;
  children?: MenuRoute[];
}
