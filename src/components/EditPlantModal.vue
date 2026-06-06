<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { usePlantsStore, type Plant } from '../stores/plants'
import { X, Flower2, Check, Droplet, Sun } from 'lucide-vue-next'

const props = defineProps<{ plant: Plant }>()
const emit = defineEmits<{ close: [] }>()

const { t } = useI18n()
const plants = usePlantsStore()

const iconOptions = [
  { bg: '#e6f3ef' }, { bg: '#fff4e5' }, { bg: '#ebf5ff' },
  { bg: '#f3e8ff' }, { bg: '#fce8e8' }, { bg: '#e8f5e9' },
]

const categories = computed(() => [
  { value: 'Indoor Plants', label: t('addPlant.categories.indoor') },
  { value: 'Outdoor Plants', label: t('addPlant.categories.outdoor') },
  { value: 'Succulents', label: t('addPlant.categories.succulents') },
  { value: 'Herbs', label: t('addPlant.categories.herbs') },
  { value: 'Vegetables', label: t('addPlant.categories.vegetables') },
  { value: 'Flowers', label: t('addPlant.categories.flowers') },
  { value: 'Trees & Shrubs', label: t('addPlant.categories.trees') },
])

const wateringOptions = computed(() => [
  { value: 'daily', label: t('addPlant.watering.daily') },
  { value: 'every-2-days', label: t('addPlant.watering.every-2-days') },
  { value: 'weekly', label: t('addPlant.watering.weekly') },
  { value: 'biweekly', label: t('addPlant.watering.biweekly') },
  { value: 'monthly', label: t('addPlant.watering.monthly') },
])

const sunlightOptions = computed(() => [
  { value: 'full-sun', label: t('addPlant.sunlight.fullSunLabel') },
  { value: 'partial', label: t('addPlant.sunlight.partialLabel') },
  { value: 'indirect', label: t('addPlant.sunlight.indirectLabel') },
  { value: 'low', label: t('addPlant.sunlight.lowLabel') },
])

// Seed the form from the existing plant.
const name = ref(props.plant.name)
const knownCategory = categories.value.some((c) => c.value === props.plant.category)
const category = ref(knownCategory ? props.plant.category : '__custom__')
const customCategory = ref(knownCategory ? '' : props.plant.category)
const wateringFrequency = ref(props.plant.wateringFrequency || 'every-2-days')
const sunlight = ref(props.plant.sunlight || 'indirect')
const wateringLevel = ref(props.plant.wateringLevel)
const notes = ref(props.plant.notes || '')
const selectedIcon = ref(Math.max(0, iconOptions.findIndex((o) => o.bg === props.plant.iconBg)))
const saving = ref(false)

const isValid = computed(
  () => name.value.trim().length > 0 &&
    (category.value !== '' && (category.value !== '__custom__' || customCategory.value.trim().length > 0)),
)

const waterStatusColor = computed(() => {
  if (wateringLevel.value <= 25) return '#ef4444'
  if (wateringLevel.value <= 50) return '#f59e0b'
  return '#37b27e'
})

function statusFor(level: number): Plant['status'] {
  if (level <= 25) return 'NEEDS WATER'
  if (level <= 50) return 'NEEDS MISTING'
  return 'HEALTHY'
}

async function save() {
  if (!isValid.value || saving.value) return
  saving.value = true
  const finalCategory = category.value === '__custom__' ? customCategory.value.trim() : category.value
  try {
    await plants.updatePlant(props.plant.id, {
      name: name.value.trim(),
      category: finalCategory,
      wateringFrequency: wateringFrequency.value,
      sunlight: sunlight.value,
      wateringLevel: wateringLevel.value,
      notes: notes.value.trim(),
      iconBg: iconOptions[selectedIcon.value]!.bg,
      status: statusFor(wateringLevel.value),
      careCard: { ...props.plant.careCard, category: finalCategory },
    })
    emit('close')
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div
    class="fixed inset-0 z-50 bg-black/40 flex items-center justify-center p-4"
    @click.self="emit('close')"
  >
    <div class="bg-surface rounded-2xl shadow-xl w-full max-w-[480px] max-h-[90vh] overflow-y-auto">
      <!-- Header -->
      <div class="flex items-start justify-between px-6 pt-6 pb-4 sticky top-0 bg-surface z-10">
        <div>
          <h2 class="text-[1.1rem] font-bold text-text-main">{{ t('plants.editTitle') }}</h2>
          <p class="text-[0.85rem] text-text-muted mt-0.5">{{ t('plants.editSubtitle', { name: plant.name }) }}</p>
        </div>
        <button
          @click="emit('close')"
          class="w-8 h-8 rounded-full bg-bg-app flex items-center justify-center text-text-muted hover:bg-[#e6e8eb] transition-colors shrink-0"
        >
          <X width="16" height="16" />
        </button>
      </div>

      <div class="px-6 pb-6 flex flex-col gap-5">
        <!-- Icon color + name -->
        <div class="flex gap-4">
          <div
            class="w-14 h-14 rounded-2xl flex items-center justify-center shrink-0 border border-border"
            :style="{ backgroundColor: iconOptions[selectedIcon]!.bg }"
          >
            <Flower2 width="28" height="28" style="color: #1a5641" />
          </div>
          <div class="flex flex-col gap-2 flex-1">
            <label class="text-[0.85rem] font-bold text-text-main">{{ t('addPlant.nameLabel') }}</label>
            <input
              v-model="name"
              type="text"
              class="px-4 py-2.5 rounded-xl border border-border bg-bg-app text-text-main text-[0.95rem] outline-none focus:border-success-green focus:bg-surface transition-colors"
            />
          </div>
        </div>

        <!-- Color swatches -->
        <div class="flex flex-wrap gap-2">
          <button
            v-for="(opt, i) in iconOptions"
            :key="i"
            type="button"
            @click="selectedIcon = i"
            class="w-8 h-8 rounded-full border-2 transition-all flex items-center justify-center"
            :class="selectedIcon === i ? 'border-success-green scale-110' : 'border-transparent hover:scale-105'"
            :style="{ backgroundColor: opt.bg }"
          >
            <Check v-if="selectedIcon === i" width="13" height="13" style="color: #1a5641" stroke-width="2.5" />
          </button>
        </div>

        <!-- Category -->
        <div class="flex flex-col gap-2">
          <label class="text-[0.85rem] font-bold text-text-main">{{ t('addPlant.categoryLabel') }}</label>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="cat in categories"
              :key="cat.value"
              type="button"
              @click="category = cat.value; customCategory = ''"
              class="px-3.5 py-1.5 rounded-[20px] text-[0.78rem] font-semibold border-2 transition-all"
              :class="category === cat.value
                ? 'border-success-green bg-light-green-bg text-primary'
                : 'border-border bg-bg-app text-text-muted hover:border-success-green'"
            >
              {{ cat.label }}
            </button>
            <button
              type="button"
              @click="category = '__custom__'"
              class="px-3.5 py-1.5 rounded-[20px] text-[0.78rem] font-semibold border-2 transition-all"
              :class="category === '__custom__'
                ? 'border-success-green bg-light-green-bg text-primary'
                : 'border-border bg-bg-app text-text-muted hover:border-success-green'"
            >
              {{ t('addPlant.other') }}
            </button>
          </div>
          <input
            v-if="category === '__custom__'"
            v-model="customCategory"
            type="text"
            :placeholder="t('addPlant.customCategoryPlaceholder')"
            class="mt-1 px-4 py-2.5 rounded-xl border border-border bg-bg-app text-text-main text-[0.95rem] outline-none focus:border-success-green focus:bg-surface transition-colors"
          />
        </div>

        <!-- Watering frequency -->
        <div class="flex flex-col gap-2">
          <label class="flex items-center gap-1.5 text-[0.85rem] font-bold text-text-main">
            <Droplet width="14" height="14" class="text-[#3b82f6]" />{{ t('addPlant.wateringFrequency') }}
          </label>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="opt in wateringOptions"
              :key="opt.value"
              type="button"
              @click="wateringFrequency = opt.value"
              class="px-3.5 py-1.5 rounded-[20px] text-[0.78rem] font-semibold border-2 transition-all"
              :class="wateringFrequency === opt.value
                ? 'border-[#3b82f6] bg-[#ebf5ff] text-[#3b82f6]'
                : 'border-border bg-bg-app text-text-muted hover:border-[#3b82f6]'"
            >
              {{ opt.label }}
            </button>
          </div>
        </div>

        <!-- Water level -->
        <div class="flex flex-col gap-2">
          <label class="flex items-center gap-1.5 text-[0.85rem] font-bold text-text-main">
            <Droplet width="14" height="14" class="text-[#37b27e]" />{{ t('addPlant.currentWaterLevel') }}
            <span class="ml-auto font-extrabold" :style="{ color: waterStatusColor }">{{ wateringLevel }}%</span>
          </label>
          <input
            v-model.number="wateringLevel"
            type="range"
            min="0"
            max="100"
            step="5"
            class="water-slider w-full h-2 rounded-full appearance-none cursor-pointer"
            :style="`--fill: ${waterStatusColor}; background: linear-gradient(to right, ${waterStatusColor} ${wateringLevel}%, #e2e8e4 ${wateringLevel}%)`"
          />
        </div>

        <!-- Sunlight -->
        <div class="flex flex-col gap-2">
          <label class="flex items-center gap-1.5 text-[0.85rem] font-bold text-text-main">
            <Sun width="14" height="14" class="text-[#f59e0b]" />{{ t('addPlant.sunlightNeeds') }}
          </label>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="opt in sunlightOptions"
              :key="opt.value"
              type="button"
              @click="sunlight = opt.value"
              class="px-3.5 py-1.5 rounded-[20px] text-[0.78rem] font-semibold border-2 transition-all"
              :class="sunlight === opt.value
                ? 'border-[#f59e0b] bg-[#fff9eb] text-[#b45309]'
                : 'border-border bg-bg-app text-text-muted hover:border-[#f59e0b]'"
            >
              {{ opt.label }}
            </button>
          </div>
        </div>

        <!-- Notes -->
        <div class="flex flex-col gap-2">
          <label class="text-[0.85rem] font-bold text-text-main">{{ t('addPlant.notesOptional') }}</label>
          <textarea
            v-model="notes"
            rows="2"
            :placeholder="t('addPlant.notesPlaceholder')"
            class="w-full px-4 py-2.5 rounded-xl border border-border bg-bg-app text-text-main text-[0.95rem] outline-none focus:border-success-green focus:bg-surface transition-colors resize-none font-[inherit]"
          ></textarea>
        </div>

        <!-- Actions -->
        <div class="flex justify-end gap-3 pt-1">
          <button
            @click="emit('close')"
            class="px-5 py-2.5 rounded-[24px] text-[0.9rem] font-bold border border-border text-text-muted hover:bg-bg-app transition-colors"
          >
            {{ t('common.cancel') }}
          </button>
          <button
            @click="save"
            :disabled="!isValid || saving"
            class="px-6 py-2.5 rounded-[24px] text-[0.9rem] font-bold bg-success-green text-white hover:bg-[#2ea06e] transition-colors disabled:opacity-60"
          >
            {{ t('common.saveChanges') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.water-slider::-webkit-slider-thumb {
  -webkit-appearance: none;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: white;
  border: 2.5px solid var(--fill, #37b27e);
  cursor: pointer;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.18);
}
.water-slider::-moz-range-thumb {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: white;
  border: 2.5px solid var(--fill, #37b27e);
  cursor: pointer;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.18);
}
</style>
