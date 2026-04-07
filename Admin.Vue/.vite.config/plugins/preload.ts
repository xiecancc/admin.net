import type { PluginOption } from 'vite'
import injectPreload from 'unplugin-inject-preload/vite'

/**
 * 预加载插件配置
 */
export const createPreloadPlugin = (): PluginOption =>
  injectPreload({
    files: [
      {
        attributes: {
          as: 'image',
          fetchpriority: 'high',
          rel: 'preload',
        },
        outputMatch: /\.(png|jpe?g|webp|avif|svg)$/,
      },
    ],
    injectTo: 'head',
  })
