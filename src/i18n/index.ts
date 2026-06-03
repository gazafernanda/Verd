import { createI18n } from "vue-i18n";
import en from "./locales/en";
import id from "./locales/id";

export const SUPPORTED_LOCALES = [
  { code: "id", label: "Bahasa Indonesia", short: "ID" },
  { code: "en", label: "English", short: "EN" },
] as const;

export type LocaleCode = (typeof SUPPORTED_LOCALES)[number]["code"];

const STORAGE_KEY = "verd_locale";

function initialLocale(): LocaleCode {
  const stored = localStorage.getItem(STORAGE_KEY);
  if (stored === "en" || stored === "id") return stored;
  return "id"; // Indonesian by default
}

const i18n = createI18n({
  legacy: false,
  globalInjection: true,
  locale: initialLocale(),
  fallbackLocale: "en",
  messages: { en, id },
});

export function setLocale(locale: LocaleCode) {
  i18n.global.locale.value = locale;
  localStorage.setItem(STORAGE_KEY, locale);
  document.documentElement.lang = locale;
}

// Keep <html lang> in sync on first load.
document.documentElement.lang = i18n.global.locale.value;

export default i18n;
