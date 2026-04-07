import type { DepOptimizationOptions } from 'vite'
import { manualChunks } from './index'

/**
 * Vite 依赖优化配置 - 使用 Vite 8 新特性优化
 */
export const createOptimizationConfig = (): DepOptimizationOptions => ({
  // 1. include: 默认 undefined，强制预构建的依赖
  include: [
    'vue',
    'vue-router',
    'pinia',
    'element-plus',
    '@vueuse/core',
    'axios',
    '@element-plus/icons-vue',
    'pinia-plugin-persistedstate',
  ],

  // 2. exclude: 默认 undefined，不优化的依赖
  exclude: [],

  // 3. needsInterop: 默认 undefined，强制 ESM 互操作的依赖
  needsInterop: [],

  // 4. 配置依赖解析策略 (Vite 8 推荐使用 Rolldown)
  rolldownOptions: {
    // 优化依赖的输出配置
    output: {
      // 启用代码分割
      manualChunks,
    },
  },

  // 5. extensions: 默认 undefined，可优化的文件扩展名
  extensions: ['.mjs', '.js', '.ts', '.mts', '.vue'],

  // 6. noDiscovery: 默认 false，自动依赖发现
  noDiscovery: false,

  // 7. holdUntilCrawlEnd: 默认 true，冷启动时保持优化结果直到所有静态导入被爬取
  holdUntilCrawlEnd: true,

  // 8. ignoreOutdatedRequests: 默认 false，当请求过时的优化依赖时不抛出错误
  ignoreOutdatedRequests: false,

  // 9. entries: 默认 undefined，自定义条目
  entries: ['./index.html', './src/main.ts'],

  // 10. force: 默认 undefined，强制依赖预优化
  force: false,
})
