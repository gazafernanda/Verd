<script setup lang="ts">
import { useWeatherStore } from '../../stores/weather'
const weather = useWeatherStore()
</script>

<template>
  <div class="grid grid-cols-3 gap-6 max-[900px]:grid-cols-1">
    <!-- Soil Moisture -->
    <div class="bg-surface rounded-xl p-6 shadow-sm border border-border flex flex-col">
      <div class="flex justify-between items-center mb-2">
        <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px]">HYDRATION</span>
        <div class="w-8 h-8 rounded-full flex items-center justify-center bg-[#e6f6ef] text-success-green">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M174.55,101.44a80,80,0,1,1-93.1,0A153.25,153.25,0,0,1,128,31.76a153.25,153.25,0,0,1,46.55,69.68Z"/></svg>
        </div>
      </div>
      <h3 class="text-[1.1rem] font-extrabold text-text-main mb-4">Soil Moisture</h3>
      <div class="flex items-center gap-3 mb-6 flex-1">
        <span class="text-[3rem] font-extrabold text-text-main leading-none tracking-[-1px]">{{ weather.soilMoisture }}%</span>
        <span class="flex items-center gap-0.5 text-[0.85rem] font-bold"
          :class="weather.soilMoisture < 25 ? 'text-red-500' : 'text-success-green'">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 256 256"><path fill="currentColor" d="M205.66,202.34a8,8,0,0,1-11.32,11.32l-96-96a8,8,0,0,1,0-11.32l96-96a8,8,0,0,1,11.32,11.32L115.31,112Z" transform="rotate(-90 128 128)"/></svg>
          {{ weather.soilMoisture < 25 ? '5%' : '2%' }}
        </span>
      </div>
      <div class="w-full h-1.5 bg-border rounded mb-3 overflow-hidden">
        <div class="h-full rounded transition-all duration-500"
          :class="weather.soilMoisture < 25 ? 'bg-red-500' : 'bg-success-green'"
          :style="`width: ${weather.soilMoisture}%`"></div>
      </div>
      <p class="text-[0.75rem] m-0 font-medium"
        :class="weather.soilMoisture < 25 ? 'text-red-500' : 'text-success-green'">
        {{ weather.soilMoisture < 25 ? 'Critical: Watering needed within 2 hours' : 'Adequate moisture levels' }}
      </p>
    </div>

    <!-- UV Index -->
    <div class="bg-surface rounded-xl p-6 shadow-sm border border-[rgba(251,146,60,0.3)] shadow-[0_4px_12px_rgba(251,146,60,0.05)] flex flex-col">
      <div class="flex justify-between items-center mb-2">
        <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px]">EXPOSURE</span>
        <div class="w-8 h-8 rounded-full flex items-center justify-center bg-[#fff4e5] text-[#f59e0b]">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M120,40V16a8,8,0,0,1,16,0V40a8,8,0,0,1-16,0Zm8,24a64,64,0,1,0,64,64A64.07,64.07,0,0,0,128,64Zm0,112a48,48,0,1,1,48-48A48.05,48.05,0,0,1,128,176ZM58.34,69.66A8,8,0,0,0,69.66,58.34l-17-17a8,8,0,0,0-11.32,11.32Zm0,116.68-17,17a8,8,0,0,0,11.32,11.32l17-17a8,8,0,0,0-11.32-11.32ZM203.31,197.66a8,8,0,0,0,11.33-11.32l-17-17a8,8,0,0,0-11.32,11.32ZM216,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,216,120ZM40,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,40,120ZM183,52.48a8,8,0,0,0-5.63-13.68H144a8,8,0,0,0,0,16h33.35A8,8,0,0,0,183,52.48Z"/></svg>
        </div>
      </div>
      <h3 class="text-[1.1rem] font-extrabold text-text-main mb-4">UV Index</h3>
      <div class="flex items-center gap-3 mb-6 flex-1">
        <span class="text-[3rem] font-extrabold text-text-main leading-none tracking-[-1px]">{{ weather.uvIndex }}</span>
        <span class="bg-[#fff4e5] text-[#f59e0b] px-2 py-1 rounded text-[0.65rem] font-extrabold">{{ weather.uvLabel.toUpperCase() }}</span>
      </div>
      <div class="flex gap-1 mb-3">
        <div v-for="i in 5" :key="i" class="h-1.5 flex-1 rounded"
          :class="i <= Math.ceil(weather.uvIndex / 2) ? 'bg-[#f59e0b]' : 'bg-border'"></div>
      </div>
      <p class="text-[0.75rem] text-text-muted m-0 font-medium">Peak intensity expected at 1:45 PM</p>
    </div>

    <!-- Humidity -->
    <div class="bg-surface rounded-xl p-6 shadow-sm border border-border flex flex-col">
      <div class="flex justify-between items-center mb-2">
        <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px]">ATMOSPHERE</span>
        <div class="w-8 h-8 rounded-full flex items-center justify-center bg-[#ebf5ff] text-[#3b82f6]">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M176.81,95A64.12,64.12,0,0,0,128,64a64.12,64.12,0,0,0-48.81,31C61.85,123,54.7,159.08,71,185.81a64,64,0,0,0,114,0C201.3,159.08,194.15,123,176.81,95Z"/></svg>
        </div>
      </div>
      <h3 class="text-[1.1rem] font-extrabold text-text-main mb-4">Humidity</h3>
      <div class="flex items-center gap-3 mb-6 flex-1">
        <span class="text-[3rem] font-extrabold text-text-main leading-none tracking-[-1px]">{{ weather.humidity }}%</span>
        <span class="flex items-center gap-0.5 text-[0.85rem] font-bold text-success-green">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 256 256"><path fill="currentColor" d="M205.66,202.34a8,8,0,0,1-11.32,11.32l-96-96a8,8,0,0,1,0-11.32l96-96a8,8,0,0,1,11.32,11.32L115.31,112Z" transform="rotate(90 128 128)"/></svg>
          1%
        </span>
      </div>
      <div class="h-1.5 mb-3"></div>
      <p class="text-[0.75rem] text-success-green m-0 font-medium">Stable environment for leafy greens</p>
    </div>
  </div>
</template>
