<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../stores/user'
import { useNotificationsStore } from '../stores/notifications'
import { setLocale, SUPPORTED_LOCALES, type LocaleCode } from '../i18n'
import { X, Home, Sun, Leaf, Sprout, MessageCircle, Bell, User, LogOut, Languages, ShieldCheck } from 'lucide-vue-next'

const router = useRouter()
const { t, locale } = useI18n()
const user = useUserStore()
const notifications = useNotificationsStore()
const emit = defineEmits(['close'])

async function logout() {
  user.logout()
  emit('close')
  try {
    await router.replace({ name: 'login' })
  } catch {
    // Fallback if the router navigation is interrupted for any reason.
    window.location.assign(`${import.meta.env.BASE_URL}#/login`)
  }
}

function changeLanguage(code: LocaleCode) {
  setLocale(code)
}
</script>

<template>
  <aside class="relative w-[260px] h-full bg-surface border-r border-border flex flex-col py-8">
    <!-- Mobile close button -->
    <button
      @click="emit('close')"
      class="lg:hidden absolute top-4 right-4 p-1.5 rounded-lg text-text-muted hover:bg-bg-app transition-colors"
    >
      <X width="20" height="20" />
    </button>

    <div class="px-8 mb-10">
      <div class="flex items-center gap-3">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="#1a5641" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/></svg>
        <span class="text-[1.25rem] font-bold text-primary">Verd</span>
      </div>
    </div>

    <nav>
      <ul class="flex flex-col gap-2 px-5">
        <li>
          <router-link
            to="/"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Home class="shrink-0" width="24" height="24" />
            {{ t('nav.dashboard') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/weather"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Sun class="shrink-0" width="24" height="24" />
            {{ t('nav.weather') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/plants"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Sprout class="shrink-0" width="24" height="24" />
            {{ t('nav.plants') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/recommendation"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Leaf class="shrink-0" width="24" height="24" />
            {{ t('nav.recommendation') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/chat"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <MessageCircle class="shrink-0" width="24" height="24" />
            {{ t('nav.chat') }}
          </router-link>
        </li>
        <li>
          <router-link
            to="/notifications"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <Bell class="shrink-0" width="24" height="24" />
            {{ t('nav.notifications') }}
            <span
              v-if="notifications.unreadCount > 0"
              class="ml-auto min-w-[20px] h-5 px-1.5 rounded-full bg-success-green text-white text-[0.7rem] font-bold flex items-center justify-center"
            >{{ notifications.unreadCount }}</span>
          </router-link>
        </li>
        <li>
          <router-link
            to="/profile"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <User class="shrink-0" width="24" height="24" />
            {{ t('nav.profile') }}
          </router-link>
        </li>
        <li v-if="user.isAdmin">
          <router-link
            to="/admin"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
            @click="emit('close')"
          >
            <ShieldCheck class="shrink-0" width="24" height="24" />
            {{ t('nav.admin') }}
          </router-link>
        </li>
      </ul>
    </nav>

    <div class="flex-1"></div>

    <!-- Language switcher -->
    <div class="px-5 mb-2">
      <div class="flex items-center gap-2 px-5 mb-2 text-text-light">
        <Languages class="shrink-0" width="16" height="16" />
        <span class="text-[0.7rem] font-bold tracking-[0.5px] uppercase">{{ t('language.label') }}</span>
      </div>
      <div class="flex gap-1 bg-bg-app rounded-lg p-1">
        <button
          v-for="loc in SUPPORTED_LOCALES"
          :key="loc.code"
          @click="changeLanguage(loc.code)"
          class="flex-1 py-2 rounded-md text-[0.8rem] font-semibold transition-colors"
          :class="locale === loc.code
            ? 'bg-surface text-primary shadow-sm'
            : 'text-text-muted hover:text-text-main'"
        >
          {{ loc.short }}
        </button>
      </div>
    </div>

    <div class="px-5">
      <button @click="logout" class="w-full flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main">
        <LogOut class="shrink-0" width="24" height="24" />
        {{ t('nav.logout') }}
      </button>
    </div>
  </aside>
</template>
