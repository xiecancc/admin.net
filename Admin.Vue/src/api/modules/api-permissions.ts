import { ModuleApi } from '../nswags'
import { ApiPermissionsSwagApi } from '../nswags/auto'
import type {
  ApiPermissionCreateDTO,
  ApiPermissionDetailDTO,
  ApiPermissionListDTO,
  ApiPermissionPagedDTO,
  ApiPermissionUpdateDTO,
} from '../nswags/auto'

/**
 * API权限管理 API
 * 提供API接口权限的增删改查、树形结构、批量操作等功能
 */
export class ApiPermissionsApi extends ModuleApi<
  ApiPermissionsSwagApi,
  ApiPermissionListDTO,
  ApiPermissionPagedDTO,
  ApiPermissionDetailDTO,
  ApiPermissionCreateDTO,
  ApiPermissionUpdateDTO
> {
  constructor() {
    super(ApiPermissionsSwagApi)
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
