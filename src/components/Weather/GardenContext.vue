<script setup lang="ts">
import { usePlantsStore } from '../../stores/plants'
const plants = usePlantsStore()
</script>

<template>
  <div class="bg-surface rounded-xl p-6 shadow-sm mb-6">
    <h3 class="text-base font-bold text-text-main mb-4">Garden Context</h3>
    <div class="flex flex-col gap-3">
      <div v-for="(category, index) in plants.gardenCategories" :key="index"
           class="px-4 py-3 bg-[#f9fafb] rounded-[20px] text-[0.85rem] font-medium text-text-main">
        {{ category }}
      </div>
    </div>
  </div>
</template>
