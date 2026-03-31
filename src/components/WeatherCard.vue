<script setup lang="ts">
import { useWeatherStore } from '../stores/weather'
const weather = useWeatherStore()
</script>

<template>
  <div class="bg-surface rounded-lg p-6 shadow-sm flex-1 flex flex-col">
    <div class="flex justify-between items-start mb-6">
      <div>
        <p class="text-[0.75rem] font-bold text-success-green tracking-[1px] mb-1">WEATHER FORECAST</p>
        <h3 class="text-2xl font-bold text-text-main">{{ weather.condition }}</h3>
      </div>
      <div class="w-12 h-12 bg-[#fff9eb] rounded-full flex items-center justify-center shrink-0">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#fbbd06" d="M120,40V16a8,8,0,0,1,16,0V40a8,8,0,0,1-16,0Zm8,24a64,64,0,1,0,64,64A64.07,64.07,0,0,0,128,64Zm0,112a48,48,0,1,1,48-48A48.05,48.05,0,0,1,128,176ZM58.34,69.66A8,8,0,0,0,69.66,58.34l-17-17a8,8,0,0,0-11.32,11.32Zm0,116.68-17,17a8,8,0,0,0,11.32,11.32l17-17a8,8,0,0,0-11.32-11.32ZM203.31,197.66a8,8,0,0,0,11.33-11.32l-17-17a8,8,0,0,0-11.32,11.32ZM216,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,216,120ZM40,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,40,120ZM183,52.48a8,8,0,0,0-5.63-13.68H144a8,8,0,0,0,0,16h33.35A8,8,0,0,0,183,52.48Z"/></svg>
      </div>
    </div>

    <div class="flex justify-between items-center mb-8">
      <div class="flex items-start">
        <span class="text-[4.5rem] font-extrabold leading-none tracking-[-2px] text-text-main">{{ weather.temp }}°</span>
        <span class="text-[2rem] font-medium text-text-light mt-2">F</span>
      </div>
      <div class="w-[72px] h-[72px] rounded-[20px] bg-gradient-to-br from-[#a3c4cb] to-[#4b8994] flex items-center justify-center shadow-[0_8px_16px_rgba(75,137,148,0.3)]">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#ffffff" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24Zm0,192a88,88,0,1,1,88-88A88.1,88.1,0,0,1,128,216Zm40-88a40,40,0,1,1-40-40A40,40,0,0,1,168,128Z"/></svg>
      </div>
    </div>

    <div class="flex gap-4">
      <div class="flex items-center gap-1.5 px-4 py-1.5 rounded-[20px] text-[0.85rem] font-semibold text-[#4b8ae6] bg-[#f0f5ff]">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M176.81,95A64.12,64.12,0,0,0,128,64a64.12,64.12,0,0,0-48.81,31C61.85,123,54.7,159.08,71,185.81a64,64,0,0,0,114,0C201.3,159.08,194.15,123,176.81,95Zm-12.06,82.47a48,48,0,0,1-73.5,0c-11.83-19.34-6.42-47.53,7.57-69.51A48.06,48.06,0,0,1,128,80a48.06,48.06,0,0,1,29.18,27.93C171.17,130,176.58,158.13,164.75,177.47Z"/></svg>
        {{ weather.humidity }}%
      </div>
      <div class="flex items-center gap-1.5 px-4 py-1.5 rounded-[20px] text-[0.85rem] font-semibold text-success-green bg-light-green-bg">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M184,184a32,32,0,0,1-32,32H40a8,8,0,0,1,0-16H152a16,16,0,0,0,0-32H24a8,8,0,0,1,0-16H184A32,32,0,0,1,184,184Zm32-80a32,32,0,0,0-32-32H72a8,8,0,0,0,0,16H184a16,16,0,0,1,0,32H32a8,8,0,0,0,0,16H184A32,32,0,0,0,216,104Z"/></svg>
        {{ weather.windSpeed }} mph
      </div>
    </div>
  </div>
</template>
