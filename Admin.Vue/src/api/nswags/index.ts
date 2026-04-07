import type { AxiosInstance } from "axios";
import { createAxios } from "@/utils/request.util";
import type { StandardSwagApi } from "./type";

/** NSwag 专用 Axios 实例 */
const nswagAxios = createAxios({
  transformResponse: [],
});

/**
 * SWAG API 基类
 * 封装 NSwag 生成的 API 客户端实例化逻辑
 * @template T API 客户端类型
 */
export class SwagApi<T> {
  protected _api: T;

  constructor(Swag: new (baseUrl?: string, instance?: AxiosInstance) => T) {
    this._api = new Swag(undefined, nswagAxios);
  }

  /** 获取底层 API 客户端实例 */
  get api(): T {
    return this._api;
  }
}

/**
 * 模块 API 类
 * 封装标准 CRUD 和批量操作，适用于后端标准化模块
 * @template T 标准 SWAG API 接口类型
 * @template ListDTO 列表数据类型
 * @template PagedDTO 分页数据类型
 * @template DetailDTO 详情数据类型
 * @template CreateDTO 创建数据类型
 * @template UpdateDTO 更新数据类型
 */
export class ModuleApi<
  T extends StandardSwagApi<ListDTO, PagedDTO, DetailDTO, CreateDTO, UpdateDTO>,
  ListDTO,
  PagedDTO,
  DetailDTO,
  CreateDTO,
  UpdateDTO,
> extends SwagApi<T> {
  /** 获取列表 */
  async getList() {
    return this._api.getList();
  }

  /** 获取分页数据 */
  async getPaged(page: number, size: number) {
    return this._api.getPaged(page, size);
  }

  /** 获取详情 */
  async get(id: string) {
    return this._api.get(id);
  }

  /** 创建 */
  async create(body: CreateDTO) {
    return this._api.create(body);
  }

  /** 更新 */
  async update(id: string, body: UpdateDTO) {
    return this._api.update(id, body);
  }

  /** 删除 */
  async delete(id: string) {
    return this._api.delete(id);
  }

  /** 恢复 */
  async restore(id: string) {
    return this._api.restore(id);
  }

  /** 批量创建 */
  async batchCreate(body: CreateDTO[]) {
    return this._api.create2(body);
  }

  /** 批量更新 */
  async batchUpdate(body: UpdateDTO[]) {
    return this._api.update2(body);
  }

  /** 批量删除 */
  async batchDelete(ids: string[]) {
    return this._api.delete2(ids);
  }

  /** 批量恢复 */
  async batchRestore(ids: string[]) {
    return this._api.restore2(ids);
  }
}
