import type { MessageParams } from 'element-plus'

export function useMessage() {
  const showMessage = (options: MessageParams) => ElMessage(options)

  const success = (message: string) => ElMessage.success(message)

  const error = (message: string) => ElMessage.error(message)

  const warning = (message: string) => ElMessage.warning(message)

  const info = (message: string) => ElMessage.info(message)

  return {
    error,
    info,
    showMessage,
    success,
    warning,
  }
}
