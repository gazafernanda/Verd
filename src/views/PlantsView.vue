<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { usePlantsStore, type Plant } from '../stores/plants'
import { Plus, Leaf, Flower2, Droplet, Sun, Trash2, Search, TriangleAlert } from 'lucide-vue-next'
import EditPlantModal from '../components/EditPlantModal.vue'

const router = useRouter()
const { t } = useI18n()
const plants = usePlantsStore()

// Clicking a card opens the edit modal.
const editing = ref<Plant | null>(null)

function openPlant(plant: Plant) {
  editing.value = plant
}

// Delete confirmation.
const deleteTarget = ref<Plant | null>(null)
const deleting = ref(false)

async function confirmDelete() {
  if (!deleteTarget.value || deleting.value) return
  deleting.value = true
  try {
    await plants.deletePlant(deleteTarget.value.id)
    deleteTarget.value = null
  } finally {
    deleting.value = false
  }
}

const wateringId = ref<number | null>(null)

async function waterPlant(id: number) {
  if (wateringId.value !== null) return
  wateringId.value = id
  try {
    await plants.logCare(id, 'watered')
  } finally {
    wateringId.value = null
  }
}

const query = ref('')
const activeFilter = ref<'all' | 'healthy' | 'needs-care'>('all')

const filters = computed(() => [
  { key: 'all', label: t('plants.filterAll') },
  { key: 'healthy', label: t('plants.filterHealthy') },
  { key: 'needs-care', label: t('plants.filterNeedsCare') },
])

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

const sunlightKeyMap: Record<string, string> = {
  'full-sun': 'fullSunLabel',
  partial: 'partialLabel',
  indirect: 'indirectLabel',
  low: 'lowLabel',
}

function statusLabel(status: string) {
  return t(`status.${status}`)
}

function wateringLabel(freq?: string) {
  return freq ? t(`addPlant.watering.${freq}`) : t('addPlant.watering.weekly')
}

function sunlightLabel(value?: string) {
  const key = sunlightKeyMap[value ?? 'indirect'] ?? 'indirectLabel'
  return t(`addPlant.sunlight.${key}`)
}

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
        <span class="font-semibold text-success-green text-[0.9rem]">{{ t('plants.breadcrumb') }}</span>
        <h1 class="text-[2.2rem] max-sm:text-[1.6rem] font-extrabold text-text-main mb-2 mt-2 tracking-[-0.5px]">{{ t('plants.title') }}</h1>
        <p class="text-text-muted text-[0.95rem] font-medium">
          {{ t('plants.inGarden', plants.totalPlants) }}
        </p>
      </div>
      <button
        @click="router.push({ name: 'add-plant' })"
        class="flex items-center gap-2 px-5 py-[11px] bg-primary text-white rounded-[24px] font-semibold text-[0.95rem] shadow-sm hover:opacity-90 transition-opacity"
      >
        <Plus width="18" height="18" />
        {{ t('common.addNewPlant') }}
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
          :placeholder="t('plants.searchPlaceholder')"
          class="flex-1 bg-transparent text-[0.9rem] text-text-main placeholder-text-light outline-none"
        />
      </div>

      <!-- Filter pills -->
      <div class="flex gap-2">
        <button
          v-for="f in filters"
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
        <h2 class="text-[1.4rem] font-bold text-text-main mb-2">{{ t('plants.emptyTitle') }}</h2>
        <p class="text-text-muted text-[0.95rem]">{{ t('plants.emptyDesc') }}</p>
      </div>
      <button
        @click="router.push({ name: 'add-plant' })"
        class="flex items-center gap-2 px-6 py-3 bg-primary text-white rounded-[24px] font-semibold hover:opacity-90 transition-opacity"
      >
        <Plus width="18" height="18" />
        {{ t('common.addPlant') }}
      </button>
    </div>

    <!-- No search results -->
    <div v-else-if="filtered.length === 0" class="flex flex-col items-center justify-center py-16 gap-3 text-center">
      <Search class="text-text-muted" width="32" height="32" />
      <p class="text-text-muted font-medium">{{ t('plants.noResults') }}</p>
    </div>

    <!-- Plant grid -->
    <div v-else class="grid grid-cols-3 max-xl:grid-cols-2 max-sm:grid-cols-1 gap-4">
      <div
        v-for="plant in filtered"
        :key="plant.id"
        @click="openPlant(plant)"
        class="bg-surface rounded-2xl p-5 shadow-sm border border-border flex flex-col gap-4 relative group hover:shadow-md transition-shadow cursor-pointer"
      >
        <!-- Delete -->
        <button
          @click.stop="deleteTarget = plant"
          class="absolute top-3 right-3 w-7 h-7 rounded-full bg-red-50 text-red-400 flex items-center justify-center opacity-0 group-hover:opacity-100 max-sm:opacity-100 transition-opacity hover:bg-red-100"
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
            >{{ statusLabel(plant.status) }}</span>
          </div>
        </div>

        <!-- Watering level -->
        <div class="flex flex-col gap-1.5">
          <div class="flex justify-between items-center text-[0.75rem] font-semibold text-text-muted">
            <span class="flex items-center gap-1"><Droplet width="12" height="12" /> {{ t('common.waterLevel') }}</span>
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
            {{ wateringLabel(plant.wateringFrequency) }}
          </span>
          <span class="flex items-center gap-1">
            <Sun width="12" height="12" class="text-[#f59e0b]" />
            {{ sunlightLabel(plant.sunlight) }}
          </span>
        </div>

        <p class="text-[0.75rem] text-text-muted -mt-1">{{ t('common.lastWatered', { time: plant.lastWatered }) }}</p>

        <!-- Mark as watered -->
        <button
          @click.stop="waterPlant(plant.id)"
          :disabled="wateringId === plant.id"
          class="mt-1 flex items-center justify-center gap-1.5 w-full py-2.5 rounded-xl text-[0.8rem] font-semibold bg-light-green-bg text-primary hover:bg-[#d1ebe2] transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
        >
          <Droplet width="14" height="14" />
          {{ wateringId === plant.id ? t('plants.watering') : t('plants.markWatered') }}
        </button>
      </div>
    </div>

    <!-- Edit plant modal -->
    <EditPlantModal v-if="editing" :plant="editing" @close="editing = null" />

    <!-- Delete confirmation -->
    <div
      v-if="deleteTarget"
      class="fixed inset-0 z-50 bg-black/40 flex items-center justify-center p-4"
      @click.self="deleteTarget = null"
    >
      <div class="bg-surface rounded-2xl shadow-xl w-full max-w-[400px] p-6 flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-red-50 flex items-center justify-center">
          <TriangleAlert class="text-red-500" width="26" height="26" />
        </div>
        <div>
          <h2 class="text-[1.15rem] font-bold text-text-main mb-1.5">{{ t('plants.deleteTitle') }}</h2>
          <p class="text-[0.9rem] text-text-muted leading-relaxed">{{ t('plants.deleteDesc', { name: deleteTarget.name }) }}</p>
        </div>
        <div class="flex gap-3 w-full mt-1">
          <button
            @click="deleteTarget = null"
            class="flex-1 py-2.5 rounded-[24px] text-[0.9rem] font-bold border border-border text-text-main hover:bg-bg-app transition-colors"
          >
            {{ t('common.cancel') }}
          </button>
          <button
            @click="confirmDelete"
            :disabled="deleting"
            class="flex-1 py-2.5 rounded-[24px] text-[0.9rem] font-bold bg-red-500 text-white hover:bg-red-600 transition-colors disabled:opacity-60"
          >
            {{ t('plants.deleteConfirm') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
