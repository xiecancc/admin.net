import type { UserConfig } from 'vite'
import type { Env } from '../src/types/env'

/**
 * Vite 构建配置 - 使用 Vite 8 新特性优化
 */
export const createBuildConfig = (env: Env): UserConfig['build'] => ({
  // 1. target: 使用 'esnext' 最小化转译，利用现代浏览器特性
  target: 'esnext',

  // 2. modulePreload.polyfill: 禁用以减少 polyfill 注入
  modulePreload: { polyfill: false },

  // 3. cssCodeSplit: 禁用以合并所有 CSS
  cssCodeSplit: false,

  // 4. sourcemap: 非生产环境启用源映射
  sourcemap: env.NODE_ENV !== 'production',

  // 5. 构建时启用详细日志
  reportCompressedSize: true,

  // 7. chunk 大小警告阈值
  chunkSizeWarningLimit: 500,

  // 8. rollupOptions: 配置输出文件名和手动分块
  rollupOptions: {
    output: {
      // 使用更清晰的命名格式
      chunkFileNames: 'assets/js/[name]-[hash].js',
      entryFileNames: 'assets/js/[name]-[hash].js',
      assetFileNames: 'assets/[ext]/[name]-[hash].[ext]',

      // 手动分块优化
      manualChunks: id => {
        // 核心框架
        if (
          id.includes('node_modules/vue') ||
          id.includes('node_modules/vue-router') ||
          id.includes('node_modules/pinia')
        ) {
          return 'vendor'
        }
        // UI 组件库
        if (id.includes('node_modules/element-plus')) {
          return 'element'
        }
        // 工具库
        if (id.includes('node_modules/@vueuse') || id.includes('node_modules/axios')) {
          return 'utils'
        }
        // 其他 node_modules
        if (id.includes('node_modules')) {
          return 'vendor'
        }
      },
    },
  },
})
