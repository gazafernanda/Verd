<script setup lang="ts">
</script>

<template>
  <div class="flex flex-col">
    <div class="flex items-center gap-4 mb-6">
      <h2 class="text-[1.35rem] font-extrabold text-text-main m-0 whitespace-nowrap">Priority Actions</h2>
      <div class="flex-1 h-0.5 bg-border"></div>
    </div>

    <div class="flex flex-col gap-5">
      <!-- Water Action -->
      <div class="flex gap-5 bg-surface rounded-xl p-6 shadow-sm border border-[rgba(55,178,126,0.5)]">
        <div class="flex flex-col items-center shrink-0">
          <div class="w-11 h-11 rounded-full flex items-center justify-center bg-light-green-bg text-success-green">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M174.55,101.44a80,80,0,1,1-93.1,0A153.25,153.25,0,0,1,128,31.76a153.25,153.25,0,0,1,46.55,69.68Z"/></svg>
          </div>
        </div>
        <div class="flex-1 flex flex-col">
          <div class="flex justify-between items-start mb-2">
            <h3 class="text-[1.15rem] font-extrabold text-text-main m-0">Water deeply at base</h3>
            <span class="text-[0.65rem] font-extrabold px-2.5 py-1 rounded tracking-[0.5px] bg-primary text-white">IMMEDIATE</span>
          </div>
          <p class="text-[0.95rem] text-text-muted leading-relaxed mb-5 m-0 max-w-[90%]">
            Apply roughly 2 liters per m² specifically for tomatoes and zucchini to prevent heat-stress wilting.
          </p>
          <div class="flex gap-3">
            <button class="py-2.5 px-5 rounded-[20px] text-[0.85rem] font-bold cursor-pointer transition-colors duration-200 border-2 bg-primary text-white border-primary hover:bg-primary-hover">
              Mark Completed
            </button>
            <button class="py-2.5 px-5 rounded-[20px] text-[0.85rem] font-bold cursor-pointer transition-colors duration-200 border-2 bg-transparent text-text-main border-border hover:bg-surface">
              Set Reminder
            </button>
          </div>
        </div>
      </div>

      <!-- Shade Cloth Action -->
      <div class="flex gap-5 bg-surface rounded-xl p-6 shadow-sm border border-[rgba(251,146,60,0.4)]">
        <div class="flex flex-col items-center shrink-0">
          <div class="w-11 h-11 rounded-full flex items-center justify-center bg-[#fff4e5] text-[#f59e0b]">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M128,136v96a8,8,0,0,1-16,0V136a8,8,0,0,1,16,0Zm104,16H24a8,8,0,0,1,0-16H232a8,8,0,0,1,0,16Zm11.31-72-24-24a8,8,0,0,0-11.31,11.31L224,84.69V112a8,8,0,0,0,16,0V85.66A8,8,0,0,0,243.31,80ZM16,112V84.69l18.34-18.35A8,8,0,0,0,23,55l-24,24A8,8,0,0,0,1.66,85.66,8,8,0,0,0,8,88H8.69L16,80.69V112a8,8,0,0,0,16,0V85.66Z"/></svg>
          </div>
        </div>
        <div class="flex-1 flex flex-col">
          <div class="flex justify-between items-start mb-2">
            <h3 class="text-[1.15rem] font-extrabold text-text-main m-0">Deploy 30% Shade Cloth</h3>
            <span class="text-[0.65rem] font-extrabold px-2.5 py-1 rounded tracking-[0.5px] bg-[#f59e0b] text-white">RECOMMENDED</span>
          </div>
          <p class="text-[0.95rem] text-text-muted leading-relaxed mb-5 m-0 max-w-[90%]">
            Protect leafy greens from intense afternoon sun to prevent tip burn and bolting.
          </p>
          <div class="flex gap-3">
            <button class="py-2.5 px-5 rounded-[20px] text-[0.85rem] font-bold cursor-pointer transition-colors duration-200 border-2 border-transparent bg-[#fff4e5] text-[#f59e0b] hover:bg-[#fde6c6]">
              View Instructions
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
