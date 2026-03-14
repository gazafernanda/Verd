<script setup lang="ts">
import { defineProps } from 'vue'

const props = defineProps({
  isAssistant: {
    type: Boolean,
    default: false
  },
  time: {
    type: String,
    required: true
  }
})
</script>

<template>
  <div :class="['chat-message', { 'is-assistant': isAssistant, 'is-user': !isAssistant }]">
    
    <div v-if="isAssistant" class="avatar-container">
      <div class="avatar assistant">
         <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M208,64H176V56a48,48,0,0,0-96,0v8H48A16,16,0,0,0,32,80V192a16,16,0,0,0,16,16H208a16,16,0,0,0,16-16V80A16,16,0,0,0,208,64ZM96,56a32,32,0,0,1,64,0v8H96ZM208,192H48V80H208V192Zm-48-56a32,32,0,1,1-32-32A32,32,0,0,1,160,136Zm-16,0a16,16,0,1,0-16-16A16,16,0,0,0,144,136Z"/></svg>
      </div>
    </div>
    
    <div class="message-content-wrapper">
      <span class="sender-name">{{ isAssistant ? 'VERD ASSISTANT' : 'YOU' }}</span>
      
      <div class="bubble">
        <slot></slot>
      </div>
      
      <span class="timestamp">{{ time }}</span>
    </div>

    <div v-if="!isAssistant" class="avatar-container right">
      <div class="avatar user">
         <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24ZM74.08,197.5a64,64,0,0,1,107.84,0,87.83,87.83,0,0,1-107.84,0ZM96,120a32,32,0,1,1,32,32A32,32,0,0,1,96,120Zm97.76,66.41a79.66,79.66,0,0,0-36.06-28.75,48,48,0,1,0-59.4,0,79.66,79.66,0,0,0-36.06,28.75,88,88,0,1,1,131.52,0Z"/></svg>
      </div>
    </div>
    
  </div>
</template>

<style scoped>
.chat-message {
  display: flex;
  gap: 16px;
  width: 100%;
  margin-bottom: 24px;
}

.chat-message.is-user {
  justify-content: flex-end;
}

.avatar-container {
  flex-shrink: 0;
  display: flex;
  align-items: flex-start;
}

.avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: 24px; /* Align with top of bubble */
}

.avatar.assistant {
  background-color: var(--light-green-bg);
  color: var(--success-green);
}

.avatar.user {
  background-color: #f1ebd8;
  color: #c4a77d;
}

.message-content-wrapper {
  display: flex;
  flex-direction: column;
  max-width: 80%;
}

.is-user .message-content-wrapper {
  align-items: flex-end;
}

.sender-name {
  font-size: 0.65rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 0.5px;
  margin-bottom: 8px;
  text-transform: uppercase;
}

.bubble {
  padding: 24px;
  border-radius: var(--radius-lg);
  font-size: 1rem;
  line-height: 1.6;
  box-shadow: var(--shadow-sm);
}

.is-assistant .bubble {
  background-color: var(--surface-color);
  color: var(--text-main);
  border: 1px solid var(--border-color);
  border-top-left-radius: 4px;
}

.is-user .bubble {
  background-color: var(--primary-color);
  color: white;
  border-top-right-radius: 4px;
}

.timestamp {
  font-size: 0.7rem;
  color: var(--text-muted);
  margin-top: 8px;
  font-weight: 500;
}
</style>
