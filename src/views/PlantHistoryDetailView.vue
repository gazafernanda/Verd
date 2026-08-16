<script setup lang="ts">
/**
 * One planting period in full, including every monitoring entry recorded while
 * the plant was in the garden. Works for removed plants — that is what the soft
 * delete exists for.
 */
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { usePlantsStore, type PlantHistoryDetail } from '../stores/plants'
import HelpTip from '../components/HelpTip.vue'
import {
  ArrowLeft, Flower2, RefreshCw, Droplet, Sun, CalendarDays,
  CalendarCheck, ClipboardList, Sprout,
} from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const { t, locale } = useI18n()
const plants = usePlantsStore()

const detail = ref<PlantHistoryDetail | null>(null)
const loading = ref(true)

const plantId = computed(() => Number(route.params.id))

function formatDate(iso: string | null) {
  if (!iso) return t('plantHistory.stillActive')
  return new Date(iso).toLocaleDateString(locale.value === 'id' ? 'id-ID' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

function formatDateTime(iso: string) {
  return new Date(iso).toLocaleString(locale.value === 'id' ? 'id-ID' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

/** Falls back to the raw action so an unrecognised entry still renders. */
function actionLabel(action: string) {
  const key = `plantHistory.action.${action}`
  const translated = t(key)
  return translated === key ? action : translated
}

function wateringLabel(freq: string) {
  return freq ? t(`addPlant.watering.${freq}`) : '—'
}

const sunlightKeyMap: Record<string, string> = {
  'full-sun': 'fullSunLabel',
  partial: 'partialLabel',
  indirect: 'indirectLabel',
  low: 'lowLabel',
}

function sunlightLabel(value: string) {
  return t(`addPlant.sunlight.${sunlightKeyMap[value] ?? 'indirectLabel'}`)
}

onMounted(async () => {
  detail.value = await plants.fetchHistoryDetail(plantId.value)
  loading.value = false
})
</script>

<template>
  <div class="flex flex-col gap-6">
    <button
      @click="router.push({ name: 'plant-history' })"
      class="inline-flex items-center gap-1.5 text-[0.85rem] font-semibold text-text-muted hover:text-text-main transition-colors self-start"
    >
      <ArrowLeft width="15" height="15" />
      {{ t('plantHistory.title') }}
    </button>

    <!-- Loading -->
    <div v-if="loading" class="flex items-center justify-center py-24">
      <RefreshCw class="animate-spin text-text-light" width="28" height="28" />
    </div>

    <!-- Not found -->
    <div v-else-if="!detail" class="py-24 text-center text-text-muted font-medium">
      {{ t('plantHistory.notFound') }}
    </div>

    <template v-else>
      <!-- Summary -->
      <div class="bg-surface border border-border rounded-2xl p-6 max-sm:p-5 shadow-sm">
        <div class="flex items-start gap-4">
          <div
            class="w-16 h-16 rounded-xl flex items-center justify-center shrink-0 border border-border"
            :style="{ backgroundColor: detail.summary.iconBg }"
          >
            <Flower2 width="32" height="32" style="color: #1a5641" />
          </div>
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-2 flex-wrap">
              <h1 class="text-[1.6rem] max-sm:text-[1.3rem] font-extrabold text-text-main tracking-[-0.5px]">
                {{ detail.summary.name }}
              </h1>
              <span
                class="text-[0.65rem] font-extrabold px-2 py-1 rounded tracking-[0.5px]"
                :class="detail.summary.status === 'ACTIVE'
                  ? 'bg-light-green-bg text-success-green'
                  : 'bg-bg-app text-text-muted'"
              >
                {{ detail.summary.status === 'ACTIVE'
                  ? t('plantHistory.statusActive')
                  : t('plantHistory.statusEnded') }}
              </span>
            </div>
            <p class="text-text-muted text-[0.9rem] mt-0.5">{{ detail.summary.category }}</p>
          </div>
        </div>

        <!-- Period -->
        <div class="grid grid-cols-3 max-sm:grid-cols-1 gap-4 mt-6 pt-6 border-t border-border">
          <div class="flex items-start gap-3">
            <CalendarDays class="text-success-green shrink-0 mt-0.5" width="18" height="18" />
            <div>
              <p class="text-[0.72rem] font-extrabold uppercase tracking-[0.5px] text-text-muted">
                {{ t('plantHistory.colRegistered') }}
              </p>
              <p class="text-[0.95rem] font-bold text-text-main mt-0.5">
                {{ formatDate(detail.summary.registeredAt) }}
              </p>
            </div>
          </div>

          <div class="flex items-start gap-3">
            <CalendarCheck class="text-success-green shrink-0 mt-0.5" width="18" height="18" />
            <div>
              <p class="text-[0.72rem] font-extrabold uppercase tracking-[0.5px] text-text-muted">
                {{ t('plantHistory.colEnded') }}
              </p>
              <p class="text-[0.95rem] font-bold mt-0.5"
                 :class="detail.summary.endedAt ? 'text-text-main' : 'text-text-light'">
                {{ formatDate(detail.summary.endedAt) }}
              </p>
            </div>
          </div>

          <div class="flex items-start gap-3">
            <Sprout class="text-success-green shrink-0 mt-0.5" width="18" height="18" />
            <div>
              <p class="text-[0.72rem] font-extrabold uppercase tracking-[0.5px] text-text-muted">
                {{ t('plantHistory.detailTitle') }}
              </p>
              <p class="text-[0.95rem] font-bold text-text-main mt-0.5">
                {{ t('plantHistory.duration', detail.summary.durationDays) }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Care settings -->
      <div class="bg-surface border border-border rounded-2xl p-6 max-sm:p-5 shadow-sm">
        <h2 class="text-[1.05rem] font-bold text-text-main mb-4">{{ t('plantHistory.careDetails') }}</h2>
        <div class="flex flex-col gap-3">
          <div class="flex items-center gap-2.5 text-[0.9rem]">
            <Droplet width="15" height="15" class="text-[#3b82f6] shrink-0" />
            <span class="text-text-muted">{{ t('plantHistory.wateringLabel') }}:</span>
            <span class="font-semibold text-text-main">{{ wateringLabel(detail.wateringFrequency) }}</span>
          </div>
          <div class="flex items-center gap-2.5 text-[0.9rem]">
            <Sun width="15" height="15" class="text-[#f59e0b] shrink-0" />
            <span class="text-text-muted">{{ t('plantHistory.sunlightLabel') }}:</span>
            <span class="font-semibold text-text-main">{{ sunlightLabel(detail.sunlight) }}</span>
          </div>
          <div class="flex items-start gap-2.5 text-[0.9rem]">
            <ClipboardList width="15" height="15" class="text-text-muted shrink-0 mt-1" />
            <span class="text-text-muted shrink-0">{{ t('plantHistory.notesLabel') }}:</span>
            <span class="text-text-main">{{ detail.notes || t('plantHistory.noNotes') }}</span>
          </div>
        </div>
      </div>

      <!-- Monitoring data for the period -->
      <div class="bg-surface border border-border rounded-2xl p-6 max-sm:p-5 shadow-sm">
        <div class="flex items-center gap-2 mb-1">
          <h2 class="text-[1.05rem] font-bold text-text-main">{{ t('plantHistory.monitoringTitle') }}</h2>
          <HelpTip :label="t('help.monitoringLog')" />
        </div>
        <p class="text-[0.85rem] text-text-muted mb-5 leading-relaxed">{{ t('plantHistory.monitoringDesc') }}</p>

        <p v-if="detail.logs.length === 0" class="text-[0.9rem] text-text-light py-6 text-center">
          {{ t('plantHistory.noLogs') }}
        </p>

        <ol v-else class="flex flex-col">
          <li
            v-for="(log, i) in detail.logs"
            :key="log.id"
            class="flex gap-4 pb-4"
            :class="i < detail.logs.length - 1 ? 'border-l-2 border-border ml-[7px] pl-6 relative' : 'ml-[7px] pl-6 relative'"
          >
            <!-- Timeline dot -->
            <span
              class="absolute -left-[7px] top-1 w-3 h-3 rounded-full bg-success-green border-2 border-surface"
            ></span>
            <div class="min-w-0">
              <p class="font-bold text-text-main text-[0.9rem]">{{ actionLabel(log.action) }}</p>
              <p class="text-[0.78rem] text-text-muted mt-0.5">{{ formatDateTime(log.loggedAt) }}</p>
              <p v-if="log.notes" class="text-[0.85rem] text-text-main mt-1.5 leading-relaxed">{{ log.notes }}</p>
            </div>
          </li>
        </ol>

        <p v-if="detail.logs.length > 0" class="mt-2 text-[0.78rem] text-text-light">
          {{ t('plantHistory.entries', detail.logs.length) }}
        </p>
      </div>
    </template>
  </div>
</template>
