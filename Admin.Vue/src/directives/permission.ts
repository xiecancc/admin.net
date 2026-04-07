import type { Directive, DirectiveBinding } from "vue";
import { useAuthStore } from "@/stores/auth.store";

export const permission: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    const authStore = useAuthStore();
    const { value } = binding;

    if (!value) {
      return;
    }

    const permissions = Array.isArray(value) ? value : [value];
    const hasPermission = permissions.some((p) => authStore.hasPermission(p));

    if (!hasPermission) {
      el.parentNode?.removeChild(el);
    }
  },
};

export const role: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    const authStore = useAuthStore();
    const { value } = binding;

    if (!value) {
      return;
    }

    const roles = Array.isArray(value) ? value : [value];
    const hasRole = roles.some((r) => authStore.hasRole(r));

    if (!hasRole) {
      el.parentNode?.removeChild(el);
    }
  },
};

export const auth: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding<boolean>) {
    const authStore = useAuthStore();
    const { value } = binding;

    if (value && !authStore.isAuthenticated) {
      el.parentNode?.removeChild(el);
    }
  },
};
