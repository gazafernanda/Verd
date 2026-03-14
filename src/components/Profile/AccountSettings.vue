<script setup lang="ts">
</script>

<template>
  <div class="account-settings">
    <div class="widget-header">
      <h2 class="widget-title">
        <svg class="title-icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M128,80a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Zm82.25-45.72a78.89,78.89,0,0,0,0-28.56l22.21-13a8,8,0,0,0,2.15-10.89l-22.19-38.4a8,8,0,0,0-10.45-3.23l-24.64,10.6a81.1,81.1,0,0,0-24.71-14.28V41A8,8,0,0,0,144.62,33H111.38a8,8,0,0,0-8,8V56.55a81.1,81.1,0,0,0-24.71,14.28L54,60.23A8,8,0,0,0,43.58,63.5l-22.19,38.4a8,8,0,0,0,2.15,10.89l22.21,13a78.89,78.89,0,0,0,0,28.56l-22.21,13a8,8,0,0,0-2.15,10.89l22.19,38.4a8,8,0,0,0,10.45,3.23l24.64-10.6A81.1,81.1,0,0,0,103.38,199.5V215a8,8,0,0,0,8,8h33.24a8,8,0,0,0,8-8V199.5a81.1,81.1,0,0,0,24.71-14.28l24.64,10.6a8,8,0,0,0,10.45-3.23l22.19-38.4a8,8,0,0,0-2.15-10.89ZM199.1,173.07l-21.75-9.37a7.92,7.92,0,0,0-8,.8,65.23,65.23,0,0,1-30.76,17.74,8,8,0,0,0-6,7.74V207H123.4v-17.1a8,8,0,0,0-6-7.74A65.23,65.23,0,0,1,86.63,164.5a7.92,7.92,0,0,0-8-.8L56.9,173.07,44.25,151.15l19.57-11.43A8,8,0,0,0,67.64,133,63,63,0,0,1,67.64,123a8,8,0,0,0-3.82-6.79L44.25,104.85,56.9,82.93l21.75,9.37a7.92,7.92,0,0,0,8-.8,65.23,65.23,0,0,1,30.76-17.74,8,8,0,0,0,6-7.74V49h29.2v17.1a8,8,0,0,0,6,7.74,65.23,65.23,0,0,1,30.76,17.74,7.92,7.92,0,0,0,8,.8l21.75-9.37,12.65,21.92-19.57,11.43A8,8,0,0,0,188.36,123a63,63,0,0,1,0,10,8,8,0,0,0,3.82,6.79l19.57,11.43Z"/></svg>
        Account Settings
      </h2>
    </div>
    
    <div class="form-grid">
      <div class="form-group">
        <label>Email Address</label>
        <input type="email" value="alex.rivera@example.com" readonly />
      </div>
      
      <div class="form-group">
        <label>Display Name</label>
        <input type="text" value="Alex Rivera" />
      </div>
    </div>
    
    <div class="divider"></div>
    
    <div class="notifications-section">
      <h3 class="section-label">NOTIFICATIONS</h3>
      
      <div class="toggle-row">
        <div class="toggle-info">
          <h4>Weather-based alerts</h4>
          <p>Get advice when local weather changes</p>
        </div>
        
        <label class="toggle-switch">
          <input type="checkbox" checked>
          <span class="slider round"></span>
        </label>
      </div>
    </div>
    
    <div class="form-actions">
      <button class="btn btn-primary">Save Changes</button>
    </div>
  </div>
</template>

<style scoped>
.account-settings {
  background-color: var(--surface-color);
  border-radius: var(--radius-xl);
  padding: 32px;
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--border-color);
  height: 100%;
}

.widget-header {
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

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
  margin-bottom: 32px;
}

@media (max-width: 600px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 0.85rem;
  font-weight: 800;
  color: var(--text-main);
}

.form-group input {
  padding: 12px 16px;
  border-radius: var(--radius-md);
  border: 1px solid transparent; /* Mimic design minimal inputs */
  background-color: var(--bg-color);
  font-family: inherit;
  font-size: 0.95rem;
  color: var(--text-muted);
  outline: none;
  transition: var(--transition-fast);
}

.form-group input:focus {
  border-color: var(--success-green);
  background-color: var(--surface-color);
}

.divider {
  height: 1px;
  background-color: var(--border-color);
  margin: 0 0 24px 0;
}

.section-label {
  font-size: 0.75rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 1px;
  margin: 0 0 16px 0;
}

.toggle-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 40px;
}

.toggle-info h4 {
  font-size: 0.95rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0 0 4px 0;
}

.toggle-info p {
  font-size: 0.85rem;
  color: var(--text-muted);
  margin: 0;
}

/* Custom CSS Toggle Switch */
.toggle-switch {
  position: relative;
  display: inline-block;
  width: 50px;
  height: 26px;
}

.toggle-switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.slider {
  position: absolute;
  cursor: pointer;
  top: 0; left: 0; right: 0; bottom: 0;
  background-color: var(--border-color);
  transition: .4s;
}

.slider:before {
  position: absolute;
  content: "";
  height: 20px;
  width: 20px;
  left: 3px;
  bottom: 3px;
  background-color: white;
  transition: .4s;
}

input:checked + .slider {
  background-color: var(--success-green);
}

input:checked + .slider:before {
  transform: translateX(24px);
}

.slider.round {
  border-radius: 26px;
}

.slider.round:before {
  border-radius: 50%;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
}

.btn-primary {
  background-color: var(--success-green); /* Using the lighter green from mockup */
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 24px;
  font-size: 0.95rem;
  font-weight: 700;
  cursor: pointer;
  transition: var(--transition-fast);
}

.btn-primary:hover {
  background-color: #2ea06e;
}
</style>
