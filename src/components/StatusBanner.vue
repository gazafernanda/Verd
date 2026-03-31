<script setup lang="ts">
import { computed } from 'vue'
import { usePlantsStore } from '../stores/plants'

const plants = usePlantsStore()

const statusConfig = computed(() => {
  if (plants.overallStatus === 'safe') {
    return {
      borderColor: 'bg-success-green',
      iconBg: 'bg-light-green-bg',
      iconColor: '#37b27e',
      dotColor: 'bg-success-green',
      title: 'Status: Safe',
      message: 'Your garden is thriving! Current environment matches all plant requirements.',
      btnClass: 'bg-light-green-bg text-primary hover:bg-[#d1ebe2]',
    }
  }
  if (plants.overallStatus === 'warning') {
    return {
      borderColor: 'bg-[#f59e0b]',
      iconBg: 'bg-[#fff4e5]',
      iconColor: '#f59e0b',
      dotColor: 'bg-[#f59e0b]',
      title: 'Attention Needed',
      message: `${plants.plantsNeedingCare.length} plant${plants.plantsNeedingCare.length > 1 ? 's' : ''} need${plants.plantsNeedingCare.length === 1 ? 's' : ''} care: ${plants.plantsNeedingCare.map((p) => p.name).join(', ')}.`,
      btnClass: 'bg-[#fff4e5] text-[#f59e0b] hover:bg-[#fde6c6]',
    }
  }
  return {
    borderColor: 'bg-[#3b82f6]',
    iconBg: 'bg-[#ebf5ff]',
    iconColor: '#3b82f6',
    dotColor: 'bg-[#3b82f6]',
    title: 'Action Required',
    message: `${plants.plantsNeedingCare.map((p) => p.name).join(', ')} need${plants.plantsNeedingCare.length === 1 ? 's' : ''} attention soon.`,
    btnClass: 'bg-[#ebf5ff] text-[#3b82f6] hover:bg-[#daeeff]',
  }
})
</script>

<template>
  <div class="bg-surface rounded-lg relative overflow-hidden shadow-sm mb-6">
    <div class="absolute left-0 top-0 bottom-0 w-1.5" :class="statusConfig.borderColor"></div>
    <div class="px-8 py-6 flex items-center gap-5">
      <div class="w-12 h-12 rounded-full flex items-center justify-center shrink-0" :class="statusConfig.iconBg">
        <svg v-if="plants.overallStatus === 'safe'" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path :fill="statusConfig.iconColor" d="M173.66,98.34a8,8,0,0,1,0,11.32l-56,56a8,8,0,0,1-11.32,0l-24-24a8,8,0,0,1,11.32-11.32L112,148.69l50.34-50.35A8,8,0,0,1,173.66,98.34ZM224,128A96,96,0,1,1,128,32,96.11,96.11,0,0,1,224,128Zm-16,0a80,80,0,1,0-80,80A80.09,80.09,0,0,0,208,128Z"/></svg>
        <svg v-else xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path :fill="statusConfig.iconColor" d="M236.8,188.09,149.35,36.22a24.76,24.76,0,0,0-42.7,0L19.2,188.09a23.51,23.51,0,0,0,0,23.72A24.35,24.35,0,0,0,40.55,224h174.9a24.35,24.35,0,0,0,21.33-12.19A23.51,23.51,0,0,0,236.8,188.09ZM120,104a8,8,0,0,1,16,0v40a8,8,0,0,1-16,0Zm8,88a12,12,0,1,1,12-12A12,12,0,0,1,128,192Z"/></svg>
      </div>
      <div class="flex-1">
        <h3 class="text-xl font-bold text-text-main flex items-center gap-2 mb-1">
          {{ statusConfig.title }}
          <span class="w-2 h-2 rounded-full inline-block" :class="statusConfig.dotColor"></span>
        </h3>
        <p class="text-text-muted text-[0.95rem]">{{ statusConfig.message }}</p>
      </div>
      <div>
        <button class="px-5 py-2.5 rounded-[20px] font-semibold text-[0.9rem] transition-colors duration-200" :class="statusConfig.btnClass">
          View Full Report
        </button>
      </div>
    </div>
  </div>
</template>
