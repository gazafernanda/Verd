<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { Leaf } from 'lucide-vue-next'
import { useRecommendationsStore } from '../../stores/recommendations'

const { t } = useI18n()
const recs = useRecommendationsStore()
</script>

<template>
  <div class="flex flex-col">
    <div class="mb-6 h-6 flex items-center">
      <h2 class="text-[1.35rem] font-extrabold text-text-main m-0">{{ t('recommendation.botanicalInsights') }}</h2>
    </div>

    <div v-if="recs.loading" class="bg-light-green-bg rounded-xl py-8 px-7 border border-[rgba(55,178,126,0.15)] animate-pulse h-48"></div>

    <div v-else-if="recs.insight" class="bg-light-green-bg rounded-xl py-8 px-7 border border-[rgba(55,178,126,0.15)]">
      <span class="block text-[0.7rem] font-extrabold text-success-green tracking-[1px] mb-6">{{ t('recommendation.whyActions') }}</span>

      <p class="text-[1.15rem] font-bold text-text-main leading-relaxed mb-8">
        {{ recs.insight.headline }}
      </p>

      <ul class="list-none p-0 m-0">
        <li class="flex items-start gap-3">
          <div class="mt-1 text-primary shrink-0">
            <Leaf width="16" height="16" />
          </div>
          <p class="text-[0.95rem] text-text-muted leading-relaxed font-medium m-0">
            {{ recs.insight.detail }}
          </p>
        </li>
      </ul>
    </div>

    <div v-else class="bg-light-green-bg rounded-xl py-8 px-7 border border-[rgba(55,178,126,0.15)] text-text-muted text-[0.9rem]">
      {{ t('recommendation.noInsights') }}
    </div>
  </div>
</template>
