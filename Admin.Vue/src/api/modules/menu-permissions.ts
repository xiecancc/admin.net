import { ModuleApi } from "../nswags";
import { MenuPermissionsSwagApi } from "../nswags/auto";
import type {
  MenuPermissionCreateDTO,
  MenuPermissionDetailDTO,
  MenuPermissionListDTO,
  MenuPermissionPagedDTO,
  MenuPermissionUpdateDTO,
} from "../nswags/auto";

/**
 * 菜单权限管理 API
 * 提供菜单权限的增删改查、树形结构、批量操作等功能
 */
export class MenuPermissionsApi extends ModuleApi<
  MenuPermissionsSwagApi,
  MenuPermissionListDTO,
  MenuPermissionPagedDTO,
  MenuPermissionDetailDTO,
  MenuPermissionCreateDTO,
  MenuPermissionUpdateDTO
> {
  constructor() {
    super(MenuPermissionsSwagApi);
  }

  /** 获取菜单树形结构 */
  async getTree() {
    return this.api.getTree();
  }

  /** 获取所有子菜单 */
  async getAllChildren(permissionId: string) {
    return this.api.getAllChildren(permissionId);
  }
}
