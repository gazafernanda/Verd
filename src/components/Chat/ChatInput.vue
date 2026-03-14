<script setup lang="ts">
</script>

<template>
  <div class="chat-input-wrapper">
    <div class="chat-input-container">
      <button class="icon-btn attachment">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M208,56H180.28L166.65,35.56A8,8,0,0,0,160,32H96a8,8,0,0,0-6.65,3.56L75.71,56H48A24,24,0,0,0,24,80V192a24,24,0,0,0,24,24H208a24,24,0,0,0,24-24V80A24,24,0,0,0,208,56Zm8,136a8,8,0,0,1-8,8H48a8,8,0,0,1-8-8V80a8,8,0,0,1,8-8H80a8,8,0,0,0,6.66-3.56L100.28,48h55.43l13.63,20.44A8,8,0,0,0,176,72h32a8,8,0,0,1,8,8ZM128,88a44,44,0,1,0,44,44A44.05,44.05,0,0,0,128,88Zm0,72a28,28,0,1,1,28-28A28,28,0,0,1,128,160Z"/></svg>
      </button>
      
      <input type="text" placeholder="Ask about pruning, pests, or soil..." class="chat-input" />
      
      <button class="icon-btn send">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 256 256"><path fill="currentColor" d="M227.32,28.68a16,16,0,0,0-15.66-4.08l-192,64a16,16,0,0,0-2.42,29.84l85.62,40.55,40.55,85.62A15.86,15.86,0,0,0,157.74,252q.69,0,1.38-.06a15.88,15.88,0,0,0,14-11.51l64-192A16,16,0,0,0,227.32,28.68ZM157.83,231.85l-39.43-83.25L174.06,93.94a8,8,0,0,0-11.32-11.31L107.4,138.27l-83.25-39.43,184-61.32Z"/></svg>
      </button>
    </div>
  </div>
</template>

<style scoped>
.chat-input-wrapper {
  width: 100%;
  padding: 16px 0;
  background: linear-gradient(180deg, rgba(250,250,250,0) 0%, rgba(250,250,250,1) 20%);
}

.chat-input-container {
  display: flex;
  align-items: center;
  background-color: var(--surface-color);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-xl);
  padding: 8px 8px 8px 16px;
  box-shadow: 0 4px 16px rgba(0,0,0,0.05);
  margin: 0 auto;
}

.icon-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: none;
  cursor: pointer;
  transition: var(--transition-fast);
}

.icon-btn.attachment {
  color: var(--text-muted);
  width: 32px;
  height: 32px;
}
.icon-btn.attachment:hover {
  color: var(--text-main);
}

.chat-input {
  flex: 1;
  border: none;
  outline: none;
  padding: 12px 16px;
  font-size: 1rem;
  font-family: inherit;
  color: var(--text-main);
  background: transparent;
}

.chat-input::placeholder {
  color: var(--text-light);
}

.icon-btn.send {
  background-color: var(--success-green);
  color: white;
  width: 44px;
  height: 44px;
  border-radius: 50%;
  box-shadow: 0 4px 12px rgba(55, 178, 126, 0.3);
}

.icon-btn.send:hover {
  background-color: #2ea06e;
  transform: translateY(-1px);
}
</style>
