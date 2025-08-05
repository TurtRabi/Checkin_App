import js from "@eslint/js";
import pluginVue from "eslint-plugin-vue";
import prettierConfig from "eslint-config-prettier";
import globals from "globals";

export default [
  js.configs.recommended,

  // Đây là bộ quy tắc được đề xuất, ổn định cho Vue 3
  ...pluginVue.configs['flat/recommended'],

  // Đặt cuối cùng để ghi đè các quy tắc khác
  prettierConfig,

  {
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
      }
    },
    rules: {
      // Nơi để thêm các quy tắc tùy chỉnh sau này
    }
  }
];
