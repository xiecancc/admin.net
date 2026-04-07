import type { UserConfig } from "vite";
import type { Env } from "../src/types/env";

/**
 * Vite 构建配置 - 仅保留与默认值不一致的配置
 */
export const createBuildConfig = (env: Env): UserConfig["build"] => ({
  // 与默认值不一致的配置：

  // 1. target: 默认 'baseline-widely-available'，使用 'esnext' 最小化转译
  target: "esnext",

  // 2. modulePreload.polyfill: 默认 true，禁用以减少 polyfill 注入
  modulePreload: { polyfill: false },

  // 3. cssCodeSplit: 默认 true，禁用以合并所有 CSS
  cssCodeSplit: false,

  sourcemap: env.NODE_ENV !== "production",

  // 4. rollupOptions: 配置输出文件名和手动分块
  rollupOptions: {
    output: {
      chunkFileNames: "assets/js/[name]-[hash].js",
      entryFileNames: "assets/js/[name]-[hash].js",
      manualChunks: (id) => {
        if (
          id.includes("node_modules/vue") ||
          id.includes("node_modules/vue-router") ||
          id.includes("node_modules/pinia")
        ) {
          return "vendor";
        }
        if (id.includes("node_modules/element-plus")) {
          return "element";
        }
        if (id.includes("node_modules/@vueuse") || id.includes("node_modules/axios")) {
          return "utils";
        }
      },
    },
  },
});
