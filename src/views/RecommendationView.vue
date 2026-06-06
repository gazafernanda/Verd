<script setup lang="ts">
import { onMounted, ref, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import MetricsRow from '../components/Recommendation/MetricsRow.vue'
import PriorityActions from '../components/Recommendation/PriorityActions.vue'
import BotanicalInsights from '../components/Recommendation/BotanicalInsights.vue'
import { Leaf as LeafIcon, RefreshCw, Droplet, Sun, Scissors, Sprout, Leaf, CalendarDays, ChevronDown, Check } from 'lucide-vue-next'
import { useRecommendationsStore } from '../stores/recommendations'
import { usePlantsStore } from '../stores/plants'

const { t, te } = useI18n()
const recs = useRecommendationsStore()
const plantsStore = usePlantsStore()

const menuOpen = ref(false)
const menuRef = ref<HTMLElement | null>(null)

function toggleMenu() {
  menuOpen.value = !menuOpen.value
}

function selectPlant(id: number, name: string) {
  menuOpen.value = false
  recs.generateForPlant(id, name)
}

function selectAllPlants() {
  menuOpen.value = false
  recs.generatingFor = null
  recs.fetchRecommendations()
}

function onClickOutside(e: MouseEvent) {
  if (menuRef.value && !menuRef.value.contains(e.target as Node)) {
    menuOpen.value = false
  }
}

function dayLabel(day: string) {
  const key = `recommendation.days.${day}`
  return te(key) ? t(key) : day
}

const typeStyle: Record<string, string> = {
  water: 'bg-light-green-bg text-success-green',
  mist: 'bg-[#ebf5ff] text-[#3b82f6]',
  shade: 'bg-[#fff4e5] text-[#f59e0b]',
  fertilize: 'bg-[#f3e8ff] text-[#9333ea]',
  prune: 'bg-[#fce8e8] text-[#ef4444]',
  none: 'bg-bg-app text-text-muted',
}

onMounted(() => {
  if (!recs.generatingFor) recs.fetchRecommendations()
  if (plantsStore.plants.length === 0) plantsStore.fetchPlants()
  document.addEventListener('click', onClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', onClickOutside)
})
</script>

<template>
  <div class="flex flex-col gap-8 max-w-[1200px]">
    <div class="flex flex-col items-start gap-4 max-w-[600px]">
      <div class="inline-flex items-center gap-2 px-3 py-1.5 bg-light-green-bg rounded-[16px] text-[0.65rem] font-extrabold text-success-green tracking-[0.5px]">
        {{ t('recommendation.liveAnalysis') }}
        <span class="w-1.5 h-1.5 bg-red-500 rounded-full animate-pulse-dot"></span>
      </div>

      <h1 class="text-[3rem] max-lg:text-[2rem] max-sm:text-[1.6rem] font-extrabold text-text-main leading-[1.1] tracking-[-1px] m-0">
        <template v-if="recs.generatingFor">{{ recs.generatingFor }}<br></template>
        {{ t('recommendation.title') }}
      </h1>
      <p class="text-base font-medium text-text-muted leading-relaxed mb-2 m-0">
        <template v-if="recs.generatingFor">{{ t('recommendation.forPlanSubtitle', { name: recs.generatingFor }) }}</template>
        <template v-else>{{ t('recommendation.genericSubtitle') }}</template>
      </p>

      <div class="flex gap-3 max-sm:flex-col max-sm:w-full">
        <div ref="menuRef" class="relative max-sm:w-full">
          <button
            @click.stop="toggleMenu"
            class="w-full inline-flex items-center justify-between gap-2 px-5 py-2.5 rounded-xl text-[0.9rem] font-semibold cursor-pointer transition-colors duration-200 bg-transparent text-text-main border border-border hover:bg-surface">
            <span class="inline-flex items-center gap-2">
              <LeafIcon width="16" height="16" />
              {{ recs.generatingFor ?? t('recommendation.switchPlant') }}
            </span>
            <ChevronDown width="16" height="16" :class="{ 'rotate-180': menuOpen }" class="transition-transform duration-200" />
          </button>

          <div
            v-if="menuOpen"
            class="absolute left-0 top-[calc(100%+0.5rem)] z-20 min-w-full sm:min-w-[240px] bg-surface border border-border rounded-xl shadow-lg p-1.5 max-h-[320px] overflow-y-auto">
            <button
              @click="selectAllPlants"
              class="w-full flex items-center justify-between gap-2 px-3 py-2 rounded-lg text-[0.85rem] font-semibold text-text-main hover:bg-bg-app transition-colors duration-150">
              {{ t('recommendation.allPlants') }}
              <Check v-if="!recs.generatingFor" width="15" height="15" class="text-success-green" />
            </button>
            <button
              v-for="p in plantsStore.plants"
              :key="p.id"
              @click="selectPlant(p.id, p.name)"
              class="w-full flex items-center justify-between gap-2 px-3 py-2 rounded-lg text-[0.85rem] font-medium text-text-main hover:bg-bg-app transition-colors duration-150">
              <span class="truncate">{{ p.name }}</span>
              <Check v-if="recs.generatingFor === p.name" width="15" height="15" class="text-success-green shrink-0" />
            </button>
            <p v-if="plantsStore.plants.length === 0" class="px-3 py-2 text-[0.85rem] text-text-muted">
              {{ t('recommendation.noPlants') }}
            </p>
          </div>
        </div>
        <button
          @click="recs.fetchRecommendations()"
          class="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl text-[0.9rem] font-semibold cursor-pointer transition-colors duration-200 bg-primary text-white border border-primary shadow-[0_4px_12px_rgba(26,86,65,0.2)] hover:bg-primary-hover">
          <RefreshCw width="16" height="16" :class="{ 'animate-spin': recs.loading }" />
          {{ t('recommendation.updateData') }}
        </button>
      </div>
    </div>

    <!-- 7-Day Care Plan -->
    <div v-if="recs.loading || recs.weeklyPlan.length > 0" class="flex flex-col gap-5">
      <div class="flex items-center gap-3">
        <CalendarDays class="text-success-green shrink-0" width="22" height="22" />
        <div>
          <h2 class="text-[1.35rem] font-extrabold text-text-main m-0">{{ t('recommendation.weeklyPlanTitle') }}</h2>
          <p v-if="recs.generatingFor" class="text-[0.85rem] text-text-muted m-0 mt-0.5">
            {{ t('recommendation.weeklyPlanSubtitle', { name: recs.generatingFor }) }}
          </p>
        </div>
      </div>

      <!-- Loading skeleton -->
      <div v-if="recs.loading" class="grid grid-cols-7 max-lg:grid-cols-2 max-sm:grid-cols-1 gap-3">
        <div v-for="i in 7" :key="i" class="h-40 bg-surface rounded-xl border border-border animate-pulse"></div>
      </div>

      <!-- Day cards -->
      <div v-else class="grid grid-cols-7 max-lg:grid-cols-2 max-sm:grid-cols-1 gap-3">
        <div
          v-for="(d, i) in recs.weeklyPlan"
          :key="i"
          class="bg-surface rounded-xl border border-border shadow-sm p-4 flex flex-col gap-2"
          :class="{ 'border-success-green': i === 0 }"
        >
          <div class="flex flex-col">
            <span class="text-[0.7rem] font-extrabold tracking-[0.5px]" :class="i === 0 ? 'text-success-green' : 'text-text-muted'">{{ dayLabel(d.day) }}</span>
            <span class="text-[0.75rem] text-text-light font-medium">{{ d.date }}</span>
          </div>
          <span class="text-[0.7rem] font-semibold text-text-muted">{{ d.weather }}</span>
          <div
            class="w-7 h-7 rounded-full flex items-center justify-center mt-1"
            :class="typeStyle[d.type] ?? typeStyle.none"
          >
            <Droplet v-if="d.type === 'water' || d.type === 'mist'" width="14" height="14" />
            <Sun v-else-if="d.type === 'shade'" width="14" height="14" />
            <Scissors v-else-if="d.type === 'prune'" width="14" height="14" />
            <Sprout v-else-if="d.type === 'fertilize'" width="14" height="14" />
            <Leaf v-else width="14" height="14" />
          </div>
          <p class="text-[0.8rem] text-text-main leading-snug m-0 flex-1">{{ d.action }}</p>
        </div>
      </div>
    </div>

    <MetricsRow />

    <div class="grid grid-cols-[2fr_1fr] gap-8 max-lg:grid-cols-1 max-lg:gap-6">
      <PriorityActions />
      <BotanicalInsights />
    </div>
  </div>
</template>
