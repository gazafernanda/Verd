<script setup lang="ts">
/**
 * Every plant the user has ever registered, active and removed alike.
 * Removal is a soft delete, so a plant leaving the garden becomes a row here
 * rather than disappearing.
 */
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { usePlantsStore, type PlantHistoryEntry } from '../stores/plants'
import HelpTip from '../components/HelpTip.vue'
import { ArrowLeft, Flower2, History, ChevronRight, RefreshCw } from 'lucide-vue-next'

const router = useRouter()
const { t, locale } = useI18n()
const plants = usePlantsStore()

const entries = ref<PlantHistoryEntry[]>([])
const loading = ref(true)
const activeFilter = ref<'all' | 'active' | 'ended'>('all')

const filters = computed(() => [
  { key: 'all', label: t('plantHistory.filterAll') },
  { key: 'active', label: t('plantHistory.filterActive') },
  { key: 'ended', label: t('plantHistory.filterEnded') },
])

const filtered = computed(() => {
  if (activeFilter.value === 'active') return entries.value.filter((e) => e.status === 'ACTIVE')
  if (activeFilter.value === 'ended') return entries.value.filter((e) => e.status === 'ENDED')
  return entries.value
})

function formatDate(iso: string | null) {
  if (!iso) return t('plantHistory.stillActive')
  return new Date(iso).toLocaleDateString(locale.value === 'id' ? 'id-ID' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

function openDetail(entry: PlantHistoryEntry) {
  router.push({ name: 'plant-history-detail', params: { id: entry.id } })
}

onMounted(async () => {
  entries.value = await plants.fetchHistory()
  loading.value = false
})
</script>

<template>
  <div class="flex flex-col gap-6">
    <!-- Header -->
    <div>
      <button
        @click="router.push({ name: 'plants' })"
        class="inline-flex items-center gap-1.5 text-[0.85rem] font-semibold text-text-muted hover:text-text-main transition-colors mb-3"
      >
        <ArrowLeft width="15" height="15" />
        {{ t('plantHistory.backToPlants') }}
      </button>

      <span class="block font-semibold text-success-green text-[0.9rem]">{{ t('plantHistory.breadcrumb') }}</span>
      <h1 class="text-[2.2rem] max-sm:text-[1.6rem] font-extrabold text-text-main mb-2 mt-2 tracking-[-0.5px] flex items-center gap-3">
        {{ t('plantHistory.title') }}
        <HelpTip :label="t('help.plantHistory')" />
      </h1>
      <p class="text-text-muted text-[0.95rem] font-medium">
        {{ t('plantHistory.subtitle', entries.length) }}
      </p>
    </div>

    <!-- Filters -->
    <div v-if="entries.length > 0" class="flex gap-2 flex-wrap">
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

    <!-- Loading -->
    <div v-if="loading" class="flex items-center justify-center py-24">
      <RefreshCw class="animate-spin text-text-light" width="28" height="28" />
    </div>

    <!-- Empty -->
    <div v-else-if="entries.length === 0" class="flex flex-col items-center justify-center py-24 gap-5 text-center">
      <div class="w-20 h-20 rounded-full bg-light-green-bg flex items-center justify-center">
        <History class="text-success-green" width="36" height="36" />
      </div>
      <div>
        <h2 class="text-[1.4rem] font-bold text-text-main mb-2">{{ t('plantHistory.emptyTitle') }}</h2>
        <p class="text-text-muted text-[0.95rem]">{{ t('plantHistory.emptyDesc') }}</p>
      </div>
    </div>

    <div v-else-if="filtered.length === 0" class="py-16 text-center text-text-muted font-medium">
      {{ t('plantHistory.noResults') }}
    </div>

    <!-- Desktop table -->
    <div v-else class="max-md:hidden bg-surface border border-border rounded-2xl overflow-hidden shadow-sm">
      <table class="w-full text-left border-collapse">
        <thead>
          <tr class="bg-bg-app text-[0.72rem] font-extrabold uppercase tracking-[0.5px] text-text-muted">
            <th class="px-5 py-3.5">{{ t('plantHistory.colName') }}</th>
            <th class="px-5 py-3.5">{{ t('plantHistory.colRegistered') }}</th>
            <th class="px-5 py-3.5">{{ t('plantHistory.colEnded') }}</th>
            <th class="px-5 py-3.5">{{ t('plantHistory.colStatus') }}</th>
            <th class="px-5 py-3.5"></th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="entry in filtered"
            :key="entry.id"
            @click="openDetail(entry)"
            class="border-t border-border cursor-pointer hover:bg-bg-app transition-colors"
          >
            <td class="px-5 py-4">
              <div class="flex items-center gap-3">
                <div
                  class="w-10 h-10 rounded-lg flex items-center justify-center shrink-0 border border-border"
                  :style="{ backgroundColor: entry.iconBg }"
                >
                  <Flower2 width="20" height="20" style="color: #1a5641" />
                </div>
                <div class="min-w-0">
                  <p class="font-bold text-text-main text-[0.92rem] truncate">{{ entry.name }}</p>
                  <p class="text-text-muted text-[0.78rem] truncate">{{ entry.category }}</p>
                </div>
              </div>
            </td>
            <td class="px-5 py-4 text-[0.88rem] text-text-main">{{ formatDate(entry.registeredAt) }}</td>
            <td class="px-5 py-4 text-[0.88rem]" :class="entry.endedAt ? 'text-text-main' : 'text-text-light'">
              {{ formatDate(entry.endedAt) }}
            </td>
            <td class="px-5 py-4">
              <span
                class="inline-block text-[0.65rem] font-extrabold px-2 py-1 rounded tracking-[0.5px]"
                :class="entry.status === 'ACTIVE'
                  ? 'bg-light-green-bg text-success-green'
                  : 'bg-bg-app text-text-muted'"
              >
                {{ entry.status === 'ACTIVE' ? t('plantHistory.statusActive') : t('plantHistory.statusEnded') }}
              </span>
              <span class="block mt-1 text-[0.72rem] text-text-light">
                {{ t('plantHistory.duration', entry.durationDays) }}
              </span>
            </td>
            <td class="px-5 py-4 text-right">
              <ChevronRight class="text-text-light inline" width="18" height="18" />
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Mobile cards: a five-column table doesn't survive a phone width -->
    <div v-if="!loading && filtered.length > 0" class="md:hidden flex flex-col gap-3">
      <button
        v-for="entry in filtered"
        :key="entry.id"
        @click="openDetail(entry)"
        class="bg-surface border border-border rounded-2xl p-4 shadow-sm text-left flex items-center gap-3 hover:bg-bg-app transition-colors"
      >
        <div
          class="w-11 h-11 rounded-lg flex items-center justify-center shrink-0 border border-border"
          :style="{ backgroundColor: entry.iconBg }"
        >
          <Flower2 width="22" height="22" style="color: #1a5641" />
        </div>
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2">
            <p class="font-bold text-text-main text-[0.95rem] truncate">{{ entry.name }}</p>
            <span
              class="shrink-0 text-[0.6rem] font-extrabold px-1.5 py-0.5 rounded tracking-[0.5px]"
              :class="entry.status === 'ACTIVE'
                ? 'bg-light-green-bg text-success-green'
                : 'bg-bg-app text-text-muted'"
            >
              {{ entry.status === 'ACTIVE' ? t('plantHistory.statusActive') : t('plantHistory.statusEnded') }}
            </span>
          </div>
          <p class="text-[0.78rem] text-text-muted mt-0.5">
            {{ formatDate(entry.registeredAt) }} → {{ formatDate(entry.endedAt) }}
          </p>
        </div>
        <ChevronRight class="text-text-light shrink-0" width="18" height="18" />
      </button>
    </div>
  </div>
</template>
