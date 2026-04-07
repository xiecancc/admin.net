import type { PluginOption } from "vite";
import { createHtmlPlugin } from "vite-plugin-html";

/**
 * HTML 插件配置
 */
export const createHtmlPluginConfig = (): PluginOption =>
  createHtmlPlugin({
    inject: {
      tags: [
        // CDN预连接
        {
          attrs: {
            crossorigin: "",
            href: "https://cdn.jsdelivr.net",
            rel: "preconnect",
          },
          injectTo: "head",
          tag: "link",
        },
        {
          attrs: {
            crossorigin: "",
            href: "https://unpkg.com",
            rel: "preconnect",
          },
          injectTo: "head",
          tag: "link",
        },
        {
          attrs: {
            crossorigin: "",
            href: "https://cdnjs.cloudflare.com",
            rel: "preconnect",
          },
          injectTo: "head",
          tag: "link",
        },
        // 内容安全策略
        {
          attrs: {
            content:
              "default-src 'self'; script-src 'self' https://cdn.jsdelivr.net https://unpkg.com https://cdnjs.cloudflare.com; style-src 'self' https://cdn.jsdelivr.net https://unpkg.com; img-src 'self' data: https:",
            httpEquiv: "Content-Security-Policy",
          },
          injectTo: "head",
          tag: "meta",
        },
      ],
    },
    minify: true,
  });
