<template>
  <el-config-provider
    :locale="zhCn"
    :size="appStore.appSize"
  >
    <div class="app-layout">
      <router-view />
    </div>
  </el-config-provider>
</template>

<script setup lang="ts">
  import { ElConfigProvider } from 'element-plus'
  import { useAppStore } from '@/stores/app.store'
  import zhCn from 'element-plus/es/locale/lang/zh-cn'

  const appStore = useAppStore()
</script>

<style lang="scss">
  // 主布局
  .app-layout {
    height: 100vh;
    width: 100vw;
    overflow: hidden;
  }
</style>
