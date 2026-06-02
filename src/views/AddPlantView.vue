<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { usePlantsStore } from "../stores/plants";
import { useRecommendationsStore } from "../stores/recommendations";
import {
  ChevronLeft,
  Sun,
  Droplet,
  TriangleAlert,
  Check,
  Plus,
  Flower2,
  Sparkles,
} from "lucide-vue-next";

const router = useRouter();
const plants = usePlantsStore();
const recs = useRecommendationsStore();

// Form state
const name = ref("");
const category = ref("");
const customCategory = ref("");
const wateringFrequency = ref("every-2-days");
const sunlight = ref("indirect");
const wateringLevel = ref(80);
const notes = ref("");
const selectedIcon = ref(0);
const saving = ref(false);
const saved = ref(false);
const validating = ref(false);
const error = ref("");

const categories = [
  "Indoor Plants",
  "Outdoor Plants",
  "Succulents",
  "Herbs",
  "Vegetables",
  "Flowers",
  "Trees & Shrubs",
];

const wateringOptions = [
  { value: "daily", label: "Daily" },
  { value: "every-2-days", label: "Every 2 days" },
  { value: "weekly", label: "Weekly" },
  { value: "biweekly", label: "Every 2 weeks" },
  { value: "monthly", label: "Monthly" },
];

const sunlightOptions = [
  { value: "full-sun", label: "Full Sun", desc: "6+ hours direct sunlight" },
  { value: "partial", label: "Partial Sun", desc: "3–6 hours direct sunlight" },
  {
    value: "indirect",
    label: "Indirect Light",
    desc: "Bright but no direct rays",
  },
  { value: "low", label: "Low Light", desc: "Shade tolerant" },
];

const iconOptions = [
  { bg: "#e6f3ef", label: "Green" },
  { bg: "#fff4e5", label: "Warm" },
  { bg: "#ebf5ff", label: "Blue" },
  { bg: "#f3e8ff", label: "Purple" },
  { bg: "#fce8e8", label: "Rose" },
  { bg: "#e8f5e9", label: "Mint" },
];

const isValid = computed(
  () =>
    name.value.trim().length > 0 &&
    (category.value !== "" || customCategory.value.trim().length > 0),
);

const waterStatus = computed(() => {
  if (wateringLevel.value <= 25) return "NEEDS WATER";
  if (wateringLevel.value <= 50) return "NEEDS MISTING";
  return "HEALTHY";
});

const waterStatusColor = computed(() => {
  if (wateringLevel.value <= 25) return "#ef4444";
  if (wateringLevel.value <= 50) return "#f59e0b";
  return "#37b27e";
});

const waterStatusLabel = computed(() => {
  if (wateringLevel.value <= 25) return "Needs water";
  if (wateringLevel.value <= 50) return "Needs misting";
  return "Well watered";
});

async function submit() {
  if (!isValid.value) return;
  error.value = "";

  const token = localStorage.getItem("verd_token");
  if (token) {
    validating.value = true;
    try {
      const API = import.meta.env.VITE_API_URL || "http://localhost:5000";
      const res = await fetch(`${API}/api/plants/validate`, {
        method: "POST",
        headers: { "Content-Type": "application/json", Authorization: `Bearer ${token}` },
        body: JSON.stringify({ name: name.value.trim() }),
      });
      if (res.ok) {
        const data = await res.json();
        if (!data.isValid) {
          error.value = `"${name.value.trim()}" doesn't seem to be a real plant. Please enter a valid plant name.`;
          validating.value = false;
          return;
        }
      }
    } catch {
      // If validation fails to reach server, allow through
    } finally {
      validating.value = false;
    }
  }

  saving.value = true;

  const finalCategory =
    category.value === "__custom__"
      ? customCategory.value.trim()
      : category.value;

  // Build new plant object (optimistic, matches store shape)
  const newPlant = {
    id: Date.now(),
    name: name.value.trim(),
    status: waterStatus.value as "HEALTHY" | "NEEDS WATER" | "NEEDS MISTING",
    wateringLevel: wateringLevel.value,
    lastWatered: wateringLevel.value >= 80 ? "Just now" : "A while ago",
    category: finalCategory,
    iconBg: iconOptions[selectedIcon.value]!.bg,
    wateringFrequency: wateringFrequency.value,
    sunlight: sunlight.value,
    notes: notes.value.trim(),
    careCard: {
      category: finalCategory,
      title: `Care for ${name.value.trim()}`,
      description:
        notes.value.trim() ||
        `Regular care schedule for your ${name.value.trim()}.`,
      image: "",
      bgType: "green" as const,
    },
  };

  try {
    const token = localStorage.getItem("verd_token");
    if (token) {
      const API = import.meta.env.VITE_API_URL || "http://localhost:5000";
      const res = await fetch(`${API}/api/plants`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          name: newPlant.name,
          status: newPlant.status,
          wateringLevel: newPlant.wateringLevel,
          lastWatered: newPlant.lastWatered,
          category: newPlant.category,
          wateringFrequency: wateringFrequency.value,
          sunlight: sunlight.value,
          notes: notes.value.trim(),
          iconBg: newPlant.iconBg,
        }),
      });
      if (res.ok) {
        const created = await res.json();
        plants.plants.unshift({ ...newPlant, id: created.id ?? newPlant.id });
      } else {
        plants.plants.unshift(newPlant);
      }
    } else {
      // No token — push locally as a fallback
      plants.plants.unshift(newPlant);
    }
    saved.value = true;
    const savedId = plants.plants[0]?.id ?? newPlant.id ?? 0;
    await recs.generateForPlant(savedId, newPlant.name);
    router.push({ name: "recommendation" });
  } catch {
    plants.plants.unshift(newPlant);
    saved.value = true;
    router.push({ name: "recommendation" });
  } finally {
    saving.value = false;
  }
}
</script>

<template>
  <div class="max-w-[720px] mx-auto">
    <!-- Header -->
    <div class="flex items-center gap-4 mb-8">
      <button
        @click="router.back()"
        class="w-9 h-9 rounded-full bg-surface border border-border flex items-center justify-center text-text-muted hover:bg-bg-app transition-colors shrink-0"
      >
        <ChevronLeft width="18" height="18" />
      </button>
      <div>
        <h1
          class="text-[1.8rem] max-sm:text-[1.4rem] font-extrabold text-text-main leading-none"
        >
          Add New Plant
        </h1>
        <p class="text-text-muted text-[0.9rem] mt-1">
          Track a new plant in your garden
        </p>
      </div>
    </div>

    <form @submit.prevent="submit" class="flex flex-col gap-6">
      <!-- Plant Identity Card -->
      <div class="bg-surface rounded-xl border border-border shadow-sm p-6">
        <h2
          class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px] mb-5"
        >
          PLANT IDENTITY
        </h2>

        <!-- Icon picker + Name row -->
        <div class="flex gap-5 mb-6 max-sm:flex-col">
          <!-- Icon preview -->
          <div class="flex flex-col gap-2 shrink-0">
            <span class="text-[0.8rem] font-bold text-text-main">Color</span>
            <div
              class="w-16 h-16 rounded-2xl flex items-center justify-center shrink-0 border border-border"
              :style="{ backgroundColor: iconOptions[selectedIcon]!.bg }"
            >
              <Flower2 width="32" height="32" style="color: #1a5641" />
            </div>
          </div>

          <!-- Color swatches -->
          <div class="flex flex-col gap-2 flex-1">
            <span class="text-[0.8rem] font-bold text-text-main"
              >Pick a color</span
            >
            <div class="flex flex-wrap gap-2 mt-1">
              <button
                v-for="(opt, i) in iconOptions"
                :key="i"
                type="button"
                @click="selectedIcon = i"
                class="w-9 h-9 rounded-full border-2 transition-all duration-150 flex items-center justify-center"
                :class="
                  selectedIcon === i
                    ? 'border-success-green scale-110 shadow-sm'
                    : 'border-transparent hover:scale-105'
                "
                :style="{ backgroundColor: opt.bg }"
              >
                <Check
                  v-if="selectedIcon === i"
                  width="14"
                  height="14"
                  style="color: #1a5641"
                  stroke-width="2.5"
                />
              </button>
            </div>
          </div>
        </div>

        <!-- Plant name -->
        <div class="flex flex-col gap-2 mb-5">
          <label class="text-[0.85rem] font-bold text-text-main"
            >Plant Name <span class="text-red-400">*</span></label
          >
          <input
            v-model="name"
            type="text"
            placeholder="e.g. Cherry Tomato, Monstera, Basil…"
            required
            class="px-4 py-3 rounded-xl border border-border bg-bg-app text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-success-green focus:bg-surface focus:shadow-[0_0_0_3px_rgba(55,178,126,0.1)]"
          />
        </div>

        <!-- Category -->
        <div class="flex flex-col gap-2">
          <label class="text-[0.85rem] font-bold text-text-main"
            >Category <span class="text-red-400">*</span></label
          >
          <div class="flex flex-wrap gap-2">
            <button
              v-for="cat in categories"
              :key="cat"
              type="button"
              @click="
                category = cat;
                customCategory = '';
              "
              class="px-4 py-2 rounded-[20px] text-[0.8rem] font-semibold border-2 transition-all duration-150"
              :class="
                category === cat
                  ? 'border-success-green bg-light-green-bg text-primary'
                  : 'border-border bg-bg-app text-text-muted hover:border-success-green hover:text-text-main'
              "
            >
              {{ cat }}
            </button>
            <button
              type="button"
              @click="category = '__custom__'"
              class="px-4 py-2 rounded-[20px] text-[0.8rem] font-semibold border-2 transition-all duration-150"
              :class="
                category === '__custom__'
                  ? 'border-success-green bg-light-green-bg text-primary'
                  : 'border-border bg-bg-app text-text-muted hover:border-success-green hover:text-text-main'
              "
            >
              + Other
            </button>
          </div>
          <input
            v-if="category === '__custom__'"
            v-model="customCategory"
            type="text"
            placeholder="Enter custom category…"
            class="mt-2 px-4 py-3 rounded-xl border border-border bg-bg-app text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-success-green focus:bg-surface focus:shadow-[0_0_0_3px_rgba(55,178,126,0.1)]"
          />
        </div>
      </div>

      <!-- Care Preferences -->
      <div class="bg-surface rounded-xl border border-border shadow-sm p-6">
        <h2
          class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px] mb-5"
        >
          CARE PREFERENCES
        </h2>

        <!-- Watering frequency -->
        <div class="mb-6">
          <div class="flex items-center gap-2 mb-3">
            <Droplet width="16" height="16" class="text-[#3b82f6]" />
            <label class="text-[0.9rem] font-bold text-text-main"
              >Watering Frequency</label
            >
          </div>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="opt in wateringOptions"
              :key="opt.value"
              type="button"
              @click="wateringFrequency = opt.value"
              class="px-4 py-2 rounded-[20px] text-[0.8rem] font-semibold border-2 transition-all duration-150"
              :class="
                wateringFrequency === opt.value
                  ? 'border-[#3b82f6] bg-[#ebf5ff] text-[#3b82f6]'
                  : 'border-border bg-bg-app text-text-muted hover:border-[#3b82f6] hover:text-text-main'
              "
            >
              {{ opt.label }}
            </button>
          </div>
        </div>

        <!-- Water Level -->
        <div class="mb-6">
          <div class="flex items-center gap-2 mb-3">
            <Droplet width="16" height="16" class="text-[#37b27e]" />
            <label class="text-[0.9rem] font-bold text-text-main">Current Water Level</label>
          </div>
          <div class="flex items-center gap-4 mb-2">
            <span class="text-[2rem] font-extrabold text-text-main leading-none">{{ wateringLevel }}%</span>
            <span
              class="px-2.5 py-1 rounded-full text-[0.72rem] font-bold"
              :style="{ backgroundColor: waterStatusColor + '20', color: waterStatusColor }"
            >{{ waterStatusLabel }}</span>
          </div>
          <input
            v-model.number="wateringLevel"
            type="range"
            min="0"
            max="100"
            step="5"
            class="water-slider w-full h-2 rounded-full appearance-none cursor-pointer"
            :style="`--fill: ${waterStatusColor}; background: linear-gradient(to right, ${waterStatusColor} ${wateringLevel}%, #e2e8e4 ${wateringLevel}%)`"
          />
          <div class="flex justify-between text-[0.72rem] text-text-muted mt-1.5">
            <span>Needs water</span>
            <span>Well watered</span>
          </div>
        </div>

        <!-- Sunlight -->
        <div>
          <div class="flex items-center gap-2 mb-3">
            <Sun width="16" height="16" class="text-[#f59e0b]" />
            <label class="text-[0.9rem] font-bold text-text-main"
              >Sunlight Needs</label
            >
          </div>
          <div class="grid grid-cols-2 max-sm:grid-cols-1 gap-3">
            <button
              v-for="opt in sunlightOptions"
              :key="opt.value"
              type="button"
              @click="sunlight = opt.value"
              class="flex flex-col items-start px-4 py-3 rounded-xl border-2 text-left transition-all duration-150"
              :class="
                sunlight === opt.value
                  ? 'border-[#f59e0b] bg-[#fff9eb]'
                  : 'border-border bg-bg-app hover:border-[#f59e0b]'
              "
            >
              <span
                class="text-[0.85rem] font-bold"
                :class="
                  sunlight === opt.value ? 'text-[#b45309]' : 'text-text-main'
                "
              >
                {{ opt.label }}
              </span>
              <span
                class="text-[0.75rem] mt-0.5"
                :class="
                  sunlight === opt.value ? 'text-[#92400e]' : 'text-text-muted'
                "
              >
                {{ opt.desc }}
              </span>
            </button>
          </div>
        </div>
      </div>

      <!-- Notes -->
      <div class="bg-surface rounded-xl border border-border shadow-sm p-6">
        <h2
          class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px] mb-5"
        >
          NOTES (OPTIONAL)
        </h2>
        <textarea
          v-model="notes"
          rows="3"
          placeholder="Any special care instructions, where it's located, reminders…"
          class="w-full px-4 py-3 rounded-xl border border-border bg-bg-app text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-success-green focus:bg-surface focus:shadow-[0_0_0_3px_rgba(55,178,126,0.1)] resize-none font-[inherit] leading-relaxed"
        ></textarea>
      </div>

      <!-- Error -->
      <div
        v-if="error"
        class="flex items-start gap-2 px-4 py-3 rounded-xl bg-red-50 border border-red-200 text-red-600 text-[0.9rem] font-medium"
      >
        <TriangleAlert width="16" height="16" class="shrink-0 mt-0.5" />
        {{ error }}
      </div>

      <!-- Actions -->
      <div class="flex gap-3 max-sm:flex-col pb-4">
        <button
          type="button"
          @click="router.back()"
          class="flex-1 py-3.5 rounded-[24px] text-[0.95rem] font-bold border-2 border-border bg-transparent text-text-main hover:bg-bg-app transition-colors"
        >
          Cancel
        </button>
        <button
          type="submit"
          :disabled="!isValid || validating || saving || saved"
          class="flex-1 py-3.5 rounded-[24px] text-[0.95rem] font-bold flex items-center justify-center gap-2 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
          :class="
            saved
              ? 'bg-success-green text-white'
              : 'bg-primary text-white hover:bg-primary-hover shadow-[0_4px_12px_rgba(26,86,65,0.2)] hover:-translate-y-px'
          "
        >
          <Sparkles v-if="saved && recs.loading" width="18" height="18" class="animate-pulse" />
          <Check v-else-if="saved" width="18" height="18" stroke-width="2.5" />
          <Plus v-else-if="!validating && !saving" width="18" height="18" />
          <svg v-else class="animate-spin" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M21 12a9 9 0 1 1-6.22-8.56" />
          </svg>
          {{ saved && recs.loading ? "Getting AI recommendations…" : saved ? "Plant added!" : validating ? "Checking plant…" : saving ? "Saving…" : "Add Plant" }}
        </button>
      </div>
    </form>
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
  box-shadow: 0 1px 4px rgba(0,0,0,0.18);
}
.water-slider::-moz-range-thumb {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: white;
  border: 2.5px solid var(--fill, #37b27e);
  cursor: pointer;
  box-shadow: 0 1px 4px rgba(0,0,0,0.18);
}
</style>
