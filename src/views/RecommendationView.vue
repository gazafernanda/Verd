<script setup lang="ts">
import MetricsRow from '../components/Recommendation/MetricsRow.vue'
import PriorityActions from '../components/Recommendation/PriorityActions.vue'
import BotanicalInsights from '../components/Recommendation/BotanicalInsights.vue'
</script>

<template>
  <div class="recommendation-view">
    <div class="header-section">
      <div class="live-pill">
        <span class="pill-text">LIVE ANALYSIS</span>
        <span class="pulsing-dot"></span>
      </div>
      
      <h1 class="page-title">Vegetables<br>Recommendations</h1>
      <p class="page-subtitle">Hyper-local agricultural insights based on real-time soil and weather sensors.</p>
      
      <div class="header-actions">
        <button class="btn-secondary">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,16a8,8,0,0,0-8,8V48H40a16,16,0,0,0-16,16V208a16,16,0,0,0,16,16H216a16,16,0,0,0,16-16V64a16,16,0,0,0-16-16H136V24A8,8,0,0,0,128,16ZM40,64H119a8,8,0,0,0,0-16H40ZM216,64V208H40V64H216ZM152,104a8,8,0,0,1,8-8h24a8,8,0,0,1,0,16H160A8,8,0,0,1,152,104ZM96,160a24,24,0,1,1-24-24A24,24,0,0,1,96,160Zm0,0v16H80v-16Z"/></svg>
          Switch Category
        </button>
        <button class="btn-primary">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M224,128a96,96,0,1,1-96-96,8,8,0,0,1,0,16,80,80,0,1,0,80,80A8,8,0,0,1,224,128Z"/></svg>
          Update Data
        </button>
      </div>
    </div>
    
    <div class="metrics-container">
      <MetricsRow />
    </div>
    
    <div class="content-split">
      <div class="left-col">
        <PriorityActions />
      </div>
      <div class="right-col">
        <BotanicalInsights />
      </div>
    </div>
  </div>
</template>

<style scoped>
.recommendation-view {
  display: flex;
  flex-direction: column;
  gap: 32px;
  max-width: 1200px;
}

.header-section {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 16px;
  max-width: 600px;
}

.live-pill {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 6px 12px;
  background-color: var(--light-green-bg);
  border-radius: 16px;
  font-size: 0.65rem;
  font-weight: 800;
  color: var(--success-green);
  letter-spacing: 0.5px;
}

.pulsing-dot {
  width: 6px;
  height: 6px;
  background-color: #ef4444; /* red dot for live */
  border-radius: 50%;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0.7); }
  70% { transform: scale(1); box-shadow: 0 0 0 4px rgba(239, 68, 68, 0); }
  100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0); }
}

.page-title {
  font-size: 3rem;
  font-weight: 800;
  color: var(--text-main);
  line-height: 1.1;
  letter-spacing: -1px;
  margin: 0;
}

.page-subtitle {
  font-size: 1rem;
  font-weight: 500;
  color: var(--text-muted);
  line-height: 1.6;
  margin: 0 0 8px 0;
}

.header-actions {
  display: flex;
  gap: 16px;
}

.btn-secondary, .btn-primary {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border-radius: 24px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition-fast);
}

.btn-secondary {
  background-color: transparent;
  color: var(--text-main);
  border: 1px solid var(--border-color);
}

.btn-secondary:hover {
  background-color: var(--surface-color);
}

.btn-primary {
  background-color: var(--primary-color);
  color: white;
  border: 1px solid var(--primary-color);
  box-shadow: 0 4px 12px rgba(26, 86, 65, 0.2);
}

.btn-primary:hover {
  background-color: var(--primary-hover);
}

.content-split {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 32px;
}

@media (max-width: 1024px) {
  .content-split {
    grid-template-columns: 1fr;
  }
}
</style>
