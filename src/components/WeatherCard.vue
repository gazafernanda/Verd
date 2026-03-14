<script setup lang="ts">
</script>

<template>
  <div class="weather-card">
    <div class="card-header">
      <div>
        <p class="section-label">WEATHER FORECAST</p>
        <h3 class="card-title">Partly Cloudy</h3>
      </div>
      <div class="weather-icon-top">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#fbbd06" d="M120,40V16a8,8,0,0,1,16,0V40a8,8,0,0,1-16,0Zm8,24a64,64,0,1,0,64,64A64.07,64.07,0,0,0,128,64Zm0,112a48,48,0,1,1,48-48A48.05,48.05,0,0,1,128,176ZM58.34,69.66A8,8,0,0,0,69.66,58.34l-17-17a8,8,0,0,0-11.32,11.32Zm0,116.68-17,17a8,8,0,0,0,11.32,11.32l17-17a8,8,0,0,0-11.32-11.32ZM203.31,197.66a8,8,0,0,0,11.33-11.32l-17-17a8,8,0,0,0-11.32,11.32ZM216,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,216,120ZM40,120a8,8,0,0,0,8-8V88a8,8,0,0,0-16,0v24A8,8,0,0,0,40,120ZM183,52.48a8,8,0,0,0-5.63-13.68H144a8,8,0,0,0,0,16h33.35A8,8,0,0,0,183,52.48Z"/></svg>
      </div>
    </div>
    
    <div class="weather-content">
      <div class="temperature">
        <span class="value">72°</span><span class="unit">F</span>
      </div>
      <div class="radar-box">
        <div class="radar-gradient">
           <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#ffffff" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24Zm0,192a88,88,0,1,1,88-88A88.1,88.1,0,0,1,128,216Zm40-88a40,40,0,1,1-40-40A40,40,0,0,1,168,128Z"/></svg>
        </div>
      </div>
    </div>

    <div class="weather-details">
      <div class="detail-badge">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M176.81,95A64.12,64.12,0,0,0,128,64a64.12,64.12,0,0,0-48.81,31C61.85,123,54.7,159.08,71,185.81a64,64,0,0,0,114,0C201.3,159.08,194.15,123,176.81,95Zm-12.06,82.47a48,48,0,0,1-73.5,0c-11.83-19.34-6.42-47.53,7.57-69.51A48.06,48.06,0,0,1,128,80a48.06,48.06,0,0,1,29.18,27.93C171.17,130,176.58,158.13,164.75,177.47Z"/></svg>
        45%
      </div>
      <div class="detail-badge is-green">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M184,184a32,32,0,0,1-32,32H40a8,8,0,0,1,0-16H152a16,16,0,0,0,0-32H24a8,8,0,0,1,0-16H184A32,32,0,0,1,184,184Zm32-80a32,32,0,0,0-32-32H72a8,8,0,0,0,0,16H184a16,16,0,0,1,0,32H32a8,8,0,0,0,0,16H184A32,32,0,0,0,216,104Z"/></svg>
        12 mph
      </div>
    </div>
  </div>
</template>

<style scoped>
.weather-card {
  background-color: var(--surface-color);
  border-radius: var(--radius-lg);
  padding: 24px;
  box-shadow: var(--shadow-sm);
  flex: 1;
  display: flex;
  flex-direction: column;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 24px;
}

.section-label {
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--success-green);
  letter-spacing: 1px;
  margin-bottom: 4px;
}

.card-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text-main);
}

.weather-icon-top {
  width: 48px;
  height: 48px;
  background-color: #fff9eb;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.weather-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
}

.temperature {
  display: flex;
  align-items: flex-start;
}

.value {
  font-size: 4.5rem;
  font-weight: 800;
  line-height: 1;
  letter-spacing: -2px;
  color: var(--text-main);
}

.unit {
  font-size: 2rem;
  font-weight: 500;
  color: var(--text-light);
  margin-top: 8px;
}

.radar-box {
  width: 72px;
  height: 72px;
  border-radius: 20px;
  background: linear-gradient(135deg, #a3c4cb 0%, #4b8994 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8px 16px rgba(75, 137, 148, 0.3);
}

.radar-gradient {
  display: flex;
  align-items: center;
  justify-content: center;
}

.weather-details {
  display: flex;
  gap: 16px;
}

.detail-badge {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 16px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  color: #4b8ae6;
  background-color: #f0f5ff;
}

.detail-badge.is-green {
  color: var(--success-green);
  background-color: var(--light-green-bg);
}
</style>
