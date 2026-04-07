/**
 * API 统一导出入口
 * 提供所有业务模块 API 实例的统一访问点
 */
import { AuthApi } from './modules/auth'
import { UserPermissionsApi } from './modules/user-permissions'
import { ApiPermissionsApi } from './modules/api-permissions'
import { ButtonPermissionsApi } from './modules/button-permissions'
import { MenuPermissionsApi } from './modules/menu-permissions'
import { RolesApi } from './modules/roles'
import { UsersApi } from './modules/users'

/** 认证 API 实例 */
export const authApi = new AuthApi()
/** 用户权限 API 实例 */
export const userPermissionsApi = new UserPermissionsApi()
/** API权限 API 实例 */
export const apiPermissionsApi = new ApiPermissionsApi()
/** 按钮权限 API 实例 */
export const buttonPermissionsApi = new ButtonPermissionsApi()
/** 菜单权限 API 实例 */
export const menuPermissionsApi = new MenuPermissionsApi()
/** 角色 API 实例 */
export const rolesApi = new RolesApi()
/** 用户 API 实例 */
export const usersApi = new UsersApi()

export const api = {
  apiPermissions: apiPermissionsApi,
  auth: authApi,
  buttonPermissions: buttonPermissionsApi,
  menuPermissions: menuPermissionsApi,
  roles: rolesApi,
  userPermissions: userPermissionsApi,
  users: usersApi,
}
