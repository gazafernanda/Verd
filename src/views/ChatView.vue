<script setup lang="ts">
import ChatMessage from '../components/Chat/ChatMessage.vue'
import ChatInput from '../components/Chat/ChatInput.vue'
</script>

<template>
  <div class="chat-view">
    <div class="chat-container">
      
      <div class="chat-scroll-area">
        <ChatMessage :isAssistant="true" time="10:24 AM">
          Hello Alex! I'm your botanical specialist. I see you're checking in on your Monstera Deliciosa. How have the leaves been looking since our last watering schedule update?
        </ChatMessage>

        <ChatMessage :isAssistant="false" time="10:26 AM">
          <p class="mb-4">Actually, I'm a bit worried. The edges of the lower leaves are starting to turn yellow and feel a bit soft. I've attached a photo of the worst one.</p>
          <div class="image-attachment">
            <img src="/src/assets/sick_leaf.png" alt="Yellowing Monstera Leaf" />
          </div>
        </ChatMessage>

        <ChatMessage :isAssistant="true" time="10:27 AM">
          <p class="mb-4">Based on the photo, those yellow, soft edges typically indicate <strong>overwatering or poor drainage</strong>.</p>
          
          <div class="care-tip-card">
            <div class="tip-header">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M174.55,101.44a80,80,0,1,1-93.1,0A153.25,153.25,0,0,1,128,31.76a153.25,153.25,0,0,1,46.55,69.68Z"/><path fill="white" d="M168,128a8,8,0,0,1-8,8H136v24a8,8,0,0,1-16,0V136H96a8,8,0,0,1,0-16h24V96a8,8,0,0,1,16,0v24h24A8,8,0,0,1,168,128Z"/></svg>
              Care Tip:
            </div>
            <ul class="tip-list">
              <li>Check soil moisture 2 inches deep.</li>
              <li>Allow top 50% of soil to dry before watering.</li>
            </ul>
          </div>
        </ChatMessage>
      </div>
      
      <div class="chat-input-area">
        <ChatInput />
      </div>

    </div>
  </div>
</template>

<style scoped>
.chat-view {
  height: calc(100vh - 48px); /* Account for app padding */
  display: flex;
  flex-direction: column;
}

.chat-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  background-color: transparent;
  max-width: 900px;
  margin: 0 auto;
  width: 100%;
  position: relative;
}

.chat-scroll-area {
  flex: 1;
  overflow-y: auto;
  padding: 24px 24px 120px 24px; /* padding bottom for input */
  scrollbar-width: none; /* Hide scrollbar Firefox */
  -ms-overflow-style: none; /* IE/Edge */
}

.chat-scroll-area::-webkit-scrollbar {
  display: none; /* Hide scrollbar Chrome/Safari/Webkit */
}

.chat-input-area {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 0 24px;
}

/* Internal overrides for chat bubbles matching the design */
.mb-4 {
  margin-bottom: 16px;
  margin-top: 0;
}

.image-attachment {
  border-radius: var(--radius-lg);
  overflow: hidden;
  max-width: 100%;
}

.image-attachment img {
  display: block;
  width: 100%;
  height: auto;
  border-radius: var(--radius-lg);
}

.care-tip-card {
  background-color: var(--bg-color);
  border-radius: var(--radius-lg);
  padding: 24px;
  margin-top: 16px;
  border-left: 4px solid var(--success-green);
}

.tip-header {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 700;
  color: var(--success-green);
  margin-bottom: 12px;
}

.tip-list {
  margin: 0;
  padding-left: 20px;
  color: var(--text-main);
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.tip-list li {
  line-height: 1.5;
}
</style>
