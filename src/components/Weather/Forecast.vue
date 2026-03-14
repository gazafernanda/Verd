<script setup lang="ts">
const forecastData = [
  { day: 'TODAY', date: 'May 12', icon: 'sun', tempHi: 72, tempLo: 54, active: true },
  { day: 'MON', date: 'May 13', icon: 'cloud-sun', tempHi: 68, tempLo: 51 },
  { day: 'TUE', date: 'May 14', icon: 'rain', tempHi: 62, tempLo: 49 },
  { day: 'WED', date: 'May 15', icon: 'cloud', tempHi: 65, tempLo: 50 },
  { day: 'THU', date: 'May 16', icon: 'sun', tempHi: 70, tempLo: 52 },
]
</script>

<template>
  <div class="forecast-section">
    <div class="section-header">
      <h2 class="section-title">7-Day Forecast</h2>
      <div class="nav-arrows">
        <button class="arrow-btn">
           <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M165.66,202.34a8,8,0,0,1-11.32,11.32l-80-80a8,8,0,0,1,0-11.32l80-80a8,8,0,0,1,11.32,11.32L91.31,128Z"/></svg>
        </button>
        <button class="arrow-btn">
           <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M181.66,133.66l-80,80a8,8,0,0,1-11.32-11.32L164.69,128,90.34,53.66a8,8,0,0,1,11.32-11.32l80,80A8,8,0,0,1,181.66,133.66Z"/></svg>
        </button>
      </div>
    </div>
    
    <div class="forecast-list">
      <div 
        v-for="(item, index) in forecastData" 
        :key="index"
        :class="['forecast-card', { active: item.active }]"
      >
        <div class="day-info">
          <span class="day-name">{{ item.day }}</span>
          <span class="day-date">{{ item.date }}</span>
        </div>
        
        <div class="weather-icon">
          <!-- Mocking icons strictly for presentation -->
          <svg v-if="item.icon === 'sun'" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#fbbd06" d="M120,40V16a8,8,0,0,1,16,0V40a8,8,0,0,1-16,0Zm8,24a64,64,0,1,0,64,64A64.07,64.07,0,0,0,128,64Zm0,112a48,48,0,1,1,48-48A48.05,48.05,0,0,1,128,176Z"/></svg>
          <svg v-if="item.icon === 'cloud-sun'" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#4b8ae6" d="M128,128a32,32,0,1,1,32-32A32,32,0,0,1,128,128Zm82,72H88A40,40,0,0,1,88,120a40.59,40.59,0,0,1,8,.8A48,48,0,0,1,184,104a47.88,47.88,0,0,1,28.29,9.2C226.78,123.47,238.16,140.23,230.13,165.71a40,40,0,0,1-20.13,34.29Z"/></svg>
          <svg v-if="item.icon === 'rain'" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#3b82f6" d="M176,144H80a40,40,0,0,1,0-80,40,40,0,0,1,40-40,40,40,0,0,1,40,40,40,40,0,0,1,16,74.69V144Zm-16,32v32h16V176Zm-48,0v32h16V176ZM80,176v32H96V176Z"/></svg>
          <svg v-if="item.icon === 'cloud'" xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#9caaa4" d="M160,208H72A56,56,0,0,1,72,96a57.5,57.5,0,0,1,13.9,1.7,80,80,0,0,1,146.2,26.5A56,56,0,0,1,160,208Z"/></svg>
        </div>
        
        <div class="temps">
          <span class="hi">{{ item.tempHi }}°</span>
          <span class="lo">{{ item.tempLo }}°</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.forecast-section {
  margin-bottom: 32px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section-title {
  font-size: 1.4rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0;
}

.nav-arrows {
  display: flex;
  gap: 8px;
}

.arrow-btn {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: 1px solid var(--border-color);
  background-color: var(--surface-color);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  box-shadow: var(--shadow-sm);
  transition: var(--transition-fast);
}

.arrow-btn:hover {
  background-color: var(--bg-color);
  color: var(--text-main);
}

.forecast-list {
  display: flex;
  gap: 16px;
  overflow-x: auto;
  padding-bottom: 8px;
  scrollbar-width: none;
}

.forecast-list::-webkit-scrollbar {
  display: none;
}

.forecast-card {
  flex: 1;
  min-width: 100px;
  background-color: var(--surface-color);
  border: 2px solid transparent;
  border-radius: 40px;
  padding: 24px 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  box-shadow: var(--shadow-sm);
  transition: var(--transition-fast);
}

.forecast-card.active {
  border-color: var(--success-green);
  box-shadow: 0 8px 16px rgba(55, 178, 126, 0.15);
}

.day-info {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
}

.day-name {
  font-size: 0.8rem;
  font-weight: 700;
  color: var(--text-main);
}

.day-date {
  font-size: 0.75rem;
  font-weight: 500;
  color: var(--text-muted);
}

.temps {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
}

.hi {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--text-main);
}

.lo {
  font-size: 0.9rem;
  font-weight: 500;
  color: var(--text-muted);
}
</style>
