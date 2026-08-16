<script setup lang="ts">
/**
 * Requests a password reset link.
 *
 * The success screen is shown whether or not the address is registered — the
 * server returns the same neutral response either way, and the UI must not leak
 * what the API deliberately hides.
 */
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../stores/user'
import AuthCard from '../components/Auth/AuthCard.vue'
import { KeyRound, MailCheck, TriangleAlert, RefreshCw, ArrowLeft } from 'lucide-vue-next'

const { t } = useI18n()
const user = useUserStore()

const email = ref('')
const loading = ref(false)
const error = ref('')
const sent = ref(false)
const serverMessage = ref('')

async function submit() {
  if (loading.value) return
  error.value = ''
  loading.value = true
  try {
    serverMessage.value = await user.forgotPassword(email.value)
    sent.value = true
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : t('auth.somethingWrong')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <AuthCard>
    <!-- Neutral confirmation -->
    <div v-if="sent" class="flex flex-col items-center text-center gap-4">
      <div class="w-14 h-14 rounded-full bg-light-green-bg flex items-center justify-center">
        <MailCheck class="text-success-green" width="28" height="28" />
      </div>
      <div>
        <h1 class="text-[1.3rem] font-extrabold text-text-main mb-2">{{ t('auth.forgot.sentTitle') }}</h1>
        <p class="text-[0.9rem] text-text-muted leading-relaxed">
          {{ serverMessage || t('auth.forgot.sentDesc') }}
        </p>
      </div>
      <p class="text-[0.8rem] text-text-light leading-relaxed">{{ t('auth.forgot.spamHint') }}</p>
      <router-link
        to="/login"
        class="mt-1 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] transition-all duration-200 hover:bg-accent-green-hover text-center"
      >
        {{ t('auth.verify.backToLogin') }}
      </router-link>
    </div>

    <!-- Request form -->
    <div v-else>
      <div class="w-14 h-14 rounded-full bg-light-green-bg flex items-center justify-center mb-5">
        <KeyRound class="text-success-green" width="26" height="26" />
      </div>

      <h1 class="text-[1.5rem] font-extrabold text-text-main mb-2 tracking-tight">{{ t('auth.forgot.title') }}</h1>
      <p class="text-[0.9rem] text-text-muted mb-6 leading-relaxed">{{ t('auth.forgot.subtitle') }}</p>

      <div
        v-if="error"
        class="mb-5 rounded-xl border border-red-200 bg-red-50 px-4 py-3 flex items-start gap-2 text-sm font-medium text-red-600"
      >
        <TriangleAlert width="16" height="16" class="shrink-0 mt-px" />
        {{ error }}
      </div>

      <form @submit.prevent="submit" class="flex flex-col gap-5">
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

        <button
          type="submit"
          :disabled="loading"
          class="w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] shadow-[0_4px_12px_rgba(41,156,119,0.3)] transition-all duration-200 hover:bg-accent-green-hover hover:-translate-y-px disabled:opacity-60 disabled:cursor-not-allowed disabled:translate-y-0 flex items-center justify-center gap-2"
        >
          <RefreshCw v-if="loading" class="animate-spin" width="18" height="18" />
          {{ loading ? t('auth.forgot.submitting') : t('auth.forgot.submit') }}
        </button>
      </form>
    </div>

    <template #footer>
      <router-link
        v-if="!sent"
        to="/login"
        class="inline-flex items-center gap-1.5 text-[0.85rem] font-semibold text-text-muted hover:text-text-main transition-colors"
      >
        <ArrowLeft width="14" height="14" />
        {{ t('auth.verify.backToLogin') }}
      </router-link>
    </template>
  </AuthCard>
</template>
