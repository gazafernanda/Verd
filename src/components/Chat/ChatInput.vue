<script setup lang="ts">
import { ref } from 'vue'

const emit = defineEmits<{ send: [text: string] }>()

defineProps<{ disabled?: boolean }>()

const inputText = ref('')

function submit() {
  const text = inputText.value.trim()
  if (!text) return
  emit('send', text)
  inputText.value = ''
}
</script>

<template>
  <div class="w-full py-4 bg-gradient-to-b from-transparent to-[rgba(250,250,250,1)]">
    <div class="flex items-center bg-surface border border-border rounded-xl pl-4 pr-2 py-2 shadow-[0_4px_16px_rgba(0,0,0,0.05)]">
      <button class="flex items-center justify-center w-8 h-8 text-text-muted transition-colors duration-200 hover:text-text-main">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M208,56H180.28L166.65,35.56A8,8,0,0,0,160,32H96a8,8,0,0,0-6.65,3.56L75.71,56H48A24,24,0,0,0,24,80V192a24,24,0,0,0,24,24H208a24,24,0,0,0,24-24V80A24,24,0,0,0,208,56Zm8,136a8,8,0,0,1-8,8H48a8,8,0,0,1-8-8V80a8,8,0,0,1,8-8H80a8,8,0,0,0,6.66-3.56L100.28,48h55.43l13.63,20.44A8,8,0,0,0,176,72h32a8,8,0,0,1,8,8ZM128,88a44,44,0,1,0,44,44A44.05,44.05,0,0,0,128,88Zm0,72a28,28,0,1,1,28-28A28,28,0,0,1,128,160Z"/></svg>
      </button>

      <input
        v-model="inputText"
        type="text"
        placeholder="Ask about pruning, pests, or soil..."
        :disabled="disabled"
        class="flex-1 border-none outline-none px-4 py-3 text-base font-[inherit] text-text-main bg-transparent placeholder:text-text-light disabled:opacity-50"
        @keydown.enter="submit"
      />

      <button
        :disabled="disabled || !inputText.trim()"
        class="flex items-center justify-center bg-success-green text-white w-11 h-11 rounded-full shadow-[0_4px_12px_rgba(55,178,126,0.3)] transition-all duration-200 hover:bg-[#2ea06e] hover:-translate-y-px disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:translate-y-0"
        @click="submit"
      >
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M227.32,28.68a16,16,0,0,0-15.66-4.08l-192,64a16,16,0,0,0-2.42,29.84l85.62,40.55,40.55,85.62A15.86,15.86,0,0,0,157.74,252q.69,0,1.38-.06a15.88,15.88,0,0,0,14-11.51l64-192A16,16,0,0,0,227.32,28.68ZM157.83,231.85l-39.43-83.25L174.06,93.94a8,8,0,0,0-11.32-11.31L107.4,138.27l-83.25-39.43,184-61.32Z"/></svg>
      </button>
    </div>
  </div>
</template>
