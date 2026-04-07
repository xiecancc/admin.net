import type { PluginOption } from 'vite'
import { VitePWA } from 'vite-plugin-pwa'

// 缓存时间常量
const ONE_DAY = 24 * 60 * 60 // 1 day in seconds
const ONE_WEEK = 7 * ONE_DAY
const ONE_MONTH = 30 * ONE_DAY

/**
 * PWA 插件配置 - 使用新版本特性优化
 */
export const createPwaPlugin = (): PluginOption =>
  VitePWA({
    // 注册类型：自动更新
    registerType: 'autoUpdate',
    // 包含的静态资源
    includeAssets: ['favicon.ico', 'robots.txt'],
    // 应用清单配置 - 使用完整 PWA manifest
    manifest: {
      description: 'Fund administration system',
      display: 'standalone',
      orientation: 'portrait',
      icons: [
        {
          sizes: '48x48 72x72 96x96 128x128 256x256 512x512',
          src: 'favicon.ico',
          type: 'image/x-icon',
          purpose: 'any',
        },
        {
          sizes: '192x192',
          src: 'favicon.ico',
          type: 'image/x-icon',
          purpose: 'maskable',
        },
        {
          sizes: '512x512',
          src: 'favicon.ico',
          type: 'image/x-icon',
          purpose: 'any',
        },
      ],
      name: 'Fund Admin',
      short_name: 'Admin',
      theme_color: '#ffffff',
      background_color: '#ffffff',
      start_url: '/',
      categories: ['business', 'productivity'],
    },
    // Service Worker 配置 - 使用优化的缓存策略
    workbox: {
      // 跳过等待，直接激活新的 Service Worker
      skipWaiting: true,
      // 客户端导航时激活新的 Service Worker
      clientsClaim: true,
      // 清理旧缓存
      cleanupOutdatedCaches: true,
      // 预缓存所有静态资源
      globPatterns: ['**/*.{js,css,html,ico,png,svg,webp,woff,woff2}'],
      // 运行时缓存配置
      runtimeCaching: [
        {
          // API 缓存策略 - NetworkFirst 优先网络
          urlPattern: /^https:\/\/.*\/api\/.*/i,
          handler: 'NetworkFirst',
          options: {
            cacheName: 'api-cache',
            expiration: {
              maxAgeSeconds: ONE_DAY,
              maxEntries: 100,
            },
            networkTimeoutSeconds: 10,
            cacheableResponse: {
              statuses: [0, 200],
            },
          },
        },
        {
          // 图片缓存策略 - CacheFirst 优先缓存
          urlPattern: /^https?:\/\/.*\.(png|jpg|jpeg|gif|webp|svg|ico)$/i,
          handler: 'CacheFirst',
          options: {
            cacheName: 'images-cache',
            expiration: {
              maxAgeSeconds: ONE_WEEK,
              maxEntries: 50,
            },
            cacheableResponse: {
              statuses: [0, 200],
            },
          },
        },
        {
          // 字体缓存策略 - CacheFirst 长期缓存
          urlPattern: /^https?:\/\/.*\.(woff|woff2|ttf|otf|eot)$/i,
          handler: 'CacheFirst',
          options: {
            cacheName: 'fonts-cache',
            expiration: {
              maxAgeSeconds: ONE_MONTH,
              maxEntries: 20,
            },
            cacheableResponse: {
              statuses: [0, 200],
            },
          },
        },
        {
          // JS/CSS 文件缓存 - StaleWhileRevalidate
          urlPattern: /^https?:\/\/.*\.(js|css)$/i,
          handler: 'StaleWhileRevalidate',
          options: {
            cacheName: 'assets-cache',
            expiration: {
              maxAgeSeconds: ONE_WEEK,
              maxEntries: 50,
            },
            cacheableResponse: {
              statuses: [0, 200],
            },
          },
        },
      ],
    },
    // 构建优化
    buildStrategies: 'workbox',
    // 自更新配置
    selfDestroying: true,
    // 开发模式配置
    devOptions: {
      enabled: true,
      type: 'module',
    },
    // TypeScript 支持
    typescript: true,
  })
