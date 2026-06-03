<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useWeatherStore } from '../../stores/weather'
import { Sun } from 'lucide-vue-next'
const { t } = useI18n()
const weather = useWeatherStore()
</script>

<template>
  <div class="bg-success-green rounded-xl p-6 text-white relative overflow-hidden shadow-[0_4px_16px_rgba(55,178,126,0.2)]">
    <div class="flex items-center gap-2 text-base font-bold mb-6 relative z-10">
      <Sun width="20" height="20" />
      {{ t('profile.localWeather') }}
    </div>

    <div class="flex items-center gap-4 relative z-10">
      <div class="text-5xl font-extrabold leading-none">{{ weather.temp }}°F</div>
      <div class="flex flex-col">
        <span class="text-lg font-bold">{{ weather.condition }}</span>
        <span class="text-[0.8rem] opacity-90">{{ t('profile.humidityLabel', { value: weather.humidity }) }}</span>
      </div>
    </div>

    <div class="absolute right-[-10px] bottom-[-10px] scale-[2] origin-bottom-right z-[1] pointer-events-none">
      <svg xmlns="http://www.w3.org/2000/svg" width="60" height="60" viewBox="0 0 256 256"><path fill="rgba(255,255,255,0.15)" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/></svg>
    </div>
  </div>
</template>
