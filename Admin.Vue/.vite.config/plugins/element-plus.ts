import type { PluginOption } from "vite";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { ElementPlusResolver } from "unplugin-vue-components/resolvers";

/**
 * Element Plus 自动导入插件配置
 */
export const createElementPlusPlugins = (): PluginOption[] => [
  // 按需导入插件
  AutoImport({
    resolvers: [ElementPlusResolver()],
    imports: ["vue", "vue-router", "pinia"],
    // 优化导入路径
    dirs: ["./src/composables", "./src/stores"],
  }),
  Components({
    resolvers: [ElementPlusResolver()],
    directoryAsNamespace: false,
    globalNamespaces: [],
    dirs: ["src/components"],
    // 优化组件扫描
    deep: true,
  }),
];
