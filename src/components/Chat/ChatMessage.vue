<script setup lang="ts">
import { marked } from 'marked'
import { useI18n } from 'vue-i18n'
import { Flower2, CircleUser } from 'lucide-vue-next'

const { t } = useI18n()

const props = defineProps({
  isAssistant: {
    type: Boolean,
    default: false
  },
  time: {
    type: String,
    required: true
  },
  content: {
    type: String,
    default: ''
  }
})

function renderMarkdown(text: string): string {
  return marked.parse(text) as string
}
</script>

<template>
  <div :class="['flex gap-3 w-full mb-4', { 'justify-end': !isAssistant }]">

    <div v-if="isAssistant" class="shrink-0 flex items-start">
      <div class="w-8 h-8 rounded-full flex items-center justify-center mt-[1.4rem] bg-light-green-bg text-success-green">
        <Flower2 width="16" height="16" />
      </div>
    </div>

    <div :class="['flex flex-col max-w-[80%] max-sm:max-w-[90%]', { 'items-end': !isAssistant }]">
      <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[0.5px] mb-2 uppercase">
        {{ isAssistant ? t('chat.assistantLabel') : t('chat.youLabel') }}
      </span>

      <div :class="[
        'p-4 max-sm:p-3 rounded-lg text-[0.9rem] max-sm:text-[0.85rem] leading-relaxed max-sm:leading-normal shadow-sm',
        isAssistant
          ? 'bg-surface text-text-main border border-border rounded-tl-[4px]'
          : 'bg-primary text-white rounded-tr-[4px]'
      ]">
        <div v-if="isAssistant" class="markdown" v-html="renderMarkdown(content)"></div>
        <slot v-else></slot>
      </div>

      <span class="text-[0.7rem] text-text-muted mt-2 font-medium">{{ time }}</span>
    </div>

    <div v-if="!isAssistant" class="shrink-0 flex items-start">
      <div class="w-8 h-8 rounded-full flex items-center justify-center mt-[1.4rem] bg-[#f1ebd8] text-[#c4a77d]">
        <CircleUser width="16" height="16" />
      </div>
    </div>

  </div>
</template>

<style scoped>
.markdown :deep(p) { margin-bottom: 0.5rem; }
.markdown :deep(p:last-child) { margin-bottom: 0; }
.markdown :deep(strong) { font-weight: 600; }
.markdown :deep(ol) { list-style: decimal; padding-left: 1.25rem; margin: 0.5rem 0; }
.markdown :deep(ul) { list-style: disc; padding-left: 1.25rem; margin: 0.5rem 0; }
.markdown :deep(li) { margin-bottom: 0.25rem; }
</style>
