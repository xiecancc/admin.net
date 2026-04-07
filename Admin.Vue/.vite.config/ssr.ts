import type { SSROptions } from 'vite'

/**
 * Vite SSR 配置 - 使用 Vite 8 新特性优化
 */
export const createSsrConfig = (): SSROptions => ({
  // 1. noExternal: 控制哪些依赖不应该被外部化
  noExternal: [],

  // 2. external: 控制哪些依赖应该被外部化
  external: [],

  // 3. target: SSR构建的目标
  target: 'node',

  // 4. optimizeDeps: SSR依赖优化配置
  optimizeDeps: {
    noDiscovery: false,
  },

  // 5. resolve: 解析选项
  resolve: {
    // 插件管道中使用的条件
    conditions: ['node', 'import'],
    // SSR导入外部依赖时使用的条件
    externalConditions: ['node', 'module-sync'],
    // 模块解析字段
    mainFields: ['module', 'jsnext:main', 'jsnext', 'main'],
  },
})
