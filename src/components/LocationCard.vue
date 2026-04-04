<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '../stores/user'
import { EditPencil } from '@iconoir/vue'
import LocationSearch from './LocationSearch.vue'

const user = useUserStore()
const showLocationSearch = ref(false)
</script>

<template>
  <div class="bg-surface rounded-lg p-6 shadow-sm flex-1 flex flex-col">
    <div class="flex justify-between items-start mb-5">
      <div>
        <p class="text-[0.75rem] font-bold text-success-green tracking-[1px] mb-1">CURRENT LOCATION</p>
        <h3 class="text-2xl font-bold text-text-main">{{ user.location }}</h3>
      </div>
      <button
        @click="showLocationSearch = true"
        class="w-8 h-8 rounded-full bg-bg-app flex items-center justify-center text-success-green transition-colors duration-200 hover:bg-[#e6e8eb]"
      >
        <EditPencil width="16" height="16" />
      </button>
    </div>

    <div class="w-full h-40 rounded-md overflow-hidden bg-[#e8eaed] flex-1">
      <img src="https://assets.objkt.media/file/assets-003/QmaMhB38M65KST4x6D7B4W3J9dDq7hA7Z7K9Z1bXbQXXs4/artifact" alt="Map" class="w-full h-full object-cover" />
    </div>
  </div>

  <LocationSearch v-if="showLocationSearch" @close="showLocationSearch = false" />
</template>
