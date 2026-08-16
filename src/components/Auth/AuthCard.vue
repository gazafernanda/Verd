<script setup lang="ts">
/** Centred card shell for the secondary auth pages (verify, forgot, reset). */
</script>

<template>
  <div class="min-h-screen flex flex-col items-center justify-center bg-bg-app px-5 py-12">
    <div class="flex items-center gap-2 mb-8">
      <svg xmlns="http://www.w3.org/2000/svg" width="26" height="26" viewBox="0 0 256 256">
        <path
          fill="#1a5641"
          d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"
        />
      </svg>
      <span class="text-[1.4rem] font-bold text-primary">Verd</span>
    </div>

    <div class="w-full max-w-[440px] bg-surface border border-border rounded-2xl shadow-sm p-8 max-sm:p-6">
      <slot />
    </div>

    <div class="mt-6">
      <slot name="footer" />
    </div>
  </div>
</template>
