<script setup lang="ts">
import CurrentConditions from '../components/Weather/CurrentConditions.vue'
import Forecast from '../components/Weather/Forecast.vue'
import PlantCareAlert from '../components/Weather/PlantCareAlert.vue'
import RainfallOutlook from '../components/Weather/RainfallOutlook.vue'
import GardenContext from '../components/Weather/GardenContext.vue'
import { useUserStore } from '../stores/user'

const user = useUserStore()
</script>

<template>
  <div class="flex flex-col gap-6">
    <div>
      <!-- Title & actions -->
      <div class="flex justify-between items-end mb-6">
        <div>
          <span class="font-semibold text-success-green text-[0.9rem]">Home / Weather Analysis / {{ user.location }}</span>
          <h1 class="text-[2.2rem] font-extrabold text-text-main mb-2 mt-2 tracking-[-0.5px]">Weather Analysis</h1>
          <p class="text-text-muted text-[0.95rem] flex items-center gap-1.5 font-medium">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256" class="text-success-green"><path fill="currentColor" d="M128,64a40,40,0,1,0,40,40A40,40,0,0,0,128,64Zm0,64a24,24,0,1,1,24-24A24,24,0,0,1,128,128Zm0-112a88.1,88.1,0,0,0-88,88c0,31.4,14.51,64.68,42,96.25a254.19,254.19,0,0,0,41.45,38.3,8,8,0,0,0,9.18,0A254.19,254.19,0,0,0,174,200.25c27.45-31.57,42-64.85,42-96.25A88.1,88.1,0,0,0,128,16Zm0,206c-16.53-13-72-60.75-72-118a72,72,0,0,1,144,0C200,161.21,144.53,209,128,222Z"/></svg>
            {{ user.location }} • Updated 2 mins ago
          </p>
        </div>
        <button class="flex items-center gap-2 px-5 py-[10px] bg-surface text-text-main border border-border rounded-[24px] font-semibold text-[0.95rem] shadow-sm hover:bg-bg-app transition-colors">
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M227.32,73.37,182.63,28.68a16,16,0,0,0-22.63,0L36.68,152A15.89,15.89,0,0,0,32,163.31V208a16,16,0,0,0,16,16H92.69A15.86,15.86,0,0,0,104,219.31L227.32,96a16,16,0,0,0,0-22.63ZM92.69,208H48V163.31l88-88L180.69,120ZM192,108.68,147.31,64l24-24L216,84.68Z"/></svg>
          Change Location
        </button>
      </div>
    </div>

    <!-- Layout -->
    <div class="grid grid-cols-[1fr_320px] max-lg:grid-cols-1 gap-6">
      <!-- Left column -->
      <div class="flex flex-col gap-6">
        <CurrentConditions />
        <Forecast />

        <!-- Temperature Trends placeholder -->
        <div class="bg-surface rounded-2xl p-8 shadow-sm">
          <div class="flex justify-between items-end mb-1">
            <h3 class="text-[1.25rem] font-bold text-text-main m-0">Temperature Trends</h3>
            <div class="flex gap-4 text-[0.75rem] font-semibold text-text-muted">
              <span class="flex items-center gap-1.5">
                <span class="w-2 h-2 rounded-full bg-success-green inline-block"></span> Temperature
              </span>
              <span class="flex items-center gap-1.5">
                <span class="w-2 h-2 rounded-full bg-[#d1d5db] inline-block"></span> Avg. 10Y
              </span>
            </div>
          </div>
          <p class="text-[0.85rem] text-text-muted font-medium mb-6">Next 24 hours expectation</p>
          <div class="w-full h-60 flex flex-col justify-between pt-4">
            <div class="flex-1 flex flex-col justify-between pb-4">
              <div class="h-px w-full bg-border"></div>
              <div class="h-px w-full bg-border"></div>
              <div class="h-px w-full bg-border"></div>
            </div>
            <div class="flex justify-between px-4 text-[0.65rem] font-semibold text-text-light">
              <span>12 AM</span>
              <span>4 AM</span>
              <span>8 AM</span>
              <span>12 PM</span>
              <span>4 PM</span>
              <span>8 PM</span>
              <span>12 AM</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Right column -->
      <div class="flex flex-col gap-6">
        <PlantCareAlert />
        <RainfallOutlook />
        <GardenContext />

        <!-- Regional Radar placeholder -->
        <div class="h-[200px] rounded-2xl overflow-hidden relative bg-[#e5e7eb]">
          <img src="/src/assets/radar.png" alt="Map Radar" class="w-full h-full object-cover opacity-80" />
          <div class="absolute inset-0 flex flex-col items-center justify-center bg-[radial-gradient(circle_at_center,rgba(255,255,255,0.7)_0%,transparent_70%)]">
            <div class="relative w-12 h-12 flex items-center justify-center mb-2">
              <div class="absolute inset-0 border-2 border-success-green rounded-full opacity-50 animate-ping-slow"></div>
              <div class="w-3 h-3 bg-success-green rounded-full border-2 border-white z-10"></div>
            </div>
            <h4 class="text-[0.8rem] font-bold text-text-main tracking-widest m-0">REGIONAL RADAR</h4>
            <p class="text-[0.65rem] text-text-muted">Interactive Map View</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
