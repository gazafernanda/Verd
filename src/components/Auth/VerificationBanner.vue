<script setup lang="ts">
/**
 * Persistent reminder for a signed-in account that hasn't verified its email.
 * The router keeps these accounts off the core pages; this explains why and
 * offers the way out without making the user hunt for it.
 */
import { ref, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../../stores/user'
import { MailWarning, RefreshCw } from 'lucide-vue-next'

const { t } = useI18n()
const user = useUserStore()

const sending = ref(false)
const notice = ref('')

// Mirrors the server's resend cooldown so the button explains its own disabling.
const cooldown = ref(0)
let timer: ReturnType<typeof setInterval> | undefined

async function resend() {
  if (sending.value || cooldown.value > 0) return
  sending.value = true
  notice.value = ''
  try {
    const result = await user.resendVerification()
    notice.value = result.message
    cooldown.value = result.retryAfter ?? 60
    clearInterval(timer)
    timer = setInterval(() => {
      cooldown.value -= 1
      if (cooldown.value <= 0) clearInterval(timer)
    }, 1000)
  } catch {
    notice.value = t('auth.somethingWrong')
  } finally {
    sending.value = false
  }
}

onUnmounted(() => clearInterval(timer))
</script>

<template>
  <div class="rounded-2xl border border-[#fde3bd] bg-[#fff4e5] px-5 py-4 flex items-start gap-4 max-sm:flex-col max-sm:gap-3">
    <div class="w-10 h-10 rounded-full bg-[#fde3bd] flex items-center justify-center shrink-0">
      <MailWarning class="text-[#b45309]" width="20" height="20" />
    </div>

    <div class="flex-1 min-w-0">
      <p class="font-bold text-[#92400e] text-[0.95rem]">{{ t('auth.verify.bannerTitle') }}</p>
      <p class="text-[0.85rem] text-[#b45309] mt-0.5 leading-relaxed">
        {{ t('auth.verify.bannerDesc', { email: user.email }) }}
      </p>
      <p v-if="notice" class="text-[0.82rem] font-semibold text-[#92400e] mt-2">{{ notice }}</p>
    </div>

    <div class="flex items-center gap-2 shrink-0 max-sm:w-full">
      <router-link
        to="/verify-email"
        class="px-3.5 py-2 rounded-xl text-[0.82rem] font-bold text-[#92400e] hover:bg-white/60 transition-colors whitespace-nowrap"
      >
        {{ t('auth.verify.bannerOpen') }}
      </router-link>
      <button
        @click="resend"
        :disabled="sending || cooldown > 0"
        class="px-3.5 py-2 rounded-xl text-[0.82rem] font-bold bg-[#b45309] text-white hover:bg-[#92400e] transition-colors disabled:opacity-60 disabled:cursor-not-allowed flex items-center gap-1.5 whitespace-nowrap"
      >
        <RefreshCw v-if="sending" class="animate-spin" width="14" height="14" />
        {{ cooldown > 0 ? t('auth.verify.resendIn', { seconds: cooldown }) : t('auth.verify.bannerAction') }}
      </button>
    </div>
  </div>
</template>
