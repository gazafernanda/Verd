<script setup lang="ts">
/**
 * Renders Google's own "Sign in with Google" button via Google Identity Services.
 *
 * The script is loaded on demand rather than in index.html so the auth pages stay
 * the only place that pays for it. When the server has no client id configured the
 * component renders nothing at all — a dead button is worse than no button.
 */
import { ref, onMounted, useTemplateRef } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../../stores/user'

const props = defineProps<{ disabled?: boolean }>()
const emit = defineEmits<{ success: []; error: [message: string] }>()

const { t } = useI18n()
const user = useUserStore()

const container = useTemplateRef<HTMLDivElement>('container')
const enabled = ref(false)
const ready = ref(false)
const busy = ref(false)

const GSI_SRC = 'https://accounts.google.com/gsi/client'

interface GoogleCredentialResponse { credential?: string }

declare global {
  interface Window {
    google?: {
      accounts: {
        id: {
          initialize(config: {
            client_id: string
            callback: (response: GoogleCredentialResponse) => void
            auto_select?: boolean
          }): void
          renderButton(parent: HTMLElement, options: Record<string, unknown>): void
        }
      }
    }
  }
}

function loadScript(): Promise<void> {
  if (window.google?.accounts?.id) return Promise.resolve()

  return new Promise((resolve, reject) => {
    const existing = document.querySelector<HTMLScriptElement>(`script[src="${GSI_SRC}"]`)
    if (existing) {
      existing.addEventListener('load', () => resolve())
      existing.addEventListener('error', () => reject(new Error('gsi')))
      return
    }
    const script = document.createElement('script')
    script.src = GSI_SRC
    script.async = true
    script.defer = true
    script.onload = () => resolve()
    script.onerror = () => reject(new Error('gsi'))
    document.head.appendChild(script)
  })
}

async function handleCredential(response: GoogleCredentialResponse) {
  if (!response.credential || busy.value) return
  busy.value = true
  try {
    await user.googleSignIn(response.credential)
    emit('success')
  } catch (e: unknown) {
    emit('error', e instanceof Error ? e.message : t('auth.somethingWrong'))
  } finally {
    busy.value = false
  }
}

onMounted(async () => {
  const config = await user.fetchGoogleConfig()
  if (!config.enabled || !config.clientId) return
  enabled.value = true

  try {
    await loadScript()
  } catch {
    // Offline, or the script is blocked — fall back to hiding the button rather
    // than leaving a control that silently does nothing.
    enabled.value = false
    return
  }

  const gsi = window.google?.accounts?.id
  if (!gsi || !container.value) {
    enabled.value = false
    return
  }

  gsi.initialize({
    client_id: config.clientId,
    callback: handleCredential,
    auto_select: false,
  })

  gsi.renderButton(container.value, {
    type: 'standard',
    theme: 'outline',
    size: 'large',
    text: 'continue_with',
    shape: 'pill',
    logo_alignment: 'left',
    width: 360,
  })

  ready.value = true
})
</script>

<template>
  <div v-if="enabled" class="flex flex-col gap-5">
    <!-- Google renders its own button inside this node. -->
    <div
      class="flex justify-center min-h-[44px]"
      :class="{ 'opacity-60 pointer-events-none': props.disabled || busy }"
    >
      <div ref="container"></div>
      <div
        v-if="!ready"
        class="h-11 w-full max-w-[360px] rounded-[24px] border border-border bg-surface animate-pulse"
      ></div>
    </div>

    <!-- Divider -->
    <div class="flex items-center gap-3">
      <span class="h-px flex-1 bg-border"></span>
      <span class="text-[0.75rem] font-semibold uppercase tracking-[0.5px] text-text-light">
        {{ t('auth.or') }}
      </span>
      <span class="h-px flex-1 bg-border"></span>
    </div>
  </div>
</template>
