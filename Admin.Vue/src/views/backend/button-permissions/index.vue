<template>
  <div class="button-permissions-container">
    <h1 class="button-permissions-title">按钮权限管理</h1>
    <el-row :gutter="20">
      <el-col :span="24">
        <el-card shadow="hover" class="button-permissions-card">
          <template #header>
            <div class="card-header">
              <span>按钮权限列表</span>
              <el-button type="primary" @click="handleAddPermission">
                <el-icon>
                  <Plus />
                </el-icon>
                新增按钮权限
              </el-button>
            </div>
          </template>
          <el-table
            :data="buttonPermissions"
            v-loading="loading"
            row-key="id"
            :tree-props="{ children: 'children' }"
            style="width: 100%"
          >
            <el-table-column prop="name" label="按钮名称" width="200" />
            <el-table-column prop="code" label="权限编码" width="250" />
            <el-table-column prop="parentName" label="父权限" width="200">
              <template #default="{ row }">
                {{ row.parentName || "-" }}
              </template>
            </el-table-column>
            <el-table-column prop="sort" label="排序" width="100" />
            <el-table-column prop="createdAt" label="创建时间" width="180">
              <template #default="{ row }">
                {{ formatDate(row.createdAt) }}
              </template>
            </el-table-column>
            <el-table-column label="操作" width="240" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleEditPermission(row)"> 编辑 </el-button>
                <el-button type="success" size="small" @click="handleAddChildPermission(row)"> 添加子权限 </el-button>
                <el-button type="danger" size="small" @click="handleDeletePermission(row.id)"> 删除 </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="600px" @close="handleDialogClose">
      <el-form ref="permissionFormRef" :model="permissionForm" :rules="permissionRules" label-width="120px">
        <el-form-item label="按钮名称" prop="name">
          <el-input v-model="permissionForm.name" placeholder="请输入按钮名称（如：新增用户）" />
        </el-form-item>
        <el-form-item label="权限编码" prop="code">
          <el-input v-model="permissionForm.code" placeholder="请输入权限编码（如：user:add）" />
        </el-form-item>
        <el-form-item label="父级权限" prop="parentId">
          <el-tree-select
            v-model="permissionForm.parentId"
            :data="buttonTreeOptions"
            :props="treeProps"
            placeholder="请选择父级权限（可选）"
            clearable
            style="width: 100%"
            :disabled="buttonTreeOptions.length === 0"
          />
        </el-form-item>
        <el-form-item label="排序" prop="sort">
          <el-input-number v-model="permissionForm.sort" :min="0" :max="999" style="width: 100%" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit" :loading="submitting"> 确定 </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, reactive } from "vue";
import { Plus } from "@element-plus/icons-vue";
import { buttonPermissionsApi } from "@/api";
import type {
  ButtonPermissionListDTO,
  ButtonPermissionCreateDTO,
  ButtonPermissionUpdateDTO,
  ButtonPermissionTreeNodeDTO,
} from "@/api/nswags/auto";
import type { FormInstance, FormRules } from "element-plus";

const buttonPermissions = ref<ButtonPermissionTreeNodeDTO[]>([]);
const loading = ref(false);
const dialogVisible = ref(false);
const dialogTitle = ref("");
const submitting = ref(false);
const permissionFormRef = ref<FormInstance>();

const permissionForm = reactive<ButtonPermissionCreateDTO & { id?: string }>({
  name: "",
  code: "",
  parentId: undefined,
  sort: 0,
});

const permissionRules = reactive<FormRules>({
  name: [{ message: "请输入按钮名称", required: true, trigger: "blur" }],
  code: [{ message: "请输入权限编码", required: true, trigger: "blur" }],
});

const treeProps = {
  children: "children",
  label: "name",
  value: "id",
};

const buttonTreeOptions = computed(() => {
  const options = [...buttonPermissions.value];
  if (permissionForm.id) {
    return filterChildren(options, permissionForm.id);
  }
  return options;
});

const filterChildren = (nodes: any[], excludeId: string): any[] => {
  return nodes
    .filter((node) => node.id !== excludeId)
    .map((node) => ({
      ...node,
      children: filterChildren(node.children || [], excludeId),
    }));
};

const formatDate = (date: Date | string) => {
  if (!date) {
    return "-";
  }
  const d = new Date(date);
  return d.toLocaleString("zh-CN");
};

const loadButtonPermissions = async () => {
  loading.value = true;
  try {
    const res = await buttonPermissionsApi.getTree();
    if (res.success && res.data) {
      buttonPermissions.value = res.data;
    }
  } catch (error) {
    console.error("获取按钮权限失败:", error);
    ElMessage.error("获取按钮权限失败");
  } finally {
    loading.value = false;
  }
};

const handleAddPermission = () => {
  dialogTitle.value = "新增按钮权限";
  resetForm();
  dialogVisible.value = true;
};

const handleAddChildPermission = (parent: ButtonPermissionListDTO) => {
  dialogTitle.value = "新增子按钮权限";
  resetForm();
  permissionForm.parentId = parent.id;
  dialogVisible.value = true;
};

const handleEditPermission = (permission: ButtonPermissionListDTO) => {
  dialogTitle.value = "编辑按钮权限";
  permissionForm.id = permission.id;
  permissionForm.name = permission.name;
  permissionForm.code = permission.code;
  permissionForm.parentId = permission.parentId;
  permissionForm.sort = permission.sort;
  dialogVisible.value = true;
};

const handleDeletePermission = async (id: string) => {
  try {
    await ElMessageBox.confirm("确定要删除该按钮权限吗？", "提示", {
      confirmButtonText: "确定",
      cancelButtonText: "取消",
      type: "warning",
    });

    const res = await buttonPermissionsApi.delete(id);
    if (res.success) {
      ElMessage.success("删除成功");
      loadButtonPermissions();
    }
  } catch (error: any) {
    if (error !== "cancel") {
      console.error("删除按钮权限失败:", error);
      ElMessage.error("删除按钮权限失败");
    }
  }
};

const handleSubmit = async () => {
  if (!permissionFormRef.value) {
    return;
  }

  try {
    await permissionFormRef.value.validate();
    submitting.value = true;

    const submitData: ButtonPermissionCreateDTO = {
      name: permissionForm.name,
      code: permissionForm.code,
      parentId: permissionForm.parentId,
      sort: permissionForm.sort,
    };

    if (permissionForm.id) {
      const res = await buttonPermissionsApi.update(permissionForm.id, submitData as ButtonPermissionUpdateDTO);
      if (res.success) {
        ElMessage.success("更新成功");
        dialogVisible.value = false;
        loadButtonPermissions();
      }
    } else {
      const res = await buttonPermissionsApi.create(submitData);
      if (res.success) {
        ElMessage.success("创建成功");
        dialogVisible.value = false;
        loadButtonPermissions();
      }
    }
  } catch (error: any) {
    if (error !== "cancel") {
      console.error("保存按钮权限失败:", error);
    }
  } finally {
    submitting.value = false;
  }
};

const handleDialogClose = () => {
  permissionFormRef.value?.resetFields();
};

const resetForm = () => {
  permissionForm.id = undefined;
  permissionForm.name = "";
  permissionForm.code = "";
  permissionForm.parentId = undefined;
  permissionForm.sort = 0;
};

onMounted(() => {
  loadButtonPermissions();
});
</script>

<style scoped lang="scss">
.button-permissions-container {
  padding: 1rem;
  background-color: var(--el-bg-color-page);
}

.button-permissions-title {
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
