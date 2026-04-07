import type { UserConfig } from "vite";

/**
 * Vite Worker 配置 - 仅保留与默认值不一致的配置
 */
export const createWorkerConfig = (): UserConfig["worker"] => ({
  // 与默认值不一致的配置：

  // 1. format: 默认 'es'，使用 'iife' 格式
  format: "iife",
});
