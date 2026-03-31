<script setup lang="ts">
import { usePlantsStore } from '../../stores/plants'
const plants = usePlantsStore()

function statusClass(status: string) {
  if (status === 'HEALTHY') return 'bg-light-green-bg text-success-green'
  if (status === 'NEEDS WATER') return 'bg-[#fff4e5] text-[#f59e0b]'
  return 'bg-[#ebf5ff] text-[#3b82f6]'
}

function barClass(status: string) {
  if (status === 'HEALTHY') return 'bg-success-green'
  if (status === 'NEEDS WATER') return 'bg-[#f59e0b]'
  return 'bg-[#3b82f6]'
}
</script>

<template>
  <div class="bg-surface rounded-xl py-6 px-8 shadow-sm border border-border h-full">
    <div class="flex justify-between items-center mb-6">
      <h2 class="flex items-center gap-3 text-xl font-extrabold text-text-main m-0">
        <svg class="text-success-green" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M216,40H185.34a71.84,71.84,0,0,0-57.34,28.75A71.84,71.84,0,0,0,70.66,40H40a8,8,0,0,0-8,8v42.66A72.08,72.08,0,0,0,104,162v14H48a8,8,0,0,0-8,8v40a8,8,0,0,0,8,8H208a8,8,0,0,0,8-8V184a8,8,0,0,0-8-8H152V162a72.08,72.08,0,0,0,72-71.34V48A8,8,0,0,0,216,40ZM56,216V200H200v16ZM208,90.66A56.06,56.06,0,0,1,152,146.66H141.52A71.64,71.64,0,0,0,165.65,99l16.15-16.14A8,8,0,1,0,170.49,71.5L154.34,87.65A71.74,71.74,0,0,0,128,143.51V104a8,8,0,0,0-16,0v39.51a71.74,71.74,0,0,0-26.34-55.86L69.51,71.5a8,8,0,1,0-11.32,11.31L74.35,99a71.64,71.64,0,0,0,24.13,47.66H88A56.06,56.06,0,0,1,32,90.66V56H70.66A55.83,55.83,0,0,1,126,101.4a55.83,55.83,0,0,1,55.34-45.4H224v34.66Z"/></svg>
        My Plants
      </h2>
      <a href="#" class="text-[0.85rem] font-bold text-success-green">View All</a>
    </div>

    <div class="flex gap-4 overflow-x-auto pb-2 scrollbar-none">
      <div
        v-for="plant in plants.plants"
        :key="plant.id"
        class="basis-[calc(50%-8px)] min-w-[200px] bg-bg-app rounded-lg p-4 flex gap-4 border border-[rgba(26,86,65,0.05)] shrink-0"
      >
        <div class="w-16 h-16 rounded-md shrink-0 flex items-center justify-center border border-border"
          :style="{ backgroundColor: plant.iconBg }">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#1a5641" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24ZM74.08,197.5a64,64,0,0,1,107.84,0,87.83,87.83,0,0,1-107.84,0ZM96,120a32,32,0,1,1,32,32A32,32,0,0,1,96,120Zm97.76,66.41a79.66,79.66,0,0,0-36.06-28.75,48,48,0,1,0-59.4,0,79.66,79.66,0,0,0-36.06,28.75,88,88,0,1,1,131.52,0Z"/></svg>
        </div>
        <div class="flex-1 flex flex-col min-w-0">
          <div class="flex justify-between items-start mb-2 gap-2">
            <h3 class="text-[0.95rem] font-extrabold text-text-main m-0 overflow-hidden text-ellipsis whitespace-nowrap">{{ plant.name }}</h3>
            <span class="text-[0.55rem] font-extrabold px-1.5 py-1 rounded tracking-[0.5px] whitespace-nowrap shrink-0"
              :class="statusClass(plant.status)">{{ plant.status }}</span>
          </div>
          <p class="text-[0.75rem] text-text-muted mb-3 leading-snug flex-1">Last watered: {{ plant.lastWatered }}</p>
          <div class="h-1.5 bg-border rounded w-full overflow-hidden">
            <div class="h-full rounded transition-all duration-500"
              :class="barClass(plant.status)"
              :style="`width: ${plant.wateringLevel}%`"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
