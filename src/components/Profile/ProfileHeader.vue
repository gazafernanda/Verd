<script setup lang="ts">
import { useUserStore } from '../../stores/user'
const user = useUserStore()
</script>

<template>
  <div class="bg-surface rounded-2xl px-10 py-8 flex justify-between items-center shadow-sm border border-border max-md:flex-col max-md:items-start max-md:gap-6 max-md:px-6 max-md:py-6">
    <div class="flex items-center gap-6">
      <div class="relative w-24 h-24 rounded-full border-4 border-[#f6cfb0] bg-[#fcece0] flex items-center justify-center">
        <img src="/src/assets/alex_avatar.png" :alt="user.name" class="w-[88px] h-[88px] rounded-full object-cover" />
        <div class="absolute bottom-0 -right-1 bg-success-green text-white w-6 h-6 rounded-full flex items-center justify-center border-2 border-surface">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 256 256"><path fill="currentColor" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24Zm45.66,85.66-56,56a8,8,0,0,1-11.32,0l-24-24a8,8,0,0,1,11.32-11.32L112,148.69l50.34-50.35a8,8,0,0,1,11.32,11.32Z"/></svg>
        </div>
      </div>

      <div class="flex flex-col">
        <h1 class="text-[2rem] font-extrabold text-text-main leading-none mb-2">{{ user.name }}</h1>
        <div class="flex items-center gap-2 mb-3 text-[0.95rem] font-medium">
          <span class="flex items-center gap-1 text-text-muted">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 256 256"><path fill="currentColor" d="M128,64a40,40,0,1,0,40,40A40,40,0,0,0,128,64Zm0,64a24,24,0,1,1,24-24A24,24,0,0,1,128,128Zm0-112a88.1,88.1,0,0,0-88,88c0,31.4,14.51,64.68,42,96.25a254.19,254.19,0,0,0,41.45,38.3,8,8,0,0,0,9.18,0A254.19,254.19,0,0,0,174,200.25c27.45-31.57,42-64.85,42-96.25A88.1,88.1,0,0,0,128,16Zm0,206c-16.53-13-72-60.75-72-118a72,72,0,0,1,144,0C200,161.23,144.53,209,128,222Z"/></svg>
            {{ user.location }}
          </span>
          <span class="text-text-muted">•</span>
          <span class="text-success-green">{{ user.tier }} Tier</span>
        </div>
        <p class="text-[0.7rem] font-extrabold text-text-muted tracking-widest m-0">MEMBER SINCE {{ user.memberSince.toUpperCase() }}</p>
      </div>
    </div>

    <div class="flex gap-4">
      <button class="px-6 py-3 rounded-[24px] text-[0.9rem] font-bold cursor-pointer bg-transparent text-text-main border border-border hover:bg-bg-app transition-colors">Edit Profile</button>
      <button class="px-6 py-3 rounded-[24px] text-[0.9rem] font-bold cursor-pointer bg-primary text-white border border-primary shadow-[0_4px_12px_rgba(26,86,65,0.2)] hover:bg-primary-hover transition-colors">Share Profile</button>
    </div>
  </div>
</template>
