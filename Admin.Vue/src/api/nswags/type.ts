import type { BooleanResult } from "./auto";

/**
 * API结果接口，包含成功、数据和消息字段。
 * 与后端 Result 返回的格式保持一致。
 * @template T 数据类型
 */
export interface Result<T> {
  /** 是否成功 */
  success: boolean;
  /** 错误代码 */
  code: string | null;
  /** 消息 */
  message: string | null;
  /** 数据 */
  data: T | null;
}

/**
 * 分页响应接口，包含列表、总页数、当前页、每页数量、总页数、是否有上一页和下一页。
 * 与后端 PagedResponse 返回的格式保持一致。
 * @template T 数据类型
 */
export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  size: number;
  readonly pages: number;
  readonly hasPrevious: boolean;
  readonly hasNext: boolean;
}

/**
 * 标准的SWAG API接口，包含列表、分页、详情、创建、更新、删除和批量操作。
 * @template ListDTO 列表数据类型
 * @template PagedDTO 分页数据类型
 * @template DetailDTO 详情数据类型
 * @template CreateDTO 创建数据类型
 * @template UpdateDTO 更新数据类型
 */
export interface StandardSwagApi<ListDTO, PagedDTO, DetailDTO, CreateDTO, UpdateDTO> {
  getList(): Promise<Result<ListDTO[]>>;
  getPaged(page: number, size: number): Promise<Result<PagedResponse<PagedDTO>>>;
  get(id: string): Promise<Result<DetailDTO>>;
  create(body: CreateDTO): Promise<BooleanResult>;
  update(id: string, body: UpdateDTO): Promise<BooleanResult>;
  delete(id: string): Promise<BooleanResult>;
  restore(id: string): Promise<BooleanResult>;
  create2(body: CreateDTO[]): Promise<BooleanResult>;
  update2(body: UpdateDTO[]): Promise<BooleanResult>;
  delete2(ids: string[]): Promise<BooleanResult>;
  restore2(ids: string[]): Promise<BooleanResult>;
}
