import { defineConfig, loadEnv } from 'vite'
import path from 'node:path'
import { URL, fileURLToPath } from 'node:url'
import { createVitePlugins } from './.vite.config/plugins'
import { toString } from './src/utils/convert.util'
import { createBuildConfig } from './.vite.config/build'
import { createServerConfig } from './.vite.config/server'
import { createPreviewConfig } from './.vite.config/preview'
import { createOptimizationConfig } from './.vite.config/optimization'
import { createWorkerConfig } from './.vite.config/worker'
import { createSsrConfig } from './.vite.config/ssr'
import type { Env } from './src/types/env'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  // 加载环境变量，第二个参数是 .env 目录
  const env = loadEnv(mode, path.resolve(process.cwd(), '.env'), '') as unknown as Env

  return {
    // 启用 Vite 8 新特性：持久化缓存
    cacheDir: 'node_modules/.vite',
    
    // 1. base: 默认 '/'，使用环境变量配置
    base: toString(env.VITE_BASE_URL, '/'),

    // 2. plugins: 默认 []，配置插件
    plugins: createVitePlugins(),

    // 3. resolve.alias: 默认 {}，配置路径别名
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
      // 4. resolve.dedupe: 默认 []，去重依赖
      dedupe: ['vue', 'vue-router', 'pinia'],
    },

    // 5. css.devSourcemap: 默认 false，启用 CSS 源映射
    css: {
      devSourcemap: true,
    },

    // 6. 环境变量配置
    envPrefix: 'VITE_',
    envDir: path.resolve(process.cwd(), '.env'),

    // 7. 日志配置
    logLevel: env.NODE_ENV === 'production' ? 'info' : 'warn',
    clearScreen: true,

    // 8. 启用 Vite 8 新特性：未来行为
    future: 'warn',

    // 9. 实验性特性
    experimental: {
      // 启用实验性特性
      renderBuiltUrl: (filename: string) => {
        return {
          relative: true,
        };
      },
    },
    
    build: createBuildConfig(env),
    optimizeDeps: createOptimizationConfig(),
    preview: createPreviewConfig(env),
    server: createServerConfig(env),
    ssr: createSsrConfig(),
    worker: createWorkerConfig(),
  }
})
