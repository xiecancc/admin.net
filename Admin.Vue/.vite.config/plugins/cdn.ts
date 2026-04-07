import type { PluginOption } from 'vite'
import { Plugin as importToCDN } from 'vite-plugin-cdn-import'

/**
 * CDN 插件配置
 */
export const createCdnPlugin = (): PluginOption =>
  importToCDN({
    modules: [
      {
        name: 'vue',
        path: 'https://cdn.jsdelivr.net/npm/vue@3.5.29/dist/vue.global.prod.js',
        var: 'Vue',
      },
      {
        name: 'vue-router',
        path: 'https://cdn.jsdelivr.net/npm/vue-router@5.0.3/dist/vue-router.global.prod.js',
        var: 'VueRouter',
      },
      {
        name: 'pinia',
        path: 'https://cdn.jsdelivr.net/npm/pinia@3.0.4/dist/pinia.iife.prod.js',
        var: 'Pinia',
      },
      {
        name: 'axios',
        path: 'https://cdn.jsdelivr.net/npm/axios@1.13.6/dist/axios.min.js',
        var: 'axios',
      },
    ],
    prodUrl: 'https://cdn.jsdelivr.net/npm/{name}@{version}/{path}',
  })
