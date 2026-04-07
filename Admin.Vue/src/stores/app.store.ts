import { defineStore } from 'pinia'
import { useDark, useToggle } from '@vueuse/core'
import { ref } from 'vue'
import { STORAGE_KEYS } from '@/constants/app.constant'

export const useAppStore = defineStore(
  'app',
  () => {
    const appSize = ref<'large' | 'default' | 'small'>('small')
    const appTheme = useDark({
      attribute: 'class',
      selector: 'html',
      storage: localStorage,
      storageKey: STORAGE_KEYS.THEME,
      valueDark: 'dark',
      valueLight: 'light',
    })
    const isSidebarCollapsed = ref<boolean>(false)

    const setAppSize = (newSize: 'large' | 'default' | 'small') => {
      appSize.value = newSize
    }

    const toggleAppTheme = useToggle(appTheme)
    const toggleSidebar = useToggle(isSidebarCollapsed)

    return {
      appSize,
      appTheme,
      isSidebarCollapsed,
      setAppSize,
      toggleAppTheme,
      toggleSidebar,
    }
  },
  {
    persist: {
      key: STORAGE_KEYS.APP,
      pick: ['appSize', 'isSidebarCollapsed'],
      storage: localStorage,
    },
  }
)
