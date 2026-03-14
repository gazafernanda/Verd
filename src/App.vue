<script setup lang="ts">
import Sidebar from './components/Sidebar.vue'
import { RouterView } from 'vue-router'
</script>

<template>
  <main class="app-container">
    <Sidebar />
    <div class="content">
      <RouterView />
    </div>
  </main>
</template>

<style scoped>
.app-container {
  width: 100%;
  height: 100vh;
  display: flex;
  background-color: var(--bg-color);
  overflow: hidden;
}

.content {
  flex: 1;
  padding: 40px 48px;
  overflow-y: auto;
}
</style>
