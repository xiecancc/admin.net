import type { PluginOption } from 'vite'
import { visualizer } from 'rollup-plugin-visualizer'

/**
 * 构建分析插件配置
 */
export const createVisualizerPlugin = (): PluginOption =>
  visualizer({
    brotliSize: true,
    filename: 'dist/stats.html',
    gzipSize: true,
    open: true,
  })
