<script setup lang="ts">
</script>

<template>
  <div class="profile-weather">
    <div class="weather-title">
      <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M128,40a8,8,0,0,1,8-8h0a8,8,0,0,1,8,8V64a8,8,0,0,1-16,0V40Zm0,152a8,8,0,0,0-8,8v24a8,8,0,0,0,16,0V200A8,8,0,0,0,128,192ZM64,128a8,8,0,0,0-8-8H32a8,8,0,0,0,0,16H56A8,8,0,0,0,64,128Zm160-8H200a8,8,0,0,0,0,16h24a8,8,0,0,0,0-16ZM82.75,82.75a8,8,0,0,0,11.31-11.32l-17-17a8,8,0,0,0-11.31,11.32ZM190.25,179a8,8,0,0,1-11.31,11.32l-17-17A8,8,0,0,1,173.25,162Zm-11.31-96.2A8,8,0,0,0,190.25,71.43l17-17a8,8,0,0,0-11.31-11.32l-17,17A8,8,0,0,0,178.94,82.75ZM65.75,179l-17,17A8,8,0,0,0,60,207.31l17-17A8,8,0,0,0,65.75,179ZM128,72a56,56,0,1,0,56,56A56.06,56.06,0,0,0,128,72Zm0,96a40,40,0,1,1,40-40A40,40,0,0,1,128,168Z"/></svg>
      Local Weather
    </div>
    
    <div class="weather-content">
      <div class="temp">72°F</div>
      <div class="conditions">
        <span class="desc">Partly Cloudy</span>
        <span class="humidity">Humidity: 45%</span>
      </div>
      
      <!-- Minimal aesthetic background swoosh logo idea from mockup -->
      <div class="bg-shape">
        <svg xmlns="http://www.w3.org/2000/svg" width="60" height="60" viewBox="0 0 256 256"><path fill="rgba(255,255,255,0.15)" d="M226.79,150.29A103.49,103.49,0,0,0,144,32H96a8,8,0,0,0-8,8v48a103.49,103.49,0,0,0,82.79,118.29,8,8,0,0,0,9.65-9.65A55.85,55.85,0,0,1,168,152a56,56,0,0,1,56.76-55.93,8,8,0,0,0,7.9-9.65,103.11,103.11,0,0,0-5.87-36.13Z"/></svg>
      </div>
    </div>
  </div>
</template>

<style scoped>
.profile-weather {
  background-color: var(--success-green); /* Using the lighter solid green from the mockup */
  border-radius: var(--radius-xl);
  padding: 24px;
  color: white;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 16px rgba(55, 178, 126, 0.2);
}

.weather-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 1rem;
  font-weight: 700;
  margin-bottom: 24px;
  position: relative;
  z-index: 2;
}

.weather-content {
  display: flex;
  align-items: center;
  gap: 16px;
  position: relative;
  z-index: 2;
}

.temp {
  font-size: 3rem;
  font-weight: 800;
  line-height: 1;
}

.conditions {
  display: flex;
  flex-direction: column;
}

.desc {
  font-size: 1.1rem;
  font-weight: 700;
}

.humidity {
  font-size: 0.8rem;
  opacity: 0.9;
}

.bg-shape {
  position: absolute;
  right: -10px;
  bottom: -10px;
  transform: scale(2);
  transform-origin: bottom right;
  z-index: 1;
  pointer-events: none;
}
</style>
