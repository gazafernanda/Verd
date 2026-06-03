<script setup lang="ts">
import { onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import MetricsRow from '../components/Recommendation/MetricsRow.vue'
import PriorityActions from '../components/Recommendation/PriorityActions.vue'
import BotanicalInsights from '../components/Recommendation/BotanicalInsights.vue'
import { Calendar, RefreshCw } from 'lucide-vue-next'
import { useRecommendationsStore } from '../stores/recommendations'

const { t } = useI18n()
const recs = useRecommendationsStore()

onMounted(() => {
  if (!recs.generatingFor) recs.fetchRecommendations()
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
        <button class="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl text-[0.9rem] font-semibold cursor-pointer transition-colors duration-200 bg-transparent text-text-main border border-border hover:bg-surface">
          <Calendar width="16" height="16" />
          {{ t('recommendation.switchCategory') }}
        </button>
        <button
          @click="recs.fetchRecommendations()"
          class="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl text-[0.9rem] font-semibold cursor-pointer transition-colors duration-200 bg-primary text-white border border-primary shadow-[0_4px_12px_rgba(26,86,65,0.2)] hover:bg-primary-hover">
          <RefreshCw width="16" height="16" :class="{ 'animate-spin': recs.loading }" />
          {{ t('recommendation.updateData') }}
        </button>
      </div>
    </div>

    <MetricsRow />

    <div class="grid grid-cols-[2fr_1fr] gap-8 max-lg:grid-cols-1 max-lg:gap-6">
      <PriorityActions />
      <BotanicalInsights />
    </div>
  </div>
</template>
