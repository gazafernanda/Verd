<script setup lang="ts">
</script>

<template>
  <div class="activity-stats">
    <div class="widget-header">
      <h2 class="widget-title">Activity Stats</h2>
    </div>
    
    <div class="stats-container">
      <div class="stat-card">
        <span class="stat-value">12</span>
        <span class="stat-label">TOTAL PLANTS</span>
      </div>
      
      <div class="stat-card">
        <span class="stat-value">45</span>
        <span class="stat-label">CARE STREAK</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.activity-stats {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 24px;
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--border-color);
}

.widget-header {
  margin-bottom: 24px;
}

.widget-title {
  font-size: 1.25rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0;
}

.stats-container {
  display: flex;
  gap: 16px;
}

.stat-card {
  flex: 1;
  background-color: var(--bg-color);
  border-radius: var(--radius-lg);
  padding: 24px 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border: 1px solid rgba(26, 86, 65, 0.05);
}

.stat-value {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--primary-color);
  line-height: 1;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 0.65rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 0.5px;
  text-transform: uppercase;
}
</style>
