import type { ServerOptions } from 'vite'
import type { Env } from '../src/types/env'
import { toNumber, toString } from '../src/utils/convert.util'

/**
 * Vite 服务器配置 - 使用 Vite 8 新特性优化
 */
export const createServerConfig = (env: Env): ServerOptions => ({
  // 1. port: 默认 5173，自定义为 5000
  port: toNumber(env.VITE_SERVER_PORT, 5000),

  // 2. strictPort: 默认 false，禁用严格端口检查
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

  // 7. cors: 默认 false，启用 CORS
  cors: true,

  // 8. headers: 默认 undefined，设置响应头
  headers: {
    'X-Powered-By': 'Vite 8',
    'Access-Control-Allow-Origin': '*',
  },

  // 9. watch: 文件监听配置
  watch: {
    ignored: ['**/node_modules/**', '**/dist/**'],
    // 启用更高效的文件监听
    usePolling: false,
    interval: 100,
  },

  // 10. hmr: 热模块替换配置
  hmr: {
    overlay: true,
    // 启用更快速的热模块替换
    protocol: 'ws',
    port: toNumber(env.VITE_SERVER_PORT, 5000),
  },

  // 11. warmup: 预热关键文件
  warmup: {
    clientFiles: [
      './index.html',
      './src/main.ts',
      './src/App.vue',
      './src/router/index.ts',
      './src/stores/app.store.ts',
      './src/stores/auth.store.ts',
      './src/layouts/backend/index.vue',
      './src/layouts/frontend/index.vue',
    ],
  },

  // 12. fs: 文件系统配置
  fs: {
    strict: true,
    allow: ['.'],
  },

  // 13. origin: 资源URL的源
  origin: `http://localhost:${toNumber(env.VITE_SERVER_PORT, 5000)}`,

  // 14. 启用 Vite 8 新特性：更快的启动速度
  preTransformRequests: true,

  // 15. sourcemapIgnoreList: 源映射忽略列表
  sourcemapIgnoreList: sourcePath => sourcePath.includes('node_modules'),

  // 16. forwardConsole: 控制台转发
  forwardConsole: true,

  // 17. ws: 是否禁用WebSocket连接
  ws: undefined,

  // 18. middlewareMode: 中间件模式
  middlewareMode: false,

  // 19. perEnvironmentStartEndDuringDev: 每个环境的构建开始和结束钩子
  perEnvironmentStartEndDuringDev: false,

  // 20. perEnvironmentWatchChangeDuringDev: 每个环境的文件变更钩子
  perEnvironmentWatchChangeDuringDev: false,

  // 21. hotUpdateEnvironments: HMR任务运行
  hotUpdateEnvironments: undefined,
})
