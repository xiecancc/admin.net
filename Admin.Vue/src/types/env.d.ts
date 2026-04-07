/**
 * 环境变量类型定义
 */
export interface Env {
  // 应用配置
  VITE_BASE_URL: string
  VITE_APP_VERSION: string
  VITE_APP_TITLE: string
  NODE_ENV: string

  // 服务器配置
  VITE_SERVER_PORT: string
  VITE_API_BASE_URL: string
  VITE_API_TIMEOUT: string

  // 调试配置
  VITE_DEBUG_MODE: string
  VITE_SHOW_CONSOLE_LOG: string

  // 构建配置
  VITE_BUILD_ANALYZE: string
  VITE_BUILD_COMPRESS: string
}

/**
 * 扩展 ImportMetaEnv 接口，添加强类型环境变量支持
 */
declare interface ImportMetaEnv extends Env {
  // Vite 默认环境变量
  BASE_URL: string
  MODE: string
  PROD: boolean
  DEV: boolean
  SSR: boolean

  // 兼容任意字符串键值对
  [key: string]: string | boolean
}
