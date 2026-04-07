import { ModuleApi } from '../nswags'
import { UsersSwagApi } from '../nswags/auto'
import type { UserCreateDTO, UserDetailDTO, UserListDTO, UserPagedDTO, UserUpdateDTO } from '../nswags/auto'

/**
 * 用户管理 API
 * 提供用户的增删改查、分页查询、批量操作等功能
 */
export class UsersApi extends ModuleApi<
  UsersSwagApi,
  UserListDTO,
  UserPagedDTO,
  UserDetailDTO,
  UserCreateDTO,
  UserUpdateDTO
> {
  constructor() {
    super(UsersSwagApi)
  }
}
