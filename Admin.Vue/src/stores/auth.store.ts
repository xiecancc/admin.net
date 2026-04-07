import { defineStore } from "pinia";
import { computed, ref } from "vue";
import { STORAGE_KEYS } from "@/constants/app.constant";
import { authApi } from "@/api";
import type { LoginRequestDTO, LoginResponseDTO, UserInfoDTO } from "@/api/nswags/auto";

export const useAuthStore = defineStore(
  "auth",
  () => {
    const token = ref<string>("");
    const refreshToken = ref<string>("");
    const userInfo = ref<UserInfoDTO | null>(null);

    const isAuthenticated = computed(() => Boolean(token.value));
    const userRoles = computed(() => userInfo.value?.roles || []);
    const userPermissions = computed(() => [] as string[]);

    const setToken = (accessToken: string, refresh?: string) => {
      token.value = accessToken;
      if (refresh) {
        refreshToken.value = refresh;
      }
    };

    const setUserInfo = (info: UserInfoDTO) => {
      userInfo.value = info;
    };

    const clearAuth = () => {
      token.value = "";
      refreshToken.value = "";
      userInfo.value = null;
    };

    const login = async (credentials: LoginRequestDTO): Promise<LoginResponseDTO> => {
      const res = await authApi.login(credentials);
      console.log("登录响应:", res);
      if (res.success && res.data) {
        setToken(res.data.accessToken, res.data.refreshToken);
        setUserInfo(res.data.user);
        return res.data;
      }

      throw new Error(res.message || "登录失败");
    };

    const logout = async (): Promise<void> => {
      try {
        await authApi.logout();
      } catch {
        // Ignore
      } finally {
        clearAuth();
      }
    };

    const getCurrentUser = async (): Promise<UserInfoDTO> => {
      const res = await authApi.getCurrentUser();

      if (res.success && res.data) {
        setUserInfo(res.data);
        return res.data;
      }

      throw new Error(res.message || "获取用户信息失败");
    };

    const hasPermission = (permission: string): boolean => {
      if (!userInfo.value) {
        return false;
      }
      return userPermissions.value.includes("*") || userPermissions.value.includes(permission);
    };

    const hasRole = (role: string): boolean => userRoles.value.includes(role);

    return {
      clearAuth,
      getCurrentUser,
      hasPermission,
      hasRole,
      isAuthenticated,
      login,
      logout,
      refreshToken,
      setToken,
      setUserInfo,
      token,
      userInfo,
      userPermissions,
      userRoles,
    };
  },
  {
    persist: {
      key: STORAGE_KEYS.AUTH,
      pick: ["token", "refreshToken", "userInfo"],
      storage: localStorage,
    },
  },
);
