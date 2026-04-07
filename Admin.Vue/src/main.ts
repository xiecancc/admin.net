import "normalize.css";
import "element-plus/theme-chalk/dark/css-vars.css";

import { createApp } from "vue";
import icons from "./plugins/icons.plugin";
import pinia from "./plugins/pinia.plugin";
import { setupDirectives } from "./directives";

import App from "./app.vue";
import router from "./router";

const app = createApp(App);

app.use(pinia);
app.use(router);
app.use(icons);

setupDirectives(app);

app.mount("#app");
