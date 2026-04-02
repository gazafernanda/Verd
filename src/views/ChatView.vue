<script setup lang="ts">
import { ref, nextTick } from 'vue'
import ChatMessage from '../components/Chat/ChatMessage.vue'
import ChatInput from '../components/Chat/ChatInput.vue'
import { useUserStore } from '../stores/user'

const API = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

interface Message {
  role: 'user' | 'assistant'
  content: string
  time: string
}

const user = useUserStore()
const loading = ref(false)
const scrollContainer = ref<HTMLElement | null>(null)

const messages = ref<Message[]>([
  {
    role: 'assistant',
    content: `Hello${user.displayName ? ' ' + user.displayName : ''}! I'm your botanical specialist. Ask me anything about your plants — watering schedules, pests, soil, or care tips.`,
    time: formatTime(new Date()),
  },
])

function formatTime(d: Date) {
  return d.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true })
}

async function handleSend(text: string) {
  if (loading.value) return

  messages.value.push({ role: 'user', content: text, time: formatTime(new Date()) })
  loading.value = true
  await scrollToBottom()

  try {
    const history = messages.value
      .slice(0, -1)
      .map(({ role, content }) => ({ role, content }))

    const res = await fetch(`${API}/api/chat`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${user.token}`,
      },
      body: JSON.stringify({ message: text, history }),
    })

    if (res.ok) {
      const data = await res.json()
      messages.value.push({ role: 'assistant', content: data.reply, time: formatTime(new Date()) })
    } else {
      messages.value.push({
        role: 'assistant',
        content: 'Sorry, I had trouble responding. Please try again.',
        time: formatTime(new Date()),
      })
    }
  } catch {
    messages.value.push({
      role: 'assistant',
      content: 'Cannot reach the server. Make sure the .NET API is running.',
      time: formatTime(new Date()),
    })
  } finally {
    loading.value = false
    await scrollToBottom()
  }
}

async function scrollToBottom() {
  await nextTick()
  if (scrollContainer.value) {
    scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight
  }
}
</script>

<template>
  <div class="h-[calc(100vh-80px)] flex flex-col">
    <div class="flex-1 flex flex-col bg-transparent max-w-[900px] mx-auto w-full relative">

      <div ref="scrollContainer" class="flex-1 overflow-y-auto px-6 pb-[120px] pt-6 scrollbar-none">
        <ChatMessage
          v-for="(msg, i) in messages"
          :key="i"
          :isAssistant="msg.role === 'assistant'"
          :time="msg.time"
        >
          {{ msg.content }}
        </ChatMessage>

        <div v-if="loading" class="flex gap-4 w-full mb-6">
          <div class="shrink-0 flex items-start">
            <div class="w-9 h-9 rounded-full flex items-center justify-center mt-6 bg-light-green-bg text-success-green">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M208,64H176V56a48,48,0,0,0-96,0v8H48A16,16,0,0,0,32,80V192a16,16,0,0,0,16,16H208a16,16,0,0,0,16-16V80A16,16,0,0,0,208,64ZM96,56a32,32,0,0,1,64,0v8H96ZM208,192H48V80H208V192Zm-48-56a32,32,0,1,1-32-32A32,32,0,0,1,160,136Zm-16,0a16,16,0,1,0-16-16A16,16,0,0,0,144,136Z"/></svg>
            </div>
          </div>
          <div class="flex flex-col max-w-[80%]">
            <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[0.5px] mb-2 uppercase">VERD ASSISTANT</span>
            <div class="p-6 rounded-lg bg-surface border border-border rounded-tl-[4px] flex gap-1.5 items-center">
              <span class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:0ms]"></span>
              <span class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:150ms]"></span>
              <span class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:300ms]"></span>
            </div>
          </div>
        </div>
      </div>

      <div class="absolute bottom-0 left-0 right-0 px-6">
        <ChatInput :disabled="loading" @send="handleSend" />
      </div>

    </div>
  </div>
</template>
