<template>
  <div class="api-permissions-container">
    <h1 class="api-permissions-title">API 权限管理</h1>
    <el-row :gutter="20">
      <el-col :span="24">
        <el-card
          shadow="hover"
          class="api-permissions-card"
        >
          <template #header>
            <div class="card-header">
              <span>API 权限列表</span>
              <el-button
                type="primary"
                @click="handleAddPermission"
              >
                <el-icon>
                  <Plus />
                </el-icon>
                新增 API 权限
              </el-button>
            </div>
          </template>
          <el-table
            :data="apiPermissions"
            v-loading="loading"
            row-key="id"
            :tree-props="{ children: 'children' }"
            style="width: 100%"
          >
            <el-table-column
              prop="name"
              label="API 名称"
              width="250"
            />
            <el-table-column
              prop="code"
              label="权限编码"
              width="250"
            />
            <el-table-column
              prop="path"
              label="API 路径"
              width="200"
            >
              <template #default="{ row }">
                <el-tag
                  size="small"
                  type="info"
                  >{{ (row as any).method || 'GET' }}</el-tag
                >
                <span style="margin-left: 8px">{{ (row as any).path || '-' }}</span>
              </template>
            </el-table-column>
            <el-table-column
              prop="parentName"
              label="父权限"
              width="200"
            >
              <template #default="{ row }">
                {{ row.parentName || '-' }}
              </template>
            </el-table-column>
            <el-table-column
              prop="sort"
              label="排序"
              width="100"
            />
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
              width="240"
              fixed="right"
            >
              <template #default="{ row }">
                <el-button
                  type="primary"
                  size="small"
                  @click="handleEditPermission(row)"
                >
                  编辑
                </el-button>
                <el-button
                  type="success"
                  size="small"
                  @click="handleAddChildPermission(row)"
                >
                  添加子权限
                </el-button>
                <el-button
                  type="danger"
                  size="small"
                  @click="handleDeletePermission(row.id)"
                >
                  删除
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="700px"
      @close="handleDialogClose"
    >
      <el-form
        ref="permissionFormRef"
        :model="permissionForm"
        :rules="permissionRules"
        label-width="120px"
      >
        <el-form-item
          label="API 名称"
          prop="name"
        >
          <el-input
            v-model="permissionForm.name"
            placeholder="请输入 API 名称（如：获取用户列表）"
          />
        </el-form-item>
        <el-form-item
          label="权限编码"
          prop="code"
        >
          <el-input
            v-model="permissionForm.code"
            placeholder="请输入权限编码（如：user:list）"
          />
        </el-form-item>
        <el-form-item
          label="父级权限"
          prop="parentId"
        >
          <el-tree-select
            v-model="permissionForm.parentId"
            :data="apiTreeOptions"
            :props="treeProps"
            placeholder="请选择父级权限（可选）"
            clearable
            style="width: 100%"
            :disabled="apiTreeOptions.length === 0"
          />
        </el-form-item>
        <el-form-item
          label="HTTP 方法"
          prop="method"
        >
          <el-select
            v-model="permissionForm.method"
            placeholder="请选择 HTTP 方法"
            style="width: 100%"
          >
            <el-option
              label="GET"
              value="GET"
            />
            <el-option
              label="POST"
              value="POST"
            />
            <el-option
              label="PUT"
              value="PUT"
            />
            <el-option
              label="DELETE"
              value="DELETE"
            />
            <el-option
              label="PATCH"
              value="PATCH"
            />
          </el-select>
        </el-form-item>
        <el-form-item
          label="API 路径"
          prop="path"
        >
          <el-input
            v-model="permissionForm.path"
            placeholder="请输入 API 路径（如：/api/users）"
          />
        </el-form-item>
        <el-form-item
          label="排序"
          prop="sort"
        >
          <el-input-number
            v-model="permissionForm.sort"
            :min="0"
            :max="999"
            style="width: 100%"
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
  import { Plus } from '@element-plus/icons-vue'
  import { apiPermissionsApi } from '@/api'
  import type {
    ApiPermissionCreateDTO,
    ApiPermissionListDTO,
    ApiPermissionTreeNodeDTO,
    ApiPermissionUpdateDTO,
  } from '@/api/nswags/auto'
  import type { FormInstance, FormRules } from 'element-plus'

  const apiPermissions = ref<ApiPermissionTreeNodeDTO[]>([])
  const loading = ref(false)
  const dialogVisible = ref(false)
  const dialogTitle = ref('')
  const submitting = ref(false)
  const permissionFormRef = ref<FormInstance>()

  const permissionForm = reactive<ApiPermissionCreateDTO & { id?: string; method?: string; path?: string }>({
    code: '',
    method: 'GET',
    name: '',
    parentId: undefined,
    path: '',
    sort: 0,
  })

  const permissionRules = reactive<FormRules>({
    code: [{ message: '请输入权限编码', required: true, trigger: 'blur' }],
    method: [{ message: '请选择 HTTP 方法', required: true, trigger: 'change' }],
    name: [{ message: '请输入 API 名称', required: true, trigger: 'blur' }],
    path: [{ message: '请输入 API 路径', required: true, trigger: 'blur' }],
  })

  const treeProps = {
    children: 'children',
    label: 'name',
    value: 'id',
  }

  const apiTreeOptions = computed(() => {
    const options = [...apiPermissions.value]
    if (permissionForm.id) {
      return filterChildren(options, permissionForm.id)
    }
    return options
  })

  const filterChildren = (nodes: any[], excludeId: string): any[] =>
    nodes
      .filter(node => node.id !== excludeId)
      .map(node => ({
        ...node,
        children: filterChildren(node.children || [], excludeId),
      }))

  const formatDate = (date: Date | string) => {
    if (!date) {
      return '-'
    }
    const d = new Date(date)
    return d.toLocaleString('zh-CN')
  }

  const loadApiPermissions = async () => {
    loading.value = true
    try {
      const res = await apiPermissionsApi.getTree()
      if (res.success && res.data) {
        apiPermissions.value = res.data
      }
    } catch (error) {
      console.error('获取 API 权限失败:', error)
      ElMessage.error('获取 API 权限失败')
    } finally {
      loading.value = false
    }
  }

  const handleAddPermission = () => {
    dialogTitle.value = '新增 API 权限'
    resetForm()
    dialogVisible.value = true
  }

  const handleAddChildPermission = (parent: ApiPermissionListDTO) => {
    dialogTitle.value = '新增子 API 权限'
    resetForm()
    permissionForm.parentId = parent.id
    dialogVisible.value = true
  }

  const handleEditPermission = (permission: ApiPermissionListDTO) => {
    dialogTitle.value = '编辑 API 权限'
    permissionForm.id = permission.id
    permissionForm.name = permission.name
    permissionForm.code = permission.code
    permissionForm.parentId = permission.parentId
    permissionForm.sort = permission.sort
    permissionForm.method = (permission as any).method || 'GET'
    permissionForm.path = (permission as any).path || ''
    dialogVisible.value = true
  }

  const handleDeletePermission = async (id: string) => {
    try {
      await ElMessageBox.confirm('确定要删除该 API 权限吗？', '提示', {
        cancelButtonText: '取消',
        confirmButtonText: '确定',
        type: 'warning',
      })

      const res = await apiPermissionsApi.delete(id)
      if (res.success) {
        ElMessage.success('删除成功')
        loadApiPermissions()
      }
    } catch (error: any) {
      if (error !== 'cancel') {
        console.error('删除 API 权限失败:', error)
        ElMessage.error('删除 API 权限失败')
      }
    }
  }

  const handleSubmit = async () => {
    if (!permissionFormRef.value) {
      return
    }

    try {
      await permissionFormRef.value.validate()
      submitting.value = true

      const submitData: ApiPermissionCreateDTO & { method?: string; path?: string } = {
        code: permissionForm.code,
        method: permissionForm.method,
        name: permissionForm.name,
        parentId: permissionForm.parentId,
        path: permissionForm.path,
        sort: permissionForm.sort,
      }

      if (permissionForm.id) {
        const res = await apiPermissionsApi.update(permissionForm.id, submitData as ApiPermissionUpdateDTO)
        if (res.success) {
          ElMessage.success('更新成功')
          dialogVisible.value = false
          loadApiPermissions()
        }
      } else {
        const res = await apiPermissionsApi.create(submitData)
        if (res.success) {
          ElMessage.success('创建成功')
          dialogVisible.value = false
          loadApiPermissions()
        }
      }
    } catch (error: any) {
      if (error !== 'cancel') {
        console.error('保存 API 权限失败:', error)
      }
    } finally {
      submitting.value = false
    }
  }

  const handleDialogClose = () => {
    permissionFormRef.value?.resetFields()
  }

  const resetForm = () => {
    permissionForm.id = undefined
    permissionForm.name = ''
    permissionForm.code = ''
    permissionForm.parentId = undefined
    permissionForm.sort = 0
    permissionForm.method = 'GET'
    permissionForm.path = ''
  }

  onMounted(() => {
    loadApiPermissions()
  })
</script>

<style scoped lang="scss">
  .api-permissions-container {
    padding: 1rem;
    background-color: var(--el-bg-color-page);
  }

  .api-permissions-title {
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
</style>
