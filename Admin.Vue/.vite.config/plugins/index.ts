import type { PluginOption } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import vueDevTools from 'vite-plugin-vue-devtools'
import inspect from 'vite-plugin-inspect'
import { createCompressionPlugins } from './compression'
import { createHtmlPluginConfig } from './html'
import { createCheckerPlugin } from './checker'
import { createVisualizerPlugin } from './visualizer'
import { createCdnPlugin } from './cdn'
import { createImageOptimizerPlugin } from './image-optimizer'
import { createPreloadPlugin } from './preload'
import { createElementPlusPlugins } from './element-plus'
import { createPwaPlugin } from './pwa'

/**
 * Vite 插件配置
 */
export const createVitePlugins = (): PluginOption[] => [
  // 核心插件
  vue({
    // 优化模板编译
    template: {
      compilerOptions: {
        // 禁用 isCustomElement 检查，提高编译速度
        isCustomElement: _tag => false,
      },
    },
  }),
  vueJsx(),

  // 开发工具
  vueDevTools(),
  inspect(),

  // 功能插件
  createHtmlPluginConfig(),
  createCheckerPlugin(),
  createVisualizerPlugin(),
  createCdnPlugin(),
  createImageOptimizerPlugin(),
  createPreloadPlugin(),
  createPwaPlugin(),

  // Element Plus 插件
  ...createElementPlusPlugins(),

  // 压缩插件（最后）
  ...createCompressionPlugins(),
]
