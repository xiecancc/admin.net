<template>
  <el-container class="layout-container">
    <!-- 侧边栏 -->
    <el-aside :width="sidebarCollapsed ? '64px' : '220px'" class="layout-aside">
      <div class="aside-header">
        <el-icon :size="24">
          <Shop />
        </el-icon>
        <span class="aside-title" v-show="!sidebarCollapsed">Admin Vue</span>
      </div>

      <el-menu
        :default-active="activeMenu"
        :collapse="sidebarCollapsed"
        :unique-opened="true"
        :collapse-transition="true"
        router
        class="aside-menu"
      >
        <el-menu-item index="/admin/dashboard">
          <el-icon>
            <House />
          </el-icon>
          <template #title>仪表盘</template>
        </el-menu-item>

        <el-sub-menu index="user-management">
          <template #title>
            <el-icon>
              <User />
            </el-icon>
            <span>用户管理</span>
          </template>
          <el-menu-item index="/admin/users">用户管理</el-menu-item>
          <el-menu-item index="/admin/roles">角色管理</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="permission-management">
          <template #title>
            <el-icon>
              <Lock />
            </el-icon>
            <span>权限管理</span>
          </template>
          <el-menu-item index="/admin/menu-permissions">菜单权限</el-menu-item>
          <el-menu-item index="/admin/button-permissions">按钮权限</el-menu-item>
          <el-menu-item index="/admin/api-permissions">API 权限</el-menu-item>
        </el-sub-menu>

        <el-menu-item index="/admin/settings">
          <el-icon>
            <Setting />
          </el-icon>
          <template #title>系统设置</template>
        </el-menu-item>
      </el-menu>
    </el-aside>

    <!-- 主内容区 -->
    <el-container class="main-container">
      <!-- 顶部导航 -->
      <el-header class="layout-header">
        <div class="header-left">
          <el-icon class="collapse-btn" @click="toggleSidebar">
            <component :is="sidebarCollapsed ? 'Expand' : 'Fold'" />
          </el-icon>
          <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: '/admin/dashboard' }">首页</el-breadcrumb-item>
            <el-breadcrumb-item v-for="item in breadcrumbs" :key="item.path">
              {{ item.title }}
            </el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        <div class="header-right">
          <el-button :icon="FullScreen" circle @click="toggleFullscreen" title="全屏" />
          <el-button :icon="Refresh" circle @click="refreshPage" :loading="refreshing" title="刷新" />
          <el-divider direction="vertical" />
          <el-dropdown trigger="click">
            <div class="user-info">
              <el-avatar :size="32" :src="userAvatar" />
              <span class="user-name" v-show="!sidebarCollapsed">{{ userName }}</span>
              <el-icon>
                <ArrowDown />
              </el-icon>
            </div>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item @click="goToProfile">
                  <el-icon>
                    <User />
                  </el-icon>
                  个人中心
                </el-dropdown-item>
                <el-dropdown-item @click="openSettings">
                  <el-icon>
                    <Setting />
                  </el-icon>
                  个人设置
                </el-dropdown-item>
                <el-dropdown-item divided @click="logout">
                  <el-icon>
                    <SwitchButton />
                  </el-icon>
                  退出登录
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>

      <!-- 内容区域 -->
      <el-main class="layout-main">
        <router-view v-slot="{ Component }">
          <transition name="fade-transform" mode="out-in">
            <keep-alive>
              <component :is="Component" />
            </keep-alive>
          </transition>
        </router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth.store";
import { ROUTES } from "@/constants/app.constant";
import { useMessage } from "@/hooks/useMessage";
import { FullScreen, Refresh } from "@/plugins/icons.plugin";

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const { success } = useMessage();

const sidebarCollapsed = ref(false);
const refreshing = ref(false);

const activeMenu = computed(() => route.path);

const breadcrumbs = computed(() => {
  const matched = route.matched.filter((item) => item.meta && item.meta.title);
  return matched.map((item) => ({
    path: item.path,
    title: item.meta.title as string,
  }));
});

const userAvatar = computed(
  () => authStore.userInfo?.avatar || "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d50",
);
const userName = computed(() => authStore.userInfo?.nickName || authStore.userInfo?.email || "用户");

const toggleSidebar = () => {
  sidebarCollapsed.value = !sidebarCollapsed.value;
};

const toggleFullscreen = () => {
  if (!document.fullscreenElement) {
    document.documentElement.requestFullscreen();
  } else {
    document.exitFullscreen();
  }
};

const refreshPage = async () => {
  refreshing.value = true;
  await router.replace({
    path: "/redirect" + route.fullPath,
  });
  setTimeout(() => {
    refreshing.value = false;
  }, 500);
};

const goToProfile = () => {
  router.push("/profile");
};

const openSettings = () => {
  router.push("/admin/settings");
};

const logout = () => {
  authStore.logout();
  success("已退出登录");
  router.push(ROUTES.LOGIN);
};

watch(
  () => route.path,
  () => {
    sidebarCollapsed.value = window.innerWidth < 768;
  },
  { immediate: true },
);
</script>

<style scoped lang="scss">
.layout-container {
  height: 100vh;
  background-color: var(--el-bg-color-page);
}

.layout-aside {
  background-color: var(--el-bg-color);
  border-right: 1px solid var(--el-border-color-light);
  display: flex;
  flex-direction: column;
}

.aside-header {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  border-bottom: 1px solid var(--el-border-color-light);

  .aside-title {
    font-size: 16px;
    font-weight: 600;
    color: var(--el-text-color-primary);
  }
}

.aside-menu {
  border-right: none;
  flex: 1;
  overflow-y: auto;
  background-color: transparent;
}

.layout-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
  background-color: var(--el-bg-color);
  border-bottom: 1px solid var(--el-border-color-light);

  .header-left {
    display: flex;
    align-items: center;
    gap: 16px;

    .collapse-btn {
      font-size: 20px;
      cursor: pointer;
      color: var(--el-text-color-regular);
      transition: color 0.3s;

      &:hover {
        color: var(--el-color-primary);
      }
    }
  }

  .header-right {
    display: flex;
    align-items: center;
    gap: 12px;

    .user-info {
      display: flex;
      align-items: center;
      gap: 8px;
      cursor: pointer;
      padding: 4px 8px;
      border-radius: var(--el-border-radius-base);
      transition: background-color 0.3s;

      &:hover {
        background-color: var(--el-fill-color-light);
      }

      .user-name {
        font-size: 14px;
        color: var(--el-text-color-regular);
      }
    }
  }
}

.layout-main {
  padding: 20px;
  background-color: var(--el-bg-color-page);
}
</style>
