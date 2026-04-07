import { defineConfig } from 'oxlint'

const prod = process.env.NODE_ENV === 'production'

export default defineConfig({
  categories: {
    correctness: 'error',
    nursery: 'error',
    perf: 'error',
    style: prod ? 'error' : 'warn',
    suspicious: 'error',
  },
  env: {
    browser: true,
    es2024: true,
    node: true,
  },
  globals: {
    defineEmits: 'readonly',
    defineExpose: 'readonly',
    defineProps: 'readonly',
    withDefaults: 'readonly',
    ElMessage: 'readonly',
    ElMessageBox: 'readonly',
    ElLoading: 'readonly',
  },
  ignorePatterns: ['node_modules', 'dist', 'coverage', '*.d.ts', '*.min.js', 'public'],
  plugins: ['typescript', 'unicorn', 'oxc', 'vue', 'import', 'react'],
  rules: {
    // 所有环境都保持 error 的规则（核心正确性规则）
    'no-debugger': 'error',
    'typescript/no-unused-vars': ['error', { argsIgnorePattern: '^_', varsIgnorePattern: '^_' }],
    'typescript/no-import-type-side-effects': 'error',
    'typescript/no-unnecessary-type-constraint': 'error',
    'typescript/no-require-imports': 'error',

    // 开发模式为 warn，生产模式为 error 的规则
    'no-console': prod ? 'error' : 'warn',
    'typescript/no-explicit-any': prod ? 'error' : 'warn',

    // 代码风格相关 - 开发模式 warn，生产模式 error
    'unicorn/prefer-array-some': prod ? 'error' : 'warn',
    'unicorn/prefer-array-find': prod ? 'error' : 'warn',
    'unicorn/prefer-includes': prod ? 'error' : 'warn',
    'unicorn/prefer-optional-catch-binding': prod ? 'error' : 'warn',
    'unicorn/prefer-modern-math-apis': prod ? 'error' : 'warn',
    'unicorn/prefer-number-properties': prod ? 'error' : 'warn',
    'unicorn/prefer-string-starts-ends-with': prod ? 'error' : 'warn',
    'unicorn/prefer-string-trim-start-end': prod ? 'error' : 'warn',
    'unicorn/prefer-structured-clone': prod ? 'error' : 'warn',
    'unicorn/prefer-blob-reading-methods': prod ? 'error' : 'warn',
    'unicorn/no-useless-spread': prod ? 'error' : 'warn',
    'unicorn/no-useless-switch-case': prod ? 'error' : 'warn',
    'unicorn/no-useless-undefined': prod ? 'error' : 'warn',
    'unicorn/no-null': prod ? 'error' : 'warn',
    'unicorn/prefer-ternary': prod ? 'error' : 'warn',
    'unicorn/prefer-logical-operator-over-ternary': prod ? 'error' : 'warn',
    'unicorn/prefer-dom-node-text-content': prod ? 'error' : 'warn',
    'unicorn/prefer-dom-node-append': prod ? 'error' : 'warn',
    'unicorn/prefer-dom-node-remove': prod ? 'error' : 'warn',
    'unicorn/prefer-event-target': prod ? 'error' : 'warn',
    'vue/no-unused-vars': 'off',
    'import/no-duplicates': prod ? 'error' : 'warn',
    'import/order': prod ? 'error' : 'warn',
  },
})
