<script setup lang="ts">
</script>

<template>
  <div class="flex flex-col">
    <div class="mb-6 h-6 flex items-center">
      <h2 class="text-[1.35rem] font-extrabold text-text-main m-0">Botanical Insights</h2>
    </div>

    <div class="bg-light-green-bg rounded-xl py-8 px-7 border border-[rgba(55,178,126,0.15)]">
      <span class="block text-[0.7rem] font-extrabold text-success-green tracking-[1px] mb-6">WHY THESE ACTIONS?</span>

      <p class="text-[1.15rem] font-bold text-text-main leading-relaxed mb-8">
        High solar radiation (UV 8+) significantly increases the
        <span class="italic text-success-green border-b-2 border-success-green">transpiration rate</span>
        of vegetable crops.
      </p>

      <ul class="list-none p-0 m-0">
        <li class="flex items-start gap-3">
          <div class="mt-1 text-primary shrink-0">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,24a8,8,0,0,1,8,8v88H224a8,8,0,0,1,0,16H136v88a8,8,0,0,1-16,0V136H32a8,8,0,0,1,0-16H120V32A8,8,0,0,1,128,24Z" transform="translate(128, -53) rotate(45) "/></svg>
          </div>
          <p class="text-[0.95rem] text-text-muted leading-relaxed font-medium m-0">
            Stomata are currently wide open to facilitate cooling, leading to rapid water loss from soil surface.
          </p>
        </li>
      </ul>
    </div>
  </div>
</template>
