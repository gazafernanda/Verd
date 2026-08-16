<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../stores/user'
import GoogleSignInButton from '../components/Auth/GoogleSignInButton.vue'
import { TriangleAlert, Eye, EyeOff, RefreshCw, Check, Leaf } from 'lucide-vue-next'

const router = useRouter()
const { t } = useI18n()
const user = useUserStore()

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const loading = ref(false)
const error = ref('')

const features = computed(() => [
  t('auth.features.weather'),
  t('auth.features.schedules'),
  t('auth.features.ai'),
])

async function submit() {
  error.value = ''
  loading.value = true
  try {
    await user.login(email.value, password.value)
    router.replace({ name: 'dashboard' })
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : t('auth.somethingWrong')
  } finally {
    loading.value = false
  }
}

function onGoogleSuccess() {
  error.value = ''
  router.replace({ name: 'dashboard' })
}
</script>

<template>
  <div class="min-h-screen flex">
    <!-- Left panel -->
    <div class="hidden lg:flex lg:w-[45%] bg-primary flex-col justify-between p-12 relative overflow-hidden">
      <!-- Background pattern -->
      <div class="absolute inset-0 overflow-hidden pointer-events-none">
        <div class="absolute -top-24 -left-24 w-96 h-96 rounded-full bg-white opacity-[0.03]"></div>
        <div class="absolute top-1/3 -right-32 w-[500px] h-[500px] rounded-full bg-white opacity-[0.04]"></div>
        <div class="absolute -bottom-20 left-1/4 w-72 h-72 rounded-full bg-accent-green opacity-[0.15]"></div>
      </div>

      <!-- Logo -->
      <div class="flex items-center gap-3 relative z-10">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256">
          <path fill="#ffffff" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/>
        </svg>
        <span class="text-2xl font-bold text-white">Verd</span>
      </div>

      <!-- Center content -->
      <div class="relative z-10">
        <div class="w-16 h-16 bg-white bg-opacity-10 rounded-2xl flex items-center justify-center mb-8">
          <Leaf width="32" height="32" color="#ffffff" />
        </div>
        <h1 class="text-4xl font-extrabold text-white leading-tight mb-4 whitespace-pre-line">{{ t('auth.heroTitle') }}</h1>
        <p class="text-white text-opacity-70 text-lg leading-relaxed max-w-sm" style="color: rgba(255,255,255,0.7)">
          {{ t('auth.heroSubtitle') }}
        </p>

        <!-- Feature bullets -->
        <div class="mt-10 flex flex-col gap-4">
          <div v-for="item in features" :key="item"
               class="flex items-center gap-3">
            <div class="w-5 h-5 rounded-full bg-accent-green flex items-center justify-center shrink-0">
              <Check width="10" height="10" color="white" stroke-width="2.5" />
            </div>
            <span class="text-sm font-medium" style="color: rgba(255,255,255,0.85)">{{ item }}</span>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <p class="text-xs relative z-10" style="color: rgba(255,255,255,0.4)">{{ t('auth.copyright') }}</p>
    </div>

    <!-- Right panel -->
    <div class="flex-1 flex items-center justify-center px-8 max-sm:px-5 py-12 bg-bg-app">
      <div class="w-full max-w-[420px]">
        <!-- Mobile logo -->
        <div class="flex items-center gap-2 mb-10 lg:hidden">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256">
            <path fill="#1a5641" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/>
          </svg>
          <span class="text-xl font-bold text-primary">Verd</span>
        </div>

        <h2 class="text-[2rem] max-sm:text-[1.6rem] font-extrabold text-text-main mb-2 tracking-tight">{{ t('auth.login.title') }}</h2>
        <p class="text-text-muted mb-8">{{ t('auth.login.subtitle') }}</p>

        <!-- Error -->
        <div v-if="error" class="mb-6 rounded-xl border border-red-200 bg-red-50 px-4 py-3 flex items-start gap-2 text-sm font-medium text-red-600">
          <TriangleAlert width="16" height="16" class="shrink-0 mt-px" />
          {{ error }}
        </div>

        <!-- Google sign-in (hidden when the server has no client id configured) -->
        <div class="mb-6">
          <GoogleSignInButton :disabled="loading" @success="onGoogleSuccess" @error="error = $event" />
        </div>

        <form @submit.prevent="submit" class="flex flex-col gap-5">
          <!-- Email -->
          <div class="flex flex-col gap-2">
            <label class="text-sm font-bold text-text-main">{{ t('auth.emailLabel') }}</label>
            <input
              v-model="email"
              type="email"
              :placeholder="t('auth.emailPlaceholder')"
              required
              autocomplete="email"
              class="px-4 py-3.5 rounded-xl border border-border bg-surface text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-accent-green focus:shadow-[0_0_0_3px_rgba(41,156,119,0.12)]"
            />
          </div>

          <!-- Password -->
          <div class="flex flex-col gap-2">
            <div class="flex justify-between items-center gap-3">
              <label class="text-sm font-bold text-text-main">{{ t('auth.passwordLabel') }}</label>
              <router-link
                to="/forgot-password"
                class="text-[0.8rem] font-semibold text-accent-green hover:text-accent-green-hover transition-colors"
              >
                {{ t('auth.forgot.link') }}
              </router-link>
            </div>
            <div class="relative">
              <input
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="••••••••"
                required
                autocomplete="current-password"
                class="w-full px-4 py-3.5 pr-12 rounded-xl border border-border bg-surface text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-accent-green focus:shadow-[0_0_0_3px_rgba(41,156,119,0.12)]"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute right-4 top-1/2 -translate-y-1/2 text-text-light hover:text-text-muted transition-colors"
              >
                <Eye v-if="!showPassword" width="18" height="18" />
                <EyeOff v-else width="18" height="18" />
              </button>
            </div>
          </div>

          <!-- Submit -->
          <button
            type="submit"
            :disabled="loading"
            class="mt-2 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] shadow-[0_4px_12px_rgba(41,156,119,0.3)] transition-all duration-200 hover:bg-accent-green-hover hover:-translate-y-px disabled:opacity-60 disabled:cursor-not-allowed disabled:translate-y-0 flex items-center justify-center gap-2"
          >
            <RefreshCw v-if="loading" class="animate-spin" width="18" height="18" />
            {{ loading ? t('auth.login.submitting') : t('auth.login.submit') }}
          </button>
        </form>

        <p class="mt-8 text-center text-sm text-text-muted">
          {{ t('auth.login.noAccount') }}
          <router-link to="/register" class="font-bold text-accent-green hover:text-accent-green-hover transition-colors">
            {{ t('auth.login.createOne') }}
          </router-link>
        </p>
      </div>
    </div>
  </div>
</template>
