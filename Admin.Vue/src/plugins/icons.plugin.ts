import type { App } from 'vue'
import {
  ArrowDown,
  Delete,
  Edit,
  Expand,
  Fold,
  FullScreen,
  House,
  Moon,
  Plus,
  Refresh,
  Search,
  Setting,
  Shop,
  Sunny,
  SwitchButton,
  User,
  UserFilled,
} from '@element-plus/icons-vue'

// 图标组件对象
const iconComponents = {
  ArrowDown,
  Delete,
  Edit,
  Expand,
  Fold,
  FullScreen,
  House,
  Moon,
  Plus,
  Refresh,
  Search,
  Setting,
  Shop,
  Sunny,
  SwitchButton,
  User,
  UserFilled,
}

// 导出所有图标组件
export {
  ArrowDown,
  Plus,
  Delete,
  Refresh,
  Search,
  FullScreen,
  UserFilled,
  Edit,
  User,
  Setting,
  Sunny,
  Moon,
  Expand,
  Fold,
  House,
  Shop,
  SwitchButton,
}

// 直接导出插件对象
export default {
  install(app: App) {
    // 注册需要的图标为全局组件
    Object.entries(iconComponents).forEach(([name, component]) => {
      app.component(name, component)
    })
  },
}
