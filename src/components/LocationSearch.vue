<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '../stores/user'
import { MapPin, Search, Refresh, Check, Xmark, LocateUser } from '@iconoir/vue'

const emit = defineEmits<{ close: [] }>()
const user = useUserStore()

// tabs: 'detect' | 'search'
const activeTab = ref<'detect' | 'search'>('detect')

// detect state
const detecting = ref(false)
const detected = ref('')
const detectError = ref('')

async function runDetect() {
  detecting.value = true
  detected.value = ''
  detectError.value = ''
  try {
    const res = await fetch('https://ipwho.is/')
    const data = await res.json()
    if (data.success && data.city) {
      detected.value = data.region ? `${data.city}, ${data.region}` : data.city
    } else {
      detectError.value = 'Could not detect location. Try searching instead.'
    }
  } catch {
    detectError.value = 'Network error. Try searching instead.'
  } finally {
    detecting.value = false
  }
}

async function confirmDetected() {
  if (!detected.value) return
  await user.saveSettings({ location: detected.value })
  emit('close')
}

// search state
const query = ref('')
const results = ref<{ name: string; admin1: string; country: string }[]>([])
const searching = ref(false)
let debounceTimer: ReturnType<typeof setTimeout>

async function onQueryInput() {
  clearTimeout(debounceTimer)
  results.value = []
  if (query.value.trim().length < 2) return
  debounceTimer = setTimeout(async () => {
    searching.value = true
    try {
      const url = `https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(query.value.trim())}&count=6&language=en&format=json`
      const res = await fetch(url)
      const data = await res.json()
      results.value = (data.results ?? []).map((r: { name: string; admin1?: string; country: string }) => ({
        name: r.name,
        admin1: r.admin1 ?? '',
        country: r.country,
      }))
    } catch {
      // silent fail
    } finally {
      searching.value = false
    }
  }, 400)
}

async function selectResult(r: { name: string; admin1: string; country: string }) {
  const loc = r.admin1 ? `${r.name}, ${r.admin1}` : `${r.name}, ${r.country}`
  await user.saveSettings({ location: loc })
  emit('close')
}
</script>

<template>
  <!-- Backdrop -->
  <div
    class="fixed inset-0 z-50 bg-black/40 flex items-center justify-center p-4"
    @click.self="emit('close')"
  >
    <!-- Modal card -->
    <div class="bg-surface rounded-2xl shadow-xl w-full max-w-[420px] overflow-hidden">

      <!-- Header -->
      <div class="flex items-center justify-between px-6 pt-6 pb-4">
        <div class="flex items-center gap-2">
          <MapPin class="text-success-green" width="20" height="20" />
          <h2 class="text-[1.1rem] font-bold text-text-main">Change Location</h2>
        </div>
        <button
          @click="emit('close')"
          class="w-8 h-8 rounded-full bg-bg-app flex items-center justify-center text-text-muted hover:bg-[#e6e8eb] transition-colors"
        >
          <Xmark width="16" height="16" />
        </button>
      </div>

      <!-- Tabs -->
      <div class="flex mx-6 mb-5 bg-bg-app rounded-xl p-1 gap-1">
        <button
          @click="activeTab = 'detect'"
          :class="[
            'flex-1 flex items-center justify-center gap-2 py-2 rounded-lg text-[0.85rem] font-semibold transition-colors',
            activeTab === 'detect'
              ? 'bg-surface text-text-main shadow-sm'
              : 'text-text-muted hover:text-text-main',
          ]"
        >
          <LocateUser width="16" height="16" />
          Auto-detect
        </button>
        <button
          @click="activeTab = 'search'"
          :class="[
            'flex-1 flex items-center justify-center gap-2 py-2 rounded-lg text-[0.85rem] font-semibold transition-colors',
            activeTab === 'search'
              ? 'bg-surface text-text-main shadow-sm'
              : 'text-text-muted hover:text-text-main',
          ]"
        >
          <Search width="16" height="16" />
          Search
        </button>
      </div>

      <!-- Detect tab -->
      <div v-if="activeTab === 'detect'" class="px-6 pb-6">
        <p class="text-[0.875rem] text-text-muted mb-5">
          Detect your approximate location based on your IP address. No GPS required.
        </p>

        <!-- Not yet detected -->
        <div v-if="!detected">
          <button
            @click="runDetect"
            :disabled="detecting"
            class="w-full flex items-center justify-center gap-2 py-3 bg-primary text-white rounded-xl font-semibold text-[0.9rem] hover:opacity-90 transition-opacity disabled:opacity-60"
          >
            <Refresh v-if="detecting" width="18" height="18" class="animate-spin" />
            <LocateUser v-else width="18" height="18" />
            {{ detecting ? 'Detecting…' : 'Detect my location' }}
          </button>
          <p v-if="detectError" class="mt-3 text-[0.8rem] text-red-500 text-center">{{ detectError }}</p>
        </div>

        <!-- Detected result -->
        <div v-else class="flex flex-col gap-3">
          <div class="flex items-center gap-3 px-4 py-3 bg-bg-app rounded-xl border border-border">
            <MapPin class="text-success-green shrink-0" width="18" height="18" />
            <span class="text-[0.95rem] font-semibold text-text-main flex-1">{{ detected }}</span>
          </div>
          <div class="flex gap-3">
            <button
              @click="detected = ''"
              class="flex-1 py-2.5 border border-border rounded-xl text-[0.875rem] font-semibold text-text-muted hover:bg-bg-app transition-colors"
            >
              Re-detect
            </button>
            <button
              @click="confirmDetected"
              class="flex-1 flex items-center justify-center gap-2 py-2.5 bg-primary text-white rounded-xl text-[0.875rem] font-semibold hover:opacity-90 transition-opacity"
            >
              <Check width="16" height="16" />
              Use this location
            </button>
          </div>
        </div>
      </div>

      <!-- Search tab -->
      <div v-else class="px-6 pb-6">
        <!-- Input -->
        <div class="flex items-center gap-2 px-4 py-3 bg-bg-app border border-border rounded-xl mb-3 focus-within:border-success-green transition-colors">
          <Search class="text-text-muted shrink-0" width="16" height="16" />
          <input
            v-model="query"
            @input="onQueryInput"
            type="text"
            placeholder="Search city or region…"
            class="flex-1 bg-transparent text-[0.9rem] text-text-main placeholder-text-light outline-none"
            autofocus
          />
          <Refresh v-if="searching" width="14" height="14" class="text-text-muted animate-spin shrink-0" />
        </div>

        <!-- Results -->
        <ul v-if="results.length" class="flex flex-col gap-1 max-h-52 overflow-y-auto">
          <li v-for="r in results" :key="`${r.name}-${r.admin1}-${r.country}`">
            <button
              @click="selectResult(r)"
              class="w-full flex items-center gap-3 px-4 py-3 rounded-xl hover:bg-bg-app transition-colors text-left"
            >
              <MapPin class="text-success-green shrink-0" width="16" height="16" />
              <div>
                <p class="text-[0.9rem] font-semibold text-text-main leading-tight">{{ r.name }}<span v-if="r.admin1">, {{ r.admin1 }}</span></p>
                <p class="text-[0.75rem] text-text-muted">{{ r.country }}</p>
              </div>
            </button>
          </li>
        </ul>

        <!-- Empty hint -->
        <p v-else-if="!searching && query.trim().length >= 2" class="text-[0.85rem] text-text-muted text-center py-4">
          No results found. Try a different spelling.
        </p>
        <p v-else-if="!query.trim()" class="text-[0.85rem] text-text-muted text-center py-4">
          Start typing to search for a city.
        </p>
      </div>

    </div>
  </div>
</template>
