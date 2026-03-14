<script setup lang="ts">
</script>

<template>
  <div class="profile-header">
    <div class="user-info-section">
      <div class="avatar-wrapper">
        <img src="/src/assets/alex_avatar.png" alt="Alex Rivera" class="user-avatar" />
        <div class="status-badge">
           <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 256 256"><path fill="currentColor" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24Zm45.66,85.66-56,56a8,8,0,0,1-11.32,0l-24-24a8,8,0,0,1,11.32-11.32L112,148.69l50.34-50.35a8,8,0,0,1,11.32,11.32Z"/></svg>
        </div>
      </div>
      
      <div class="user-details">
        <h1 class="user-name">Alex Rivera</h1>
        <div class="user-meta">
          <span class="location">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 256 256"><path fill="currentColor" d="M128,64a40,40,0,1,0,40,40A40,40,0,0,0,128,64Zm0,64a24,24,0,1,1,24-24A24,24,0,0,1,128,128Zm0-112a88.1,88.1,0,0,0-88,88c0,31.4,14.51,64.68,42,96.25a254.19,254.19,0,0,0,41.45,38.3,8,8,0,0,0,9.18,0A254.19,254.19,0,0,0,174,200.25c27.45-31.57,42-64.85,42-96.25A88.1,88.1,0,0,0,128,16Zm0,206c-16.53-13-72-60.75-72-118a72,72,0,0,1,144,0C200,161.23,144.53,209,128,222Z"/></svg>
            Brooklyn, NY
          </span>
          <span class="dot-separator">•</span>
          <span class="tier">Green Thumb Tier</span>
        </div>
        <p class="join-date">MEMBER SINCE JAN 2023</p>
      </div>
    </div>
    
    <div class="action-buttons">
      <button class="btn btn-outline">Edit Profile</button>
      <button class="btn btn-primary">Share Profile</button>
    </div>
  </div>
</template>

<style scoped>
.profile-header {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 32px 40px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--border-color);
}

.user-info-section {
  display: flex;
  align-items: center;
  gap: 24px;
}

.avatar-wrapper {
  position: relative;
  width: 96px;
  height: 96px;
  border-radius: 50%;
  border: 4px solid #f6cfb0; /* Match design peach border */
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #fcece0;
}

.user-avatar {
  width: 88px;
  height: 88px;
  border-radius: 50%;
  object-fit: cover;
}

.status-badge {
  position: absolute;
  bottom: 0;
  right: -4px;
  background-color: var(--success-green);
  color: white;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid var(--surface-color);
}

.user-details {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-size: 2rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0 0 8px 0;
  line-height: 1;
}

.user-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
  font-size: 0.95rem;
  font-weight: 500;
}

.location {
  display: flex;
  align-items: center;
  gap: 4px;
  color: var(--text-muted);
}

.dot-separator {
  color: var(--text-muted);
}

.tier {
  color: var(--success-green);
}

.join-date {
  font-size: 0.7rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 1px;
  margin: 0;
}

.action-buttons {
  display: flex;
  gap: 16px;
}

.btn {
  padding: 12px 24px;
  border-radius: 24px;
  font-size: 0.9rem;
  font-weight: 700;
  cursor: pointer;
  transition: var(--transition-fast);
}

.btn-outline {
  background-color: transparent;
  color: var(--text-main);
  border: 1px solid var(--border-color);
}
.btn-outline:hover {
  background-color: var(--bg-color);
}

.btn-primary {
  background-color: var(--primary-color);
  color: white;
  border: 1px solid var(--primary-color);
  box-shadow: 0 4px 12px rgba(26, 86, 65, 0.2);
}
.btn-primary:hover {
  background-color: var(--primary-hover);
}

@media (max-width: 768px) {
  .profile-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 24px;
    padding: 24px;
  }
}
</style>
