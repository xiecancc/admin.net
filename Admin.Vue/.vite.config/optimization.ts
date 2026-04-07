import type { UserConfig } from 'vite'

/**
 * Vite 依赖优化配置 - 仅保留与默认值不一致的配置
 */
export const createOptimizationConfig = (): UserConfig['optimizeDeps'] => ({
  // 与默认值不一致的配置：

  // 1. include: 默认 undefined，强制预构建的依赖
  include: ['vue', 'vue-router', 'pinia', 'element-plus', '@vueuse/core', 'axios'],
})
