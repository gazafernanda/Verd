<script setup lang="ts">
import CurrentConditions from '../components/Weather/CurrentConditions.vue'
import Forecast from '../components/Weather/Forecast.vue'
import PlantCareAlert from '../components/Weather/PlantCareAlert.vue'
import RainfallOutlook from '../components/Weather/RainfallOutlook.vue'
import GardenContext from '../components/Weather/GardenContext.vue'
</script>

<template>
  <div class="weather-view">
    <div class="header-section">
      <!-- Top header with Breadcrumb and User Profile -->
      <div class="top-nav">
         <div class="breadcrumb">
            <span class="breadcrumb-text">Home / Weather Analysis / Seattle, WA</span>
         </div>
         <div class="user-profile">
            <div class="user-info">
              <span class="user-name">Alex Gardener</span>
              <span class="user-role">EXPERT MEMBER</span>
            </div>
            <div class="user-avatar">
              <img src="https://api.dicebear.com/7.x/avataaars/svg?seed=Alex" alt="Alex Gardener" />
            </div>
         </div>
      </div>
      
      <!-- Weather Title and Actions -->
      <div class="title-section">
         <div>
            <h1 class="page-title">Weather Analysis</h1>
            <p class="page-subtitle">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256" class="location-icon"><path fill="currentColor" d="M128,64a40,40,0,1,0,40,40A40,40,0,0,0,128,64Zm0,64a24,24,0,1,1,24-24A24,24,0,0,1,128,128Zm0-112a88.1,88.1,0,0,0-88,88c0,31.4,14.51,64.68,42,96.25a254.19,254.19,0,0,0,41.45,38.3,8,8,0,0,0,9.18,0A254.19,254.19,0,0,0,174,200.25c27.45-31.57,42-64.85,42-96.25A88.1,88.1,0,0,0,128,16Zm0,206c-16.53-13-72-60.75-72-118a72,72,0,0,1,144,0C200,161.21,144.53,209,128,222Z"/></svg>
              Seattle, WA • Updated 2 mins ago
            </p>
         </div>
         <div class="actions">
            <button class="btn-secondary">
               <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256" class="map-icon"><path fill="currentColor" d="M227.32,73.37,182.63,28.68a16,16,0,0,0-22.63,0L36.68,152A15.89,15.89,0,0,0,32,163.31V208a16,16,0,0,0,16,16H92.69A15.86,15.86,0,0,0,104,219.31L227.32,96a16,16,0,0,0,0-22.63ZM92.69,208H48V163.31l88-88L180.69,120ZM192,108.68,147.31,64l24-24L216,84.68Z"/></svg>
               Change Location
            </button>
         </div>
      </div>
    </div>
    
    <div class="weather-layout">
      <!-- Left Column -->
      <div class="main-column">
        <!-- Current Conditions -->
        <CurrentConditions />
        
        <!-- 7-Day Forecast -->
        <Forecast />
        
        <!-- Temperature Trends Placeholder -->
        <div class="trends-placeholder">
           <div class="chart-header">
              <h3 class="chart-title">Temperature Trends</h3>
              <div class="chart-legend">
                 <span class="legend-item"><span class="dot green"></span> Temperature</span>
                 <span class="legend-item"><span class="dot grey"></span> Avg. 10Y</span>
              </div>
           </div>
           <p class="chart-subtitle">Next 24 hours expectation</p>
           
           <div class="chart-box">
             <!-- Placeholder lines -->
             <div class="chart-grid">
                <div class="grid-line"></div>
                <div class="grid-line"></div>
                <div class="grid-line"></div>
             </div>
             
             <div class="x-axis">
               <span>12 AM</span>
               <span>4 AM</span>
               <span>8 AM</span>
               <span>12 PM</span>
               <span>4 PM</span>
               <span>8 PM</span>
               <span>12 AM</span>
             </div>
           </div>
        </div>
      </div>
      
      <!-- Right Column -->
      <div class="side-column">
        <!-- Plant Care Alert -->
        <PlantCareAlert />
        
        <!-- Rainfall Outlook -->
        <RainfallOutlook />
        
        <!-- Garden Context -->
        <GardenContext />
        
        <!-- Regional Radar Placeholder -->
        <div class="radar-container">
           <img src="/src/assets/radar.png" alt="Map Radar" class="radar-bg" />
           <div class="radar-content">
              <div class="radar-pin">
                <div class="pin-ring"></div>
                <div class="pin-dot"></div>
              </div>
              <h4 class="radar-label">REGIONAL RADAR</h4>
              <p class="radar-sub">Interactive Map View</p>
           </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.weather-view {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.top-nav {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.breadcrumb-text {
  font-weight: 600;
  color: var(--success-green);
  font-size: 0.9rem;
}

.user-profile {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.user-name {
  font-weight: 700;
  font-size: 0.95rem;
  color: var(--text-main);
  line-height: 1.2;
}

.user-role {
  font-size: 0.7rem;
  color: var(--text-muted);
  font-weight: 600;
  letter-spacing: 0.5px;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: 2px solid var(--success-green);
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--surface-color);
}

.user-avatar img {
  width: 100%;
  height: 100%;
}

.title-section {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 24px;
}

.page-title {
  font-size: 2.2rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 8px;
  letter-spacing: -0.5px;
}

.page-subtitle {
  color: var(--text-muted);
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;
}

.location-icon {
  color: var(--success-green);
}

.btn-secondary {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  background-color: var(--surface-color);
  color: var(--text-main);
  border: 1px solid var(--border-color);
  border-radius: 24px;
  font-weight: 600;
  font-size: 0.95rem;
  box-shadow: var(--shadow-sm);
  transition: var(--transition-fast);
}

.btn-secondary:hover {
  background-color: var(--bg-color);
}

.map-icon {
  color: var(--text-main);
}

.weather-layout {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 24px;
}

@media (max-width: 1024px) {
  .weather-layout {
    grid-template-columns: 1fr;
  }
}

.trends-placeholder {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 32px;
  box-shadow: var(--shadow-sm);
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 4px;
}

.chart-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0;
}

.chart-legend {
  display: flex;
  gap: 16px;
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--text-muted);
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 6px;
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.dot.green {
  background-color: var(--success-green);
}

.dot.grey {
  background-color: #d1d5db;
}

.chart-subtitle {
  font-size: 0.85rem;
  color: var(--text-muted);
  font-weight: 500;
  margin-bottom: 24px;
}

.chart-box {
  width: 100%;
  height: 240px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding-top: 16px;
}

.chart-grid {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding-bottom: 16px;
}

.grid-line {
  height: 1px;
  width: 100%;
  background-color: var(--border-color);
}

.x-axis {
  display: flex;
  justify-content: space-between;
  padding: 0 16px;
  font-size: 0.65rem;
  font-weight: 600;
  color: var(--text-light);
}

.radar-container {
  height: 200px;
  border-radius: var(--radius-xl);
  overflow: hidden;
  position: relative;
  background-color: #e5e7eb;
}

.radar-bg {
  width: 100%;
  height: 100%;
  object-fit: cover;
  opacity: 0.8;
}

.radar-content {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: radial-gradient(circle at center, rgba(255,255,255,0.7) 0%, transparent 70%);
}

.radar-pin {
  position: relative;
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
}

.pin-dot {
  width: 12px;
  height: 12px;
  background-color: var(--success-green);
  border-radius: 50%;
  border: 2px solid white;
  z-index: 2;
}

.pin-ring {
  position: absolute;
  width: 100%;
  height: 100%;
  border: 2px solid var(--success-green);
  border-radius: 50%;
  opacity: 0.5;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% { transform: scale(0.5); opacity: 0.8; }
  100% { transform: scale(1.2); opacity: 0; }
}

.radar-label {
  font-size: 0.8rem;
  font-weight: 700;
  color: var(--text-main);
  letter-spacing: 1px;
  margin: 0;
}

.radar-sub {
  font-size: 0.65rem;
  color: var(--text-muted);
}
</style>
