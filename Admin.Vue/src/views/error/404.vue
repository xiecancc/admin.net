<template>
  <div class="error-page">
    <div class="error-content">
      <h1 class="error-code">404</h1>
      <h2 class="error-title">页面不存在</h2>
      <p class="error-desc">抱歉，您访问的页面不存在或已被删除</p>
      <el-button
        type="primary"
        @click="goHome"
        >返回首页</el-button
      >
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useRouter } from 'vue-router'
  import { ROUTES } from '@/constants/app.constant'

  const router = useRouter()

  const goHome = () => {
    router.push(ROUTES.DASHBOARD)
  }
</script>

<style scoped lang="scss">
  .error-page {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background-color: var(--el-bg-color-page);
  }

  .error-content {
    text-align: center;
    color: var(--el-text-color-primary);
  }

  .error-code {
    font-size: 120px;
    font-weight: 700;
    margin: 0;
    line-height: 1;
    color: var(--el-color-primary);
  }

  .error-title {
    font-size: 32px;
    font-weight: 500;
    margin: 20px 0 10px;
    color: var(--el-text-color-primary);
  }

  .error-desc {
    font-size: 16px;
    color: var(--el-text-color-secondary);
    margin-bottom: 30px;
  }
</style>
