import type { PluginOption } from "vite";
import compression from "vite-plugin-compression";

/**
 * 压缩插件配置
 */
export const createCompressionPlugins = (): PluginOption[] => [
  // Gzip压缩插件
  compression({
    algorithm: "gzip",
    compressionOptions: { level: 5 },
    deleteOriginFile: false,
    ext: ".gz",
    filter: /\.(js|mjs|json|css|html)$/i,
    threshold: 10_240,
    verbose: false,
  }),

  // Brotli压缩插件
  compression({
    algorithm: "brotliCompress",
    compressionOptions: { level: 5 },
    deleteOriginFile: false,
    ext: ".br",
    filter: /\.(js|mjs|json|css|html)$/i,
    threshold: 10_240,
    verbose: false,
  }),
];
