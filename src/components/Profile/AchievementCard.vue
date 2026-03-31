<script setup lang="ts">
</script>

<template>
  <div class="bg-surface rounded-xl py-8 px-6 shadow-sm border border-border flex flex-col items-center text-center">
    <div class="w-16 h-16 rounded-full flex items-center justify-center mb-4 bg-light-green-bg text-success-green">
      <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="currentColor" d="M224,64H192V48a16,16,0,0,0-16-16H80A16,16,0,0,0,64,48V64H32A16,16,0,0,0,16,80v24a64.07,64.07,0,0,0,64,64h1.16A79.79,79.79,0,0,0,120,207.26V224H96a8,8,0,0,0,0,16h64a8,8,0,0,0,0-16H136V207.26A79.79,79.79,0,0,0,174.84,168H176a64.07,64.07,0,0,0,64-64V80A16,16,0,0,0,224,64ZM80,152a64,64,0,0,1-48-60.8V80H64Zm96,39.38a64,64,0,0,1-96,0V48h96ZM224,104a48.05,48.05,0,0,1-48,48H175.76a64,64,0,0,0,2-16V80h26.24v11.2Z"/></svg>
    </div>

    <h3 class="text-[1.35rem] font-extrabold text-text-main mb-2">Urban Jungler</h3>
    <p class="text-[0.95rem] text-text-muted leading-relaxed mb-6 max-w-[80%]">You've reached level 8 of plant mastery. Keep going!</p>

    <div class="w-full">
      <div class="w-full h-2 bg-border rounded mb-2 overflow-hidden">
        <div class="h-full rounded bg-success-green" style="width: 75%"></div>
      </div>
      <span class="text-[0.7rem] font-extrabold text-success-green tracking-[1px]">750 / 1000 XP</span>
    </div>
  </div>
</template>
