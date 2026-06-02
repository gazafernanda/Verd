<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue'
import CurrentConditions from '../components/Weather/CurrentConditions.vue'
import Forecast from '../components/Weather/Forecast.vue'
import PlantCareAlert from '../components/Weather/PlantCareAlert.vue'
import RainfallOutlook from '../components/Weather/RainfallOutlook.vue'
import GardenContext from '../components/Weather/GardenContext.vue'
import LocationSearch from '../components/LocationSearch.vue'
import { useUserStore } from '../stores/user'
import { useWeatherStore } from '../stores/weather'
import { MapPin, Pencil } from 'lucide-vue-next'

const user = useUserStore()
const weather = useWeatherStore()
const showLocationSearch = ref(false)

const resolvedLat = ref<number | null>(user.lat)
const resolvedLon = ref<number | null>(user.lon)

const mapSrc = computed(() => {
  if (resolvedLat.value == null || resolvedLon.value == null) return null
  const d = 0.15
  const w = resolvedLon.value - d
  const s = resolvedLat.value - d
  const e = resolvedLon.value + d
  const n = resolvedLat.value + d
  return `https://www.openstreetmap.org/export/embed.html?bbox=${w},${s},${e},${n}&layer=mapnik&marker=${resolvedLat.value},${resolvedLon.value}`
})

async function resolveCoords() {
  if (user.lat != null && user.lon != null) {
    resolvedLat.value = user.lat
    resolvedLon.value = user.lon
    return
  }
  if (!user.location) return
  try {
    const url = `https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(user.location)}&count=1&language=en&format=json`
    const res = await fetch(url)
    const data = await res.json()
    const r = data.results?.[0]
    if (r) {
      resolvedLat.value = r.latitude
      resolvedLon.value = r.longitude
      await user.saveSettings({ lat: r.latitude, lon: r.longitude })
    }
  } catch {
    // silent fail
  }
}

async function loadWeather() {
  await resolveCoords()
  if (resolvedLat.value != null && resolvedLon.value != null) {
    weather.fetchWeatherByCoords(resolvedLat.value, resolvedLon.value)
  } else {
    weather.fetchWeather()
  }
}

onMounted(() => {
  if (!user.location) {
    showLocationSearch.value = true
  } else {
    loadWeather()
  }
})

watch(
  () => user.location,
  (newLoc) => {
    if (newLoc) {
      resolvedLat.value = user.lat
      resolvedLon.value = user.lon
      loadWeather()
    }
  }
)
</script>

<template>
  <div class="flex flex-col gap-6">
    <!-- No location empty state -->
    <div v-if="!user.location" class="flex flex-col items-center justify-center min-h-[60vh] gap-6 text-center">
      <div class="w-20 h-20 rounded-full bg-[#f0faf6] flex items-center justify-center">
        <MapPin class="text-success-green" width="36" height="36" />
      </div>
      <div>
        <h2 class="text-[1.6rem] font-bold text-text-main mb-2">Set your location</h2>
        <p class="text-text-muted text-[0.95rem] max-w-xs mx-auto">
          Choose your location to see real-time weather data and a 7-day forecast for your garden.
        </p>
      </div>
      <button
        @click="showLocationSearch = true"
        class="flex items-center gap-2 px-6 py-3 bg-primary text-white rounded-[24px] font-semibold text-[0.95rem] hover:opacity-90 transition-opacity"
      >
        <MapPin width="18" height="18" />
        Set Location
      </button>
    </div>

    <!-- Main weather content -->
    <template v-else>
      <!-- Title & actions -->
      <div class="flex justify-between items-end mb-2 max-lg:flex-col max-lg:items-start max-lg:gap-4">
        <div>
          <span class="font-semibold text-success-green text-[0.9rem]">Home / Weather Analysis / {{ user.location }}</span>
          <h1 class="text-[2.2rem] font-extrabold text-text-main mb-2 mt-2 tracking-[-0.5px]">Weather Analysis</h1>
          <p class="text-text-muted text-[0.95rem] flex items-center gap-1.5 font-medium">
            <MapPin class="text-success-green" width="16" height="16" />
            {{ user.location }} • Updated just now
          </p>
        </div>
        <button
          @click="showLocationSearch = true"
          class="flex items-center gap-2 px-5 py-[10px] bg-surface text-text-main border border-border rounded-[24px] font-semibold text-[0.95rem] shadow-sm hover:bg-bg-app transition-colors"
        >
          <Pencil width="20" height="20" />
          Change Location
        </button>
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
          <!-- Current Location card -->
          <div class="bg-surface rounded-2xl p-6 shadow-sm">
            <div class="flex justify-between items-start mb-4">
              <div>
                <span class="text-[0.75rem] font-bold text-success-green tracking-[1px]">CURRENT LOCATION</span>
                <h3 class="text-[1.4rem] font-extrabold text-text-main mt-1 mb-0 leading-tight">{{ user.location }}</h3>
              </div>
              <button
                @click="showLocationSearch = true"
                class="w-9 h-9 rounded-full bg-[#f0faf6] flex items-center justify-center text-success-green hover:bg-[#e0f5ec] transition-colors"
              >
                <Pencil width="16" height="16" />
              </button>
            </div>

            <!-- Map embed -->
            <div class="w-full h-[200px] rounded-xl overflow-hidden bg-[#e8edf0] relative">
              <iframe
                v-if="mapSrc"
                :src="mapSrc"
                class="w-full h-full border-0"
                scrolling="no"
                title="Location map"
              />
              <div v-else class="w-full h-full flex items-center justify-center text-text-muted text-[0.85rem] font-medium">
                Map unavailable
              </div>
            </div>
          </div>

          <PlantCareAlert />
          <RainfallOutlook />
          <GardenContext />

        </div>
      </div>
    </template>

    <LocationSearch v-if="showLocationSearch" @close="showLocationSearch = false" />
  </div>
</template>
