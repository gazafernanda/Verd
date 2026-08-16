<script setup lang="ts">
/**
 * Sets a new password from an emailed reset link.
 *
 * The token is checked before the form is shown so a dead link fails immediately
 * instead of after the user has typed a password twice. Strength and confirmation
 * are validated here for fast feedback and again on the server, which is the
 * only check that actually counts.
 */
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../stores/user'
import AuthCard from '../components/Auth/AuthCard.vue'
import { LockKeyhole, CircleCheck, TriangleAlert, RefreshCw, Eye, EyeOff } from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const user = useUserStore()

const token = computed(() => {
  const raw = route.query.token
  return typeof raw === 'string' ? raw : ''
})

type State = 'checking' | 'ready' | 'invalid' | 'done'
const state = ref<State>('checking')

const password = ref('')
const confirmPassword = ref('')
const showPassword = ref(false)
const loading = ref(false)
const error = ref('')

// Mirrors the server's rule: at least 8 characters, with a letter and a digit.
const rules = computed(() => [
  { key: 'length', ok: password.value.length >= 8, label: t('auth.reset.ruleLength') },
  { key: 'letter', ok: /[A-Za-z]/.test(password.value), label: t('auth.reset.ruleLetter') },
  { key: 'digit', ok: /[0-9]/.test(password.value), label: t('auth.reset.ruleDigit') },
])

const strong = computed(() => rules.value.every((r) => r.ok))
const matches = computed(() => password.value.length > 0 && password.value === confirmPassword.value)
const canSubmit = computed(() => strong.value && matches.value && !loading.value)

async function submit() {
  if (!canSubmit.value) return
  error.value = ''
  loading.value = true
  try {
    await user.resetPassword(token.value, password.value, confirmPassword.value)
    // The old session (if any) was minted before the password changed.
    user.logout()
    state.value = 'done'
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : t('auth.somethingWrong')
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  if (!token.value) {
    state.value = 'invalid'
    return
  }
  state.value = (await user.isResetTokenValid(token.value)) ? 'ready' : 'invalid'
})
</script>

<template>
  <AuthCard>
    <!-- Checking the link -->
    <div v-if="state === 'checking'" class="flex flex-col items-center text-center gap-4">
      <RefreshCw class="animate-spin text-accent-green" width="32" height="32" />
      <h1 class="text-[1.3rem] font-extrabold text-text-main">{{ t('auth.reset.checking') }}</h1>
    </div>

    <!-- Used, expired, or forged link -->
    <div v-else-if="state === 'invalid'" class="flex flex-col items-center text-center gap-4">
      <div class="w-14 h-14 rounded-full bg-red-50 flex items-center justify-center">
        <TriangleAlert class="text-red-500" width="28" height="28" />
      </div>
      <div>
        <h1 class="text-[1.3rem] font-extrabold text-text-main mb-2">{{ t('auth.reset.invalidTitle') }}</h1>
        <p class="text-[0.9rem] text-text-muted leading-relaxed">{{ t('auth.reset.invalidDesc') }}</p>
      </div>
      <router-link
        to="/forgot-password"
        class="mt-1 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] transition-all duration-200 hover:bg-accent-green-hover text-center"
      >
        {{ t('auth.reset.requestNew') }}
      </router-link>
    </div>

    <!-- Done -->
    <div v-else-if="state === 'done'" class="flex flex-col items-center text-center gap-4">
      <div class="w-14 h-14 rounded-full bg-light-green-bg flex items-center justify-center">
        <CircleCheck class="text-success-green" width="28" height="28" />
      </div>
      <div>
        <h1 class="text-[1.3rem] font-extrabold text-text-main mb-2">{{ t('auth.reset.doneTitle') }}</h1>
        <p class="text-[0.9rem] text-text-muted leading-relaxed">{{ t('auth.reset.doneDesc') }}</p>
      </div>
      <button
        @click="router.replace({ name: 'login' })"
        class="mt-1 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] transition-all duration-200 hover:bg-accent-green-hover"
      >
        {{ t('auth.reset.goToLogin') }}
      </button>
    </div>

    <!-- New password form -->
    <div v-else>
      <div class="w-14 h-14 rounded-full bg-light-green-bg flex items-center justify-center mb-5">
        <LockKeyhole class="text-success-green" width="26" height="26" />
      </div>

      <h1 class="text-[1.5rem] font-extrabold text-text-main mb-2 tracking-tight">{{ t('auth.reset.title') }}</h1>
      <p class="text-[0.9rem] text-text-muted mb-6 leading-relaxed">{{ t('auth.reset.subtitle') }}</p>

      <div
        v-if="error"
        class="mb-5 rounded-xl border border-red-200 bg-red-50 px-4 py-3 flex items-start gap-2 text-sm font-medium text-red-600"
      >
        <TriangleAlert width="16" height="16" class="shrink-0 mt-px" />
        {{ error }}
      </div>

      <form @submit.prevent="submit" class="flex flex-col gap-5">
        <!-- New password -->
        <div class="flex flex-col gap-2">
          <label class="text-sm font-bold text-text-main">{{ t('auth.reset.newPassword') }}</label>
          <div class="relative">
            <input
              v-model="password"
              :type="showPassword ? 'text' : 'password'"
              placeholder="••••••••"
              required
              autocomplete="new-password"
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

          <!-- Strength checklist -->
          <ul class="flex flex-col gap-1 mt-1">
            <li
              v-for="rule in rules"
              :key="rule.key"
              class="flex items-center gap-2 text-[0.8rem] transition-colors"
              :class="rule.ok ? 'text-success-green font-semibold' : 'text-text-light'"
            >
              <CircleCheck width="13" height="13" :class="rule.ok ? '' : 'opacity-40'" />
              {{ rule.label }}
            </li>
          </ul>
        </div>

        <!-- Confirm -->
        <div class="flex flex-col gap-2">
          <label class="text-sm font-bold text-text-main">{{ t('auth.reset.confirmPassword') }}</label>
          <input
            v-model="confirmPassword"
            :type="showPassword ? 'text' : 'password'"
            placeholder="••••••••"
            required
            autocomplete="new-password"
            class="w-full px-4 py-3.5 rounded-xl border bg-surface text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:shadow-[0_0_0_3px_rgba(41,156,119,0.12)]"
            :class="confirmPassword && !matches ? 'border-red-300 focus:border-red-400' : 'border-border focus:border-accent-green'"
          />
          <p v-if="confirmPassword && !matches" class="text-[0.8rem] font-medium text-red-500">
            {{ t('auth.reset.mismatch') }}
          </p>
        </div>

        <button
          type="submit"
          :disabled="!canSubmit"
          class="w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] shadow-[0_4px_12px_rgba(41,156,119,0.3)] transition-all duration-200 hover:bg-accent-green-hover hover:-translate-y-px disabled:opacity-60 disabled:cursor-not-allowed disabled:translate-y-0 disabled:shadow-none flex items-center justify-center gap-2"
        >
          <RefreshCw v-if="loading" class="animate-spin" width="18" height="18" />
          {{ loading ? t('auth.reset.submitting') : t('auth.reset.submit') }}
        </button>
      </form>
    </div>
  </AuthCard>
</template>
