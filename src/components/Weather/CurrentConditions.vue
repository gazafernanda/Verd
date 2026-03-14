<script setup lang="ts">
</script>

<template>
  <div class="current-conditions">
    <div class="card-header">
      <span class="section-label">CURRENT CONDITIONS</span>
      <div class="aqi-badge">
        <span class="aqi-label">AQI INDEX</span>
        <span class="aqi-value">24 - Excellent</span>
      </div>
    </div>
    
    <div class="main-temp-section">
      <div class="temperature">
        <span class="value">72°</span><span class="unit">F</span>
      </div>
      <div class="weather-icon">
        <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 256 256"><path fill="#fbbd06" d="M120,40V16a8,8,0,0,1,16,0V40a8,8,0,0,1-16,0Zm8,24a64,64,0,1,0,64,64A64.07,64.07,0,0,0,128,64Zm0,112a48,48,0,1,1,48-48A48.05,48.05,0,0,1,128,176ZM58.34,69.66A8,8,0,0,0,69.66,58.34l-17-17a8,8,0,0,0-11.32,11.32Zm0,116.68-17,17a8,8,0,0,0,11.32,11.32l17-17a8,8,0,0,0-11.32-11.32ZM203.31,197.66a8,8,0,0,0,11.33-11.32l-17-17a8,8,0,0,0-11.32,11.32ZM216,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,216,120ZM40,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,40,120ZM183,52.48a8,8,0,0,0-5.63-13.68H144a8,8,0,0,0,0,16h33.35A8,8,0,0,0,183,52.48Z"/></svg>
      </div>
    </div>
    
    <p class="condition-text">Sunny • Feels like 75°F</p>

    <div class="metrics-grid">
      <div class="metric-pill">
        <div class="icon-circle">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M176.81,95A64.12,64.12,0,0,0,128,64a64.12,64.12,0,0,0-48.81,31C61.85,123,54.7,159.08,71,185.81a64,64,0,0,0,114,0C201.3,159.08,194.15,123,176.81,95Zm-12.06,82.47a48,48,0,0,1-73.5,0c-11.83-19.34-6.42-47.53,7.57-69.51A48.06,48.06,0,0,1,128,80a48.06,48.06,0,0,1,29.18,27.93C171.17,130,176.58,158.13,164.75,177.47Z"/></svg>
        </div>
        <div class="metric-info">
          <span class="metric-label">HUMIDITY</span>
          <span class="metric-value">45%</span>
        </div>
      </div>
      
      <div class="metric-pill">
        <div class="icon-circle">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,24a104,104,0,1,0,104,104A104.11,104.11,0,0,0,128,24Zm0,192a88,88,0,1,1,88-88A88.1,88.1,0,0,1,128,216ZM168,128a40,40,0,1,1-40-40A40,40,0,0,1,168,128Z"/></svg>
        </div>
        <div class="metric-info">
          <span class="metric-label">UV INDEX</span>
          <span class="metric-value">6 (High)</span>
        </div>
      </div>
      
      <div class="metric-pill">
        <div class="icon-circle">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M174.55,101.44a80,80,0,1,1-93.1,0A153.25,153.25,0,0,1,128,31.76a153.25,153.25,0,0,1,46.55,69.68Z"/></svg>
        </div>
        <div class="metric-info">
          <span class="metric-label">SOIL MOISTURE</span>
          <span class="metric-value">32%</span>
        </div>
      </div>
      
      <div class="metric-pill">
        <div class="icon-circle">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M184,184a32,32,0,0,1-32,32H40a8,8,0,0,1,0-16H152a16,16,0,0,0,0-32H24a8,8,0,0,1,0-16H184A32,32,0,0,1,184,184Zm32-80a32,32,0,0,0-32-32H72a8,8,0,0,0,0,16H184a16,16,0,0,1,0,32H32a8,8,0,0,0,0,16H184A32,32,0,0,0,216,104Z"/></svg>
        </div>
        <div class="metric-info">
          <span class="metric-label">WIND SPEED</span>
          <span class="metric-value">8 mph</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.current-conditions {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 32px;
  box-shadow: var(--shadow-sm);
  margin-bottom: 32px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 24px;
}

.section-label {
  font-size: 0.8rem;
  font-weight: 700;
  color: var(--success-green);
  letter-spacing: 1px;
}

.aqi-badge {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  border: 1px solid var(--success-green);
  border-radius: 20px;
  padding: 6px 16px;
  background-color: var(--surface-color);
}

.aqi-label {
  font-size: 0.6rem;
  font-weight: 700;
  color: var(--text-muted);
  letter-spacing: 0.5px;
}

.aqi-value {
  font-size: 0.85rem;
  font-weight: 700;
  color: var(--success-green);
}

.main-temp-section {
  display: flex;
  align-items: center;
  gap: 24px;
  margin-bottom: 12px;
}

.temperature {
  display: flex;
  align-items: flex-start;
}

.value {
  font-size: 6.5rem;
  font-weight: 800;
  line-height: 0.9;
  letter-spacing: -3px;
  color: var(--text-main);
}

.unit {
  font-size: 2.5rem;
  font-weight: 600;
  color: var(--text-main);
  margin-top: 8px;
}

.condition-text {
  font-size: 1.25rem;
  color: var(--text-muted);
  font-weight: 500;
  margin-bottom: 32px;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}

.metric-pill {
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 20px 24px;
  border-radius: 40px;
  background-color: #f6fbfa;
  box-shadow: 0 4px 12px rgba(55, 178, 126, 0.05);
}

.icon-circle {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background-color: var(--success-green);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.metric-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.metric-label {
  font-size: 0.65rem;
  font-weight: 700;
  color: var(--text-muted);
  letter-spacing: 0.5px;
}

.metric-value {
  font-size: 1.1rem;
  font-weight: 700;
  color: var(--text-main);
}
</style>
