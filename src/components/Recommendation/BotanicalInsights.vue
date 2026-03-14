<script setup lang="ts">
</script>

<template>
  <div class="botanical-insights">
    <div class="section-header">
      <h2 class="section-title">Botanical Insights</h2>
    </div>
    
    <div class="insight-card">
      <span class="label">WHY THESE ACTIONS?</span>
      
      <p class="highlight-text">
        High solar radiation (UV 8+) significantly increases the <span class="italic-green">transpiration rate</span> of vegetable crops.
      </p>
      
      <ul class="insight-list">
        <li>
          <div class="bullet-icon">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,24a8,8,0,0,1,8,8v88H224a8,8,0,0,1,0,16H136v88a8,8,0,0,1-16,0V136H32a8,8,0,0,1,0-16H120V32A8,8,0,0,1,128,24Z" transform="translate(128, -53) rotate(45) "/></svg>
          </div>
          <p>Stomata are currently wide open to facilitate cooling, leading to rapid water loss from soil surface.</p>
        </li>
      </ul>
    </div>
  </div>
</template>

<style scoped>
.botanical-insights {
  display: flex;
  flex-direction: column;
}

.section-header {
  margin-bottom: 24px;
  height: 24px; /* match line height of Priority Actions to align properly */
  display: flex;
  align-items: center;
}

.section-title {
  font-size: 1.35rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0;
}

.insight-card {
  background-color: var(--light-green-bg);
  border-radius: var(--radius-xl);
  padding: 32px 28px;
  border: 1px solid rgba(55, 178, 126, 0.15);
}

.label {
  display: block;
  font-size: 0.7rem;
  font-weight: 800;
  color: var(--success-green);
  letter-spacing: 1px;
  margin-bottom: 24px;
}

.highlight-text {
  font-size: 1.15rem;
  font-weight: 700;
  color: var(--text-main);
  line-height: 1.6;
  margin-bottom: 32px;
}

.italic-green {
  font-style: italic;
  color: var(--success-green);
  border-bottom: 2px solid var(--success-green);
}

.insight-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.insight-list li {
  display: flex;
  align-items: flex-start;
  gap: 12px;
}

.bullet-icon {
  margin-top: 4px;
  color: var(--primary-color);
}

.insight-list p {
  font-size: 0.95rem;
  color: var(--text-muted);
  line-height: 1.6;
  font-weight: 500;
  margin: 0;
}
</style>
