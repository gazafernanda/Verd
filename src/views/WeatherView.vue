<script setup lang="ts">
import { ref } from 'vue'
import CurrentConditions from '../components/Weather/CurrentConditions.vue'
import Forecast from '../components/Weather/Forecast.vue'
import PlantCareAlert from '../components/Weather/PlantCareAlert.vue'
import RainfallOutlook from '../components/Weather/RainfallOutlook.vue'
import GardenContext from '../components/Weather/GardenContext.vue'
import LocationSearch from '../components/LocationSearch.vue'
import { useUserStore } from '../stores/user'
import { MapPin, EditPencil } from '@iconoir/vue'

const user = useUserStore()
const showLocationSearch = ref(false)
</script>

<template>
  <div class="flex flex-col gap-6">
    <div>
      <!-- Title & actions -->
      <div class="flex justify-between items-end mb-6 max-lg:flex-col max-lg:items-start max-lg:gap-4">
        <div>
          <span class="font-semibold text-success-green text-[0.9rem]">Home / Weather Analysis / {{ user.location }}</span>
          <h1 class="text-[2.2rem] font-extrabold text-text-main mb-2 mt-2 tracking-[-0.5px]">Weather Analysis</h1>
          <p class="text-text-muted text-[0.95rem] flex items-center gap-1.5 font-medium">
            <MapPin class="text-success-green" width="16" height="16" />
            {{ user.location }} • Updated 2 mins ago
          </p>
        </div>
        <button
          @click="showLocationSearch = true"
          class="flex items-center gap-2 px-5 py-[10px] bg-surface text-text-main border border-border rounded-[24px] font-semibold text-[0.95rem] shadow-sm hover:bg-bg-app transition-colors"
        >
          <EditPencil width="20" height="20" />
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
          <div class="flex justify-between items-start mb-1 gap-4 max-lg:flex-col max-lg:gap-2">
            <h3 class="text-[1.25rem] font-bold text-text-main m-0">Temperature Trends</h3>
            <div class="flex gap-4 text-[0.75rem] font-semibold text-text-muted shrink-0">
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

  <LocationSearch v-if="showLocationSearch" @close="showLocationSearch = false" />
</template>
