<script setup lang="ts">
import ChatMessage from '../components/Chat/ChatMessage.vue'
import ChatInput from '../components/Chat/ChatInput.vue'
</script>

<template>
  <div class="h-[calc(100vh-80px)] flex flex-col">
    <div class="flex-1 flex flex-col bg-transparent max-w-[900px] mx-auto w-full relative">

      <div class="flex-1 overflow-y-auto px-6 pb-[120px] pt-6 scrollbar-none">
        <ChatMessage :isAssistant="true" time="10:24 AM">
          Hello Alex! I'm your botanical specialist. I see you're checking in on your Monstera Deliciosa. How have the leaves been looking since our last watering schedule update?
        </ChatMessage>

        <ChatMessage :isAssistant="false" time="10:26 AM">
          <p class="mb-4">Actually, I'm a bit worried. The edges of the lower leaves are starting to turn yellow and feel a bit soft. I've attached a photo of the worst one.</p>
          <div class="rounded-lg overflow-hidden max-w-full">
            <img src="/src/assets/sick_leaf.png" alt="Yellowing Monstera Leaf" class="block w-full h-auto rounded-lg" />
          </div>
        </ChatMessage>

        <ChatMessage :isAssistant="true" time="10:27 AM">
          <p class="mb-4">Based on the photo, those yellow, soft edges typically indicate <strong>overwatering or poor drainage</strong>.</p>

          <div class="bg-bg-app rounded-lg p-6 mt-4 border-l-4 border-success-green">
            <div class="flex items-center gap-2 font-bold text-success-green mb-3">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 256 256"><path fill="currentColor" d="M174.55,101.44a80,80,0,1,1-93.1,0A153.25,153.25,0,0,1,128,31.76a153.25,153.25,0,0,1,46.55,69.68Z"/><path fill="white" d="M168,128a8,8,0,0,1-8,8H136v24a8,8,0,0,1-16,0V136H96a8,8,0,0,1,0-16h24V96a8,8,0,0,1,16,0v24h24A8,8,0,0,1,168,128Z"/></svg>
              Care Tip:
            </div>
            <ul class="m-0 pl-5 text-text-main flex flex-col gap-2">
              <li class="leading-relaxed">Check soil moisture 2 inches deep.</li>
              <li class="leading-relaxed">Allow top 50% of soil to dry before watering.</li>
            </ul>
          </div>
        </ChatMessage>
      </div>

      <div class="absolute bottom-0 left-0 right-0 px-6">
        <ChatInput />
      </div>

    </div>
  </div>
</template>
