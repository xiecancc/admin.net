import { CancelToken } from "axios";
import { SwagApi } from "../nswags";
import { AuthSwagApi } from "../nswags/auto";
import type { LoginRequestDTO } from "../nswags/auto";

/**
 * 认证 API
 * 提供用户认证相关操作，包括登录、登出、获取当前用户信息
 */
export class AuthApi extends SwagApi<AuthSwagApi> {
  constructor() {
    super(AuthSwagApi);
  }

  /** 用户登录 */
  login(credentials: LoginRequestDTO, cancelToken?: CancelToken) {
    return this.api.login(credentials, cancelToken);
  }

  /** 用户登出 */
  logout(cancelToken?: CancelToken) {
    return this.api.logout(cancelToken);
  }

  /** 获取当前登录用户信息 */
  getCurrentUser(cancelToken?: CancelToken) {
    return this.api.getCurrentUser(cancelToken);
  }
}
