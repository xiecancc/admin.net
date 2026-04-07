import { ModuleApi } from '../nswags'
import { ButtonPermissionsSwagApi } from '../nswags/auto'
import type {
  ButtonPermissionCreateDTO,
  ButtonPermissionDetailDTO,
  ButtonPermissionListDTO,
  ButtonPermissionPagedDTO,
  ButtonPermissionUpdateDTO,
} from '../nswags/auto'

/**
 * 按钮权限管理 API
 * 提供按钮级权限的增删改查、树形结构、批量操作等功能
 */
export class ButtonPermissionsApi extends ModuleApi<
  ButtonPermissionsSwagApi,
  ButtonPermissionListDTO,
  ButtonPermissionPagedDTO,
  ButtonPermissionDetailDTO,
  ButtonPermissionCreateDTO,
  ButtonPermissionUpdateDTO
> {
  constructor() {
    super(ButtonPermissionsSwagApi)
  }

  /** 获取权限树形结构 */
  async getTree() {
    return this.api.getTree()
  }

  /** 获取所有子权限 */
  async getAllChildren(permissionId: string) {
    return this.api.getAllChildren(permissionId)
  }
}
