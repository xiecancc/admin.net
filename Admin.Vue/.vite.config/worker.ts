import type { UserConfig } from 'vite'
import { manualChunks } from './index'

/**
 * Vite Worker 配置 - 使用 Vite 8 新特性优化
 */
export const createWorkerConfig = (): UserConfig['worker'] => ({
  // 1. format: 输出格式，使用 'iife' 格式
  format: 'iife',

  // 2. plugins: 应用于 Worker 打包的插件
  plugins: () => [],

  // 3. rolldownOptions: Worker 打包的 Rolldown 配置
  rolldownOptions: {
    output: {
      // 启用代码分割
      manualChunks,
    },
  },
})
