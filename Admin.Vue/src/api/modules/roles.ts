import { ModuleApi } from "../nswags";
import { RolesSwagApi } from "../nswags/auto";
import type { RoleCreateDTO, RoleDetailDTO, RoleListDTO, RolePagedDTO, RoleUpdateDTO } from "../nswags/auto";

/**
 * 角色管理 API
 * 提供角色的增删改查、权限分配、树形结构、继承关系等功能
 */
export class RolesApi extends ModuleApi<
  RolesSwagApi,
  RoleListDTO,
  RolePagedDTO,
  RoleDetailDTO,
  RoleCreateDTO,
  RoleUpdateDTO
> {
  constructor() {
    super(RolesSwagApi);
  }

  /** 获取角色树形结构 */
  async getTree() {
    return this.api.getTree();
  }

  /** 根据编码查找角色 */
  async findByCode(code: string) {
    return this.api.findByCode(code);
  }

  /** 获取角色及其所有权限 */
  async getRoleWithAllPermissions(roleId: string) {
    return this.api.getRoleWithAllPermissions(roleId);
  }

  /** 为角色分配权限 */
  async assignPermissions(roleId: string, permissions: string[]) {
    return this.api.assignPermissions(roleId, permissions);
  }

  /** 获取所有子角色 */
  async getAllChildren(roleId: string) {
    return this.api.getAllChildren(roleId);
  }

  /** 获取角色继承链 */
  async getInheritanceChain(roleId: string) {
    return this.api.getInheritanceChain(roleId);
  }
}
