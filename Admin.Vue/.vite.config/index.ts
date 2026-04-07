/**
 * 通用的 manualChunks 函数
 * 用于在不同配置文件中共享代码分割逻辑
 */
export const manualChunks = (id: string) => {
  // 核心框架
  if (id.includes('node_modules/vue') || id.includes('node_modules/vue-router') || id.includes('node_modules/pinia')) {
    return 'core'
  }
  // UI 组件库（包括图标库）
  else if (id.includes('node_modules/element-plus') || id.includes('node_modules/@element-plus/icons-vue')) {
    return 'element'
  }
  // 工具库
  else if (id.includes('node_modules/@vueuse') || id.includes('node_modules/axios')) {
    return 'utils'
  }
  // 其他 node_modules
  else if (id.includes('node_modules')) {
    return 'vendor'
  }
}

export default {
  manualChunks,
}
