<script setup lang="ts">
</script>

<template>
  <div class="my-plants-list">
    <div class="widget-header">
      <h2 class="widget-title">
         <svg class="title-icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M216,40H185.34a71.84,71.84,0,0,0-57.34,28.75A71.84,71.84,0,0,0,70.66,40H40a8,8,0,0,0-8,8v42.66A72.08,72.08,0,0,0,104,162v14H48a8,8,0,0,0-8,8v40a8,8,0,0,0,8,8H208a8,8,0,0,0,8-8V184a8,8,0,0,0-8-8H152V162a72.08,72.08,0,0,0,72-71.34V48A8,8,0,0,0,216,40ZM56,216V200H200v16ZM208,90.66A56.06,56.06,0,0,1,152,146.66H141.52A71.64,71.64,0,0,0,165.65,99l16.15-16.14A8,8,0,1,0,170.49,71.5L154.34,87.65A71.74,71.74,0,0,0,128,143.51V104a8,8,0,0,0-16,0v39.51a71.74,71.74,0,0,0-26.34-55.86L69.51,71.5a8,8,0,1,0-11.32,11.31L74.35,99a71.64,71.64,0,0,0,24.13,47.66H88A56.06,56.06,0,0,1,32,90.66V56H70.66A55.83,55.83,0,0,1,126,101.4a55.83,55.83,0,0,1,55.34-45.4H224v34.66Z"/></svg>
         My Plants
      </h2>
      <a href="#" class="view-all">View All</a>
    </div>
    
    <div class="plants-carousel">
      <!-- Fiddle Leaf -->
      <div class="plant-card">
        <div class="plant-thumb placeholder bg-light">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#1a5641" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24ZM74.08,197.5a64,64,0,0,1,107.84,0,87.83,87.83,0,0,1-107.84,0ZM96,120a32,32,0,1,1,32,32A32,32,0,0,1,96,120Zm97.76,66.41a79.66,79.66,0,0,0-36.06-28.75,48,48,0,1,0-59.4,0,79.66,79.66,0,0,0-36.06,28.75,88,88,0,1,1,131.52,0Z"/></svg>
        </div>
        
        <div class="plant-info">
          <div class="title-row">
            <h3 class="plant-name">Fiddle Le...</h3>
            <span class="badge bg-green">HEALTHY</span>
          </div>
          <p class="water-status">Last watered: 2 days ago</p>
          <div class="hydration-bar">
             <div class="bar-fill green" style="width: 75%;"></div>
          </div>
        </div>
      </div>
      
      <!-- Snake Plant -->
      <div class="plant-card">
        <div class="plant-thumb placeholder bg-dark">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="#cfd6c9" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24ZM74.08,197.5a64,64,0,0,1,107.84,0,87.83,87.83,0,0,1-107.84,0ZM96,120a32,32,0,1,1,32,32A32,32,0,0,1,96,120Zm97.76,66.41a79.66,79.66,0,0,0-36.06-28.75,48,48,0,1,0-59.4,0,79.66,79.66,0,0,0-36.06,28.75,88,88,0,1,1,131.52,0Z"/></svg>
        </div>
        
        <div class="plant-info">
          <div class="title-row">
            <h3 class="plant-name">Snake ...</h3>
            <span class="badge bg-orange">NEEDS WATER</span>
          </div>
          <p class="water-status">Last watered: 14 days ago</p>
          <div class="hydration-bar">
             <div class="bar-fill orange" style="width: 15%;"></div>
          </div>
        </div>
      </div>
      
    </div>
  </div>
</template>

<style scoped>
.my-plants-list {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 24px 32px;
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--border-color);
  height: 100%;
}

.widget-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.widget-title {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 1.25rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0;
}

.title-icon {
  color: var(--success-green);
}

.view-all {
  font-size: 0.85rem;
  font-weight: 700;
  color: var(--success-green);
  text-decoration: none;
}

.plants-carousel {
  display: flex;
  gap: 16px;
  overflow-x: auto;
  padding-bottom: 8px; /* space for scrollbar */
}

.plant-card {
  flex: 0 0 calc(50% - 8px);
  min-width: 200px;
  background-color: var(--bg-color);
  border-radius: var(--radius-lg);
  padding: 16px;
  display: flex;
  gap: 16px;
  border: 1px solid rgba(26, 86, 65, 0.05);
}

.plant-thumb {
  width: 64px;
  height: 64px;
  border-radius: var(--radius-md);
  flex-shrink: 0;
  object-fit: cover;
}

/* Fallback styles since API hit Quota limit */
.plant-thumb.placeholder {
  display: flex;
  align-items: center;
  justify-content: center;
}
.plant-thumb.bg-light { background-color: white; border: 1px solid var(--border-color); }
.plant-thumb.bg-dark { background-color: #111; }

.plant-info {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.title-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 8px;
  gap: 8px;
}

.plant-name {
  font-size: 0.95rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.badge {
  font-size: 0.55rem;
  font-weight: 800;
  padding: 4px 6px;
  border-radius: 4px;
  letter-spacing: 0.5px;
  white-space: nowrap;
}

.badge.bg-green {
  background-color: var(--light-green-bg);
  color: var(--success-green);
}

.badge.bg-orange {
  background-color: #fff4e5;
  color: #f59e0b;
}

.water-status {
  font-size: 0.75rem;
  color: var(--text-muted);
  margin: 0 0 12px 0;
  line-height: 1.4;
  flex: 1; /* pushes hydration bar to bottom */
}

.hydration-bar {
  height: 6px;
  background-color: var(--border-color);
  border-radius: 3px;
  width: 100%;
  overflow: hidden;
}

.bar-fill {
  height: 100%;
  border-radius: 3px;
}
.bar-fill.green { background-color: var(--success-green); }
.bar-fill.orange { background-color: #f59e0b; }

@media (max-width: 600px) {
  .plant-card {
    flex: 0 0 100%;
  }
}
</style>
