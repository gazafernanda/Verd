<script setup lang="ts">
import StatusBanner from '../components/StatusBanner.vue'
import LocationCard from '../components/LocationCard.vue'
import WeatherCard from '../components/WeatherCard.vue'
import CareRecommendations from '../components/CareRecommendations.vue'
import { useUserStore } from '../stores/user'

const user = useUserStore()
</script>

<template>
  <div class="flex flex-col gap-6">
    <div>
      <!-- Top nav -->
      <div class="flex justify-between items-center mb-6">
        <div></div>
        <div class="flex items-center gap-3">
          <span class="font-semibold text-[0.95rem] text-text-main">{{ user.name }}</span>
          <div class="w-10 h-10 rounded-full bg-gradient-to-br from-[#f08b5e] to-[#e85d46] shadow-sm"></div>
        </div>
      </div>

      <!-- Title + actions -->
      <div class="flex justify-between items-end mb-4">
        <div>
          <h1 class="text-[2rem] font-bold text-text-main mb-1 tracking-[-0.5px]">Main Dashboard</h1>
          <p class="text-text-muted text-[0.95rem]">Monitoring your indoor sanctuary</p>
        </div>
        <div class="flex gap-4">
          <button class="flex items-center gap-2 px-4 py-2.5 bg-surface border border-border rounded-md text-text-main font-semibold text-[0.95rem] transition-colors duration-200 hover:bg-bg-app">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M208,32H184V24a8,8,0,0,0-16,0v8H88V24a8,8,0,0,0-16,0v8H48A16,16,0,0,0,32,48V208a16,16,0,0,0,16,16H208a16,16,0,0,0,16-16V48A16,16,0,0,0,208,32ZM72,48v8a8,8,0,0,0,16,0V48h80v8a8,8,0,0,0,16,0V48h24V80H48V48Zm136,160H48V96H208V208Z"/></svg>
            Oct 27, 2023
          </button>
          <button class="flex items-center gap-2 px-5 py-2.5 bg-accent-green text-white rounded-xl font-semibold text-[0.95rem] shadow-[0_4px_12px_rgba(41,156,119,0.3)] transition-all duration-200 hover:bg-accent-green-hover hover:-translate-y-px">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M224,128a8,8,0,0,1-8,8H136v80a8,8,0,0,1-16,0V136H40a8,8,0,0,1,0-16h80V40a8,8,0,0,1,16,0v80h80A8,8,0,0,1,224,128Z"/></svg>
            Add New Plant
          </button>
        </div>
      </div>
    </div>

    <StatusBanner />

    <div class="flex gap-6 mb-8">
      <LocationCard />
      <WeatherCard />
    </div>

    <CareRecommendations />
  </div>
</template>
