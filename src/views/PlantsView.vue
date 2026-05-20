<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { usePlantsStore } from '../stores/plants'
import { Plus, Leaf, Flower2, Droplet, Sun, Trash2, Search } from 'lucide-vue-next'

const router = useRouter()
const plants = usePlantsStore()

const query = ref('')
const activeFilter = ref<'all' | 'healthy' | 'needs-care'>('all')

const filtered = computed(() => {
  let list = plants.plants
  if (activeFilter.value === 'healthy') list = list.filter(p => p.status === 'HEALTHY')
  if (activeFilter.value === 'needs-care') list = list.filter(p => p.status !== 'HEALTHY')
  if (query.value.trim()) {
    const q = query.value.toLowerCase()
    list = list.filter(p => p.name.toLowerCase().includes(q) || p.category.toLowerCase().includes(q))
  }
  return list
})

function statusStyle(status: string) {
  if (status === 'HEALTHY') return 'bg-light-green-bg text-success-green'
  if (status === 'NEEDS WATER') return 'bg-[#fff4e5] text-[#f59e0b]'
  return 'bg-[#ebf5ff] text-[#3b82f6]'
}

function barColor(status: string) {
  if (status === 'HEALTHY') return 'bg-success-green'
  if (status === 'NEEDS WATER') return 'bg-[#f59e0b]'
  return 'bg-[#3b82f6]'
}

onMounted(() => plants.fetchPlants())
</script>

<template>
  <div class="flex flex-col gap-6">
    <!-- Header -->
    <div class="flex justify-between items-end max-lg:flex-col max-lg:items-start max-lg:gap-4">
      <div>
        <span class="font-semibold text-success-green text-[0.9rem]">Home / My Plants</span>
        <h1 class="text-[2.2rem] font-extrabold text-text-main mb-2 mt-2 tracking-[-0.5px]">My Plants</h1>
        <p class="text-text-muted text-[0.95rem] font-medium">
          {{ plants.totalPlants }} plant{{ plants.totalPlants !== 1 ? 's' : '' }} in your garden
        </p>
      </div>
      <button
        @click="router.push({ name: 'add-plant' })"
        class="flex items-center gap-2 px-5 py-[11px] bg-primary text-white rounded-[24px] font-semibold text-[0.95rem] shadow-sm hover:opacity-90 transition-opacity"
      >
        <Plus width="18" height="18" />
        Add New Plant
      </button>
    </div>

    <!-- Filters & Search -->
    <div class="flex gap-3 max-sm:flex-col">
      <!-- Search -->
      <div class="flex items-center gap-2 px-4 py-2.5 bg-surface border border-border rounded-xl flex-1 focus-within:border-success-green transition-colors">
        <Search class="text-text-muted shrink-0" width="16" height="16" />
        <input
          v-model="query"
          type="text"
          placeholder="Search by name or category…"
          class="flex-1 bg-transparent text-[0.9rem] text-text-main placeholder-text-light outline-none"
        />
      </div>

      <!-- Filter pills -->
      <div class="flex gap-2">
        <button
          v-for="f in [{ key: 'all', label: 'All' }, { key: 'healthy', label: 'Healthy' }, { key: 'needs-care', label: 'Needs Care' }]"
          :key="f.key"
          @click="activeFilter = f.key as typeof activeFilter"
          class="px-4 py-2.5 rounded-xl text-[0.85rem] font-semibold border-2 transition-all"
          :class="activeFilter === f.key
            ? 'border-success-green bg-light-green-bg text-primary'
            : 'border-border bg-surface text-text-muted hover:border-success-green hover:text-text-main'"
        >
          {{ f.label }}
        </button>
      </div>
    </div>

    <!-- Empty state -->
    <div v-if="plants.plants.length === 0" class="flex flex-col items-center justify-center py-24 gap-5 text-center">
      <div class="w-20 h-20 rounded-full bg-light-green-bg flex items-center justify-center">
        <Leaf class="text-success-green" width="36" height="36" />
      </div>
      <div>
        <h2 class="text-[1.4rem] font-bold text-text-main mb-2">No plants yet</h2>
        <p class="text-text-muted text-[0.95rem]">Add your first plant to start tracking your garden.</p>
      </div>
      <button
        @click="router.push({ name: 'add-plant' })"
        class="flex items-center gap-2 px-6 py-3 bg-primary text-white rounded-[24px] font-semibold hover:opacity-90 transition-opacity"
      >
        <Plus width="18" height="18" />
        Add Plant
      </button>
    </div>

    <!-- No search results -->
    <div v-else-if="filtered.length === 0" class="flex flex-col items-center justify-center py-16 gap-3 text-center">
      <Search class="text-text-muted" width="32" height="32" />
      <p class="text-text-muted font-medium">No plants match your search.</p>
    </div>

    <!-- Plant grid -->
    <div v-else class="grid grid-cols-3 max-xl:grid-cols-2 max-sm:grid-cols-1 gap-4">
      <div
        v-for="plant in filtered"
        :key="plant.id"
        class="bg-surface rounded-2xl p-5 shadow-sm border border-border flex flex-col gap-4 relative group hover:shadow-md transition-shadow"
      >
        <!-- Delete -->
        <button
          @click="plants.deletePlant(plant.id)"
          class="absolute top-3 right-3 w-7 h-7 rounded-full bg-red-50 text-red-400 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity hover:bg-red-100"
        >
          <Trash2 width="13" height="13" />
        </button>

        <!-- Icon + status -->
        <div class="flex items-start gap-4">
          <div
            class="w-14 h-14 rounded-xl flex items-center justify-center shrink-0 border border-border"
            :style="{ backgroundColor: plant.iconBg }"
          >
            <Flower2 width="28" height="28" style="color: #1a5641" />
          </div>
          <div class="flex-1 min-w-0">
            <h3 class="font-extrabold text-text-main text-[1rem] leading-tight truncate pr-6">{{ plant.name }}</h3>
            <p class="text-text-muted text-[0.8rem] mt-0.5 truncate">{{ plant.category }}</p>
            <span
              class="inline-block mt-1.5 text-[0.65rem] font-extrabold px-2 py-0.5 rounded tracking-[0.5px]"
              :class="statusStyle(plant.status)"
            >{{ plant.status }}</span>
          </div>
        </div>

        <!-- Watering level -->
        <div class="flex flex-col gap-1.5">
          <div class="flex justify-between items-center text-[0.75rem] font-semibold text-text-muted">
            <span class="flex items-center gap-1"><Droplet width="12" height="12" /> Water level</span>
            <span>{{ plant.wateringLevel }}%</span>
          </div>
          <div class="h-1.5 bg-border rounded-full overflow-hidden">
            <div
              class="h-full rounded-full transition-all duration-500"
              :class="barColor(plant.status)"
              :style="`width: ${plant.wateringLevel}%`"
            />
          </div>
        </div>

        <!-- Meta row -->
        <div class="flex gap-3 text-[0.75rem] text-text-muted font-medium border-t border-border pt-3">
          <span class="flex items-center gap-1">
            <Droplet width="12" height="12" class="text-[#3b82f6]" />
            {{ plant.wateringFrequency ?? 'Weekly' }}
          </span>
          <span class="flex items-center gap-1">
            <Sun width="12" height="12" class="text-[#f59e0b]" />
            {{ plant.sunlight ?? 'Indirect' }}
          </span>
        </div>

        <p class="text-[0.75rem] text-text-muted -mt-1">Last watered: {{ plant.lastWatered }}</p>
      </div>
    </div>
  </div>
</template>
