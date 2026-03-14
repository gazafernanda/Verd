<script setup lang="ts">
import CareCard from './CareCard.vue'
</script>

<template>
  <div class="care-recommendations">
    <div class="section-header">
      <div class="title-with-icon">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="#1a5641" d="M216,40a16,16,0,0,0-16-16H56A16,16,0,0,0,40,40V216a16,16,0,0,0,16,16H200a16,16,0,0,0,16-16ZM56,40H200V216H56ZM96,160a32,32,0,1,1,32-32A32,32,0,0,1,96,160Zm0-48a16,16,0,1,0,16,16A16,16,0,0,0,96,112Z"/></svg>
        <h2 class="section-title">Care Recommendations</h2>
      </div>
      <a href="#" class="view-link">View Schedule</a>
    </div>

    <div class="cards-slider">
      <div class="slider-track">
        <div class="slider-item">
          <CareCard 
            category="WATERING" 
            title="Check the Monstera" 
            description="Soil is likely dry. The current humidity of..." 
            image="/src/assets/monstera.png" 
            bgType="blue" 
          />
        </div>
        <div class="slider-item">
          <CareCard 
            category="POSITIONING" 
            title="Rotate Succulents" 
            description="Ensure even growth by rotating your..." 
            image="/src/assets/succulents.png" 
            bgType="yellow" 
          />
        </div>
        <div class="slider-item">
          <CareCard 
            category="MAINTENANCE" 
            title="Seasonal Pruning" 
            description="A good time to remove dead stems..." 
            image="/src/assets/pruning.png" 
            bgType="green" 
          />
        </div>
        <!-- Extra placeholder card so slider can actually scroll if window is wide enough -->
        <div class="slider-item">
           <CareCard 
            category="FERTILIZING" 
            title="Feed the Ficus" 
            description="Spring is here, time to add liquid fertilizer..." 
            image="https://images.unsplash.com/photo-1614594975525-e45190c55d40?w=600&h=400&fit=crop" 
            bgType="green" 
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.care-recommendations {
  display: flex;
  flex-direction: column;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.title-with-icon {
  display: flex;
  align-items: center;
  gap: 12px;
}

.section-title {
  font-size: 1.4rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0;
}

.view-link {
  font-size: 0.95rem;
  font-weight: 700;
  color: var(--primary-color);
  transition: var(--transition-fast);
}

.view-link:hover {
  text-decoration: underline;
  color: var(--primary-hover);
}

.cards-slider {
  width: 100%;
  overflow-x: auto;
  padding-bottom: 16px; /* Space for hover shadow */
  margin-bottom: -16px;
  
  /* Hide scrollbar for cleaner look */
  scrollbar-width: none;
  -ms-overflow-style: none;
}

.cards-slider::-webkit-scrollbar {
  display: none;
}

.slider-track {
  display: flex;
  gap: 24px;
  width: max-content;
  padding-right: 24px; /* padding at the end of scroll */
}

.slider-item {
  width: 300px; /* Fixed width for slider items */
  flex-shrink: 0;
  /* Allow card height to stretch if needed, though they are usually fixed by content */
  align-self: stretch; 
}
</style>
