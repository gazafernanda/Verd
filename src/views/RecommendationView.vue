<script setup lang="ts">
import MetricsRow from '../components/Recommendation/MetricsRow.vue'
import PriorityActions from '../components/Recommendation/PriorityActions.vue'
import BotanicalInsights from '../components/Recommendation/BotanicalInsights.vue'
</script>

<template>
  <div class="flex flex-col gap-8 max-w-[1200px]">
    <div class="flex flex-col items-start gap-4 max-w-[600px]">
      <div class="inline-flex items-center gap-2 px-3 py-1.5 bg-light-green-bg rounded-[16px] text-[0.65rem] font-extrabold text-success-green tracking-[0.5px]">
        LIVE ANALYSIS
        <span class="w-1.5 h-1.5 bg-red-500 rounded-full animate-pulse-dot"></span>
      </div>

      <h1 class="text-[3rem] font-extrabold text-text-main leading-[1.1] tracking-[-1px] m-0">
        Vegetables<br>Recommendations
      </h1>
      <p class="text-base font-medium text-text-muted leading-relaxed mb-2 m-0">
        Hyper-local agricultural insights based on real-time soil and weather sensors.
      </p>

      <div class="flex gap-4">
        <button class="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl text-[0.9rem] font-semibold cursor-pointer transition-colors duration-200 bg-transparent text-text-main border border-border hover:bg-surface">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,16a8,8,0,0,0-8,8V48H40a16,16,0,0,0-16,16V208a16,16,0,0,0,16,16H216a16,16,0,0,0,16-16V64a16,16,0,0,0-16-16H136V24A8,8,0,0,0,128,16ZM40,64H119a8,8,0,0,0,0-16H40ZM216,64V208H40V64H216ZM152,104a8,8,0,0,1,8-8h24a8,8,0,0,1,0,16H160A8,8,0,0,1,152,104ZM96,160a24,24,0,1,1-24-24A24,24,0,0,1,96,160Zm0,0v16H80v-16Z"/></svg>
          Switch Category
        </button>
        <button class="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl text-[0.9rem] font-semibold cursor-pointer transition-colors duration-200 bg-primary text-white border border-primary shadow-[0_4px_12px_rgba(26,86,65,0.2)] hover:bg-primary-hover">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M224,128a96,96,0,1,1-96-96,8,8,0,0,1,0,16,80,80,0,1,0,80,80A8,8,0,0,1,224,128Z"/></svg>
          Update Data
        </button>
      </div>
    </div>

    <MetricsRow />

    <div class="grid grid-cols-[2fr_1fr] gap-8 max-lg:grid-cols-1">
      <PriorityActions />
      <BotanicalInsights />
    </div>
  </div>
</template>
