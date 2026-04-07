<template>
  <div class="login-container">
    <div class="login-form-wrapper">
      <h2 class="login-title">用户登录</h2>
      <el-form
        :model="loginForm"
        :rules="loginRules"
        ref="loginFormRef"
        label-width="80px"
      >
        <el-form-item
          label="邮箱"
          prop="email"
        >
          <el-input
            v-model="loginForm.email"
            type="email"
            placeholder="请输入邮箱"
          />
        </el-form-item>
        <el-form-item
          label="密码"
          prop="password"
        >
          <el-input
            v-model="loginForm.password"
            type="password"
            placeholder="请输入密码"
            show-password
          />
        </el-form-item>
        <el-form-item>
          <el-checkbox v-model="loginForm.rememberMe">记住我</el-checkbox>
        </el-form-item>
        <el-form-item>
          <el-button
            type="primary"
            @click="handleLogin"
            :loading="loading"
            >登录</el-button
          >
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
        <el-form-item>
          <router-link
            to="/register"
            class="register-link"
            >还没有账号？立即注册</router-link
          >
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref } from 'vue'
  import { useRoute, useRouter } from 'vue-router'
  import type { FormInstance, FormRules } from 'element-plus'
  import { useAuthStore } from '@/stores/auth.store'
  import type { LoginRequestDTO } from '@/api/nswags/auto'
  import { ROUTES } from '@/constants/app.constant'

  const router = useRouter()
  const route = useRoute()
  const authStore = useAuthStore()
  const loginFormRef = ref<FormInstance>()
  const loading = ref(false)

  const loginForm = reactive<LoginRequestDTO>({
    email: 'admin@example.com',
    password: 'Admin123!',
    rememberMe: false,
  })

  const loginRules = reactive<FormRules>({
    email: [
      { message: '请输入邮箱', required: true, trigger: 'blur' },
      { message: '请输入正确的邮箱格式', trigger: 'blur', type: 'email' },
    ],
    password: [{ message: '请输入密码', required: true, trigger: 'blur' }],
  })

  const handleLogin = async () => {
    if (!loginFormRef.value) {
      return
    }

    try {
      await loginFormRef.value.validate()
      loading.value = true

      await authStore.login(loginForm)

      ElMessage.success('登录成功')

      const redirect = route.query.redirect as string
      router.push(redirect || ROUTES.DASHBOARD)
    } catch (error) {
      console.error('登录失败:', error)
      ElMessage.error('登录失败，请检查用户名和密码')
    } finally {
      loading.value = false
    }
  }

  const resetForm = () => {
    loginFormRef.value?.resetFields()
  }
</script>

<style scoped lang="scss">
  .login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 60vh;
  }

  .login-form-wrapper {
    width: 100%;
    max-width: 400px;
    padding: 2rem;
    background-color: var(--el-bg-color);
    border-radius: var(--el-border-radius-base);
    box-shadow: var(--el-box-shadow-light);
  }

  .login-title {
    text-align: center;
    margin-bottom: 1.5rem;
    color: var(--el-text-color-primary);
    font-size: var(--el-font-size-large);
    font-weight: 600;
  }

  .register-link {
    display: block;
    text-align: right;
    margin-top: 1rem;
    color: var(--el-color-primary);
    text-decoration: none;
    font-size: var(--el-font-size-base);

    &:hover {
      color: var(--el-color-primary-light-3);
    }
  }
</style>
