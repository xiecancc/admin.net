import { defineConfig } from "oxlint";

const prod = process.env.NODE_ENV === "production";

export default defineConfig({
  categories: {
    correctness: "error",
    nursery: "error",
    perf: "error",
    style: prod ? "error" : "warn",
    suspicious: "error",
  },
  env: {
    browser: true,
    es2024: true,
    node: true,
  },
  globals: {
    defineEmits: "readonly",
    defineExpose: "readonly",
    defineProps: "readonly",
    withDefaults: "readonly",
  },
  ignorePatterns: ["node_modules", "dist", "coverage", "*.d.ts", "*.min.js", "public"],
  plugins: ["typescript", "unicorn", "oxc", "vue", "import"],
  rules: {
    // 所有环境都保持 error 的规则（核心正确性规则）
    "no-unused-vars": "error",
    "no-debugger": "error",
    "typescript/no-unused-vars": ["error", { argsIgnorePattern: "^_", varsIgnorePattern: "^_" }],
    "vue/no-unused-vars": "error",
    semi: ["error", "never"],
    "import/first": "error",
    "import/newline-after-import": "error",

    // 开发模式为 warn，生产模式为 error 的规则
    "no-console": prod ? "error" : "warn",
    "typescript/no-explicit-any": prod ? "error" : "warn",
    "no-duplicate-imports": prod ? "error" : "warn",

    // 代码风格相关 - 开发模式 warn，生产模式 error
    "sort-imports": prod ? "error" : "warn",
    "sort-keys": prod ? "error" : "warn",
    "import/exports-last": prod ? "error" : "warn",
    "exports-last": prod ? "error" : "warn",
    "import/no-anonymous-default-export": prod ? "error" : "warn",
    "import/no-duplicates": prod ? "error" : "warn",
    "import/no-named-export": prod ? "error" : "warn",
    "import/prefer-default-export": prod ? "error" : "warn",
    "import/group-exports": prod ? "error" : "warn",
    "import/no-unassigned-import": prod ? "error" : "warn",
    "import/no-nodejs-modules": prod ? "error" : "warn",
    "import/no-named-as-default": prod ? "error" : "warn",
    "eslint/max-statements": prod ? "error" : "warn",
    "eslint/no-magic-numbers": prod ? "error" : "warn",
    "eslint/no-ternary": prod ? "error" : "warn",
    "eslint/no-shadow": prod ? "error" : "warn",
    "eslint/new-cap": prod ? "error" : "warn",
    "eslint/no-undef": prod ? "error" : "warn",
    "unicorn/no-null": prod ? "error" : "warn",
    "unicorn/prefer-global-this": prod ? "error" : "warn",
    "unicorn/filename-case": prod ? "error" : "warn",
    "unicorn/prefer-ternary": prod ? "error" : "warn",
    "unicorn/consistent-function-scoping": prod ? "error" : "warn",
    "func-style": prod ? "error" : "warn",
    "id-length": prod ? "error" : "warn",
    "no-shadow": prod ? "error" : "warn",
  },
});
