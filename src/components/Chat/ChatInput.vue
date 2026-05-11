<script setup lang="ts">
import { ref } from 'vue'
import { Send } from 'lucide-vue-next'

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
    <div class="flex items-center bg-surface border border-border rounded-xl px-4 pr-2 py-2 shadow-[0_4px_16px_rgba(0,0,0,0.05)]">
      <input
        v-model="inputText"
        type="text"
        placeholder="Ask about your plants..."
        :disabled="disabled"
        class="flex-1 min-w-0 border-none outline-none px-3 py-3 text-[0.95rem] font-[inherit] text-text-main bg-transparent placeholder:text-text-light disabled:opacity-50"
        @keydown.enter="submit"
      />

      <button
        :disabled="disabled || !inputText.trim()"
        class="flex items-center justify-center bg-success-green text-white w-11 h-11 rounded-full shadow-[0_4px_12px_rgba(55,178,126,0.3)] transition-all duration-200 hover:bg-[#2ea06e] hover:-translate-y-px disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:translate-y-0"
        @click="submit"
      >
        <Send width="20" height="20" />
      </button>
    </div>
  </div>
</template>
