import type { UserConfig } from "vite";
import { URL, fileURLToPath } from "node:url";
import { createVitePlugins } from "./plugins";
import { toString } from "../src/utils/convert.util";
import type { Env } from "../src/types/env";

/**
 * Vite 共享配置 - 仅保留与默认值不一致的配置
 */
export const createSharedConfig = (env: Env): Partial<UserConfig> => ({
  // 与默认值不一致的配置：

  // 1. base: 默认 '/'，使用环境变量配置
  base: toString(env.VITE_BASE_URL, "/"),

  // 2. plugins: 默认 []，配置插件
  plugins: createVitePlugins(),

  // 3. resolve.alias: 默认 {}，配置路径别名
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("../src", import.meta.url)),
    },
    // 4. resolve.dedupe: 默认 []，去重依赖
    dedupe: ["vue", "vue-router", "pinia"],
  },

  // 5. css.devSourcemap: 默认 false，启用 CSS 源映射
  css: {
    devSourcemap: true,
  },
  optimizeDeps: {
    include: ["vue", "vue-router", "pinia", "element-plus"],
  },
});
