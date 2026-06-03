<script setup lang="ts">
import { ref, computed } from 'vue'
import { Sprout } from 'lucide-vue-next'

const props = defineProps<{
  category: string
  title: string
  description: string
  image: string
  bgType?: 'blue' | 'yellow' | 'green'
}>()

const errored = ref(false)

// Only treat http(s)/data URLs as real images. Empty strings and old
// "/src/assets/..." dev paths don't resolve in the production build, so
// fall back to a styled placeholder instead of a broken-image icon.
const hasImage = computed(
  () =>
    !errored.value &&
    /^(https?:|data:)/.test(props.image ?? ''),
)
</script>

<template>
  <div class="bg-[var(--surface-color)] rounded-[var(--radius-lg)] overflow-hidden shadow-[var(--shadow-sm)] flex flex-col transition-all duration-200 ease-in-out cursor-pointer h-full hover:-translate-y-1 hover:shadow-[var(--shadow-md)] group">
    <div class="h-[180px] relative overflow-hidden">
      <img
        v-if="hasImage"
        :src="image"
        :alt="title"
        class="w-full h-full object-cover transition-transform duration-300 ease-in-out group-hover:scale-105"
        @error="errored = true"
      />
      <div
        v-else
        class="w-full h-full flex items-center justify-center transition-transform duration-300 ease-in-out group-hover:scale-105"
        :class="{
          'bg-gradient-to-br from-[#dbeafe] to-[#bfdbfe] text-[#3e81db]': bgType === 'blue' || !bgType,
          'bg-gradient-to-br from-[#fef3c7] to-[#fde68a] text-[#d97706]': bgType === 'yellow',
          'bg-gradient-to-br from-[#d1fae5] to-[#a7f3d0] text-[var(--success-green)]': bgType === 'green',
        }"
      >
        <Sprout width="56" height="56" :stroke-width="1.5" />
      </div>
      <span
        class="absolute top-4 left-4 py-1.5 px-3 rounded-full text-[0.7rem] font-bold uppercase tracking-wider text-white"
        :class="{
          'bg-[#3e81db]': bgType === 'blue' || !bgType,
          'bg-[#f0a324]': bgType === 'yellow',
          'bg-[var(--success-green)]': bgType === 'green'
        }"
      >
        {{ category }}
      </span>
    </div>
    <div class="p-5 flex-1 flex flex-col">
      <h4 class="text-[1.1rem] font-bold text-[var(--text-main)] mb-2">{{ title }}</h4>
      <p class="text-[0.9rem] text-[var(--text-muted)] leading-relaxed">{{ description }}</p>
    </div>
  </div>
</template>

<style scoped>
/* Tailwind handles the styling via utility classes above */
</style>
