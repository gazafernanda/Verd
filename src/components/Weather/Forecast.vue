<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useWeatherStore } from '../../stores/weather'
import { ChevronLeft, ChevronRight, Sun, CloudSun, CloudRain, Cloud, ArrowUp, ArrowDown } from 'lucide-vue-next'
const { t } = useI18n()
const weather = useWeatherStore()
</script>

<template>
  <div class="mb-8">
    <div class="flex justify-between items-center mb-5">
      <h2 class="text-[1.4rem] font-bold text-text-main m-0">{{ t('weather.forecastTitle') }}</h2>
      <div class="flex gap-2">
        <button class="w-8 h-8 rounded-full border border-border bg-surface flex items-center justify-center text-text-muted shadow-sm transition-colors duration-200 hover:bg-bg-app hover:text-text-main">
          <ChevronLeft width="16" height="16" />
        </button>
        <button class="w-8 h-8 rounded-full border border-border bg-surface flex items-center justify-center text-text-muted shadow-sm transition-colors duration-200 hover:bg-bg-app hover:text-text-main">
          <ChevronRight width="16" height="16" />
        </button>
      </div>
    </div>

    <div class="flex gap-4 overflow-x-auto pb-2 scrollbar-none">
      <div
        v-for="(item, index) in weather.forecast"
        :key="index"
        :class="[
          'flex-1 min-w-[100px] bg-surface border-2 rounded-[40px] py-6 px-4 flex flex-col items-center gap-5 shadow-sm transition-all duration-200',
          item.active ? 'border-success-green shadow-[0_8px_16px_rgba(55,178,126,0.15)]' : 'border-transparent'
        ]"
      >
        <div class="flex flex-col items-center gap-1">
          <span class="text-[0.8rem] font-bold text-text-main">{{ item.day }}</span>
          <span class="text-[0.75rem] font-medium text-text-muted">{{ item.date }}</span>
        </div>

        <div>
          <Sun v-if="item.icon === 'sun'" width="32" height="32" color="#fbbd06" />
          <CloudSun v-else-if="item.icon === 'cloud-sun'" width="32" height="32" color="#4b8ae6" />
          <CloudRain v-else-if="item.icon === 'rain'" width="32" height="32" color="#3b82f6" />
          <Cloud v-else-if="item.icon === 'cloud'" width="32" height="32" color="#9caaa4" />
        </div>

        <div class="flex flex-col items-center gap-1">
          <span class="inline-flex items-center gap-1 text-xl font-bold text-text-main">
            <ArrowUp width="14" height="14" class="text-text-muted" />{{ item.tempHi }}°
          </span>
          <span class="inline-flex items-center gap-1 text-[0.9rem] font-medium text-text-muted">
            <ArrowDown width="13" height="13" />{{ item.tempLo }}°
          </span>
        </div>
      </div>
    </div>
  </div>
</template>
