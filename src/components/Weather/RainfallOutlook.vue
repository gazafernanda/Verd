<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { CloudRain } from 'lucide-vue-next'
const { t } = useI18n()
</script>

<template>
  <div class="bg-surface rounded-xl p-6 shadow-sm mb-6">
    <div class="flex items-center gap-3 mb-6">
      <div class="w-6 h-6 bg-[#e6f0ff] rounded-full flex items-center justify-center text-[#3b82f6] shrink-0">
        <CloudRain width="16" height="16" />
      </div>
      <h3 class="text-base font-bold text-text-main m-0">{{ t('weather.rainfallTitle') }}</h3>
    </div>

    <div class="flex flex-col gap-4 mb-5">
      <div class="flex items-center gap-3">
        <span class="w-[50px] text-[0.65rem] font-bold text-text-muted leading-tight whitespace-pre-line">{{ t('weather.rainfallThisWeek') }}</span>
        <div class="flex-1 h-2 bg-[#f3f4f6] rounded overflow-hidden">
          <div class="h-full rounded bg-[#3b82f6]" style="width: 60%"></div>
        </div>
        <span class="w-9 text-[0.8rem] font-bold text-text-main text-right">12mm</span>
      </div>
      <div class="flex items-center gap-3">
        <span class="w-[50px] text-[0.65rem] font-bold text-text-muted leading-tight">{{ t('weather.rainfallAverage') }}</span>
        <div class="flex-1 h-2 bg-[#f3f4f6] rounded overflow-hidden">
          <div class="h-full rounded bg-[#e5e7eb]" style="width: 40%"></div>
        </div>
        <span class="w-9 text-[0.8rem] font-bold text-text-main text-right">8mm</span>
      </div>
    </div>

    <p class="text-[0.75rem] text-text-muted leading-relaxed">
      {{ t('weather.rainfallNote') }}
    </p>
  </div>
</template>
