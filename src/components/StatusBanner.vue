<script setup lang="ts">
</script>

<template>
  <div class="status-banner">
    <div class="status-indicator"></div>
    <div class="status-content">
      <div class="status-icon">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="#ffffff" d="M173.66,98.34a8,8,0,0,1,0,11.32l-56,56a8,8,0,0,1-11.32,0l-24-24a8,8,0,0,1,11.32-11.32L112,148.69l50.34-50.35A8,8,0,0,1,173.66,98.34ZM224,128A96,96,0,1,1,128,32,96.11,96.11,0,0,1,224,128Zm-16,0a80,80,0,1,0-80,80A80.09,80.09,0,0,0,208,128Z"/></svg>
      </div>
      <div class="status-text">
        <h3 class="status-title">Status: Safe <span class="dot"></span></h3>
        <p class="status-desc">Your garden is thriving! Current environment matches all plant requirements.</p>
      </div>
      <div class="status-action">
        <button class="btn-report">View Full Report</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.status-banner {
  background-color: var(--surface-color);
  border-radius: var(--radius-lg);
  position: relative;
  overflow: hidden;
  box-shadow: var(--shadow-sm);
  margin-bottom: 24px;
}

.status-indicator {
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 6px;
  background-color: var(--success-green);
}

.status-content {
  padding: 24px 32px;
  display: flex;
  align-items: center;
  gap: 20px;
}

.status-icon {
  width: 48px;
  height: 48px;
  background-color: var(--light-green-bg);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.status-icon svg {
  fill: var(--success-green);
  width: 32px;
  height: 32px;
}

.status-text {
  flex: 1;
}

.status-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--text-main);
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 4px;
}

.dot {
  width: 8px;
  height: 8px;
  background-color: var(--success-green);
  border-radius: 50%;
}

.status-desc {
  color: var(--text-muted);
  font-size: 0.95rem;
}

.btn-report {
  padding: 10px 20px;
  background-color: var(--light-green-bg);
  color: var(--primary-color);
  border-radius: 20px;
  font-weight: 600;
  font-size: 0.9rem;
  transition: var(--transition-fast);
}

.btn-report:hover {
  background-color: #d1ebe2;
}
</style>
