<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { usePlantsStore } from '../../stores/plants'
const { t } = useI18n()
const plants = usePlantsStore()
</script>

<template>
  <div class="bg-surface rounded-xl p-6 shadow-sm border border-border">
    <div class="mb-6">
      <h2 class="text-xl font-extrabold text-text-main m-0">{{ t('profile.activityStats') }}</h2>
    </div>

    <div class="flex gap-4">
      <div class="flex-1 bg-bg-app rounded-lg py-6 px-4 flex flex-col items-center justify-center border border-[rgba(26,86,65,0.05)]">
        <span class="text-[2.5rem] font-extrabold text-primary leading-none mb-2">{{ plants.totalPlants }}</span>
        <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[0.5px] uppercase">{{ t('profile.totalPlants') }}</span>
      </div>
      <div class="flex-1 bg-bg-app rounded-lg py-6 px-4 flex flex-col items-center justify-center border border-[rgba(26,86,65,0.05)]">
        <span class="text-[2.5rem] font-extrabold text-primary leading-none mb-2">{{ plants.careStreak }}</span>
        <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[0.5px] uppercase">{{ t('profile.careStreak') }}</span>
      </div>
    </div>
  </div>
</template>
