<script setup lang="ts">
import { useWeatherStore } from '../../stores/weather'
import { Droplets, Sun, Droplet, ChevronDown, ChevronUp } from 'lucide-vue-next'
const weather = useWeatherStore()
</script>

<template>
  <div class="grid grid-cols-3 gap-6 max-[900px]:grid-cols-1">
    <!-- Soil Moisture -->
    <div class="bg-surface rounded-xl p-6 shadow-sm border border-border flex flex-col">
      <div class="flex justify-between items-center mb-2">
        <span class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px]">HYDRATION</span>
        <div class="w-8 h-8 rounded-full flex items-center justify-center bg-[#e6f6ef] text-success-green">
          <Droplets width="16" height="16" />
        </div>
      </div>
      <h3 class="text-[1.1rem] font-extrabold text-text-main mb-4">Soil Moisture</h3>
      <div class="flex items-center gap-3 mb-6 flex-1">
        <span class="text-[3rem] font-extrabold text-text-main leading-none tracking-[-1px]">{{ weather.soilMoisture }}%</span>
        <span class="flex items-center gap-0.5 text-[0.85rem] font-bold"
          :class="weather.soilMoisture < 25 ? 'text-red-500' : 'text-success-green'">
          <ChevronDown v-if="weather.soilMoisture < 25" width="12" height="12" />
          <ChevronUp v-else width="12" height="12" />
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
          <Sun width="16" height="16" />
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
          <Droplet width="16" height="16" />
        </div>
      </div>
      <h3 class="text-[1.1rem] font-extrabold text-text-main mb-4">Humidity</h3>
      <div class="flex items-center gap-3 mb-6 flex-1">
        <span class="text-[3rem] font-extrabold text-text-main leading-none tracking-[-1px]">{{ weather.humidity }}%</span>
        <span class="flex items-center gap-0.5 text-[0.85rem] font-bold text-success-green">
          <ChevronUp width="12" height="12" />
          1%
        </span>
      </div>
      <div class="h-1.5 mb-3"></div>
      <p class="text-[0.75rem] text-success-green m-0 font-medium">Stable environment for leafy greens</p>
    </div>
  </div>
</template>
