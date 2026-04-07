import type { PluginOption } from "vite";
import { VitePWA } from "vite-plugin-pwa";

/**
 * PWA 插件配置
 */
export const createPwaPlugin = (): PluginOption =>
  VitePWA({
    // 注册类型：自动更新
    registerType: "autoUpdate",
    // 包含的静态资源
    includeAssets: ["favicon.ico", "robots.txt"],
    // 应用清单配置
    manifest: {
      description: "Fund administration system",
      icons: [
        {
          sizes: "48x48 72x72 96x96 128x128 256x256 512x512",
          src: "favicon.ico",
          type: "image/x-icon",
        },
      ],
      name: "Fund Admin",
      short_name: "Admin",
      theme_color: "#ffffff",
    },
    // Service Worker 配置
    workbox: {
      // 运行时缓存配置
      runtimeCaching: [
        {
          // API 缓存策略
          handler: "NetworkFirst",
          options: {
            cacheName: "api-cache",
            expiration: {
              maxAgeSeconds: 60 * 60 * 24,
              maxEntries: 100, // 1 day
            },
            networkTimeoutSeconds: 10,
          },
          urlPattern: /^https:\/\/.*\/api\/.*/i,
        },
        {
          // 图片缓存策略
          handler: "CacheFirst",
          options: {
            cacheName: "images-cache",
            expiration: {
              maxAgeSeconds: 60 * 60 * 24 * 7,
              maxEntries: 50, // 7 days
            },
          },
          urlPattern: /^https:\/\/.*\/.*\.(png|jpg|jpeg|gif|webp)$/i,
        },
        {
          // 字体缓存策略
          handler: "CacheFirst",
          options: {
            cacheName: "fonts-cache",
            expiration: {
              maxAgeSeconds: 60 * 60 * 24 * 30,
              maxEntries: 20, // 30 days
            },
          },
          urlPattern: /^https:\/\/.*\/.*\.(woff|woff2|ttf|otf|eot)$/i,
        },
      ],
      // 跳过等待，直接激活新的 Service Worker
      skipWaiting: true,
      // 客户端导航时激活新的 Service Worker
      clientsClaim: true,
    },
    // 开发模式配置
    devOptions: {
      enabled: true,
      type: "module",
    },
  });
