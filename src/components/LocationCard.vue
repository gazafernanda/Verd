<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useUserStore } from "../stores/user";
import { Pencil } from "lucide-vue-next";
import LocationSearch from "./LocationSearch.vue";

const user = useUserStore();
const showLocationSearch = ref(false);

const resolvedLat = ref<number | null>(user.lat);
const resolvedLon = ref<number | null>(user.lon);

const mapSrc = computed(() => {
  if (resolvedLat.value == null || resolvedLon.value == null) return null;
  const d = 0.15;
  const w = resolvedLon.value - d;
  const s = resolvedLat.value - d;
  const e = resolvedLon.value + d;
  const n = resolvedLat.value + d;
  return `https://www.openstreetmap.org/export/embed.html?bbox=${w},${s},${e},${n}&layer=mapnik&marker=${resolvedLat.value},${resolvedLon.value}`;
});

async function resolveCoords() {
  if (user.lat != null && user.lon != null) {
    resolvedLat.value = user.lat;
    resolvedLon.value = user.lon;
    return;
  }
  if (!user.location) return;
  try {
    const url = `https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(user.location)}&count=1&language=en&format=json`;
    const res = await fetch(url);
    const data = await res.json();
    const r = data.results?.[0];
    if (r) {
      resolvedLat.value = r.latitude;
      resolvedLon.value = r.longitude;
      await user.saveSettings({ lat: r.latitude, lon: r.longitude });
    }
  } catch {
    // silent fail
  }
}

onMounted(resolveCoords);

watch(
  () => user.location,
  () => {
    resolvedLat.value = user.lat;
    resolvedLon.value = user.lon;
    resolveCoords();
  }
);
</script>

<template>
  <div class="bg-surface rounded-lg p-6 shadow-sm flex-1 flex flex-col">
    <div class="flex justify-between items-start mb-5">
      <div>
        <p class="text-[0.75rem] font-bold text-success-green tracking-[1px] mb-1">
          CURRENT LOCATION
        </p>
        <h3 class="text-2xl font-bold text-text-main">{{ user.location }}</h3>
      </div>
      <button
        @click="showLocationSearch = true"
        class="w-8 h-8 rounded-full bg-bg-app flex items-center justify-center text-success-green transition-colors duration-200 hover:bg-[#e6e8eb]"
      >
        <Pencil width="16" height="16" />
      </button>
    </div>

    <div class="w-full h-40 rounded-md overflow-hidden bg-[#e8eaed] flex-1">
      <iframe
        v-if="mapSrc"
        :src="mapSrc"
        class="w-full h-full border-0"
        scrolling="no"
        title="Location map"
      />
      <div v-else class="w-full h-full flex items-center justify-center text-text-muted text-sm">
        Map loading…
      </div>
    </div>

    <LocationSearch v-if="showLocationSearch" @close="showLocationSearch = false" />
  </div>
</template>
