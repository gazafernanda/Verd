<script setup lang="ts">
import StatusBanner from '../components/StatusBanner.vue'
import LocationCard from '../components/LocationCard.vue'
import WeatherCard from '../components/WeatherCard.vue'
import CareRecommendations from '../components/CareRecommendations.vue'
</script>

<template>
  <div class="dashboard-view">
    <div class="header-section">
      <!-- Top header with Breadcrumb and User Profile -->
      <div class="top-nav">
         <div class="breadcrumb">
         </div>
         <div class="user-profile">
            <span class="user-name">Jamie Peterson</span>
            <div class="user-avatar">
              <!-- Avatar image will go here -->
            </div>
         </div>
      </div>

      <!-- Main Dashboard Title and Actions -->
      <div class="title-section">
         <div>
            <h1 class="page-title">Main Dashboard</h1>
            <p class="page-subtitle">Monitoring your indoor sanctuary</p>
         </div>
         <div class="actions">
            <!-- Date Picker Placeholder -->
            <button class="date-picker">
               <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M208,32H184V24a8,8,0,0,0-16,0v8H88V24a8,8,0,0,0-16,0v8H48A16,16,0,0,0,32,48V208a16,16,0,0,0,16,16H208a16,16,0,0,0,16-16V48A16,16,0,0,0,208,32ZM72,48v8a8,8,0,0,0,16,0V48h80v8a8,8,0,0,0,16,0V48h24V80H48V48Zm136,160H48V96H208V208Z"/></svg>
               Oct 27, 2023
            </button>
            <button class="btn-primary">
               <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M224,128a8,8,0,0,1-8,8H136v80a8,8,0,0,1-16,0V136H40a8,8,0,0,1,0-16h80V40a8,8,0,0,1,16,0v80h80A8,8,0,0,1,224,128Z"/></svg>
               Add New Plant
            </button>
         </div>
      </div>
    </div>

    <StatusBanner />
    <div class="widgets-row">
      <LocationCard />
      <WeatherCard />
    </div>

    <CareRecommendations />
  </div>
</template>

<style scoped>
.dashboard-view {
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

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 8px;
}

.breadcrumb-text {
  font-weight: 700;
  color: var(--primary-color);
  font-size: 1.1rem;
}

.user-profile {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-name {
  font-weight: 600;
  font-size: 0.95rem;
  color: var(--text-main);
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, #f08b5e, #e85d46);
  box-shadow: var(--shadow-sm);
}

.title-section {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 16px;
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 4px;
  letter-spacing: -0.5px;
}

.page-subtitle {
  color: var(--text-muted);
  font-size: 0.95rem;
}

.actions {
  display: flex;
  gap: 16px;
}

.date-picker {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  background-color: var(--surface-color);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  color: var(--text-main);
  font-weight: 600;
  font-size: 0.95rem;
  transition: var(--transition-fast);
}

.date-picker:hover {
  background-color: var(--bg-color);
}

.btn-primary {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  background-color: var(--accent-green);
  color: white;
  border-radius: 24px;
  font-weight: 600;
  font-size: 0.95rem;
  box-shadow: 0 4px 12px rgba(41, 156, 119, 0.3);
  transition: var(--transition-fast);
}

.btn-primary:hover {
  background-color: var(--accent-green-hover);
  transform: translateY(-1px);
}
.widgets-row {
  display: flex;
  gap: 24px;
  margin-bottom: 32px;
}
</style>
