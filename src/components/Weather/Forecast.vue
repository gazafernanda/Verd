<script setup lang="ts">
import { useWeatherStore } from '../../stores/weather'
const weather = useWeatherStore()
</script>

<template>
  <div class="mb-8">
    <div class="flex justify-between items-center mb-5">
      <h2 class="text-[1.4rem] font-bold text-text-main m-0">7-Day Forecast</h2>
      <div class="flex gap-2">
        <button class="w-8 h-8 rounded-full border border-border bg-surface flex items-center justify-center text-text-muted shadow-sm transition-colors duration-200 hover:bg-bg-app hover:text-text-main">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M165.66,202.34a8,8,0,0,1-11.32,11.32l-80-80a8,8,0,0,1,0-11.32l80-80a8,8,0,0,1,11.32,11.32L91.31,128Z"/></svg>
        </button>
        <button class="w-8 h-8 rounded-full border border-border bg-surface flex items-center justify-center text-text-muted shadow-sm transition-colors duration-200 hover:bg-bg-app hover:text-text-main">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M181.66,133.66l-80,80a8,8,0,0,1-11.32-11.32L164.69,128,90.34,53.66a8,8,0,0,1,11.32-11.32l80,80A8,8,0,0,1,181.66,133.66Z"/></svg>
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
          <svg v-if="item.icon === 'sun'" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#fbbd06" d="M120,40V16a8,8,0,0,1,16,0V40a8,8,0,0,1-16,0Zm8,24a64,64,0,1,0,64,64A64.07,64.07,0,0,0,128,64Zm0,112a48,48,0,1,1,48-48A48.05,48.05,0,0,1,128,176Z"/></svg>
          <svg v-if="item.icon === 'cloud-sun'" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#4b8ae6" d="M128,128a32,32,0,1,1,32-32A32,32,0,0,1,128,128Zm82,72H88A40,40,0,0,1,88,120a40.59,40.59,0,0,1,8,.8A48,48,0,0,1,184,104a47.88,47.88,0,0,1,28.29,9.2C226.78,123.47,238.16,140.23,230.13,165.71a40,40,0,0,1-20.13,34.29Z"/></svg>
          <svg v-if="item.icon === 'rain'" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#3b82f6" d="M176,144H80a40,40,0,0,1,0-80,40,40,0,0,1,40-40,40,40,0,0,1,40,40,40,40,0,0,1,16,74.69V144Zm-16,32v32h16V176Zm-48,0v32h16V176ZM80,176v32H96V176Z"/></svg>
          <svg v-if="item.icon === 'cloud'" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#9caaa4" d="M160,208H72A56,56,0,0,1,72,96a57.5,57.5,0,0,1,13.9,1.7,80,80,0,0,1,146.2,26.5A56,56,0,0,1,160,208Z"/></svg>
        </div>

        <div class="flex flex-col items-center gap-1">
          <span class="text-xl font-bold text-text-main">{{ item.tempHi }}°</span>
          <span class="text-[0.9rem] font-medium text-text-muted">{{ item.tempLo }}°</span>
        </div>
      </div>
    </div>
  </div>
</template>
