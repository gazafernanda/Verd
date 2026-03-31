import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

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
  const plants = ref<Plant[]>([
    {
      id: 1,
      name: 'Monstera Deliciosa',
      status: 'HEALTHY',
      wateringLevel: 65,
      lastWatered: '3 days ago',
      category: 'Tropical Plants',
      iconBg: '#f0f9f4',
      careCard: {
        category: 'WATERING',
        title: 'Check the Monstera',
        description: 'Soil is likely dry. The current humidity may not be sufficient for optimal growth.',
        image: '/src/assets/monstera.png',
        bgType: 'blue',
      },
    },
    {
      id: 2,
      name: 'Snake Plant',
      status: 'NEEDS WATER',
      wateringLevel: 15,
      lastWatered: '14 days ago',
      category: 'Succulents & Cacti',
      iconBg: '#111111',
      careCard: {
        category: 'POSITIONING',
        title: 'Rotate Succulents',
        description: 'Ensure even growth by rotating your succulents toward the light source.',
        image: '/src/assets/succulents.png',
        bgType: 'yellow',
      },
    },
    {
      id: 3,
      name: 'Fiddle Leaf Fig',
      status: 'HEALTHY',
      wateringLevel: 75,
      lastWatered: '2 days ago',
      category: 'Tropical Plants',
      iconBg: '#f0f9f4',
      careCard: {
        category: 'MAINTENANCE',
        title: 'Seasonal Pruning',
        description: 'A good time to remove dead stems and shape the plant for new growth.',
        image: '/src/assets/pruning.png',
        bgType: 'green',
      },
    },
    {
      id: 4,
      name: 'Boston Fern',
      status: 'NEEDS MISTING',
      wateringLevel: 50,
      lastWatered: '1 day ago',
      category: 'Ferns',
      iconBg: '#e6f3ef',
      careCard: {
        category: 'FERTILIZING',
        title: 'Feed the Boston Fern',
        description: 'Spring is here, time to add liquid fertilizer for lush, healthy fronds.',
        image: 'https://images.unsplash.com/photo-1614594975525-e45190c55d40?w=600&h=400&fit=crop',
        bgType: 'green',
      },
    },
  ])

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

  return {
    plants,
    careStreak,
    totalPlants,
    plantsNeedingCare,
    plantsMistingNeeded,
    gardenCategories,
    overallStatus,
  }
})
