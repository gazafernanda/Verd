<script setup lang="ts">
defineProps<{
  category: string
  title: string
  description: string
  image: string
  bgType?: 'blue' | 'yellow' | 'green'
}>()
</script>

<template>
  <div class="bg-[var(--surface-color)] rounded-[var(--radius-lg)] overflow-hidden shadow-[var(--shadow-sm)] flex flex-col transition-all duration-200 ease-in-out cursor-pointer h-full hover:-translate-y-1 hover:shadow-[var(--shadow-md)] group">
    <div class="h-[180px] relative overflow-hidden">
      <img :src="image" :alt="title" class="w-full h-full object-cover transition-transform duration-300 ease-in-out group-hover:scale-105" />
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
