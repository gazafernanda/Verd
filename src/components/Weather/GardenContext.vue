<script setup lang="ts">
const contexts = [
  'Tropical Plants',
  'Succulents & Cacti',
  'Vegetable Garden'
]
</script>

<template>
  <div class="garden-context">
    <h3 class="title">Garden Context</h3>
    
    <div class="list">
       <div v-for="(item, index) in contexts" :key="index" class="list-item">
          {{ item }}
       </div>
    </div>
  </div>
</template>

<style scoped>
.garden-context {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 24px;
  box-shadow: var(--shadow-sm);
  margin-bottom: 24px;
}

.title {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 16px;
}

.list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.list-item {
  padding: 12px 16px;
  background-color: #f9fafb;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
  color: var(--text-main);
}
</style>
