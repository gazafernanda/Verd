<script setup lang="ts">
</script>

<template>
  <div class="rainfall-outlook">
    <div class="header">
      <div class="icon-circle">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,16a8,8,0,0,0-7.14,4.42C101.91,58.38,48,118.82,48,160a80,80,0,0,0,160,0c0-41.18-53.91-101.62-72.86-139.58A8,8,0,0,0,128,16Zm48.51,139.51a8,8,0,0,1-11,11.05A47.85,47.85,0,0,0,128,152a8,8,0,0,1,0-16,63.85,63.85,0,0,1,48.51,19.51Z"/></svg>
      </div>
      <h3 class="title">Rainfall Outlook</h3>
    </div>
    
    <div class="bars-container">
      <div class="bar-row">
        <span class="label">THIS<br>WEEK</span>
        <div class="bar-track">
           <div class="bar-fill blue" style="width: 60%;"></div>
        </div>
        <span class="value">12mm</span>
      </div>
      
      <div class="bar-row">
        <span class="label">AVERAGE</span>
        <div class="bar-track">
           <div class="bar-fill grey" style="width: 40%;"></div>
        </div>
        <span class="value">8mm</span>
      </div>
    </div>
    
    <p class="summary-text">
      Precipitation is 35% higher than the 10-year seasonal average for this week. Monitor root rot in succulents.
    </p>
  </div>
</template>

<style scoped>
.rainfall-outlook {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 24px;
  box-shadow: var(--shadow-sm);
  margin-bottom: 24px;
}

.header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 24px;
}

.icon-circle {
  width: 24px;
  height: 24px;
  background-color: #e6f0ff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #3b82f6;
}

.title {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0;
}

.bars-container {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 20px;
}

.bar-row {
  display: flex;
  align-items: center;
  gap: 12px;
}

.label {
  width: 50px;
  font-size: 0.65rem;
  font-weight: 700;
  color: var(--text-muted);
  line-height: 1.2;
}

.bar-track {
  flex: 1;
  height: 8px;
  background-color: #f3f4f6;
  border-radius: 4px;
  overflow: hidden;
}

.bar-fill {
  height: 100%;
  border-radius: 4px;
}

.bar-fill.blue {
  background-color: #3b82f6;
}

.bar-fill.grey {
  background-color: #e5e7eb;
}

.value {
  width: 36px;
  font-size: 0.8rem;
  font-weight: 700;
  color: var(--text-main);
  text-align: right;
}

.summary-text {
  font-size: 0.75rem;
  color: var(--text-muted);
  line-height: 1.5;
}
</style>
