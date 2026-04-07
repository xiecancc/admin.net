import type { App } from 'vue'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

// 直接导出插件对象
export default {
  install(app: App) {
    const pinia = createPinia()

    // 添加持久化插件
    pinia.use(piniaPluginPersistedstate)

    app.use(pinia)
  },
}
