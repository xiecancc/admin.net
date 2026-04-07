import type { PreviewOptions } from 'vite'
import type { Env } from '../src/types/env'
import { toNumber, toString } from '../src/utils/convert.util'

/**
 * Vite 预览配置 - 使用 Vite 8 新特性优化
 */
export const createPreviewConfig = (env: Env): PreviewOptions => ({
  // 1. port: 默认 4173，自定义为 5000
  port: toNumber(env.VITE_SERVER_PORT, 5000),

  // 2. strictPort: 默认 false，启用严格端口检查
  strictPort: false,

  // 3. host: 默认 'localhost'，允许所有地址
  host: '0.0.0.0',

  // 4. allowedHosts: 默认 ['localhost']，允许所有主机
  allowedHosts: true,

  // 5. open: 默认 false，启用自动打开浏览器
  open: true,

  // 6. proxy: 默认 undefined，配置 API 代理
  proxy: {
    '/api': {
      changeOrigin: true,
      rewrite: path => path.replace(/^\/api/, ''),
      target: toString(env.VITE_API_BASE_URL, 'http://localhost:5154'),
    },
  },

  // 7. cors: 默认 false，启用 CORS 跨域请求
  cors: true,

  // 8. headers: 默认 undefined，设置响应头
  headers: {
    'X-Powered-By': 'Vite 8',
    'Access-Control-Allow-Origin': '*',
  },
})
