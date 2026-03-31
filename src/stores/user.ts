import { ref } from 'vue'
import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', () => {
  const name = ref('Alex Rivera')
  const location = ref('San Francisco, CA')
  const tier = ref('Green Thumb')
  const memberSince = ref('Jan 2023')
  const displayName = ref('Alex Rivera')
  const email = ref('alex.rivera@example.com')
  const weatherAlertsEnabled = ref(true)

  function saveSettings(updates: { displayName?: string; weatherAlertsEnabled?: boolean }) {
    if (updates.displayName !== undefined) {
      displayName.value = updates.displayName
      name.value = updates.displayName
    }
    if (updates.weatherAlertsEnabled !== undefined) {
      weatherAlertsEnabled.value = updates.weatherAlertsEnabled
    }
  }

  return { name, location, tier, memberSince, displayName, email, weatherAlertsEnabled, saveSettings }
})
