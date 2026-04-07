import type { Directive } from 'vue'

interface LoadingOptions {
  text?: string
  background?: string
}

export const loading: Directive<HTMLElement, boolean | LoadingOptions> = {
  mounted(el, binding) {
    updateLoading(el, binding.value)
  },
  updated(el, binding) {
    updateLoading(el, binding.value)
  },
}

function getCssVar(name: string): string {
  return getComputedStyle(document.documentElement).getPropertyValue(name).trim()
}

function updateLoading(el: HTMLElement, value: boolean | LoadingOptions) {
  const isLoading = typeof value === 'boolean' ? value : true
  const options = typeof value === 'object' ? value : {}

  if (isLoading) {
    el.style.position = 'relative'

    const mask = document.createElement('div')
    mask.className = 'v-loading-mask'

    const bgColor = options.background || getCssVar('--el-mask-color') || 'rgba(255, 255, 255, 0.9)'
    mask.style.cssText = `
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: ${bgColor};
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    `

    const spinner = document.createElement('div')
    spinner.className = 'v-loading-spinner'

    const primaryColor = getCssVar('--el-color-primary') || '#409eff'
    spinner.innerHTML = `
      <svg class="circular" viewBox="25 25 50 50" style="width: 42px; height: 42px;">
        <circle class="path" cx="50" cy="50" r="20" fill="none" stroke="${primaryColor}" stroke-width="3" stroke-linecap="round"/>
      </svg>
    `

    mask.appendChild(spinner)

    if (options.text) {
      const text = document.createElement('p')
      text.className = 'v-loading-text'
      text.textContent = options.text
      text.style.cssText = `margin-top: 8px; color: ${primaryColor};`
      mask.appendChild(text)
    }

    el.appendChild(mask)
  } else {
    const mask = el.querySelector('.v-loading-mask')
    mask?.remove()
  }
}
