import axios from 'axios'
import type { AxiosError } from 'axios'
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios'
import { useAuthStore } from '@/stores/auth.store'
import { API, IS_DEV } from '@/constants/env.constant'
import { ROUTES } from '@/constants/app.constant'
import { ERROR_MESSAGES, HTTP_CONTENT_TYPE, HTTP_STATUS } from '@/constants/http.constant'

export interface ApiResponse<T = unknown> {
  code: number
  data: T
  message: string
}

export interface RequestConfig extends AxiosRequestConfig {
  showError?: boolean
  showSuccess?: boolean
  successMessage?: string
}

const defaultConfig: AxiosRequestConfig = {
  baseURL: API.BASE_URL,
  headers: {
    'Content-Type': HTTP_CONTENT_TYPE.JSON,
  },
  timeout: API.TIMEOUT,
}

function handleError(error: AxiosError, showError = true): void {
  const status = error.response?.status
  const message =
    (error.response?.data as { message?: string })?.message ||
    ERROR_MESSAGES[status || 0] ||
    error.message ||
    '请求失败'

  if (status === HTTP_STATUS.UNAUTHORIZED) {
    const authStore = useAuthStore()
    authStore.logout()
    window.location.href = ROUTES.LOGIN
    return
  }

  if (status === HTTP_STATUS.FORBIDDEN) {
    window.location.href = ROUTES.LOGIN
    return
  }

  if (status === HTTP_STATUS.NOT_FOUND && !IS_DEV) {
    console.warn('资源不存在:', error.config?.url)
    return
  }

  if (showError) {
    ElMessage.error(message)
  }
}

export function createAxios(config?: AxiosRequestConfig): AxiosInstance {
  const instance = axios.create({
    ...defaultConfig,
    ...config,
  })

  instance.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      const authStore = useAuthStore()
      if (authStore.token) {
        config.headers.Authorization = `Bearer ${authStore.token}`
      }
      return config
    },
    error => Promise.reject(error)
  )

  instance.interceptors.response.use(
    (response: AxiosResponse) => response,
    (error: AxiosError) => {
      const config = error.config as RequestConfig
      handleError(error, config?.showError !== false)
      return Promise.reject(error)
    }
  )

  return instance
}

const request = createAxios()

export async function get<T = unknown>(url: string, config?: RequestConfig): Promise<T> {
  return await request.get(url, config).then(response => response.data)
}

export async function post<T = unknown>(url: string, data?: unknown, config?: RequestConfig): Promise<T> {
  return await request.post(url, data, config).then(response => response.data)
}

export async function put<T = unknown>(url: string, data?: unknown, config?: RequestConfig): Promise<T> {
  return await request.put(url, data, config).then(response => response.data)
}

export async function patch<T = unknown>(url: string, data?: unknown, config?: RequestConfig): Promise<T> {
  return await request.patch(url, data, config).then(response => response.data)
}

export async function del<T = unknown>(url: string, config?: RequestConfig): Promise<T> {
  return await request.delete(url, config).then(response => response.data)
}
