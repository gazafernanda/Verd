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
  const html = marked.parse(text) as string
  const document = new DOMParser().parseFromString(html, 'text/html')

  // Existing conversations can already contain Markdown tables. Tables don't
  // fit a compact chat panel, so turn each row into a labelled, responsive
  // card rather than forcing the user to pan sideways.
  document.querySelectorAll('table').forEach((table) => {
    const headers = Array.from(table.querySelectorAll('thead th')).map((cell) => cell.textContent?.trim() ?? '')
    const rows = Array.from(table.querySelectorAll('tbody tr'))
    const cards = document.createElement('div')
    cards.className = 'markdown-table'

    rows.forEach((row) => {
      const card = document.createElement('div')
      card.className = 'markdown-table-row'
      Array.from(row.querySelectorAll('td')).forEach((cell, index) => {
        const item = document.createElement('div')
        item.className = 'markdown-table-cell'

        const label = document.createElement('span')
        label.className = 'markdown-table-label'
        label.textContent = headers[index] || `Info ${index + 1}`

        const value = document.createElement('span')
        value.className = 'markdown-table-value'
        value.innerHTML = cell.innerHTML

        item.append(label, value)
        card.append(item)
      })
      cards.append(card)
    })

    table.replaceWith(cards)
  })

  return document.body.innerHTML
}
</script>

<template>
  <div :class="['flex gap-3 w-full mb-4', { 'justify-end': !isAssistant }]">

    <div v-if="isAssistant" class="shrink-0 flex items-start">
      <div class="w-8 h-8 rounded-full flex items-center justify-center mt-[1.4rem] bg-light-green-bg text-success-green">
        <Flower2 width="16" height="16" />
      </div>
    </div>

    <div :class="['flex min-w-0 flex-col max-w-[80%] max-sm:max-w-[90%]', { 'items-end': !isAssistant }]">
      <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[0.5px] mb-2 uppercase">
        {{ isAssistant ? t('chat.assistantLabel') : t('chat.youLabel') }}
      </span>

      <div :class="[
        'min-w-0 overflow-hidden p-4 max-sm:p-3 rounded-lg text-[0.9rem] max-sm:text-[0.85rem] leading-relaxed max-sm:leading-normal shadow-sm',
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
.markdown-table { display: grid; gap: 0.625rem; margin: 0.75rem 0; }
.markdown-table-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(9rem, 1fr));
  gap: 0.5rem;
  padding: 0.625rem;
  border: 1px solid var(--color-border, #e5e7eb);
  border-radius: 0.625rem;
  background: var(--color-bg-app, #f7f9f8);
}
.markdown-table-cell { min-width: 0; display: flex; flex-direction: column; gap: 0.125rem; }
.markdown-table-label { font-size: 0.68rem; font-weight: 700; color: var(--color-text-muted, #73807a); }
.markdown-table-value { overflow-wrap: anywhere; }
</style>
