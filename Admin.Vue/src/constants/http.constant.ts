export const HTTP_STATUS = {
  BAD_GATEWAY: 502,
  BAD_REQUEST: 400,
  CREATED: 201,
  FORBIDDEN: 403,
  GATEWAY_TIMEOUT: 504,
  INTERNAL_SERVER_ERROR: 500,
  METHOD_NOT_ALLOWED: 405,
  NOT_FOUND: 404,
  NO_CONTENT: 204,
  OK: 200,
  REQUEST_TIMEOUT: 408,
  SERVICE_UNAVAILABLE: 503,
  UNAUTHORIZED: 401,
} as const;

export const ERROR_MESSAGES: Record<number, string> = {
  [HTTP_STATUS.BAD_REQUEST]: "请求参数错误",
  [HTTP_STATUS.UNAUTHORIZED]: "登录已过期，请重新登录",
  [HTTP_STATUS.FORBIDDEN]: "没有权限访问该资源",
  [HTTP_STATUS.NOT_FOUND]: "请求的资源不存在",
  [HTTP_STATUS.METHOD_NOT_ALLOWED]: "请求方法不允许",
  [HTTP_STATUS.REQUEST_TIMEOUT]: "请求超时",
  [HTTP_STATUS.INTERNAL_SERVER_ERROR]: "服务器内部错误",
  [HTTP_STATUS.BAD_GATEWAY]: "网关错误",
  [HTTP_STATUS.SERVICE_UNAVAILABLE]: "服务不可用",
  [HTTP_STATUS.GATEWAY_TIMEOUT]: "网关超时",
};

export const HTTP_CONTENT_TYPE = {
  FORM_DATA: "multipart/form-data",
  FORM_URLENCODED: "application/x-www-form-urlencoded",
  JSON: "application/json",
  OCTET_STREAM: "application/octet-stream",
} as const;
