<script setup lang="ts">
import { computed } from 'vue'
import { useWeatherStore } from '../../stores/weather'

const weather = useWeatherStore()

const metrics = computed(() => [
  {
    label: 'HUMIDITY',
    value: `${weather.humidity}%`,
    icon: '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M176.81,95A64.12,64.12,0,0,0,128,64a64.12,64.12,0,0,0-48.81,31C61.85,123,54.7,159.08,71,185.81a64,64,0,0,0,114,0C201.3,159.08,194.15,123,176.81,95Zm-12.06,82.47a48,48,0,0,1-73.5,0c-11.83-19.34-6.42-47.53,7.57-69.51A48.06,48.06,0,0,1,128,80a48.06,48.06,0,0,1,29.18,27.93C171.17,130,176.58,158.13,164.75,177.47Z"/></svg>',
  },
  {
    label: 'UV INDEX',
    value: `${weather.uvIndex} (${weather.uvLabel})`,
    icon: '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,24a104,104,0,1,0,104,104A104.11,104.11,0,0,0,128,24Zm0,192a88,88,0,1,1,88-88A88.1,88.1,0,0,1,128,216ZM168,128a40,40,0,1,1-40-40A40,40,0,0,1,168,128Z"/></svg>',
  },
  {
    label: 'SOIL MOISTURE',
    value: `${weather.soilMoisture}%`,
    icon: '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M174.55,101.44a80,80,0,1,1-93.1,0A153.25,153.25,0,0,1,128,31.76a153.25,153.25,0,0,1,46.55,69.68Z"/></svg>',
  },
  {
    label: 'WIND SPEED',
    value: `${weather.windSpeed} mph`,
    icon: '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M184,184a32,32,0,0,1-32,32H40a8,8,0,0,1,0-16H152a16,16,0,0,0,0-32H24a8,8,0,0,1,0-16H184A32,32,0,0,1,184,184Zm32-80a32,32,0,0,0-32-32H72a8,8,0,0,0,0,16H184a16,16,0,0,1,0,32H32a8,8,0,0,0,0,16H184A32,32,0,0,0,216,104Z"/></svg>',
  },
])
</script>

<template>
  <div class="bg-surface rounded-xl p-8 shadow-sm mb-8">
    <div class="flex justify-between items-start mb-6">
      <span class="text-[0.8rem] font-bold text-success-green tracking-[1px]">CURRENT CONDITIONS</span>
      <div class="flex flex-col items-end border border-success-green rounded-[20px] px-4 py-1.5 bg-surface">
        <span class="text-[0.6rem] font-bold text-text-muted tracking-[0.5px]">AQI INDEX</span>
        <span class="text-[0.85rem] font-bold text-success-green">{{ weather.aqi }} - Excellent</span>
      </div>
    </div>

    <div class="flex items-center gap-6 mb-3">
      <div class="flex items-start">
        <span class="text-[6.5rem] font-extrabold leading-[0.9] tracking-[-3px] text-text-main">{{ weather.temp }}°</span>
        <span class="text-[2.5rem] font-semibold text-text-main mt-2">F</span>
      </div>
      <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 256 256"><path fill="#fbbd06" d="M120,40V16a8,8,0,0,1,16,0V40a8,8,0,0,1-16,0Zm8,24a64,64,0,1,0,64,64A64.07,64.07,0,0,0,128,64Zm0,112a48,48,0,1,1,48-48A48.05,48.05,0,0,1,128,176ZM58.34,69.66A8,8,0,0,0,69.66,58.34l-17-17a8,8,0,0,0-11.32,11.32Zm0,116.68-17,17a8,8,0,0,0,11.32,11.32l17-17a8,8,0,0,0-11.32-11.32ZM203.31,197.66a8,8,0,0,0,11.33-11.32l-17-17a8,8,0,0,0-11.32,11.32ZM216,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,216,120ZM40,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,40,120ZM183,52.48a8,8,0,0,0-5.63-13.68H144a8,8,0,0,0,0,16h33.35A8,8,0,0,0,183,52.48Z"/></svg>
    </div>

    <p class="text-xl text-text-muted font-medium mb-8">{{ weather.condition }} • Feels like {{ weather.feelsLike }}°F</p>

    <div class="grid grid-cols-4 gap-4">
      <div v-for="metric in metrics" :key="metric.label"
           class="flex flex-col gap-4 py-5 px-6 rounded-[40px] bg-[#f6fbfa] shadow-[0_4px_12px_rgba(55,178,126,0.05)]">
        <div class="w-8 h-8 rounded-full bg-success-green flex items-center justify-center text-white" v-html="metric.icon"></div>
        <div class="flex flex-col gap-1">
          <span class="text-[0.65rem] font-bold text-text-muted tracking-[0.5px]">{{ metric.label }}</span>
          <span class="text-lg font-bold text-text-main">{{ metric.value }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
