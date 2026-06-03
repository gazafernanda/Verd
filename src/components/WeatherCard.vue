<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useWeatherStore } from '../stores/weather'
import { Sun, Droplet, Wind } from 'lucide-vue-next'
const { t } = useI18n()
const weather = useWeatherStore()
</script>

<template>
  <div class="bg-surface rounded-lg p-6 shadow-sm flex-1 flex flex-col">
    <div class="flex justify-between items-start mb-6">
      <div>
        <p class="text-[0.75rem] font-bold text-success-green tracking-[1px] mb-1">{{ t('weatherCard.label') }}</p>
        <h3 class="text-2xl font-bold text-text-main">{{ weather.condition }}</h3>
      </div>
      <div class="w-12 h-12 bg-[#fff9eb] rounded-full flex items-center justify-center shrink-0">
        <Sun width="32" height="32" color="#fbbd06" />
      </div>
    </div>

    <div class="flex justify-between items-center mb-8">
      <div class="flex items-start">
        <span class="text-[4.5rem] font-extrabold leading-none tracking-[-2px] text-text-main">{{ weather.temp }}°</span>
        <span class="text-[2rem] font-medium text-text-light mt-2">F</span>
      </div>
      <div class="w-[72px] h-[72px] rounded-[20px] bg-gradient-to-br from-[#a3c4cb] to-[#4b8994] flex items-center justify-center shadow-[0_8px_16px_rgba(75,137,148,0.3)]">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M10 14.5V5a2 2 0 0 1 4 0v9.5a3.5 3.5 0 1 1-4 0"/></svg>
      </div>
    </div>

    <div class="flex gap-4">
      <div class="flex items-center gap-1.5 px-4 py-1.5 rounded-[20px] text-[0.85rem] font-semibold text-[#4b8ae6] bg-[#f0f5ff]">
        <Droplet width="16" height="16" />
        {{ weather.humidity }}%
      </div>
      <div class="flex items-center gap-1.5 px-4 py-1.5 rounded-[20px] text-[0.85rem] font-semibold text-success-green bg-light-green-bg">
        <Wind width="16" height="16" />
        {{ weather.windSpeed }} mph
      </div>
    </div>
  </div>
</template>
