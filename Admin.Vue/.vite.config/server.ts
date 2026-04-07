import type { UserConfig } from "vite";
import type { Env } from "../src/types/env";
import { toNumber, toString } from "../src/utils/convert.util";

/**
 * Vite 服务器配置 - 仅保留与默认值不一致的配置
 */
export const createServerConfig = (env: Env): UserConfig["server"] => ({
  // 与默认值不一致的配置：

  watch: {
    ignored: ["**/node_modules/**", "**/dist/**"],
  },

  hmr: {
    overlay: true,
  },

  // 1. port: 默认 5173，自定义为 5000
  port: toNumber(env.VITE_SERVER_PORT, 5000),

  // 2. open: 默认 false，启用自动打开浏览器
  open: true,

  // 3. proxy: 默认 undefined，配置 API 代理
  proxy: {
    "/api": {
      changeOrigin: true,
      rewrite: (path) => path.replace(/^\/api/, ""),
      target: toString(env.VITE_API_BASE_URL, "http://localhost:5154"),
    },
  },

  // 4. warmup: 默认 undefined，预热关键文件
  warmup: {
    clientFiles: ["./index.html", "./src/main.ts", "./src/App.vue", "./src/router/index.ts"],
  },
});
