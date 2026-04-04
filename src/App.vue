<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Sidebar from './components/Sidebar.vue'
import { RouterView, useRoute } from 'vue-router'
import { useUserStore } from './stores/user'
import { Menu } from '@iconoir/vue'

const route = useRoute()
const sidebarOpen = ref(false)
const user = useUserStore()

onMounted(() => {
  if (user.isAuthenticated && !user.location) {
    user.detectLocationFromIP()
  }
})
</script>

<template>
  <!-- Auth pages: full screen, no sidebar -->
  <RouterView v-if="route.meta.hideLayout" v-slot="{ Component }">
    <Transition name="page" mode="out-in">
      <component :is="Component" />
    </Transition>
  </RouterView>

  <!-- App pages: sidebar layout -->
  <main v-else class="w-full h-screen flex bg-bg-app overflow-hidden">

    <!-- Mobile overlay -->
    <Transition name="fade">
      <div
        v-if="sidebarOpen"
        class="fixed inset-0 z-40 bg-black/30 lg:hidden"
        @click="sidebarOpen = false"
      />
    </Transition>

    <!-- Sidebar -->
    <div :class="[
      'max-lg:fixed max-lg:inset-y-0 max-lg:left-0 max-lg:z-50 max-lg:transition-transform max-lg:duration-300 shrink-0',
      sidebarOpen ? 'max-lg:translate-x-0' : 'max-lg:-translate-x-full'
    ]">
      <Sidebar @close="sidebarOpen = false" />
    </div>

    <div class="flex-1 py-10 px-12 max-lg:px-5 max-lg:py-5 overflow-y-auto min-w-0">

      <!-- Mobile top bar -->
      <div class="lg:hidden flex items-center gap-3 mb-6">
        <button
          @click="sidebarOpen = true"
          class="p-2 -ml-2 rounded-lg text-text-main hover:bg-surface transition-colors shrink-0"
        >
          <Menu width="22" height="22" />
        </button>
        <div class="flex items-center gap-2">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 256 256">
            <path fill="#1a5641" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/>
          </svg>
          <span class="text-[1.05rem] font-bold text-primary">Verd</span>
        </div>
        <div class="flex-1"></div>
        <div class="flex items-center gap-2.5">
          <span class="text-[0.9rem] font-semibold text-text-main">{{ user.displayName }}</span>
          <div class="w-8 h-8 rounded-full bg-gradient-to-br from-[#f08b5e] to-[#e85d46] shadow-sm shrink-0"></div>
        </div>
      </div>

      <RouterView v-slot="{ Component }">
        <Transition name="page" mode="out-in">
          <component :is="Component" />
        </Transition>
      </RouterView>
    </div>
  </main>
</template>

<style>
.page-enter-active,
.page-leave-active {
  transition: opacity 0.18s ease, transform 0.18s ease;
}
.page-enter-from {
  opacity: 0;
  transform: translateY(6px);
}
.page-leave-to {
  opacity: 0;
  transform: translateY(-6px);
}
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
