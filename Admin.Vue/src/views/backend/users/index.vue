<template>
  <div class="users-container">
    <h1 class="users-title">用户管理</h1>
    <el-card
      shadow="hover"
      class="users-card"
    >
      <template #header>
        <div class="card-header">
          <span>用户列表</span>
          <el-button
            type="primary"
            @click="handleAddUser"
          >
            <el-icon>
              <Plus />
            </el-icon>
            新增用户
          </el-button>
        </div>
      </template>
      <div class="users-filter">
        <el-input
          v-model="searchQuery"
          placeholder="搜索用户"
          :prefix-icon="Search"
          style="width: 300px; margin-right: 10px"
        />
        <el-select
          v-model="statusFilter"
          placeholder="用户状态"
          style="width: 150px"
        >
          <el-option
            label="全部"
            value=""
          />
          <el-option
            label="活跃"
            :value="0"
          />
          <el-option
            label="禁用"
            :value="1"
          />
        </el-select>
      </div>
      <el-table
        :data="filteredUsers"
        v-loading="loading"
        style="width: 100%"
      >
        <el-table-column
          prop="email"
          label="邮箱"
          width="200"
        />
        <el-table-column
          prop="nickName"
          label="昵称"
        >
          <template #default="{ row }">
            {{ row.nickName || '-' }}
          </template>
        </el-table-column>
        <el-table-column
          prop="phone"
          label="手机号"
        >
          <template #default="{ row }">
            {{ row.phone || '-' }}
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          label="状态"
          width="100"
        >
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'success' : 'danger'">
              {{ row.status === 0 ? '活跃' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          label="创建时间"
          width="180"
        >
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column
          label="操作"
          width="200"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              type="primary"
              size="small"
              @click="handleEditUser(row)"
            >
              编辑
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleDeleteUser(row.id)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="total"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form
        ref="userFormRef"
        :model="userForm"
        :rules="userRules"
        label-width="100px"
      >
        <el-form-item
          label="邮箱"
          prop="email"
        >
          <el-input
            v-model="userForm.email"
            placeholder="请输入邮箱"
          />
        </el-form-item>
        <el-form-item
          label="昵称"
          prop="nickName"
        >
          <el-input
            v-model="userForm.nickName"
            placeholder="请输入昵称"
          />
        </el-form-item>
        <el-form-item
          label="手机号"
          prop="phone"
        >
          <el-input
            v-model="userForm.phone"
            placeholder="请输入手机号"
          />
        </el-form-item>
        <el-form-item
          label="头像 URL"
          prop="avatarUrl"
        >
          <el-input
            v-model="userForm.avatarUrl"
            placeholder="请输入头像 URL"
          />
        </el-form-item>
        <el-form-item
          label="用户状态"
          prop="status"
        >
          <el-select
            v-model="userForm.status"
            placeholder="请选择用户状态"
            style="width: 100%"
          >
            <el-option
              label="活跃"
              :value="0"
            />
            <el-option
              label="禁用"
              :value="1"
            />
          </el-select>
        </el-form-item>
        <el-form-item
          v-if="!userForm.id"
          label="密码"
          prop="password"
        >
          <el-input
            v-model="userForm.password"
            type="password"
            placeholder="请输入密码"
            show-password
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button
          type="primary"
          @click="handleSubmit"
          :loading="submitting"
        >
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
  import { computed, onMounted, reactive, ref } from 'vue'
  import { Plus, Search } from '@element-plus/icons-vue'
  import { usersApi } from '@/api'
  import type { UserCreateDTO, UserListDTO, UserUpdateDTO } from '@/api/nswags/auto'
  import type { FormInstance, FormRules } from 'element-plus'

  const users = ref<UserListDTO[]>([])
  const loading = ref(false)
  const searchQuery = ref('')
  const statusFilter = ref<number | ''>('')
  const currentPage = ref<number>(1)
  const pageSize = ref<number>(10)
  const total = ref<number>(0)
  const dialogVisible = ref(false)
  const dialogTitle = ref('')
  const submitting = ref(false)
  const userFormRef = ref<FormInstance>()

  const userForm = reactive<UserCreateDTO & { id?: string }>({
    avatarUrl: '',
    email: '',
    nickName: '',
    password: '',
    phone: '',
    status: 0,
  })

  const userRules = reactive<FormRules>({
    email: [
      { message: '请输入邮箱', required: true, trigger: 'blur' },
      { type: 'email', message: '请输入正确的邮箱格式', trigger: 'blur' },
    ],
    nickName: [{ message: '请输入昵称', required: true, trigger: 'blur' }],
    password: [{ message: '请输入密码', required: true, trigger: 'blur', min: 6 }],
    status: [{ message: '请选择用户状态', required: true, trigger: 'change' }],
  })

  const filteredUsers = computed(() => {
    let result = users.value

    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase()
      result = result.filter(
        user => user.nickName?.toLowerCase().includes(query) || user.email.toLowerCase().includes(query)
      )
    }

    if (statusFilter.value !== '') {
      result = result.filter(user => user.status === statusFilter.value)
    }

    return result
  })

  const formatDate = (date: Date | string) => {
    if (!date) {
      return '-'
    }
    const d = new Date(date)
    return d.toLocaleString('zh-CN')
  }

  const fetchUsers = async () => {
    loading.value = true
    try {
      const res = await usersApi.getList()
      if (res.success && res.data) {
        users.value = res.data
        total.value = res.data.length
      }
    } catch (error) {
      console.error('获取用户列表失败:', error)
    } finally {
      loading.value = false
    }
  }

  const handleAddUser = () => {
    dialogTitle.value = '新增用户'
    userForm.id = undefined
    userForm.email = ''
    userForm.nickName = ''
    userForm.phone = ''
    userForm.avatarUrl = ''
    userForm.password = ''
    userForm.status = 0
    dialogVisible.value = true
  }

  const handleEditUser = (user: UserListDTO) => {
    dialogTitle.value = '编辑用户'
    userForm.id = user.id
    userForm.email = user.email
    userForm.nickName = user.nickName || ''
    userForm.phone = user.phone || ''
    userForm.avatarUrl = ''
    userForm.password = ''
    userForm.status = user.status
    dialogVisible.value = true
  }

  const handleDeleteUser = async (id: string) => {
    try {
      await ElMessageBox.confirm('确定要删除该用户吗？', '提示', {
        cancelButtonText: '取消',
        confirmButtonText: '确定',
        type: 'warning',
      })

      const res = await usersApi.delete(id)
      if (res.success) {
        ElMessage.success('删除成功')
        fetchUsers()
      }
    } catch (error: any) {
      if (error !== 'cancel') {
        console.error('删除用户失败:', error)
        ElMessage.error('删除用户失败')
      }
    }
  }

  const handleSubmit = async () => {
    if (!userFormRef.value) {
      return
    }

    try {
      await userFormRef.value.validate()
      submitting.value = true

      if (userForm.id) {
        const updateData: UserUpdateDTO = {
          avatarUrl: userForm.avatarUrl,
          email: userForm.email,
          nickName: userForm.nickName,
          phone: userForm.phone,
          status: userForm.status,
        }
        const res = await usersApi.update(userForm.id, updateData)
        if (res.success) {
          ElMessage.success('更新成功')
          dialogVisible.value = false
          fetchUsers()
        }
      } else {
        const res = await usersApi.create(userForm)
        if (res.success) {
          ElMessage.success('创建成功')
          dialogVisible.value = false
          fetchUsers()
        }
      }
    } catch (error: any) {
      if (error !== 'cancel') {
        console.error('保存用户失败:', error)
      }
    } finally {
      submitting.value = false
    }
  }

  const handleDialogClose = () => {
    userFormRef.value?.resetFields()
  }

  const handleSizeChange = (size: number) => {
    pageSize.value = size
    currentPage.value = 1
  }

  const handleCurrentChange = (current: number) => {
    currentPage.value = current
  }

  onMounted(() => {
    fetchUsers()
  })
</script>

<style scoped lang="scss">
  .users-container {
    padding: 1rem;
    background-color: var(--el-bg-color-page);
  }

  .users-title {
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

  .users-filter {
    margin-bottom: 1rem;
    display: flex;
  }

  .pagination-container {
    margin-top: 1rem;
    display: flex;
    justify-content: flex-end;
  }
</style>
