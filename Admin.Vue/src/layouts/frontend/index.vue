<template>
  <el-container class="frontend-layout">
    <!-- 顶部导航 -->
    <el-header class="frontend-header">
      <div class="header-content">
        <div class="logo">
          <router-link to="/">
            <el-icon :size="28">
              <Shop />
            </el-icon>
            <span class="logo-text">Admin Vue</span>
          </router-link>
        </div>
        <nav class="header-nav">
          <el-menu
            :default-active="activeMenu"
            mode="horizontal"
            class="header-menu"
            router
          >
            <el-menu-item index="/">首页</el-menu-item>
            <el-menu-item index="/about">关于</el-menu-item>
            <el-menu-item index="/products">产品</el-menu-item>
          </el-menu>
          <div class="nav-actions">
            <template v-if="isAuthenticated">
              <el-dropdown trigger="click">
                <div class="user-dropdown">
                  <el-avatar
                    :size="32"
                    :src="userAvatar"
                  />
                  <span class="user-name">{{ userName }}</span>
                  <el-icon>
                    <ArrowDown />
                  </el-icon>
                </div>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item @click="navigateTo('/profile')">
                      <el-icon>
                        <User />
                      </el-icon>
                      个人中心
                    </el-dropdown-item>
                    <el-dropdown-item @click="navigateTo('/settings')">
                      <el-icon>
                        <Setting />
                      </el-icon>
                      设置
                    </el-dropdown-item>
                    <el-dropdown-item
                      divided
                      @click="handleLogout"
                    >
                      <el-icon>
                        <SwitchButton />
                      </el-icon>
                      退出登录
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
            </template>
            <template v-else>
              <el-button
                text
                @click="navigateTo('/login')"
                >登录</el-button
              >
              <el-button
                type="primary"
                @click="navigateTo('/register')"
                >注册</el-button
              >
            </template>
          </div>
        </nav>
      </div>
    </el-header>

    <!-- 主内容区 -->
    <el-main class="frontend-main">
      <router-view />
    </el-main>

    <!-- 页脚 -->
    <el-footer class="frontend-footer">
      <div class="footer-content">
        <p>&copy; {{ new Date().getFullYear() }} Admin Vue. All rights reserved.</p>
        <div class="footer-links">
          <a
            href="#"
            @click.prevent="navigateTo('/privacy')"
            >隐私政策</a
          >
          <a
            href="#"
            @click.prevent="navigateTo('/terms')"
            >服务条款</a
          >
          <a
            href="#"
            @click.prevent="navigateTo('/contact')"
            >联系我们</a
          >
        </div>
      </div>
    </el-footer>
  </el-container>
</template>

<script setup lang="ts">
  import { computed } from 'vue'
  import { useRoute, useRouter } from 'vue-router'
  import { useAuthStore } from '@/stores/auth.store'
  import { ArrowDown, Setting, Shop, SwitchButton, User } from '@element-plus/icons-vue'

  const router = useRouter()
  const route = useRoute()
  const authStore = useAuthStore()

  const activeMenu = computed(() => route.path)

  const isAuthenticated = computed(() => authStore.isAuthenticated)

  const userAvatar = computed(
    () => authStore.userInfo?.avatar || 'https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d50'
  )

  const userName = computed(() => authStore.userInfo?.nickName || authStore.userInfo?.email || '用户')

  const navigateTo = (path: string) => {
    router.push(path)
  }

  const handleLogout = () => {
    authStore.logout()
    navigateTo('/')
  }
</script>

<style scoped lang="scss">
  .frontend-layout {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    background-color: var(--el-bg-color-page);
  }

  .frontend-header {
    padding: 0;
    background-color: var(--el-bg-color);
    box-shadow: var(--el-box-shadow-light);

    .header-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 20px;
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 60px;
    }

    .logo {
      display: flex;
      align-items: center;
      gap: 8px;

      a {
        display: flex;
        align-items: center;
        gap: 8px;
        text-decoration: none;
        color: var(--el-text-color-primary);
        font-size: 18px;
        font-weight: 600;
      }
    }

    .header-nav {
      display: flex;
      align-items: center;
      gap: 16px;

      .header-menu {
        border: none;
        background-color: transparent;
        flex: 1;
        max-width: 400px;

        .el-menu-item {
          border: none;
        }
      }

      .nav-actions {
        display: flex;
        align-items: center;
        gap: 12px;

        .user-dropdown {
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
  }

  .frontend-main {
    flex: 1;
    padding: 0;
  }

  .frontend-footer {
    padding: 20px 0;
    background-color: var(--el-bg-color);
    border-top: 1px solid var(--el-border-color-light);

    .footer-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 20px;
      display: flex;
      justify-content: space-between;
      align-items: center;

      p {
        margin: 0;
        font-size: 14px;
        color: var(--el-text-color-secondary);

        .footer-links {
          display: flex;
          gap: 20px;

          a {
            text-decoration: none;
            font-size: 14px;
            color: var(--el-text-color-secondary);
            transition: color 0.3s;

            &:hover {
              color: var(--el-color-primary);
            }
          }
        }
      }
    }
  }
</style>
