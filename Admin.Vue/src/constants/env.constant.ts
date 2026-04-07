import { toBoolean, toNumber, toString } from "@/utils/convert.util";

export const APP = {
  BASE_URL: toString(import.meta.env.VITE_BASE_URL, "/"),
  ENV: toString(import.meta.env.NODE_ENV, "development"),
  PORT: toNumber(import.meta.env.VITE_SERVER_PORT, 5000),
  TITLE: toString(import.meta.env.VITE_APP_TITLE, "Fund Admin"),
  VERSION: toString(import.meta.env.VITE_APP_VERSION, "1.0.0"),
} as const;

export const API = {
  BASE_URL: toString(import.meta.env.VITE_API_BASE_URL, "http://localhost:5154"),
  TIMEOUT: toNumber(import.meta.env.VITE_API_TIMEOUT, 30_000),
} as const;

export const DEBUG = {
  MODE: toBoolean(import.meta.env.VITE_DEBUG_MODE),
  SHOW_CONSOLE_LOG: toBoolean(import.meta.env.VITE_SHOW_CONSOLE_LOG),
} as const;

export const BUILD = {
  ANALYZE: toBoolean(import.meta.env.VITE_BUILD_ANALYZE),
  COMPRESS: toBoolean(import.meta.env.VITE_BUILD_COMPRESS),
} as const;

export const IS_DEV = APP.ENV === "development";
export const IS_PROD = APP.ENV === "production";
export const IS_TEST = APP.ENV === "test";

export const API_BASE_URL = API.BASE_URL;
export const API_TIMEOUT = API.TIMEOUT;
