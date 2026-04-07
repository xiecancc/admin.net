/**
 * 系统角色常量定义
 * 与后端 Domain.Shared.Constants.RoleConstants 保持一致
 */
export const ROLES = {
  /**
   * 超级管理员角色编码
   * 拥有系统所有权限，无需手动分配
   */
  ADMINISTRATOR: 'Administrator',

  /**
   * 普通管理员角色编码
   * 需要手动分配权限
   */
  MANAGER: 'Manager',

  /**
   * 普通用户角色编码
   * 基础权限
   */
  USER: 'User',
} as const

/**
 * 角色名称映射
 */
export const ROLE_NAMES: Record<string, string> = {
  [ROLES.ADMINISTRATOR]: '超级管理员',
  [ROLES.MANAGER]: '普通管理员',
  [ROLES.USER]: '普通用户',
}

/**
 * 内置角色列表
 */
export const BUILT_IN_ROLES = Object.values(ROLES)

/**
 * 判断是否为内置角色
 */
export function isBuiltInRole(roleCode: string): boolean {
  return BUILT_IN_ROLES.includes(roleCode as any)
}

/**
 * 判断是否为超级管理员
 */
export function isAdministrator(roleCodes: string | string[]): boolean {
  const codes = Array.isArray(roleCodes) ? roleCodes : [roleCodes]
  return codes.includes(ROLES.ADMINISTRATOR)
}

/**
 * 获取角色名称
 */
export function getRoleName(roleCode: string): string {
  return ROLE_NAMES[roleCode] || roleCode
}
