<template>
  <div class="roles-container">
    <h1 class="roles-title">角色管理</h1>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-card
          shadow="hover"
          class="roles-card"
        >
          <template #header>
            <div class="card-header">
              <span>角色列表</span>
              <el-button
                type="primary"
                @click="handleAddRole"
              >
                <el-icon>
                  <Plus />
                </el-icon>
                新增角色
              </el-button>
            </div>
          </template>
          <div class="roles-filter">
            <el-input
              v-model="searchQuery"
              placeholder="搜索角色"
              :prefix-icon="Search"
              style="width: 300px; margin-right: 10px"
            />
          </div>
          <el-table
            :data="filteredRoles"
            v-loading="loading"
            style="width: 100%"
          >
            <el-table-column
              prop="code"
              label="角色编码"
              width="150"
            />
            <el-table-column
              prop="name"
              label="角色名称"
            >
              <template #default="{ row }">
                {{ row.name }}
                <el-tag
                  v-if="isBuiltInRole(row.code)"
                  size="small"
                  type="warning"
                  style="margin-left: 8px"
                  >内置</el-tag
                >
              </template>
            </el-table-column>
            <el-table-column
              prop="parentName"
              label="父角色"
              width="150"
            >
              <template #default="{ row }">
                {{ row.parentName || '-' }}
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
              width="280"
            >
              <template #default="{ row }">
                <el-button
                  type="primary"
                  size="small"
                  @click="handleEditRole(row)"
                >
                  编辑
                </el-button>
                <el-button
                  type="success"
                  size="small"
                  @click="handleAssignPermissions(row)"
                >
                  分配权限
                </el-button>
                <el-button
                  type="danger"
                  size="small"
                  @click="handleDeleteRole(row.id)"
                  :disabled="isBuiltInRole(row.code)"
                >
                  删除
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card
          shadow="hover"
          class="roles-tree-card"
        >
          <template #header>
            <div class="card-header">
              <span>角色树形结构</span>
              <el-button
                type="primary"
                size="small"
                @click="loadRoleTree"
              >
                <el-icon>
                  <Refresh />
                </el-icon>
                刷新
              </el-button>
            </div>
          </template>
          <el-tree
            :data="roleTree"
            :props="treeProps"
            node-key="id"
            default-expand-all
            highlight-current
          >
            <template #default="{ node, data }">
              <span class="custom-tree-node">
                <el-icon>
                  <User />
                </el-icon>
                <span>{{ node.label }}</span>
                <span class="tree-node-info">({{ data.permissionCount }} 权限)</span>
              </span>
            </template>
          </el-tree>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form
        ref="roleFormRef"
        :model="roleForm"
        :rules="roleRules"
        label-width="100px"
      >
        <el-form-item
          label="角色编码"
          prop="code"
        >
          <el-input
            v-model="roleForm.code"
            placeholder="请输入角色编码（如：admin）"
          />
        </el-form-item>
        <el-form-item
          label="角色名称"
          prop="name"
        >
          <el-input
            v-model="roleForm.name"
            placeholder="请输入角色名称（如：管理员）"
          />
        </el-form-item>
        <el-form-item
          label="父角色"
          prop="parentId"
        >
          <el-select
            v-model="roleForm.parentId"
            placeholder="请选择父角色（可选）"
            clearable
            style="width: 100%"
          >
            <el-option
              v-for="role in roleOptions"
              :key="role.id"
              :label="role.name"
              :value="role.id"
              :disabled="role.id === roleForm.id"
            />
          </el-select>
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

    <el-dialog
      v-model="permissionDialogVisible"
      title="分配权限"
      width="700px"
    >
      <el-tree
        ref="permissionTreeRef"
        :data="permissionTree"
        :props="treeProps"
        show-checkbox
        node-key="id"
        default-expand-all
        :check-strictly="true"
      />
      <template #footer>
        <el-button @click="permissionDialogVisible = false">取消</el-button>
        <el-button
          type="primary"
          @click="handleSavePermissions"
          :loading="permissionSubmitting"
        >
          保存
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
  import { computed, onMounted, reactive, ref } from 'vue'
  import { Plus, Refresh, Search, User } from '@element-plus/icons-vue'
  import { apiPermissionsApi, buttonPermissionsApi, menuPermissionsApi, rolesApi } from '@/api'
  import { ROLES, isBuiltInRole } from '@/constants/roles.constant'
  import type { RoleCreateDTO, RoleListDTO, RoleTreeNodeDTO, RoleUpdateDTO } from '@/api/nswags/auto'
  import type { FormInstance, FormRules } from 'element-plus'

  const roles = ref<RoleListDTO[]>([])
  const loading = ref(false)
  const searchQuery = ref('')
  const roleTree = ref<RoleTreeNodeDTO[]>([])
  const dialogVisible = ref(false)
  const dialogTitle = ref('')
  const submitting = ref(false)
  const roleFormRef = ref<FormInstance>()
  const permissionDialogVisible = ref(false)
  const permissionSubmitting = ref(false)
  const permissionTreeRef = ref()
  const currentRoleId = ref<string>('')
  const permissionTree = ref<any[]>([])

  const roleForm = reactive<RoleCreateDTO & { id?: string }>({
    code: '',
    name: '',
    parentId: undefined,
  })

  const roleRules = reactive<FormRules>({
    code: [{ message: '请输入角色编码', required: true, trigger: 'blur' }],
    name: [{ message: '请输入角色名称', required: true, trigger: 'blur' }],
  })

  const treeProps = {
    children: 'children',
    label: 'name',
  }

  const filteredRoles = computed(() => {
    if (!searchQuery.value) {
      return roles.value
    }
    const query = searchQuery.value.toLowerCase()
    return roles.value.filter(
      role => role.code.toLowerCase().includes(query) || role.name.toLowerCase().includes(query)
    )
  })

  const roleOptions = computed(() => roles.value.filter(role => role.id !== roleForm.id))

  const formatDate = (date: Date | string) => {
    if (!date) {
      return '-'
    }
    const d = new Date(date)
    return d.toLocaleString('zh-CN')
  }

  const loadRoles = async () => {
    loading.value = true
    try {
      const res = await rolesApi.getList()
      if (res.success && res.data) {
        roles.value = res.data
      }
    } catch (error) {
      console.error('获取角色列表失败:', error)
      ElMessage.error('获取角色列表失败')
    } finally {
      loading.value = false
    }
  }

  const loadRoleTree = async () => {
    try {
      const res = await rolesApi.getTree()
      if (res.success && res.data) {
        roleTree.value = res.data
      }
    } catch (error) {
      console.error('获取角色树失败:', error)
    }
  }

  const loadPermissionTree = async () => {
    try {
      const [menuRes, buttonRes, apiRes] = await Promise.all([
        menuPermissionsApi.getTree(),
        buttonPermissionsApi.getTree(),
        apiPermissionsApi.getTree(),
      ])

      permissionTree.value = [
        {
          children: menuRes.data || [],
          code: 'menu-permissions',
          id: 'menu-permissions',
          name: '菜单权限',
        },
        {
          children: buttonRes.data || [],
          code: 'button-permissions',
          id: 'button-permissions',
          name: '按钮权限',
        },
        {
          children: apiRes.data || [],
          code: 'api-permissions',
          id: 'api-permissions',
          name: 'API 权限',
        },
      ]
    } catch (error) {
      console.error('获取权限树失败:', error)
    }
  }

  const handleAddRole = () => {
    dialogTitle.value = '新增角色'
    roleForm.code = ''
    roleForm.name = ''
    roleForm.parentId = undefined
    roleForm.id = undefined
    dialogVisible.value = true
  }

  const handleEditRole = (role: RoleListDTO) => {
    dialogTitle.value = '编辑角色'
    roleForm.code = role.code
    roleForm.name = role.name
    roleForm.parentId = role.parentId
    roleForm.id = role.id
    dialogVisible.value = true
  }

  const handleDeleteRole = async (id: string) => {
    try {
      await ElMessageBox.confirm('确定要删除该角色吗？', '提示', {
        cancelButtonText: '取消',
        confirmButtonText: '确定',
        type: 'warning',
      })

      const res = await rolesApi.delete(id)
      if (res.success) {
        ElMessage.success('删除成功')
        loadRoles()
        loadRoleTree()
      }
    } catch (error: any) {
      if (error !== 'cancel') {
        console.error('删除角色失败:', error)
        ElMessage.error('删除角色失败')
      }
    }
  }

  const handleSubmit = async () => {
    if (!roleFormRef.value) {
      return
    }

    try {
      await roleFormRef.value.validate()
      submitting.value = true

      if (roleForm.id) {
        const updateData: RoleUpdateDTO = {
          code: roleForm.code,
          name: roleForm.name,
          parentId: roleForm.parentId,
        }
        const res = await rolesApi.update(roleForm.id, updateData)
        if (res.success) {
          ElMessage.success('更新成功')
          dialogVisible.value = false
          loadRoles()
          loadRoleTree()
        }
      } else {
        const res = await rolesApi.create(roleForm)
        if (res.success) {
          ElMessage.success('创建成功')
          dialogVisible.value = false
          loadRoles()
          loadRoleTree()
        }
      }
    } catch (error: any) {
      if (error !== 'cancel') {
        console.error('保存角色失败:', error)
      }
    } finally {
      submitting.value = false
    }
  }

  const handleDialogClose = () => {
    roleFormRef.value?.resetFields()
  }

  const handleAssignPermissions = async (role: RoleListDTO) => {
    currentRoleId.value = role.id
    permissionDialogVisible.value = true
    await loadPermissionTree()

    setTimeout(() => {
      if (permissionTreeRef.value) {
        permissionTreeRef.value.setCheckedKeys([])
      }
    }, 100)
  }

  const handleSavePermissions = async () => {
    if (!currentRoleId.value) {
      return
    }

    try {
      permissionSubmitting.value = true
      const checkedKeys = permissionTreeRef.value?.getCheckedKeys() || []
      const allCheckedKeys = [...checkedKeys, ...(permissionTreeRef.value?.getHalfCheckedKeys() || [])]

      const permissionIds = allCheckedKeys.filter(
        (key: string) => !['menu-permissions', 'button-permissions', 'api-permissions'].includes(key)
      )

      const res = await rolesApi.assignPermissions(currentRoleId.value, permissionIds)
      if (res.success) {
        ElMessage.success('权限分配成功')
        permissionDialogVisible.value = false
        loadRoles()
        loadRoleTree()
      }
    } catch (error) {
      console.error('分配权限失败:', error)
      ElMessage.error('分配权限失败')
    } finally {
      permissionSubmitting.value = false
    }
  }

  onMounted(() => {
    loadRoles()
    loadRoleTree()
  })
</script>

<style scoped lang="scss">
  .roles-container {
    padding: 1rem;
    background-color: var(--el-bg-color-page);
  }

  .roles-title {
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

  .roles-filter {
    margin-bottom: 1rem;
  }

  .custom-tree-node {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 6px;

    .tree-node-info {
      font-size: 12px;
      color: var(--el-text-color-secondary);
      margin-left: 8px;
    }
  }

  .roles-tree-card {
    :deep(.el-tree) {
      max-height: 600px;
      overflow-y: auto;
    }
  }
</style>
