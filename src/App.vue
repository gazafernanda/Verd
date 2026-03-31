<script setup lang="ts">
import Sidebar from './components/Sidebar.vue'
import { RouterView, useRoute } from 'vue-router'

const route = useRoute()
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
    <Sidebar />
    <div class="flex-1 py-10 px-12 overflow-y-auto">
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
</style>
