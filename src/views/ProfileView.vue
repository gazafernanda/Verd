<script setup lang="ts">
import ProfileHeader from '../components/Profile/ProfileHeader.vue'
import MyPlantsList from '../components/Profile/MyPlantsList.vue'
import ActivityStats from '../components/Profile/ActivityStats.vue'
import ProfileWeather from '../components/Profile/ProfileWeather.vue'
import AccountSettings from '../components/Profile/AccountSettings.vue'
import AchievementCard from '../components/Profile/AchievementCard.vue'
</script>

<template>
  <div class="flex flex-col gap-6 max-w-[1200px]">
    <ProfileHeader />

    <!-- Desktop: 2-col grid with explicit placement; mobile: single column -->
    <div class="grid grid-cols-[2fr_1fr] max-lg:grid-cols-1 grid-rows-[auto_auto_auto] gap-6 items-start">
      <div class="lg:[grid-column:1] lg:[grid-row:1]">
        <MyPlantsList />
      </div>

      <div class="lg:[grid-column:1] lg:[grid-row:2_/_4]">
        <AccountSettings />
      </div>

      <div class="lg:[grid-column:2] lg:[grid-row:1]">
        <ActivityStats />
      </div>

      <div class="lg:[grid-column:2] lg:[grid-row:2]">
        <ProfileWeather />
      </div>

      <div class="lg:[grid-column:2] lg:[grid-row:3]">
        <AchievementCard />
      </div>
    </div>
  </div>
</template>
