<script setup lang="ts">
</script>

<template>
  <div class="bg-surface rounded-xl p-6 shadow-sm mb-6">
    <div class="flex items-center gap-3 mb-6">
      <div class="w-6 h-6 bg-[#e6f0ff] rounded-full flex items-center justify-center text-[#3b82f6] shrink-0">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,16a8,8,0,0,0-7.14,4.42C101.91,58.38,48,118.82,48,160a80,80,0,0,0,160,0c0-41.18-53.91-101.62-72.86-139.58A8,8,0,0,0,128,16Zm48.51,139.51a8,8,0,0,1-11,11.05A47.85,47.85,0,0,0,128,152a8,8,0,0,1,0-16,63.85,63.85,0,0,1,48.51,19.51Z"/></svg>
      </div>
      <h3 class="text-base font-bold text-text-main m-0">Rainfall Outlook</h3>
    </div>

    <div class="flex flex-col gap-4 mb-5">
      <div class="flex items-center gap-3">
        <span class="w-[50px] text-[0.65rem] font-bold text-text-muted leading-tight">THIS<br>WEEK</span>
        <div class="flex-1 h-2 bg-[#f3f4f6] rounded overflow-hidden">
          <div class="h-full rounded bg-[#3b82f6]" style="width: 60%"></div>
        </div>
        <span class="w-9 text-[0.8rem] font-bold text-text-main text-right">12mm</span>
      </div>
      <div class="flex items-center gap-3">
        <span class="w-[50px] text-[0.65rem] font-bold text-text-muted leading-tight">AVERAGE</span>
        <div class="flex-1 h-2 bg-[#f3f4f6] rounded overflow-hidden">
          <div class="h-full rounded bg-[#e5e7eb]" style="width: 40%"></div>
        </div>
        <span class="w-9 text-[0.8rem] font-bold text-text-main text-right">8mm</span>
      </div>
    </div>

    <p class="text-[0.75rem] text-text-muted leading-relaxed">
      Precipitation is 35% higher than the 10-year seasonal average for this week. Monitor root rot in succulents.
    </p>
  </div>
</template>
