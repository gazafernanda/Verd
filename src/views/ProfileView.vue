<script setup lang="ts">
import ProfileHeader from '../components/Profile/ProfileHeader.vue'
import MyPlantsList from '../components/Profile/MyPlantsList.vue'
import ActivityStats from '../components/Profile/ActivityStats.vue'
import ProfileWeather from '../components/Profile/ProfileWeather.vue'
import AccountSettings from '../components/Profile/AccountSettings.vue'
import AchievementCard from '../components/Profile/AchievementCard.vue'
</script>

<template>
  <div class="profile-view">
    <div class="profile-header-area">
      <ProfileHeader />
    </div>

    <div class="profile-grid">
      <div class="grid-plants">
        <MyPlantsList />
      </div>
      
      <div class="grid-stats">
        <ActivityStats />
      </div>
      
      <div class="grid-weather">
        <ProfileWeather />
      </div>
      
      <div class="grid-settings">
        <AccountSettings />
      </div>
      
      <div class="grid-achievements">
        <AchievementCard />
      </div>
    </div>
  </div>
</template>

<style scoped>
.profile-view {
  display: flex;
  flex-direction: column;
  gap: 24px;
  max-width: 1200px;
}

/* 
  CSS Grid Definition 
  Desktop layout matches the design:
  - Header full width
  - Row 1: Plants (span 2), Stats (span 1)
  - Row 2: Settings (span 2), Weather (span 1)
  - Row 3: Settings (span 2 - spans multiple rows), Achievements (span 1)
*/

.profile-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  grid-template-areas:
    "plants stats"
    "plants weather"
    "settings weather"
    "settings achievements";
    /* Wait, the design has:
       Left col: My Plants, Account Settings
       Right col: Activity Stats, Local Weather, Urban Jungler
    */
  grid-auto-rows: min-content;
  gap: 24px;
}

/* Correct Area Mapping */
.profile-grid {
  grid-template-areas:
    "plants stats"
    "plants weather"
    "settings achievements";
}

/* Let's simplify as just two flexible columns that wrap at smaller breakpoints */
@media (min-width: 1024px) {
  .profile-grid {
    display: grid;
    grid-template-columns: 2fr 1fr;
    grid-template-rows: auto auto auto;
    gap: 24px;
    align-items: start; /* Prevent items from stretching vertically */
  }

  .grid-plants { grid-column: 1; grid-row: 1; }
  .grid-settings { grid-column: 1; grid-row: 2 / 4; } /* Spans longer */
  
  .grid-stats { grid-column: 2; grid-row: 1; }
  .grid-weather { grid-column: 2; grid-row: 2; }
  .grid-achievements { grid-column: 2; grid-row: 3; }
}

@media (max-width: 1023px) {
  .profile-grid {
    display: flex;
    flex-direction: column;
    gap: 24px;
  }
}
</style>
