import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

const API = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

export interface Plant {
  id: number
  name: string
  status: 'HEALTHY' | 'NEEDS WATER' | 'NEEDS MISTING'
  wateringLevel: number
  lastWatered: string
  category: string
  iconBg: string
  careCard: {
    category: string
    title: string
    description: string
    image: string
    bgType: 'blue' | 'yellow' | 'green'
  }
}

export const usePlantsStore = defineStore('plants', () => {
  const plants = ref<Plant[]>([])

  const careStreak = ref(45)

  const totalPlants = computed(() => plants.value.length)

  const plantsNeedingCare = computed(() => plants.value.filter((p) => p.status !== 'HEALTHY'))

  const plantsMistingNeeded = computed(() => plants.value.filter((p) => p.status === 'NEEDS MISTING'))

  const gardenCategories = computed(() => [...new Set(plants.value.map((p) => p.category))])

  const overallStatus = computed(() => {
    if (plantsNeedingCare.value.length === 0) return 'safe'
    if (plantsNeedingCare.value.some((p) => p.status === 'NEEDS WATER')) return 'warning'
    return 'attention'
  })

  async function fetchPlants() {
    const token = localStorage.getItem('verd_token')
    if (!token || token === 'demo') return
    const res = await fetch(`${API}/api/plants`, {
      headers: { Authorization: `Bearer ${token}` },
    })
    if (res.ok) plants.value = await res.json()
  }

  return {
    plants,
    careStreak,
    totalPlants,
    plantsNeedingCare,
    plantsMistingNeeded,
    gardenCategories,
    overallStatus,
    fetchPlants,
  }
})
