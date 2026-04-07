import type { BuildEnvironmentOptions } from 'vite'
import type { Env } from '../src/types/env'
import { manualChunks } from './index'

/**
 * Vite 构建配置 - 使用 Vite 8 新特性优化
 */
export const createBuildConfig = (env: Env): BuildEnvironmentOptions => ({
  // 1. target: 使用 'esnext' 最小化转译，利用现代浏览器特性
  target: 'esnext',

  // 2. modulePreload.polyfill: 禁用以减少 polyfill 注入
  modulePreload: { polyfill: false },

  // 3. 输出目录配置
  outDir: 'dist',
  assetsDir: 'assets',

  // 4. 静态资源内联限制
  assetsInlineLimit: 4096,

  // 5. cssCodeSplit: 禁用以合并所有 CSS
  cssCodeSplit: false,

  // 6. CSS 目标和压缩配置
  cssTarget: 'esnext',
  cssMinify: 'lightningcss',

  // 7. sourcemap: 非生产环境启用源映射
  sourcemap: env.NODE_ENV !== 'production',

  // 8. 构建时启用详细日志
  reportCompressedSize: true,

  // 9. chunk 大小警告阈值
  chunkSizeWarningLimit: 500,

  // 10. 启用 Vite 8 新特性：构建优化
  minify: 'oxc',

  // 11. 清空输出目录
  emptyOutDir: true,

  // 12. 复制公共目录
  copyPublicDir: true,

  // 13. 生成构建清单
  manifest: true,

  // 14. rolldownOptions: 配置输出文件名和手动分块 (Vite 8 推荐使用 rolldownOptions 替代 rollupOptions)
  rolldownOptions: {
    output: {
      // 使用更清晰的命名格式
      chunkFileNames: 'assets/js/[name]-[hash].js',
      entryFileNames: 'assets/js/[name]-[hash].js',
      assetFileNames: 'assets/[ext]/[name]-[hash].[ext]',

      // 手动分块优化
      manualChunks,
    },
  },
})
