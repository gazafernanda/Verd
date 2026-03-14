<script setup lang="ts">
</script>

<template>
  <div class="achievement-card">
    <div class="icon-circle bg-green">
      <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 256 256"><path fill="currentColor" d="M224,64H192V48a16,16,0,0,0-16-16H80A16,16,0,0,0,64,48V64H32A16,16,0,0,0,16,80v24a64.07,64.07,0,0,0,64,64h1.16A79.79,79.79,0,0,0,120,207.26V224H96a8,8,0,0,0,0,16h64a8,8,0,0,0,0-16H136V207.26A79.79,79.79,0,0,0,174.84,168H176a64.07,64.07,0,0,0,64-64V80A16,16,0,0,0,224,64ZM80,152a64,64,0,0,1-48-60.8V80H64Zm96,39.38a64,64,0,0,1-96,0V48h96ZM224,104a48.05,48.05,0,0,1-48,48H175.76a64,64,0,0,0,2-16V80h26.24v11.2Z"/></svg>
    </div>
    
    <h3 class="achievement-title">Urban Jungler</h3>
    <p class="achievement-desc">You've reached level 8 of plant mastery. Keep going!</p>
    
    <div class="progress-section">
      <div class="xp-bar-container">
        <div class="xp-fill bg-green" style="width: 75%"></div>
      </div>
      <span class="xp-text">750 / 1000 XP</span>
    </div>
  </div>
</template>

<style scoped>
.achievement-card {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 32px 24px;
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.icon-circle {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 16px;
}

.icon-circle.bg-green {
  background-color: var(--light-green-bg);
  color: var(--success-green);
}

.achievement-title {
  font-size: 1.35rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0 0 8px 0;
}

.achievement-desc {
  font-size: 0.95rem;
  color: var(--text-muted);
  line-height: 1.5;
  margin: 0 0 24px 0;
  max-width: 80%;
}

.progress-section {
  width: 100%;
}

.xp-bar-container {
  width: 100%;
  height: 8px;
  background-color: var(--border-color);
  border-radius: 4px;
  margin-bottom: 8px;
  overflow: hidden;
}

.xp-fill {
  height: 100%;
  border-radius: 4px;
}

.xp-fill.bg-green {
  background-color: var(--success-green);
}

.xp-text {
  font-size: 0.7rem;
  font-weight: 800;
  color: var(--success-green);
  letter-spacing: 1px;
}
</style>
