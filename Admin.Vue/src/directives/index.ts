import type { App } from 'vue'
import { auth, permission, role } from './permission'
import { loading } from './loading'

export function setupDirectives(app: App) {
  app.directive('permission', permission)
  app.directive('role', role)
  app.directive('auth', auth)
  app.directive('loading', loading)
}

export { permission, role, auth, loading }
