export const STORAGE_KEYS = {
  APP: 'app',
  AUTH: 'auth',
  THEME: 'app-theme',
  TOKEN: 'auth-token',
} as const

export const ROUTES = {
  DASHBOARD: '/admin/dashboard',
  LOGIN: '/login',
  REGISTER: '/register',
  SETTINGS: '/admin/settings',
  USERS: '/admin/users',
} as const

export const WHITE_LIST: readonly string[] = ['/login', '/register', '/forgot-password']
