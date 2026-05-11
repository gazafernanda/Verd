<script setup lang="ts">
import { onMounted } from "vue";
import StatusBanner from "../components/StatusBanner.vue";
import LocationCard from "../components/LocationCard.vue";
import WeatherCard from "../components/WeatherCard.vue";
import CareRecommendations from "../components/CareRecommendations.vue";
import { useUserStore } from "../stores/user";
import { useWeatherStore } from "../stores/weather";
import { useRouter } from "vue-router";
import { Calendar, Plus } from "lucide-vue-next";

const user = useUserStore();
const weather = useWeatherStore();
const router = useRouter();

onMounted(() => {
  weather.fetchWeather();
});
</script>

<template>
  <div class="flex flex-col gap-6">
    <div>
      <!-- Top nav (desktop only) -->
      <div class="max-lg:hidden flex justify-between items-center mb-6">
        <div></div>
        <div class="flex items-center gap-3">
          <span class="font-semibold text-[0.95rem] text-text-main">{{
            user.name
          }}</span>
          <div
            class="w-10 h-10 rounded-full bg-gradient-to-br from-[#f08b5e] to-[#e85d46] shadow-sm"
          ></div>
        </div>
      </div>

      <!-- Title + actions -->
      <div
        class="flex justify-between items-end mb-4 max-lg:flex-col max-lg:items-start max-lg:gap-4"
      >
        <div>
          <h1
            class="text-[2rem] max-lg:text-[1.6rem] font-bold text-text-main mb-1 tracking-[-0.5px]"
          >
            Main Dashboard
          </h1>
          <p class="text-text-muted text-[0.95rem]">
            Monitoring your indoor sanctuary
          </p>
        </div>
        <div class="flex gap-3">
          <button
            class="flex items-center gap-2 px-4 py-2.5 bg-surface border border-border rounded-md text-text-main font-semibold text-[0.9rem] transition-colors duration-200 hover:bg-bg-app max-lg:hidden"
          >
            <Calendar width="20" height="20" />
            Oct 27, 2023
          </button>
          <button
            @click="router.push({ name: 'add-plant' })"
            class="flex items-center gap-2 px-5 py-2.5 bg-accent-green text-white rounded-xl font-semibold text-[0.9rem] shadow-[0_4px_12px_rgba(41,156,119,0.3)] transition-all duration-200 hover:bg-accent-green-hover hover:-translate-y-px"
          >
            <Plus width="20" height="20" />
            Add New Plant
          </button>
        </div>
      </div>
    </div>

    <StatusBanner />

    <div class="flex gap-6 mb-8 max-lg:flex-col">
      <LocationCard />
      <WeatherCard />
    </div>

    <CareRecommendations />
  </div>
</template>
