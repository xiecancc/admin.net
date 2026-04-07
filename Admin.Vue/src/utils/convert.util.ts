/**
 * 数据转换工具函数
 */

/**
 * 转换环境变量为布尔值
 * @param value 环境变量值
 * @returns 布尔值
 */
export function toBoolean(value: string | undefined): boolean {
  if (value === undefined) {
    return false
  }
  const lowerValue = value.toLowerCase().trim()
  return lowerValue === 'true' || lowerValue === '1' || lowerValue === 'yes' || lowerValue === 'y'
}

/**
 * 转换环境变量为数字
 * @param value 环境变量值
 * @param defaultValue 默认值
 * @returns 数字
 */
export function toNumber(value: string | undefined, defaultValue: number): number {
  if (value === undefined) {
    return defaultValue
  }
  const num = Number(value)
  return isNaN(num) ? defaultValue : num
}

/**
 * 转换环境变量为字符串
 * @param value 环境变量值
 * @param defaultValue 默认值
 * @returns 字符串
 */
export function toString(value: string | undefined, defaultValue: string): string {
  return value || defaultValue
}
