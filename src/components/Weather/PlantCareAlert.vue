<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { usePlantsStore } from '../../stores/plants'
import { Leaf } from 'lucide-vue-next'

const { t, locale } = useI18n()
const router = useRouter()
const plants = usePlantsStore()

const mistingPlants = computed(() => plants.plantsMistingNeeded)
const alertPlants = computed(() => plants.plantsNeedingCare)

function joinNames(list: { name: string }[]) {
  const conj = locale.value === 'id' ? ' dan ' : ' and '
  const names = list.map((p) => p.name).join(conj)
  return `<strong class="font-bold text-primary">${names}</strong>`
}

const alertMessage = computed(() => {
  if (mistingPlants.value.length > 0) {
    return t('weather.plantCareMisting', { names: joinNames(mistingPlants.value) })
  }
  if (alertPlants.value.length > 0) {
    return t('weather.plantCareWarning', { names: joinNames(alertPlants.value) })
  }
  return t('weather.plantCareOk')
})
</script>

<template>
  <div class="bg-light-green-bg rounded-xl p-6 border border-[rgba(55,178,126,0.2)] mb-6">
    <div class="flex items-center gap-3 mb-4">
      <div class="w-8 h-8 bg-success-green rounded-full flex items-center justify-center text-white shrink-0">
        <Leaf width="16" height="16" />
      </div>
      <h3 class="text-[1.1rem] font-bold text-text-main m-0">{{ t('weather.plantCareAlert') }}</h3>
    </div>

    <p class="text-[0.9rem] leading-relaxed text-text-main mb-6" v-html="alertMessage"></p>

    <button @click="router.push({ name: 'recommendation' })" class="w-full py-3.5 bg-success-green text-white rounded-xl font-semibold text-[0.95rem] shadow-[0_4px_12px_rgba(55,178,126,0.2)] transition-all duration-200 hover:bg-[#2ea06e] hover:-translate-y-px">
      {{ t('weather.viewCareSchedule') }}
    </button>
  </div>
</template>
