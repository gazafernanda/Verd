<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { Droplet, Sun, Scissors, Leaf, Sprout } from 'lucide-vue-next'
import { useRecommendationsStore } from '../../stores/recommendations'

const { t } = useI18n()
const recs = useRecommendationsStore()

const priorityColors: Record<string, string> = {
  IMMEDIATE: 'bg-primary text-white',
  RECOMMENDED: 'bg-[#f59e0b] text-white',
  OPTIONAL: 'bg-surface text-text-muted border border-border',
}

const typeColors: Record<string, { bg: string; icon: string }> = {
  water:     { bg: 'bg-light-green-bg text-success-green', icon: 'green' },
  mist:      { bg: 'bg-[#ebf5ff] text-[#3b82f6]', icon: 'blue' },
  shade:     { bg: 'bg-[#fff4e5] text-[#f59e0b]', icon: 'amber' },
  fertilize: { bg: 'bg-[#f3e8ff] text-[#9333ea]', icon: 'purple' },
  prune:     { bg: 'bg-[#fce8e8] text-[#ef4444]', icon: 'red' },
}

const borderColors: Record<string, string> = {
  water:     'border-[rgba(55,178,126,0.5)]',
  mist:      'border-[rgba(59,130,246,0.4)]',
  shade:     'border-[rgba(251,146,60,0.4)]',
  fertilize: 'border-[rgba(147,51,234,0.3)]',
  prune:     'border-[rgba(239,68,68,0.3)]',
}
</script>

<template>
  <div class="flex flex-col">
    <div class="flex items-center gap-4 mb-6">
      <h2 class="text-[1.35rem] font-extrabold text-text-main m-0 whitespace-nowrap">{{ t('recommendation.priorityActions') }}</h2>
      <div class="flex-1 h-0.5 bg-border"></div>
    </div>

    <div v-if="recs.loading" class="flex flex-col gap-5">
      <div v-for="i in 2" :key="i" class="h-28 bg-surface rounded-xl border border-border animate-pulse"></div>
    </div>

    <div v-else-if="recs.priorityActions.length === 0" class="py-8 text-center text-text-muted text-[0.9rem]">
      {{ t('recommendation.noActions') }}
    </div>

    <div v-else class="flex flex-col gap-5">
      <div
        v-for="action in recs.priorityActions"
        :key="action.id"
        class="flex gap-4 bg-surface rounded-xl p-5 shadow-sm border"
        :class="borderColors[action.type] ?? 'border-border'"
      >
        <div class="shrink-0">
          <div class="w-10 h-10 rounded-full flex items-center justify-center"
            :class="typeColors[action.type]?.bg ?? 'bg-surface text-text-muted'">
            <Droplet v-if="action.type === 'water' || action.type === 'mist'" width="20" height="20" />
            <Sun v-else-if="action.type === 'shade'" width="20" height="20" />
            <Scissors v-else-if="action.type === 'prune'" width="20" height="20" />
            <Sprout v-else-if="action.type === 'fertilize'" width="20" height="20" />
            <Leaf v-else width="20" height="20" />
          </div>
        </div>
        <div class="flex-1 min-w-0 flex flex-col">
          <div class="flex flex-wrap justify-between items-start gap-2 mb-2">
            <h3 class="text-[1.05rem] font-extrabold text-text-main m-0">{{ action.title }}</h3>
            <span class="text-[0.65rem] font-extrabold px-2.5 py-1 rounded tracking-[0.5px] shrink-0"
              :class="priorityColors[action.priority] ?? 'bg-surface text-text-muted'">
              {{ action.priority }}
            </span>
          </div>
          <p class="text-[0.9rem] text-text-muted leading-relaxed m-0">
            {{ action.description }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
