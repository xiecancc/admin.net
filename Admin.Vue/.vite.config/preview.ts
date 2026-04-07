import type { UserConfig } from 'vite'
import type { Env } from '../src/types/env'
import { toString } from '../src/utils/convert.util'

/**
 * Vite 预览配置 - 仅保留与默认值不一致的配置
 */
export const createPreviewConfig = (env: Env): UserConfig['preview'] => ({
  // 与默认值不一致的配置：

  // 1. open: 默认 false，启用自动打开浏览器
  open: true,

  // 2. proxy: 默认 undefined，配置 API 代理
  proxy: {
    '/api': {
      changeOrigin: true,
      rewrite: path => path.replace(/^\/api/, ''),
      target: toString(env.VITE_API_BASE_URL, 'http://localhost:5154'),
    },
  },
})
