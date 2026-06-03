import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  base: '/Verd/',
  // vue-i18n's esm-bundler build references these compile-time flags.
  // Without defining them they leak into the bundle as undefined globals
  // and crash the app at runtime (blank page in production).
  define: {
    __VUE_I18N_FULL_INSTALL__: true,
    __VUE_I18N_LEGACY_API__: false,
    __INTLIFY_DROP_MESSAGE_COMPILER__: false,
    __INTLIFY_JIT_COMPILATION__: true,
    __INTLIFY_PROD_DEVTOOLS__: false,
  },
  plugins: [
    vue(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  optimizeDeps: {
    include: ['vue', 'vue-router', 'pinia', 'marked'],
    exclude: ['vite-plugin-vue-devtools'],
  },
  server: {
    warmup: {
      clientFiles: [
        './src/main.ts',
        './src/App.vue',
        './src/views/DashboardView.vue',
        './src/components/Sidebar.vue',
      ],
    },
  },
})
