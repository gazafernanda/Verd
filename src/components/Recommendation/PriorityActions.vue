<script setup lang="ts">
</script>

<template>
  <div class="priority-actions">
    <div class="actions-header">
      <h2 class="section-title">Priority Actions</h2>
      <div class="divider"></div>
    </div>
    
    <div class="actions-list">
      <!-- Water Action -->
      <div class="action-card border-green">
        <div class="card-left">
          <div class="icon-circle bg-green">
             <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M174.55,101.44a80,80,0,1,1-93.1,0A153.25,153.25,0,0,1,128,31.76a153.25,153.25,0,0,1,46.55,69.68Z"/></svg>
          </div>
        </div>
        
        <div class="card-body">
          <div class="title-row">
            <h3 class="action-title">Water deeply at base</h3>
            <span class="badge bg-green">IMMEDIATE</span>
          </div>
          <p class="action-desc">
            Apply roughly 2 liters per m² specifically for tomatoes and zucchini to prevent heat-stress wilting.
          </p>
          
          <div class="card-actions">
            <button class="btn btn-green-fill">Mark Completed</button>
            <button class="btn btn-green-outline">Set Reminder</button>
          </div>
        </div>
      </div>
      
      <!-- Shade Cloth Action -->
      <div class="action-card border-orange">
        <div class="card-left">
          <div class="icon-circle bg-orange">
             <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M128,136v96a8,8,0,0,1-16,0V136a8,8,0,0,1,16,0Zm104,16H24a8,8,0,0,1,0-16H232a8,8,0,0,1,0,16Zm11.31-72-24-24a8,8,0,0,0-11.31,11.31L224,84.69V112a8,8,0,0,0,16,0V85.66A8,8,0,0,0,243.31,80ZM16,112V84.69l18.34-18.35A8,8,0,0,0,23,55l-24,24A8,8,0,0,0,1.66,85.66,8,8,0,0,0,8,88H8.69L16,80.69V112a8,8,0,0,0,16,0V85.66Z"/></svg>
          </div>
        </div>
        
        <div class="card-body">
          <div class="title-row">
            <h3 class="action-title">Deploy 30% Shade Cloth</h3>
            <span class="badge bg-orange">RECOMMENDED</span>
          </div>
          <p class="action-desc">
            Protect leafy greens from intense afternoon sun to prevent tip burn and bolting.
          </p>
          
          <div class="card-actions">
            <button class="btn btn-orange-fill">View Instructions</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.priority-actions {
  display: flex;
  flex-direction: column;
}

.actions-header {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
}

.section-title {
  font-size: 1.35rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0;
  white-space: nowrap;
}

.divider {
  flex: 1;
  height: 2px;
  background-color: var(--border-color);
}

.actions-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.action-card {
  display: flex;
  gap: 20px;
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 24px;
  box-shadow: var(--shadow-sm);
  border: 1px solid transparent;
}

.action-card.border-green {
  border-color: rgba(55, 178, 126, 0.5);
}

.action-card.border-orange {
  border-color: rgba(251, 146, 60, 0.4);
}

.card-left {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.icon-circle {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon-circle.bg-green {
  background-color: var(--light-green-bg);
  color: var(--success-green);
}

.icon-circle.bg-orange {
  background-color: #fff4e5;
  color: #f59e0b;
}

.card-body {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.title-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 8px;
}

.action-title {
  font-size: 1.15rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0;
}

.badge {
  font-size: 0.65rem;
  font-weight: 800;
  padding: 4px 10px;
  border-radius: 4px;
  letter-spacing: 0.5px;
}

.badge.bg-green {
  background-color: var(--primary-color);
  color: white;
}

.badge.bg-orange {
  background-color: #f59e0b;
  color: white;
}

.action-desc {
  font-size: 0.95rem;
  color: var(--text-muted);
  line-height: 1.5;
  margin: 0 0 20px 0;
  max-width: 90%;
}

.card-actions {
  display: flex;
  gap: 12px;
}

.btn {
  padding: 10px 20px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 700;
  cursor: pointer;
  transition: var(--transition-fast);
  border: 2px solid transparent;
}

.btn-green-fill {
  background-color: var(--primary-color);
  color: white;
  border-color: var(--primary-color);
}
.btn-green-fill:hover { background-color: var(--primary-hover); }

.btn-green-outline {
  background-color: transparent;
  color: var(--text-main);
  border-color: var(--border-color);
}
.btn-green-outline:hover { background-color: var(--surface-color); }

.btn-orange-fill {
  background-color: #fff4e5;
  color: #f59e0b;
  border-color: transparent;
}
.btn-orange-fill:hover { background-color: #fde6c6; }
</style>
