<script setup lang="ts">
import { useWeatherStore } from '../../stores/weather'
import { NavArrowLeft, NavArrowRight, SunLight, CloudSunny, HeavyRain, Cloud } from '@iconoir/vue'
const weather = useWeatherStore()
</script>

<template>
  <div class="mb-8">
    <div class="flex justify-between items-center mb-5">
      <h2 class="text-[1.4rem] font-bold text-text-main m-0">7-Day Forecast</h2>
      <div class="flex gap-2">
        <button class="w-8 h-8 rounded-full border border-border bg-surface flex items-center justify-center text-text-muted shadow-sm transition-colors duration-200 hover:bg-bg-app hover:text-text-main">
          <NavArrowLeft width="16" height="16" />
        </button>
        <button class="w-8 h-8 rounded-full border border-border bg-surface flex items-center justify-center text-text-muted shadow-sm transition-colors duration-200 hover:bg-bg-app hover:text-text-main">
          <NavArrowRight width="16" height="16" />
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
          <SunLight v-if="item.icon === 'sun'" width="32" height="32" color="#fbbd06" />
          <CloudSunny v-else-if="item.icon === 'cloud-sun'" width="32" height="32" color="#4b8ae6" />
          <HeavyRain v-else-if="item.icon === 'rain'" width="32" height="32" color="#3b82f6" />
          <Cloud v-else-if="item.icon === 'cloud'" width="32" height="32" color="#9caaa4" />
        </div>

        <div class="flex flex-col items-center gap-1">
          <span class="text-xl font-bold text-text-main">{{ item.tempHi }}°</span>
          <span class="text-[0.9rem] font-medium text-text-muted">{{ item.tempLo }}°</span>
        </div>
      </div>
    </div>
  </div>
</template>
