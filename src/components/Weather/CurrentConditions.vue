<script setup lang="ts">
import { computed, markRaw } from "vue";
import { useWeatherStore } from "../../stores/weather";
import { Droplet, Sun, Droplets, Wind } from "lucide-vue-next";

const weather = useWeatherStore();

const metrics = computed(() => [
  {
    label: "HUMIDITY",
    value: `${weather.humidity}%`,
    icon: markRaw(Droplet),
  },
  {
    label: "UV INDEX",
    value: `${weather.uvIndex} (${weather.uvLabel})`,
    icon: markRaw(Sun),
  },
  {
    label: "SOIL MOISTURE",
    value: `${weather.soilMoisture}%`,
    icon: markRaw(Droplets),
  },
  {
    label: "WIND SPEED",
    value: `${weather.windSpeed} km/h`,
    icon: markRaw(Wind),
  },
]);
</script>

<template>
  <div class="bg-surface rounded-xl p-8 shadow-sm mb-8">
    <div class="flex justify-between items-start mb-6">
      <span class="text-[0.8rem] font-bold text-success-green tracking-[1px]"
        >CURRENT CONDITIONS</span
      >
      <div
        class="flex flex-col items-end border border-success-green rounded-[20px] px-4 py-1.5 bg-surface"
      >
        <span class="text-[0.6rem] font-bold text-text-muted tracking-[0.5px]"
          >AQI INDEX</span
        >
        <span class="text-[0.85rem] font-bold text-success-green whitespace-nowrap"
          >{{ weather.aqi }} - Excellent</span
        >
      </div>
    </div>

    <div class="flex items-center gap-6 mb-3">
      <div class="flex items-start">
        <span
          class="text-[6.5rem] max-lg:text-[4.5rem] font-extrabold leading-[0.9] tracking-[-3px] text-text-main"
          >{{ weather.temp }}°</span
        >
        <span
          class="text-[2.5rem] max-lg:text-[1.8rem] font-semibold text-text-main mt-2"
          >C</span
        >
      </div>
      <Sun
        class="max-lg:w-10 max-lg:h-10 shrink-0"
        width="64"
        height="64"
        color="#fbbd06"
      />
    </div>

    <p class="text-xl text-text-muted font-medium mb-8">
      {{ weather.condition }} • Feels like {{ weather.feelsLike }}°C
    </p>

    <div class="grid grid-cols-4 max-lg:grid-cols-2 gap-4">
      <div
        v-for="metric in metrics"
        :key="metric.label"
        class="flex flex-col gap-4 py-5 px-4 rounded-[40px] bg-[#f6fbfa] shadow-[0_4px_12px_rgba(55,178,126,0.05)]"
      >
        <div
          class="w-8 h-8 rounded-full bg-success-green flex items-center justify-center text-white"
        >
          <component :is="metric.icon" width="16" height="16" />
        </div>
        <div class="flex flex-col gap-1">
          <span
            class="text-[0.65rem] font-bold text-text-muted tracking-[0.5px]"
            >{{ metric.label }}</span
          >
          <span class="text-lg font-bold text-text-main">{{
            metric.value
          }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
