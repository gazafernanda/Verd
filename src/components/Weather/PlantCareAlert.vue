<script setup lang="ts">
</script>

<template>
  <div class="care-alert">
    <div class="alert-header">
      <div class="icon-circle">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M222.14,58.87A8,8,0,0,0,216,56H168A88.1,88.1,0,0,0,80,144v7.35l-19.58,19.58a8,8,0,0,0,11.31,11.31L88,165.83A87.27,87.27,0,0,0,144,192c48.52,0,88-39.48,88-88V64A8,8,0,0,0,222.14,58.87ZM216,104a72.08,72.08,0,0,1-72,72,71.29,71.29,0,0,1-36.43-9.87L172,101.66a8,8,0,0,0-11.31-11.32L91.43,159.54A72.31,72.31,0,0,1,96,144a72.08,72.08,0,0,1,72-72h48Z"/></svg>
      </div>
      <h3 class="alert-title">Plant Care Alert</h3>
    </div>
    
    <p class="alert-desc">
      High evaporation predicted today. Your <strong>Boston Ferns</strong> and <strong>Peace Lilies</strong> will need extra misting to maintain humidity above 60%.
    </p>
    
    <button class="btn-schedule">View Care Schedule</button>
  </div>
</template>

<style scoped>
.care-alert {
  background-color: var(--light-green-bg);
  border-radius: var(--radius-xl);
  padding: 24px;
  border: 1px solid rgba(55, 178, 126, 0.2);
  margin-bottom: 24px;
}

.alert-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
}

.icon-circle {
  width: 32px;
  height: 32px;
  background-color: var(--success-green);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.alert-title {
  font-size: 1.1rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0;
}

.alert-desc {
  font-size: 0.9rem;
  line-height: 1.6;
  color: var(--text-main);
  margin-bottom: 24px;
}

strong {
  font-weight: 700;
  color: var(--primary-color);
}

.btn-schedule {
  width: 100%;
  padding: 14px;
  background-color: var(--success-green);
  color: white;
  border-radius: 24px;
  font-weight: 600;
  font-size: 0.95rem;
  transition: var(--transition-fast);
  box-shadow: 0 4px 12px rgba(55, 178, 126, 0.2);
}

.btn-schedule:hover {
  background-color: #2ea06e;
  transform: translateY(-1px);
}
</style>
