<script setup lang="ts">
/**
 * The assistant, reachable from every page via a floating button.
 *
 * Replaces the old standalone Chat menu item: the conversation is a panel over
 * the current page rather than a destination, so asking a question never costs
 * the user their place in the app.
 */
import { ref, computed, nextTick, watch, useTemplateRef } from 'vue'
import { useI18n } from 'vue-i18n'
import { useChatStore } from '../../stores/chat'
import { useUserStore } from '../../stores/user'
import ChatMessage from './ChatMessage.vue'
import ChatInput from './ChatInput.vue'
import { MessageCircle, X, Flower2, Trash2, TriangleAlert } from 'lucide-vue-next'

const { t, locale } = useI18n()
const chat = useChatStore()
const user = useUserStore()

const open = ref(false)
const scrollContainer = useTemplateRef<HTMLElement>('scrollContainer')
const confirmingClear = ref(false)

/**
 * Shown above the stored conversation rather than saved into it, so it stays
 * correct if the user's name or language changes between sessions.
 */
const greeting = computed(() =>
  user.displayName
    ? t('chat.greeting', { name: user.displayName })
    : t('chat.greetingNoName'),
)

function formatTime(iso: string) {
  return new Date(iso).toLocaleTimeString(locale.value === 'id' ? 'id-ID' : 'en-US', {
    hour: 'numeric',
    minute: '2-digit',
  })
}

async function scrollToBottom() {
  await nextTick()
  const el = scrollContainer.value
  if (el) el.scrollTop = el.scrollHeight
}

async function toggle() {
  open.value = !open.value
  if (!open.value) {
    confirmingClear.value = false
    return
  }
  await chat.loadHistory()
  await scrollToBottom()
}

async function handleSend(text: string) {
  await scrollToBottom()
  await chat.send(text)
  await scrollToBottom()
}

async function confirmClear() {
  await chat.clear()
  confirmingClear.value = false
}

// Any new message — sent or received — pins the view to the newest turn.
watch(() => chat.messages.length, scrollToBottom)
</script>

<template>
  <!-- Hidden until the account is usable: an unverified user can't reach the API. -->
  <div v-if="user.isAuthenticated && user.isEmailVerified">
    <!-- Panel -->
    <Transition name="chat-panel">
      <div
        v-if="open"
        class="fixed z-50 flex flex-col bg-bg-app border border-border shadow-2xl
               bottom-24 right-6 w-[400px] h-[min(600px,calc(100dvh-8rem))] rounded-2xl overflow-hidden
               max-sm:inset-0 max-sm:bottom-0 max-sm:right-0 max-sm:w-full max-sm:h-[100dvh] max-sm:rounded-none"
      >
        <!-- Header -->
        <header class="flex items-center gap-3 px-4 py-3 bg-surface border-b border-border shrink-0">
          <div class="w-9 h-9 rounded-full bg-light-green-bg text-success-green flex items-center justify-center shrink-0">
            <Flower2 width="18" height="18" />
          </div>
          <div class="flex-1 min-w-0">
            <p class="font-bold text-text-main text-[0.92rem] leading-tight">{{ t('chat.widgetTitle') }}</p>
            <p class="text-[0.72rem] text-text-muted">{{ t('chat.widgetSubtitle') }}</p>
          </div>

          <button
            v-if="!chat.isEmpty"
            @click="confirmingClear = !confirmingClear"
            class="p-2 rounded-lg text-text-muted hover:bg-bg-app hover:text-red-500 transition-colors"
            :title="t('chat.clear')"
          >
            <Trash2 width="16" height="16" />
          </button>
          <button
            @click="toggle"
            class="p-2 rounded-lg text-text-muted hover:bg-bg-app hover:text-text-main transition-colors"
            :aria-label="t('chat.close')"
          >
            <X width="18" height="18" />
          </button>
        </header>

        <!-- Clear confirmation -->
        <div v-if="confirmingClear" class="px-4 py-3 bg-[#fff4e5] border-b border-[#fde3bd] flex items-center gap-3">
          <p class="flex-1 text-[0.8rem] font-medium text-[#b45309] leading-snug">{{ t('chat.clearConfirm') }}</p>
          <button
            @click="confirmingClear = false"
            class="px-3 py-1.5 rounded-lg text-[0.78rem] font-bold text-text-muted hover:bg-white/60 transition-colors"
          >
            {{ t('common.cancel') }}
          </button>
          <button
            @click="confirmClear"
            class="px-3 py-1.5 rounded-lg text-[0.78rem] font-bold bg-red-500 text-white hover:bg-red-600 transition-colors"
          >
            {{ t('chat.clear') }}
          </button>
        </div>

        <!-- Conversation, oldest at the top, scrollable back through history -->
        <div ref="scrollContainer" class="flex-1 overflow-y-auto px-4 pt-5 pb-2 scrollbar-none min-h-0">
          <ChatMessage
            :isAssistant="true"
            :time="''"
            :content="greeting"
          />

          <p v-if="chat.loadingHistory" class="text-center text-[0.78rem] text-text-light py-3">
            {{ t('chat.loadingHistory') }}
          </p>

          <ChatMessage
            v-for="(msg, i) in chat.messages"
            :key="i"
            :isAssistant="msg.role === 'assistant'"
            :time="formatTime(msg.sentAt)"
            :content="msg.content"
          >
            {{ msg.content }}
          </ChatMessage>

          <!-- Typing indicator -->
          <div v-if="chat.loading" class="flex gap-3 w-full mb-4">
            <div class="shrink-0 flex items-start">
              <div class="w-8 h-8 rounded-full flex items-center justify-center mt-[1.4rem] bg-light-green-bg text-success-green">
                <Flower2 width="16" height="16" />
              </div>
            </div>
            <div class="flex flex-col max-w-[80%]">
              <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[0.5px] mb-2 uppercase">
                {{ t('chat.assistantLabel') }}
              </span>
              <div class="p-4 rounded-lg bg-surface border border-border rounded-tl-[4px] flex gap-1.5 items-center">
                <span class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:0ms]"></span>
                <span class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:150ms]"></span>
                <span class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:300ms]"></span>
              </div>
            </div>
          </div>

          <!-- Send failure -->
          <div
            v-if="chat.error"
            class="mb-4 flex items-start gap-2 rounded-xl border border-red-200 bg-red-50 px-3 py-2.5 text-[0.8rem] font-medium text-red-600"
          >
            <TriangleAlert width="14" height="14" class="shrink-0 mt-px" />
            {{ chat.error || t('chat.errorServer') }}
          </div>
        </div>

        <div class="px-3 pb-3 shrink-0">
          <ChatInput :disabled="chat.loading" @send="handleSend" />
        </div>
      </div>
    </Transition>

    <!-- Floating trigger, present on every page -->
    <button
      @click="toggle"
      class="fixed bottom-6 right-6 z-50 w-14 h-14 rounded-full bg-primary text-white shadow-[0_8px_24px_rgba(26,86,65,0.35)]
             flex items-center justify-center transition-transform duration-200 hover:scale-105 active:scale-95
             max-sm:bottom-5 max-sm:right-5"
      :class="{ 'max-sm:hidden': open }"
      :aria-label="open ? t('chat.close') : t('chat.open')"
      :title="open ? t('chat.close') : t('chat.open')"
    >
      <Transition name="icon" mode="out-in">
        <X v-if="open" width="24" height="24" />
        <MessageCircle v-else width="24" height="24" />
      </Transition>
    </button>
  </div>
</template>

<style scoped>
.chat-panel-enter-active,
.chat-panel-leave-active {
  transition:
    opacity 0.18s ease,
    transform 0.18s ease;
}
.chat-panel-enter-from,
.chat-panel-leave-to {
  opacity: 0;
  transform: translateY(12px) scale(0.98);
}
.icon-enter-active,
.icon-leave-active {
  transition: opacity 0.12s ease;
}
.icon-enter-from,
.icon-leave-to {
  opacity: 0;
}
</style>
