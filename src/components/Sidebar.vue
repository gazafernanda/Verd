<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/user'

const router = useRouter()
const user = useUserStore()

function logout() {
  user.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <aside class="w-[260px] bg-surface border-r border-border flex flex-col py-8">
    <div class="px-8 mb-10">
      <div class="flex items-center gap-3">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="#1a5641" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/></svg>
        <span class="text-[1.25rem] font-bold text-primary">Verd</span>
      </div>
    </div>

    <nav>
      <ul class="flex flex-col gap-2 px-5">
        <li>
          <router-link
            to="/"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
          >
            <svg class="shrink-0" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="currentColor" d="M112,48V112a8,8,0,0,1-8,8H40a8,8,0,0,1-8-8V48a8,8,0,0,1,8-8h64A8,8,0,0,1,112,48Zm104-8H152a8,8,0,0,0-8,8V112a8,8,0,0,0,8,8h64a8,8,0,0,0,8-8V48A8,8,0,0,0,216,40ZM104,136H40a8,8,0,0,0-8,8v64a8,8,0,0,0,8,8h64a8,8,0,0,0,8-8V144A8,8,0,0,0,104,136Zm112,0H152a8,8,0,0,0-8,8v64a8,8,0,0,0,8,8h64a8,8,0,0,0,8-8V144A8,8,0,0,0,216,136Z"/></svg>
            Dashboard
          </router-link>
        </li>
        <li>
          <router-link
            to="/weather"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
          >
            <svg class="shrink-0" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="currentColor" d="M128,40a8,8,0,0,1,8-8h0a8,8,0,0,1,8,8V64a8,8,0,0,1-16,0V40Zm0,152a8,8,0,0,0-8,8v24a8,8,0,0,0,16,0V200A8,8,0,0,0,128,192ZM64,128a8,8,0,0,0-8-8H32a8,8,0,0,0,0,16H56A8,8,0,0,0,64,128Zm160-8H200a8,8,0,0,0,0,16h24a8,8,0,0,0,0-16ZM82.75,82.75a8,8,0,0,0,11.31-11.32l-17-17a8,8,0,0,0-11.31,11.32ZM190.25,179a8,8,0,0,1-11.31,11.32l-17-17A8,8,0,0,1,173.25,162Zm-11.31-96.2A8,8,0,0,0,190.25,71.43l17-17a8,8,0,0,0-11.31-11.32l-17,17A8,8,0,0,0,178.94,82.75ZM65.75,179l-17,17A8,8,0,0,0,60,207.31l17-17A8,8,0,0,0,65.75,179ZM128,72a56,56,0,1,0,56,56A56.06,56.06,0,0,0,128,72Zm0,96a40,40,0,1,1,40-40A40,40,0,0,1,128,168Z"/></svg>
            Weather
          </router-link>
        </li>
        <li>
          <router-link
            to="/recommendation"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
          >
            <svg class="shrink-0" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="currentColor" d="M224,128a8,8,0,0,1-8,8H136v80a8,8,0,0,1-16,0V136H40a8,8,0,0,1,0-16h80V40a8,8,0,0,1,16,0v80h80A8,8,0,0,1,224,128Z"/></svg>
            Recommendation
          </router-link>
        </li>
        <li>
          <router-link
            to="/chat"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
          >
            <svg class="shrink-0" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="currentColor" d="M216,48H40A16,16,0,0,0,24,64V224a15.84,15.84,0,0,0,9.25,14.5A16.05,16.05,0,0,0,40,240a15.89,15.89,0,0,0,10.25-3.78l36.19-30.16A8,8,0,0,1,91.56,204H216a16,16,0,0,0,16-16V64A16,16,0,0,0,216,48Zm0,140H91.56a24,24,0,0,0-15.33,5.55L40,223.7V64H216V188Z"/></svg>
            Chat AI
          </router-link>
        </li>
        <li>
          <router-link
            to="/profile"
            class="flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main"
            active-class="bg-light-green-bg !text-primary"
          >
            <svg class="shrink-0" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="currentColor" d="M230.92,212c-15.23-26.33-38.7-45.21-66.09-54.16a72,72,0,1,0-73.66,0C63.78,166.78,40.31,185.66,25.08,212a8,8,0,1,0,13.85,8c18.84-32.56,52.14-52,89.07-52s70.23,19.44,89.07,52a8,8,0,1,0,13.85-8ZM72,96a56,56,0,1,1,56,56A56.06,56.06,0,0,1,72,96Z"/></svg>
            Profile
          </router-link>
        </li>
      </ul>
    </nav>

    <div class="flex-1"></div>

    <div class="px-5">
      <button @click="logout" class="w-full flex items-center gap-4 px-5 py-[14px] rounded-lg text-text-muted font-medium transition-colors hover:bg-bg-app hover:text-text-main">
        <svg class="shrink-0" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="currentColor" d="M120,216a8,8,0,0,1-8,8H48a8,8,0,0,1-8-8V40a8,8,0,0,1,8-8h64a8,8,0,0,1,0,16H56V208h56A8,8,0,0,1,120,216Zm109.66-93.66-40-40a8,8,0,0,0-11.32,11.32L204.69,120H104a8,8,0,0,0,0,16H204.69l-26.35,26.34a8,8,0,0,0,11.32,11.32l40-40A8,8,0,0,0,229.66,122.34Z"/></svg>
        Logout
      </button>
    </div>
  </aside>
</template>
