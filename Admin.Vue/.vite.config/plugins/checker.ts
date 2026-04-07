import type { PluginOption } from "vite";
import checker from "vite-plugin-checker";

/**
 * 代码质量检查插件配置
 * 注意：暂时禁用 vueTsc 以避免与 @vue/language-core@3.x 的兼容性问题
 */
export const createCheckerPlugin = (): PluginOption =>
  checker({
    enableBuild: true,
    overlay: true,
    terminal: true,
    typescript: true,
    vueTsc: false,
  });
