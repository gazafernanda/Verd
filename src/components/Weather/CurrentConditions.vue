<script setup lang="ts">
import { computed, markRaw } from "vue";
import { useI18n } from "vue-i18n";
import { useWeatherStore } from "../../stores/weather";
import { formatTempValue, TEMPERATURE_UNIT_LETTER } from "../../utils/temperature";
import HelpTip from "../HelpTip.vue";
import { Droplet, Sun, Droplets, Wind } from "lucide-vue-next";

const { t } = useI18n();
const weather = useWeatherStore();

// Every sensor parameter carries its own short explanation — these are the
// readings a gardener is most likely to be unsure how to act on.
const metrics = computed(() => [
  {
    label: t("weather.humidity"),
    value: `${weather.humidity}%`,
    icon: markRaw(Droplet),
    help: t("help.humidity"),
  },
  {
    label: t("weather.uvIndex"),
    value: `${weather.uvIndex} (${weather.uvLabel})`,
    icon: markRaw(Sun),
    help: t("help.uvIndex"),
  },
  {
    label: t("weather.soilMoisture"),
    value: `${weather.soilMoisture}%`,
    icon: markRaw(Droplets),
    help: t("help.soilMoisture"),
  },
  {
    label: t("weather.windSpeed"),
    value: `${weather.windSpeed} km/h`,
    icon: markRaw(Wind),
    help: t("help.windSpeed"),
  },
]);
</script>

<template>
  <div class="bg-surface rounded-xl p-8 shadow-sm mb-8">
    <div class="flex justify-between items-start mb-6">
      <span class="text-[0.8rem] font-bold text-success-green tracking-[1px]"
        >{{ t('weather.currentConditions') }}</span
      >
      <div
        class="flex flex-col items-end border border-success-green rounded-[20px] px-4 py-1.5 bg-surface"
      >
        <span class="text-[0.6rem] font-bold text-text-muted tracking-[0.5px]"
          >{{ t('weather.aqiIndex') }}</span
        >
        <span class="text-[0.85rem] font-bold text-success-green whitespace-nowrap"
          >{{ t('weather.aqiExcellent', { value: weather.aqi }) }}</span
        >
      </div>
    </div>

    <div class="flex items-center gap-6 mb-3">
      <div class="flex items-start">
        <span
          class="text-[6.5rem] max-lg:text-[4.5rem] font-extrabold leading-[0.9] tracking-[-3px] text-text-main"
          >{{ formatTempValue(weather.temp) }}°</span
        >
        <span
          class="text-[2.5rem] max-lg:text-[1.8rem] font-semibold text-text-main mt-2"
          >{{ TEMPERATURE_UNIT_LETTER }}</span
        >
      </div>
      <Sun
        class="max-lg:w-10 max-lg:h-10 shrink-0"
        width="64"
        height="64"
        color="#fbbd06"
      />
    </div>

    <p class="flex items-center gap-2 text-xl text-text-muted font-medium mb-8">
      {{ t('weather.feelsLike', { condition: weather.condition, temp: formatTempValue(weather.feelsLike) }) }}
      <HelpTip :label="t('help.feelsLike')" :size="15" />
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
            class="flex items-center gap-1.5 text-[0.65rem] font-bold text-text-muted tracking-[0.5px]"
          >
            {{ metric.label }}
            <HelpTip :label="metric.help" :size="12" />
          </span>
          <span class="text-lg font-bold text-text-main">{{
            metric.value
          }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
