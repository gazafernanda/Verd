<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { usePlantsStore } from '../stores/plants'
import { CheckCircle, TriangleAlert } from 'lucide-vue-next'

const { t } = useI18n()
const router = useRouter()
const plants = usePlantsStore()

const statusConfig = computed(() => {
  if (plants.overallStatus === 'safe') {
    return {
      borderColor: 'bg-success-green',
      iconBg: 'bg-light-green-bg',
      iconColor: '#37b27e',
      dotColor: 'bg-success-green',
      title: t('statusBanner.safeTitle'),
      message: t('statusBanner.safeMessage'),
      btnClass: 'bg-light-green-bg text-primary hover:bg-[#d1ebe2]',
    }
  }
  const names = plants.plantsNeedingCare.map((p) => p.name).join(', ')
  if (plants.overallStatus === 'warning') {
    return {
      borderColor: 'bg-[#f59e0b]',
      iconBg: 'bg-[#fff4e5]',
      iconColor: '#f59e0b',
      dotColor: 'bg-[#f59e0b]',
      title: t('statusBanner.warningTitle'),
      message: t('statusBanner.warningMessage', { names }),
      btnClass: 'bg-[#fff4e5] text-[#f59e0b] hover:bg-[#fde6c6]',
    }
  }
  return {
    borderColor: 'bg-[#3b82f6]',
    iconBg: 'bg-[#ebf5ff]',
    iconColor: '#3b82f6',
    dotColor: 'bg-[#3b82f6]',
    title: t('statusBanner.actionTitle'),
    message: t('statusBanner.actionMessage', { names }),
    btnClass: 'bg-[#ebf5ff] text-[#3b82f6] hover:bg-[#daeeff]',
  }
})
</script>

<template>
  <div class="bg-surface rounded-lg relative overflow-hidden shadow-sm mb-6">
    <div class="absolute left-0 top-0 bottom-0 w-1.5" :class="statusConfig.borderColor"></div>
    <div class="px-6 py-5 flex items-start gap-4 max-lg:flex-wrap">
      <div class="w-11 h-11 rounded-full flex items-center justify-center shrink-0 mt-0.5" :class="statusConfig.iconBg">
        <CheckCircle v-if="plants.overallStatus === 'safe'" width="24" height="24" :style="`color: ${statusConfig.iconColor}`" />
        <TriangleAlert v-else width="24" height="24" :style="`color: ${statusConfig.iconColor}`" />
      </div>
      <div class="flex-1 min-w-0">
        <h3 class="text-[1.05rem] font-bold text-text-main flex items-center gap-2 mb-1">
          {{ statusConfig.title }}
          <span class="w-2 h-2 rounded-full inline-block shrink-0" :class="statusConfig.dotColor"></span>
        </h3>
        <p class="text-text-muted text-[0.9rem]">{{ statusConfig.message }}</p>
      </div>
      <div class="max-lg:w-full max-lg:pl-[3.75rem]">
        <button @click="router.push({ name: 'recommendation' })" class="px-5 py-2 rounded-[20px] font-semibold text-[0.85rem] transition-colors duration-200 whitespace-nowrap" :class="statusConfig.btnClass">
          {{ t('statusBanner.viewReport') }}
        </button>
      </div>
    </div>
  </div>
</template>
