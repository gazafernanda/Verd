<script setup lang="ts">
import { usePlantsStore } from '../../stores/plants'
import { useRouter } from 'vue-router'
import { Leaf, Flower2, Plus, Trash2 } from 'lucide-vue-next'
const plants = usePlantsStore()
const router = useRouter()

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
  <div class="bg-surface rounded-xl py-6 px-8 max-md:px-5 shadow-sm border border-border h-full">
    <div class="flex justify-between items-center mb-6">
      <h2 class="flex items-center gap-3 text-xl font-extrabold text-text-main m-0">
        <Leaf class="text-success-green" width="20" height="20" />
        My Plants
      </h2>
      <button
        @click="router.push({ name: 'add-plant' })"
        class="flex items-center gap-1.5 text-[0.85rem] font-bold text-success-green hover:text-primary transition-colors"
      >
        <Plus width="14" height="14" />
        Add Plant
      </button>
    </div>

    <!-- Empty state -->
    <div v-if="plants.plants.length === 0" class="flex flex-col items-center justify-center py-10 text-center gap-3">
      <div class="w-14 h-14 rounded-full bg-light-green-bg flex items-center justify-center text-success-green">
        <Leaf width="24" height="24" />
      </div>
      <p class="text-[0.9rem] text-text-muted font-medium">No plants yet. Add your first one!</p>
      <button
        @click="router.push({ name: 'add-plant' })"
        class="flex items-center gap-2 px-5 py-2.5 bg-primary text-white rounded-[20px] text-[0.85rem] font-bold hover:bg-primary-hover transition-colors"
      >
        <Plus width="16" height="16" />
        Add Plant
      </button>
    </div>

    <div v-else class="flex gap-4 overflow-x-auto pb-2 scrollbar-none">
      <div
        v-for="plant in plants.plants"
        :key="plant.id"
        class="basis-[calc(50%-8px)] min-w-[min(200px,75vw)] bg-bg-app rounded-lg p-4 flex gap-4 border border-[rgba(26,86,65,0.05)] shrink-0 relative group"
      >
        <div class="w-16 h-16 rounded-md shrink-0 flex items-center justify-center border border-border"
          :style="{ backgroundColor: plant.iconBg }">
          <Flower2 width="32" height="32" style="color: #1a5641" />
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

        <!-- Delete button -->
        <button
          @click.stop="plants.deletePlant(plant.id)"
          class="absolute top-2 right-2 w-6 h-6 rounded-full bg-red-50 text-red-400 flex items-center justify-center opacity-0 group-hover:opacity-100 max-sm:opacity-100 transition-opacity hover:bg-red-100"
          title="Remove plant"
        >
          <Trash2 width="12" height="12" />
        </button>
      </div>
    </div>
  </div>
</template>
