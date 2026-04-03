<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '../../stores/user'
import { Settings } from '@iconoir/vue'

const user = useUserStore()
const displayNameInput = ref(user.displayName)
const alertsInput = ref(user.weatherAlertsEnabled)
const saved = ref(false)

function save() {
  user.saveSettings({ displayName: displayNameInput.value, weatherAlertsEnabled: alertsInput.value })
  saved.value = true
  setTimeout(() => { saved.value = false }, 2000)
}
</script>

<template>
  <div class="bg-surface rounded-2xl p-8 max-md:p-5 shadow-sm border border-border h-full">
    <div class="mb-6">
      <h2 class="flex items-center gap-3 text-[1.25rem] font-extrabold text-text-main m-0">
        <Settings class="text-success-green" width="20" height="20" />
        Account Settings
      </h2>
    </div>

    <div class="grid grid-cols-2 max-[600px]:grid-cols-1 gap-6 mb-8">
      <div class="flex flex-col gap-2">
        <label class="text-[0.85rem] font-extrabold text-text-main">Email Address</label>
        <input type="email" :value="user.email" readonly
          class="px-4 py-3 rounded-md border border-transparent bg-bg-app font-[inherit] text-[0.95rem] text-text-muted outline-none focus:border-success-green focus:bg-surface transition-colors" />
      </div>
      <div class="flex flex-col gap-2">
        <label class="text-[0.85rem] font-extrabold text-text-main">Display Name</label>
        <input type="text" v-model="displayNameInput"
          class="px-4 py-3 rounded-md border border-transparent bg-bg-app font-[inherit] text-[0.95rem] text-text-muted outline-none focus:border-success-green focus:bg-surface transition-colors" />
      </div>
    </div>

    <div class="h-px bg-border mb-6"></div>

    <div class="mb-10">
      <h3 class="text-[0.75rem] font-extrabold text-text-muted tracking-widest mb-4">NOTIFICATIONS</h3>
      <div class="flex justify-between items-center gap-4 flex-wrap">
        <div>
          <h4 class="text-[0.95rem] font-extrabold text-text-main mb-1">Weather-based alerts</h4>
          <p class="text-[0.85rem] text-text-muted m-0">Get advice when local weather changes</p>
        </div>

        <label class="relative inline-flex items-center cursor-pointer w-[50px] h-[26px]">
          <input type="checkbox" v-model="alertsInput" class="sr-only peer" />
          <div class="w-full h-full rounded-[26px] bg-border peer-checked:bg-success-green transition-colors duration-300"></div>
          <div class="absolute left-[3px] top-[3px] w-5 h-5 rounded-full bg-white shadow transition-transform duration-300 peer-checked:translate-x-6"></div>
        </label>
      </div>
    </div>

    <div class="flex justify-end">
      <button @click="save"
        class="px-6 py-3 rounded-[24px] text-[0.95rem] font-bold border-none cursor-pointer transition-all duration-200"
        :class="saved ? 'bg-primary text-white' : 'bg-success-green text-white hover:bg-[#2ea06e]'">
        {{ saved ? 'Saved!' : 'Save Changes' }}
      </button>
    </div>
  </div>
</template>
