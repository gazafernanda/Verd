<script setup lang="ts">
import { computed } from 'vue'
import { usePlantsStore } from '../../stores/plants'
import { Leaf } from '@iconoir/vue'

const plants = usePlantsStore()

const mistingPlants = computed(() => plants.plantsMistingNeeded)
const alertPlants = computed(() => plants.plantsNeedingCare)

const alertMessage = computed(() => {
  if (mistingPlants.value.length > 0) {
    const names = mistingPlants.value.map((p) => p.name).join(' and ')
    return `High evaporation predicted today. Your <strong class="font-bold text-primary">${names}</strong> will need extra misting to maintain humidity above 60%.`
  }
  if (alertPlants.value.length > 0) {
    const names = alertPlants.value.map((p) => p.name).join(' and ')
    return `Weather conditions may affect your plants. Check on <strong class="font-bold text-primary">${names}</strong> today.`
  }
  return 'All plants are well-adapted to current conditions. No urgent care needed today.'
})
</script>

<template>
  <div class="bg-light-green-bg rounded-xl p-6 border border-[rgba(55,178,126,0.2)] mb-6">
    <div class="flex items-center gap-3 mb-4">
      <div class="w-8 h-8 bg-success-green rounded-full flex items-center justify-center text-white shrink-0">
        <Leaf width="16" height="16" />
      </div>
      <h3 class="text-[1.1rem] font-bold text-text-main m-0">Plant Care Alert</h3>
    </div>

    <p class="text-[0.9rem] leading-relaxed text-text-main mb-6" v-html="alertMessage"></p>

    <button class="w-full py-3.5 bg-success-green text-white rounded-xl font-semibold text-[0.95rem] shadow-[0_4px_12px_rgba(55,178,126,0.2)] transition-all duration-200 hover:bg-[#2ea06e] hover:-translate-y-px">
      View Care Schedule
    </button>
  </div>
</template>
