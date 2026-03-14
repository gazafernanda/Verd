<script setup lang="ts">
</script>

<template>
  <div class="metrics-row">
    <!-- Soil Moisture -->
    <div class="metric-card">
      <div class="card-header">
        <span class="label">HYDRATION</span>
        <div class="icon-circle bg-green">
           <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M174.55,101.44a80,80,0,1,1-93.1,0A153.25,153.25,0,0,1,128,31.76a153.25,153.25,0,0,1,46.55,69.68Z"/></svg>
        </div>
      </div>
      <h3 class="card-title">Soil Moisture</h3>
      
      <div class="value-row">
        <span class="main-value">22%</span>
        <span class="trend down">
           <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 256 256"><path fill="currentColor" d="M205.66,202.34a8,8,0,0,1-11.32,11.32l-96-96a8,8,0,0,1,0-11.32l96-96a8,8,0,0,1,11.32,11.32L115.31,112Z" transform="rotate(-90 128 128)"/></svg>
           5%
        </span>
      </div>
      
      <div class="status-bar-container">
         <div class="status-bar red" style="width: 20%;"></div>
      </div>
      <p class="card-footer text-red">Critical: Watering needed within 2 hours</p>
    </div>

    <!-- UV Index -->
    <div class="metric-card border-orange">
      <div class="card-header">
        <span class="label">EXPOSURE</span>
        <div class="icon-circle bg-orange">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M120,40V16a8,8,0,0,1,16,0V40a8,8,0,0,1-16,0Zm8,24a64,64,0,1,0,64,64A64.07,64.07,0,0,0,128,64Zm0,112a48,48,0,1,1,48-48A48.05,48.05,0,0,1,128,176ZM58.34,69.66A8,8,0,0,0,69.66,58.34l-17-17a8,8,0,0,0-11.32,11.32Zm0,116.68-17,17a8,8,0,0,0,11.32,11.32l17-17a8,8,0,0,0-11.32-11.32ZM203.31,197.66a8,8,0,0,0,11.33-11.32l-17-17a8,8,0,0,0-11.32,11.32ZM216,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,216,120ZM40,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,40,120ZM183,52.48a8,8,0,0,0-5.63-13.68H144a8,8,0,0,0,0,16h33.35A8,8,0,0,0,183,52.48Z"/></svg>
        </div>
      </div>
      <h3 class="card-title">UV Index</h3>
      
      <div class="value-row">
        <span class="main-value">8.2</span>
        <span class="pill-warning">HIGH RISK</span>
      </div>
      
      <div class="status-multi-bar">
         <div class="segment orange active"></div>
         <div class="segment orange active"></div>
         <div class="segment orange active"></div>
         <div class="segment orange active"></div>
         <div class="segment grey"></div>
      </div>
      <p class="card-footer">Peak intensity expected at 1:45 PM</p>
    </div>

    <!-- Humidity -->
    <div class="metric-card">
      <div class="card-header">
        <span class="label">ATMOSPHERE</span>
        <div class="icon-circle bg-blue">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M176.81,95A64.12,64.12,0,0,0,128,64a64.12,64.12,0,0,0-48.81,31C61.85,123,54.7,159.08,71,185.81a64,64,0,0,0,114,0C201.3,159.08,194.15,123,176.81,95Z"/></svg>
        </div>
      </div>
      <h3 class="card-title">Humidity</h3>
      
      <div class="value-row">
        <span class="main-value">45%</span>
        <span class="trend up">
           <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 256 256"><path fill="currentColor" d="M205.66,202.34a8,8,0,0,1-11.32,11.32l-96-96a8,8,0,0,1,0-11.32l96-96a8,8,0,0,1,11.32,11.32L115.31,112Z" transform="rotate(90 128 128)"/></svg>
           1%
        </span>
      </div>
      
      <div class="status-bar-spacer"></div>
      <p class="card-footer text-green">Stable environment for leafy greens</p>
    </div>
  </div>
</template>

<style scoped>
.metrics-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 24px;
}

@media (max-width: 900px) {
  .metrics-row {
    grid-template-columns: 1fr;
  }
}

.metric-card {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 24px;
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
}

.metric-card.border-orange {
  border-color: rgba(251, 146, 60, 0.3);
  box-shadow: 0 4px 12px rgba(251, 146, 60, 0.05);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.label {
  font-size: 0.65rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 1px;
}

.icon-circle {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon-circle.bg-green { background-color: #e6f6ef; color: var(--success-green); }
.icon-circle.bg-orange { background-color: #fff4e5; color: #f59e0b; }
.icon-circle.bg-blue { background-color: #ebf5ff; color: #3b82f6; }

.card-title {
  font-size: 1.1rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0 0 16px 0;
}

.value-row {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 24px;
  flex: 1;
}

.main-value {
  font-size: 3rem;
  font-weight: 800;
  color: var(--text-main);
  line-height: 1;
  letter-spacing: -1px;
}

.trend {
  display: flex;
  align-items: center;
  gap: 2px;
  font-size: 0.85rem;
  font-weight: 700;
}

.trend.down { color: #ef4444; }
.trend.up { color: var(--success-green); }

.pill-warning {
  background-color: #fff4e5;
  color: #f59e0b;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.65rem;
  font-weight: 800;
}

.status-bar-container {
  width: 100%;
  height: 6px;
  background-color: var(--border-color);
  border-radius: 3px;
  margin-bottom: 12px;
  overflow: hidden;
}

.status-bar {
  height: 100%;
  border-radius: 3px;
}
.status-bar.red { background-color: #ef4444; }

.status-multi-bar {
  display: flex;
  gap: 4px;
  margin-bottom: 12px;
}
.segment {
  height: 6px;
  flex: 1;
  border-radius: 3px;
}
.segment.orange.active { background-color: #f59e0b; }
.segment.grey { background-color: var(--border-color); }

.status-bar-spacer {
  height: 6px;
  margin-bottom: 12px;
}

.card-footer {
  font-size: 0.75rem;
  color: var(--text-muted);
  margin: 0;
  font-weight: 500;
}
.card-footer.text-red { color: #ef4444; }
.card-footer.text-green { color: var(--success-green); }
</style>
