<script setup lang="ts">

const props = defineProps({
  isAssistant: {
    type: Boolean,
    default: false
  },
  time: {
    type: String,
    required: true
  }
})
</script>

<template>
  <div :class="['flex gap-4 w-full mb-6', { 'justify-end': !isAssistant }]">

    <div v-if="isAssistant" class="shrink-0 flex items-start">
      <div class="w-9 h-9 rounded-full flex items-center justify-center mt-6 bg-light-green-bg text-success-green">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M208,64H176V56a48,48,0,0,0-96,0v8H48A16,16,0,0,0,32,80V192a16,16,0,0,0,16,16H208a16,16,0,0,0,16-16V80A16,16,0,0,0,208,64ZM96,56a32,32,0,0,1,64,0v8H96ZM208,192H48V80H208V192Zm-48-56a32,32,0,1,1-32-32A32,32,0,0,1,160,136Zm-16,0a16,16,0,1,0-16-16A16,16,0,0,0,144,136Z"/></svg>
      </div>
    </div>

    <div :class="['flex flex-col max-w-[80%]', { 'items-end': !isAssistant }]">
      <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[0.5px] mb-2 uppercase">
        {{ isAssistant ? 'VERD ASSISTANT' : 'YOU' }}
      </span>

      <div :class="[
        'p-6 rounded-lg text-base leading-relaxed shadow-sm',
        isAssistant
          ? 'bg-surface text-text-main border border-border rounded-tl-[4px]'
          : 'bg-primary text-white rounded-tr-[4px]'
      ]">
        <slot></slot>
      </div>

      <span class="text-[0.7rem] text-text-muted mt-2 font-medium">{{ time }}</span>
    </div>

    <div v-if="!isAssistant" class="shrink-0 flex items-start">
      <div class="w-9 h-9 rounded-full flex items-center justify-center mt-6 bg-[#f1ebd8] text-[#c4a77d]">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24ZM74.08,197.5a64,64,0,0,1,107.84,0,87.83,87.83,0,0,1-107.84,0ZM96,120a32,32,0,1,1,32,32A32,32,0,0,1,96,120Zm97.76,66.41a79.66,79.66,0,0,0-36.06-28.75,48,48,0,1,0-59.4,0,79.66,79.66,0,0,0-36.06,28.75,88,88,0,1,1,131.52,0Z"/></svg>
      </div>
    </div>

  </div>
</template>
