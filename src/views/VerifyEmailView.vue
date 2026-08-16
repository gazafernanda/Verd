<script setup lang="ts">
/**
 * Handles both halves of the verification flow:
 *  - arriving from the emailed link (?token=…) → verify and report the outcome
 *  - arriving straight after registering (no token) → explain and offer a resend
 */
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../stores/user'
import AuthCard from '../components/Auth/AuthCard.vue'
import { MailCheck, CircleCheck, TriangleAlert, RefreshCw, Mail } from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const user = useUserStore()

type State = 'pending' | 'verifying' | 'verified' | 'failed'

const token = computed(() => {
  const raw = route.query.token
  return typeof raw === 'string' ? raw : ''
})

const state = ref<State>('pending')
const errorMessage = ref('')
const notice = ref('')
const resending = ref(false)

// Server-enforced cooldown, mirrored here so the button shows why it's disabled.
const cooldown = ref(0)
let cooldownTimer: ReturnType<typeof setInterval> | undefined

function startCooldown(seconds: number) {
  cooldown.value = seconds
  clearInterval(cooldownTimer)
  cooldownTimer = setInterval(() => {
    cooldown.value -= 1
    if (cooldown.value <= 0) clearInterval(cooldownTimer)
  }, 1000)
}

async function verify() {
  state.value = 'verifying'
  try {
    await user.verifyEmail(token.value)
    state.value = 'verified'
  } catch (e: unknown) {
    errorMessage.value = e instanceof Error ? e.message : t('auth.somethingWrong')
    state.value = 'failed'
  }
}

async function resend() {
  if (resending.value || cooldown.value > 0) return
  resending.value = true
  notice.value = ''
  try {
    const result = await user.resendVerification()
    notice.value = result.message
    // 60s matches the server's cooldown; the response carries the real remainder.
    startCooldown(result.retryAfter ?? 60)
  } catch {
    notice.value = t('auth.somethingWrong')
  } finally {
    resending.value = false
  }
}

function goToApp() {
  router.replace({ name: 'dashboard' })
}

onMounted(() => {
  if (token.value) verify()
})

onUnmounted(() => clearInterval(cooldownTimer))
</script>

<template>
  <AuthCard>
    <!-- Verifying the emailed token -->
    <div v-if="state === 'verifying'" class="flex flex-col items-center text-center gap-4">
      <RefreshCw class="animate-spin text-accent-green" width="32" height="32" />
      <h1 class="text-[1.3rem] font-extrabold text-text-main">{{ t('auth.verify.verifying') }}</h1>
    </div>

    <!-- Success -->
    <div v-else-if="state === 'verified'" class="flex flex-col items-center text-center gap-4">
      <div class="w-14 h-14 rounded-full bg-light-green-bg flex items-center justify-center">
        <CircleCheck class="text-success-green" width="28" height="28" />
      </div>
      <div>
        <h1 class="text-[1.3rem] font-extrabold text-text-main mb-2">{{ t('auth.verify.successTitle') }}</h1>
        <p class="text-[0.9rem] text-text-muted leading-relaxed">{{ t('auth.verify.successDesc') }}</p>
      </div>
      <button
        @click="goToApp"
        class="mt-2 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] shadow-[0_4px_12px_rgba(41,156,119,0.3)] transition-all duration-200 hover:bg-accent-green-hover"
      >
        {{ t('auth.verify.continue') }}
      </button>
    </div>

    <!-- Invalid or expired link -->
    <div v-else-if="state === 'failed'" class="flex flex-col items-center text-center gap-4">
      <div class="w-14 h-14 rounded-full bg-red-50 flex items-center justify-center">
        <TriangleAlert class="text-red-500" width="28" height="28" />
      </div>
      <div>
        <h1 class="text-[1.3rem] font-extrabold text-text-main mb-2">{{ t('auth.verify.failedTitle') }}</h1>
        <p class="text-[0.9rem] text-text-muted leading-relaxed">{{ errorMessage }}</p>
      </div>

      <p v-if="notice" class="text-[0.85rem] text-success-green font-medium">{{ notice }}</p>

      <button
        v-if="user.isAuthenticated"
        @click="resend"
        :disabled="resending || cooldown > 0"
        class="mt-1 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] transition-all duration-200 hover:bg-accent-green-hover disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2"
      >
        <RefreshCw v-if="resending" class="animate-spin" width="18" height="18" />
        {{ cooldown > 0 ? t('auth.verify.resendIn', { seconds: cooldown }) : t('auth.verify.resend') }}
      </button>

      <router-link v-else to="/login" class="text-[0.9rem] font-bold text-accent-green hover:text-accent-green-hover">
        {{ t('auth.verify.backToLogin') }}
      </router-link>
    </div>

    <!-- Just registered: waiting for the user to open their inbox -->
    <div v-else class="flex flex-col items-center text-center gap-4">
      <div class="w-14 h-14 rounded-full bg-light-green-bg flex items-center justify-center">
        <MailCheck class="text-success-green" width="28" height="28" />
      </div>
      <div>
        <h1 class="text-[1.3rem] font-extrabold text-text-main mb-2">{{ t('auth.verify.sentTitle') }}</h1>
        <p class="text-[0.9rem] text-text-muted leading-relaxed">
          {{ t('auth.verify.sentDesc') }}
        </p>
        <p v-if="user.email" class="mt-3 inline-flex items-center gap-2 px-3 py-1.5 rounded-lg bg-bg-app text-[0.85rem] font-semibold text-text-main">
          <Mail width="14" height="14" class="text-text-muted" />
          {{ user.email }}
        </p>
      </div>

      <p v-if="notice" class="text-[0.85rem] text-success-green font-medium">{{ notice }}</p>

      <button
        @click="resend"
        :disabled="resending || cooldown > 0"
        class="mt-1 w-full py-3.5 bg-accent-green text-white rounded-xl font-bold text-[0.95rem] transition-all duration-200 hover:bg-accent-green-hover disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2"
      >
        <RefreshCw v-if="resending" class="animate-spin" width="18" height="18" />
        {{ cooldown > 0 ? t('auth.verify.resendIn', { seconds: cooldown }) : t('auth.verify.resend') }}
      </button>

      <p class="text-[0.8rem] text-text-light leading-relaxed">{{ t('auth.verify.spamHint') }}</p>
    </div>

    <template #footer>
      <router-link
        v-if="state !== 'verified'"
        to="/login"
        class="text-[0.85rem] font-semibold text-text-muted hover:text-text-main transition-colors"
      >
        {{ t('auth.verify.backToLogin') }}
      </router-link>
    </template>
  </AuthCard>
</template>
