import type { PluginOption } from "vite";
import { ViteImageOptimizer } from "vite-plugin-image-optimizer";

/**
 * 图片优化插件配置
 */
export const createImageOptimizerPlugin = (): PluginOption =>
  ViteImageOptimizer({
    ansiColors: false,
    avif: {
      quality: 85,
      speed: 10,
    },
    cache: true,
    cacheLocation: "./node_modules/.cache/vite-image-optimizer",
    include: /\.(png|jpe?g|webp|avif|svg)$/i,
    includePublic: true,
    jpeg: {
      mozjpeg: true,
      quality: 85,
    },
    logStats: false,
    png: {
      effort: 3,
      quality: 85,
    },
    svg: {
      multipass: false,
    },
    webp: {
      lossless: false,
      quality: 85,
    },
  });
