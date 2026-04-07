<template>
  <div class="settings-container">
    <h1 class="settings-title">系统设置</h1>
    <el-card
      shadow="hover"
      class="settings-card"
    >
      <template #header>
        <div class="card-header">
          <span>基本设置</span>
        </div>
      </template>
      <el-form
        :model="settingsForm"
        :rules="settingsRules"
        ref="settingsFormRef"
        label-width="120px"
      >
        <el-form-item
          label="网站名称"
          prop="siteName"
        >
          <el-input
            v-model="settingsForm.siteName"
            placeholder="请输入网站名称"
          />
        </el-form-item>
        <el-form-item
          label="网站描述"
          prop="siteDescription"
        >
          <el-input
            v-model="settingsForm.siteDescription"
            type="textarea"
            placeholder="请输入网站描述"
          />
        </el-form-item>
        <el-form-item
          label="网站Logo"
          prop="siteLogo"
        >
          <el-upload
            class="avatar-uploader"
            action="#"
            :show-file-list="false"
            :on-change="handleLogoUpload"
            :before-upload="beforeLogoUpload"
          >
            <img
              v-if="settingsForm.siteLogo"
              :src="settingsForm.siteLogo"
              class="avatar"
            />
            <el-icon
              v-else
              class="avatar-uploader-icon"
            >
              <Plus />
            </el-icon>
          </el-upload>
        </el-form-item>
        <el-form-item
          label="系统语言"
          prop="language"
        >
          <el-select
            v-model="settingsForm.language"
            placeholder="请选择系统语言"
          >
            <el-option
              label="中文"
              value="zh-CN"
            ></el-option>
            <el-option
              label="英文"
              value="en-US"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item
          label="时区"
          prop="timezone"
        >
          <el-select
            v-model="settingsForm.timezone"
            placeholder="请选择时区"
          >
            <el-option
              label="Asia/Shanghai (UTC+8)"
              value="Asia/Shanghai"
            ></el-option>
            <el-option
              label="America/New_York (UTC-5)"
              value="America/New_York"
            ></el-option>
            <el-option
              label="Europe/London (UTC+0)"
              value="Europe/London"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item
          label="邮件服务器"
          prop="emailServer"
        >
          <el-input
            v-model="settingsForm.emailServer"
            placeholder="请输入邮件服务器地址"
          />
        </el-form-item>
        <el-form-item
          label="邮件端口"
          prop="emailPort"
        >
          <el-input
            v-model.number="settingsForm.emailPort"
            type="number"
            placeholder="请输入邮件端口"
          />
        </el-form-item>
        <el-form-item
          label="启用邮件通知"
          prop="enableEmailNotification"
        >
          <el-switch v-model="settingsForm.enableEmailNotification" />
        </el-form-item>
        <el-form-item>
          <el-button
            type="primary"
            @click="handleSaveSettings"
            :loading="loading"
            >保存设置</el-button
          >
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref } from 'vue'
  import { Plus } from '@element-plus/icons-vue'
  import type { FormInstance, FormRules } from 'element-plus'

  const settingsFormRef = ref<FormInstance>()
  const loading = ref(false)

  const settingsForm = reactive({
    emailPort: 587,
    emailServer: 'smtp.example.com',
    enableEmailNotification: true,
    language: 'zh-CN',
    siteDescription: 'Vue 3 + TypeScript + Vite 管理后台',
    siteLogo: '',
    siteName: 'Admin Vue',
    timezone: 'Asia/Shanghai',
  })

  const settingsRules = reactive<FormRules>({
    emailPort: [
      { message: '请输入邮件端口', required: true, trigger: 'blur' },
      { message: '请输入有效的端口号', trigger: 'blur', type: 'number' },
    ],
    emailServer: [{ message: '请输入邮件服务器地址', required: true, trigger: 'blur' }],
    siteDescription: [{ message: '请输入网站描述', required: true, trigger: 'blur' }],
    siteName: [{ message: '请输入网站名称', required: true, trigger: 'blur' }],
  })

  const handleLogoUpload = (file: any) => {
    // 模拟上传逻辑
    settingsForm.siteLogo = URL.createObjectURL(file.raw)
    return false
  }

  const beforeLogoUpload = (file: any) => {
    const isJPG = file.type === 'image/jpeg'
    const isPNG = file.type === 'image/png'
    const isLt2M = file.size / 1024 / 1024 < 2

    if (!isJPG && !isPNG) {
      ElMessage.error('只支持 JPG/PNG 格式的图片')
      return false
    }
    if (!isLt2M) {
      ElMessage.error('图片大小不能超过 2MB')
      return false
    }
    return true
  }

  const handleSaveSettings = async () => {
    if (!settingsFormRef.value) {
      return
    }

    try {
      await settingsFormRef.value.validate()
      loading.value = true

      // 模拟保存请求
      setTimeout(() => {
        loading.value = false
        ElMessage.success('设置保存成功')
      }, 1000)
    } catch (error) {
      console.error('设置验证失败:', error)
    }
  }

  const resetForm = () => {
    settingsFormRef.value?.resetFields()
  }
</script>

<style scoped lang="scss">
  .settings-container {
    padding: 1rem;
    background-color: var(--el-bg-color-page);
  }

  .settings-title {
    font-size: 1.5rem;
    margin-bottom: 1.5rem;
    color: var(--el-text-color-primary);
    font-weight: 600;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .avatar-uploader {
    border: 1px dashed var(--el-border-color);
    border-radius: var(--el-border-radius-base);
    cursor: pointer;
    position: relative;
    overflow: hidden;
    transition: all 0.3s;
    width: 120px;
    height: 120px;
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: var(--el-fill-color-light);

    &:hover {
      border-color: var(--el-color-primary);
    }
  }

  .avatar-uploader-icon {
    font-size: 28px;
    color: var(--el-text-color-secondary);
  }

  .avatar {
    width: 100%;
    height: 100%;
    display: block;
  }
</style>
