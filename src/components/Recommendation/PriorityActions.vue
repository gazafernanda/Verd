<script setup lang="ts">
import { Droplet, SunLight } from '@iconoir/vue'
</script>

<template>
  <div class="flex flex-col">
    <div class="flex items-center gap-4 mb-6">
      <h2 class="text-[1.35rem] font-extrabold text-text-main m-0 whitespace-nowrap">Priority Actions</h2>
      <div class="flex-1 h-0.5 bg-border"></div>
    </div>

    <div class="flex flex-col gap-5">
      <!-- Water Action -->
      <div class="flex gap-4 bg-surface rounded-xl p-5 shadow-sm border border-[rgba(55,178,126,0.5)]">
        <div class="shrink-0">
          <div class="w-10 h-10 rounded-full flex items-center justify-center bg-light-green-bg text-success-green">
            <Droplet width="20" height="20" />
          </div>
        </div>
        <div class="flex-1 min-w-0 flex flex-col">
          <div class="flex flex-wrap justify-between items-start gap-2 mb-2">
            <h3 class="text-[1.05rem] font-extrabold text-text-main m-0">Water deeply at base</h3>
            <span class="text-[0.65rem] font-extrabold px-2.5 py-1 rounded tracking-[0.5px] bg-primary text-white shrink-0">IMMEDIATE</span>
          </div>
          <p class="text-[0.9rem] text-text-muted leading-relaxed mb-4 m-0">
            Apply roughly 2 liters per m² specifically for tomatoes and zucchini to prevent heat-stress wilting.
          </p>
          <div class="flex flex-wrap gap-2">
            <button class="py-2 px-4 rounded-[20px] text-[0.85rem] font-bold cursor-pointer transition-colors duration-200 border-2 bg-primary text-white border-primary hover:bg-primary-hover">
              Mark Completed
            </button>
            <button class="py-2 px-4 rounded-[20px] text-[0.85rem] font-bold cursor-pointer transition-colors duration-200 border-2 bg-transparent text-text-main border-border hover:bg-surface">
              Set Reminder
            </button>
          </div>
        </div>
      </div>

      <!-- Shade Cloth Action -->
      <div class="flex gap-4 bg-surface rounded-xl p-5 shadow-sm border border-[rgba(251,146,60,0.4)]">
        <div class="shrink-0">
          <div class="w-10 h-10 rounded-full flex items-center justify-center bg-[#fff4e5] text-[#f59e0b]">
            <SunLight width="20" height="20" />
          </div>
        </div>
        <div class="flex-1 min-w-0 flex flex-col">
          <div class="flex flex-wrap justify-between items-start gap-2 mb-2">
            <h3 class="text-[1.05rem] font-extrabold text-text-main m-0">Deploy 30% Shade Cloth</h3>
            <span class="text-[0.65rem] font-extrabold px-2.5 py-1 rounded tracking-[0.5px] bg-[#f59e0b] text-white shrink-0">RECOMMENDED</span>
          </div>
          <p class="text-[0.9rem] text-text-muted leading-relaxed mb-4 m-0">
            Protect leafy greens from intense afternoon sun to prevent tip burn and bolting.
          </p>
          <div class="flex flex-wrap gap-2">
            <button class="py-2 px-4 rounded-[20px] text-[0.85rem] font-bold cursor-pointer transition-colors duration-200 border-2 border-transparent bg-[#fff4e5] text-[#f59e0b] hover:bg-[#fde6c6]">
              View Instructions
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
