import type { UserConfig } from "vite";

/**
 * Vite SSR 配置 - 仅保留与默认值不一致的配置
 */
export const createSsrConfig = (): UserConfig["ssr"] => ({
  // 与默认值不一致的配置：

  // 1. resolve.mainFields: 默认 []，指定模块解析字段
  resolve: {
    mainFields: ["module", "jsnext:main", "jsnext"],
  },
});
