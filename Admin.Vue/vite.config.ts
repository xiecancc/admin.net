import { defineConfig, loadEnv } from "vite";
import path from "node:path";
import { createSharedConfig } from "./.vite.config/shared";
import { createBuildConfig } from "./.vite.config/build";
import { createServerConfig } from "./.vite.config/server";
import { createPreviewConfig } from "./.vite.config/preview";
import { createOptimizationConfig } from "./.vite.config/optimization";
import { createWorkerConfig } from "./.vite.config/worker";
import { createSsrConfig } from "./.vite.config/ssr";
import type { Env } from "./src/types/env";

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  // 加载环境变量，第二个参数是 .env 目录
  const env = loadEnv(mode, path.resolve(process.cwd(), ".env"), "") as unknown as Env;

  return {
    ...createSharedConfig(env),
    build: createBuildConfig(env),
    optimizeDeps: createOptimizationConfig(),
    preview: createPreviewConfig(env),
    server: createServerConfig(env),
    ssr: createSsrConfig(),
    worker: createWorkerConfig(),
  };
});
