<template>
  <div class="menu-permissions-container">
    <h1 class="menu-permissions-title">菜单权限管理</h1>
    <el-row :gutter="20">
      <el-col :span="24">
        <el-card shadow="hover" class="menu-permissions-card">
          <template #header>
            <div class="card-header">
              <span>菜单权限列表</span>
              <el-button type="primary" @click="handleAddPermission">
                <el-icon>
                  <Plus />
                </el-icon>
                新增菜单
              </el-button>
            </div>
          </template>
          <el-table
            :data="menuPermissions"
            v-loading="loading"
            row-key="id"
            :tree-props="{ children: 'children' }"
            style="width: 100%"
          >
            <el-table-column prop="name" label="菜单名称" width="200" />
            <el-table-column prop="code" label="权限编码" width="200" />
            <el-table-column prop="path" label="菜单路径" width="200" />
            <el-table-column prop="component" label="组件" width="200">
              <template #default="{ row }">
                {{ row.component || "-" }}
              </template>
            </el-table-column>
            <el-table-column prop="icon" label="图标" width="100">
              <template #default="{ row }">
                <el-icon v-if="row.icon">
                  <component :is="row.icon" />
                </el-icon>
              </template>
            </el-table-column>
            <el-table-column prop="sort" label="排序" width="80" />
            <el-table-column prop="isVisible" label="是否可见" width="100">
              <template #default="{ row }">
                <el-tag :type="row.isVisible ? 'success' : 'info'">
                  {{ row.isVisible ? "可见" : "隐藏" }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="keepAlive" label="缓存" width="80">
              <template #default="{ row }">
                <el-tag :type="row.keepAlive ? 'success' : 'info'" size="small">
                  {{ row.keepAlive ? "是" : "否" }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="320" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleEditPermission(row)"> 编辑 </el-button>
                <el-button type="success" size="small" @click="handleAddChildPermission(row)"> 添加子菜单 </el-button>
                <el-button type="danger" size="small" @click="handleDeletePermission(row.id)"> 删除 </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="700px" @close="handleDialogClose">
      <el-form ref="permissionFormRef" :model="permissionForm" :rules="permissionRules" label-width="120px">
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="permissionForm.name" placeholder="请输入菜单名称" />
        </el-form-item>
        <el-form-item label="权限编码" prop="code">
          <el-input v-model="permissionForm.code" placeholder="请输入权限编码（如：user:list）" />
        </el-form-item>
        <el-form-item label="父级菜单" prop="parentId">
          <el-tree-select
            v-model="permissionForm.parentId"
            :data="menuTreeOptions"
            :props="treeProps"
            placeholder="请选择父级菜单（可选）"
            clearable
            style="width: 100%"
            :disabled="menuTreeOptions.length === 0"
          />
        </el-form-item>
        <el-form-item label="菜单路径" prop="path">
          <el-input v-model="permissionForm.path" placeholder="请输入菜单路径（如：/users）" />
        </el-form-item>
        <el-form-item label="组件" prop="component">
          <el-input v-model="permissionForm.component" placeholder="请输入组件路径（如：backend/users/index）" />
        </el-form-item>
        <el-form-item label="图标" prop="icon">
          <el-input v-model="permissionForm.icon" placeholder="请输入图标名称（如：User）" />
        </el-form-item>
        <el-form-item label="排序" prop="sort">
          <el-input-number v-model="permissionForm.sort" :min="0" :max="999" style="width: 100%" />
        </el-form-item>
        <el-form-item label="是否可见" prop="isVisible">
          <el-switch v-model="permissionForm.isVisible" />
        </el-form-item>
        <el-form-item label="是否缓存" prop="keepAlive">
          <el-switch v-model="permissionForm.keepAlive" />
        </el-form-item>
        <el-form-item label="是否外部链接" prop="isExternal">
          <el-switch v-model="permissionForm.isExternal" />
        </el-form-item>
        <el-form-item label="重定向地址" prop="redirect">
          <el-input v-model="permissionForm.redirect" placeholder="请输入重定向地址（可选）" />
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
import { menuPermissionsApi } from "@/api";
import type {
  MenuPermissionListDTO,
  MenuPermissionCreateDTO,
  MenuPermissionUpdateDTO,
  MenuPermissionTreeNodeDTO,
} from "@/api/nswags/auto";
import type { FormInstance, FormRules } from "element-plus";

const menuPermissions = ref<MenuPermissionTreeNodeDTO[]>([]);
const loading = ref(false);
const dialogVisible = ref(false);
const dialogTitle = ref("");
const submitting = ref(false);
const permissionFormRef = ref<FormInstance>();

const permissionForm = reactive<MenuPermissionCreateDTO & { id?: string }>({
  name: "",
  code: "",
  parentId: undefined,
  path: "",
  component: "",
  icon: "",
  sort: 0,
  isVisible: true,
  keepAlive: false,
  isExternal: false,
  redirect: "",
});

const permissionRules = reactive<FormRules>({
  name: [{ message: "请输入菜单名称", required: true, trigger: "blur" }],
  code: [{ message: "请输入权限编码", required: true, trigger: "blur" }],
  path: [{ message: "请输入菜单路径", required: true, trigger: "blur" }],
});

const treeProps = {
  children: "children",
  label: "name",
  value: "id",
};

const menuTreeOptions = computed(() => {
  const options = [...menuPermissions.value];
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

const loadMenuPermissions = async () => {
  loading.value = true;
  try {
    const res = await menuPermissionsApi.getTree();
    if (res.success && res.data) {
      menuPermissions.value = res.data;
    }
  } catch (error) {
    console.error("获取菜单权限失败:", error);
    ElMessage.error("获取菜单权限失败");
  } finally {
    loading.value = false;
  }
};

const handleAddPermission = () => {
  dialogTitle.value = "新增菜单";
  resetForm();
  dialogVisible.value = true;
};

const handleAddChildPermission = (parent: MenuPermissionListDTO) => {
  dialogTitle.value = "新增子菜单";
  resetForm();
  permissionForm.parentId = parent.id;
  dialogVisible.value = true;
};

const handleEditPermission = (permission: MenuPermissionListDTO) => {
  dialogTitle.value = "编辑菜单";
  permissionForm.id = permission.id;
  permissionForm.name = permission.name;
  permissionForm.code = permission.code;
  permissionForm.parentId = permission.parentId;
  permissionForm.path = permission.path || "";
  permissionForm.component = (permission as any).component || "";
  permissionForm.icon = (permission as any).icon || "";
  permissionForm.sort = permission.sort;
  permissionForm.isVisible = (permission as any).isVisible ?? true;
  permissionForm.keepAlive = (permission as any).keepAlive ?? false;
  permissionForm.isExternal = (permission as any).isExternal ?? false;
  permissionForm.redirect = (permission as any).redirect || "";
  dialogVisible.value = true;
};

const handleDeletePermission = async (id: string) => {
  try {
    await ElMessageBox.confirm("确定要删除该菜单权限吗？", "提示", {
      confirmButtonText: "确定",
      cancelButtonText: "取消",
      type: "warning",
    });

    const res = await menuPermissionsApi.delete(id);
    if (res.success) {
      ElMessage.success("删除成功");
      loadMenuPermissions();
    }
  } catch (error: any) {
    if (error !== "cancel") {
      console.error("删除菜单权限失败:", error);
      ElMessage.error("删除菜单权限失败");
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

    const submitData: MenuPermissionCreateDTO = {
      name: permissionForm.name,
      code: permissionForm.code,
      parentId: permissionForm.parentId,
      path: permissionForm.path,
      component: permissionForm.component,
      icon: permissionForm.icon,
      sort: permissionForm.sort,
      isVisible: permissionForm.isVisible,
      keepAlive: permissionForm.keepAlive,
      isExternal: permissionForm.isExternal,
      redirect: permissionForm.redirect,
    };

    if (permissionForm.id) {
      const res = await menuPermissionsApi.update(permissionForm.id, submitData as MenuPermissionUpdateDTO);
      if (res.success) {
        ElMessage.success("更新成功");
        dialogVisible.value = false;
        loadMenuPermissions();
      }
    } else {
      const res = await menuPermissionsApi.create(submitData);
      if (res.success) {
        ElMessage.success("创建成功");
        dialogVisible.value = false;
        loadMenuPermissions();
      }
    }
  } catch (error: any) {
    if (error !== "cancel") {
      console.error("保存菜单权限失败:", error);
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
  permissionForm.path = "";
  permissionForm.component = "";
  permissionForm.icon = "";
  permissionForm.sort = 0;
  permissionForm.isVisible = true;
  permissionForm.keepAlive = false;
  permissionForm.isExternal = false;
  permissionForm.redirect = "";
};

onMounted(() => {
  loadMenuPermissions();
});
</script>

<style scoped lang="scss">
.menu-permissions-container {
  padding: 1rem;
  background-color: var(--el-bg-color-page);
}

.menu-permissions-title {
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
