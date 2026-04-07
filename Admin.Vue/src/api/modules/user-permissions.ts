import { SwagApi } from "../nswags";
import { UserPermissionsSwagApi } from "../nswags/auto";

/**
 * 用户权限 API
 * 查询当前用户或指定用户的权限信息，包括菜单、按钮、API权限
 */
export class UserPermissionsApi extends SwagApi<UserPermissionsSwagApi> {
  constructor() {
    super(UserPermissionsSwagApi);
  }

  /** 获取当前用户的所有权限（菜单、按钮、API） */
  async getUserPermissions() {
    return this.api.getUserPermissions();
  }

  /** 获取当前用户的按钮权限编码列表 */
  async getUserButtons() {
    return this.api.getUserButtons();
  }

  /** 获取指定用户的按钮权限编码列表 */
  async getUserButtonsById(userId: string) {
    return this.api.getUserButtonsById(userId);
  }

  /** 获取当前用户的菜单权限列表 */
  async getUserMenus() {
    return this.api.getUserMenus();
  }

  /** 获取指定用户的菜单权限列表 */
  async getUserMenusById(userId: string) {
    return this.api.getUserMenusById(userId);
  }
}
