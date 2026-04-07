<template>
  <div class="register-container">
    <div class="register-form-wrapper">
      <h2 class="register-title">用户注册</h2>
      <el-form
        :model="registerForm"
        :rules="registerRules"
        ref="registerFormRef"
        label-width="80px"
      >
        <el-form-item
          label="邮箱"
          prop="email"
        >
          <el-input
            v-model="registerForm.email"
            type="email"
            placeholder="请输入邮箱"
          />
        </el-form-item>
        <el-form-item
          label="密码"
          prop="password"
        >
          <el-input
            v-model="registerForm.password"
            type="password"
            placeholder="请输入密码"
            show-password
          />
        </el-form-item>
        <el-form-item
          label="确认密码"
          prop="confirmPassword"
        >
          <el-input
            v-model="registerForm.confirmPassword"
            type="password"
            placeholder="请确认密码"
            show-password
          />
        </el-form-item>
        <el-form-item>
          <el-button
            type="primary"
            @click="handleRegister"
            :loading="loading"
            >注册</el-button
          >
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
        <el-form-item>
          <router-link
            to="/login"
            class="login-link"
            >已有账号？立即登录</router-link
          >
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref } from 'vue'
  import { useRouter } from 'vue-router'
  import type { FormInstance, FormRules } from 'element-plus'

  const router = useRouter()
  const registerFormRef = ref<FormInstance>()
  const loading = ref(false)

  const registerForm = reactive({
    confirmPassword: '',
    email: '',
    password: '',
  })

  const registerRules = reactive<FormRules>({
    confirmPassword: [
      { message: '请确认密码', required: true, trigger: 'blur' },
      {
        trigger: 'blur',
        validator: (rule, value, callback) => {
          if (value !== registerForm.password) {
            callback(new Error('两次输入的密码不一致'))
          } else {
            callback()
          }
        },
      },
    ],
    email: [
      { message: '请输入邮箱', required: true, trigger: 'blur' },
      { message: '请输入正确的邮箱格式', trigger: 'blur', type: 'email' },
    ],
    password: [
      { message: '请输入密码', required: true, trigger: 'blur' },
      { message: '密码长度至少为6位', min: 6, trigger: 'blur' },
    ],
  })

  const handleRegister = async () => {
    if (!registerFormRef.value) {
      return
    }

    try {
      await registerFormRef.value.validate()
      loading.value = true

      // 模拟注册请求
      setTimeout(() => {
        loading.value = false
        // 注册成功后跳转到登录页
        router.push('/login')
      }, 1000)
    } catch (error) {
      console.error('注册验证失败:', error)
    }
  }

  const resetForm = () => {
    registerFormRef.value?.resetFields()
  }
</script>

<style scoped lang="scss">
  .register-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 60vh;
    background-color: var(--el-bg-color-page);
  }

  .register-form-wrapper {
    width: 100%;
    max-width: 400px;
    padding: 2rem;
    background-color: var(--el-bg-color);
    border-radius: var(--el-border-radius-base);
    box-shadow: var(--el-box-shadow-light);
  }

  .register-title {
    text-align: center;
    margin-bottom: 1.5rem;
    color: var(--el-text-color-primary);
    font-size: var(--el-font-size-large);
    font-weight: 600;
  }

  .login-link {
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
