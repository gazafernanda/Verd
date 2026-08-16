<script setup lang="ts">
import { ref, computed, watch, onUnmounted } from "vue";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { usePlantsStore, type PlantSuggestion } from "../stores/plants";
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
  Wand2,
  RotateCcw,
} from "lucide-vue-next";

const router = useRouter();
const { t, locale } = useI18n();
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

const categories = computed(() => [
  { value: "Indoor Plants", label: t("addPlant.categories.indoor") },
  { value: "Outdoor Plants", label: t("addPlant.categories.outdoor") },
  { value: "Succulents", label: t("addPlant.categories.succulents") },
  { value: "Herbs", label: t("addPlant.categories.herbs") },
  { value: "Vegetables", label: t("addPlant.categories.vegetables") },
  { value: "Flowers", label: t("addPlant.categories.flowers") },
  { value: "Trees & Shrubs", label: t("addPlant.categories.trees") },
]);

const wateringOptions = computed(() => [
  { value: "daily", label: t("addPlant.watering.daily") },
  { value: "every-2-days", label: t("addPlant.watering.every-2-days") },
  { value: "weekly", label: t("addPlant.watering.weekly") },
  { value: "biweekly", label: t("addPlant.watering.biweekly") },
  { value: "monthly", label: t("addPlant.watering.monthly") },
]);

const sunlightOptions = computed(() => [
  { value: "full-sun", label: t("addPlant.sunlight.fullSunLabel"), desc: t("addPlant.sunlight.fullSunDesc") },
  { value: "partial", label: t("addPlant.sunlight.partialLabel"), desc: t("addPlant.sunlight.partialDesc") },
  { value: "indirect", label: t("addPlant.sunlight.indirectLabel"), desc: t("addPlant.sunlight.indirectDesc") },
  { value: "low", label: t("addPlant.sunlight.lowLabel"), desc: t("addPlant.sunlight.lowDesc") },
]);

const iconOptions = [
  { bg: "#e6f3ef", label: "Green" },
  { bg: "#fff4e5", label: "Warm" },
  { bg: "#ebf5ff", label: "Blue" },
  { bg: "#f3e8ff", label: "Purple" },
  { bg: "#fce8e8", label: "Rose" },
  { bg: "#e8f5e9", label: "Mint" },
];

// ── AI autofill ─────────────────────────────────────────────────────────────
// Typing a plant name asks the API what that plant usually needs and fills the
// rest of the form in. It is a head start, not a decision: anything the user has
// already set by hand is never overwritten, and the whole thing can be undone.

const suggestion = ref<PlantSuggestion | null>(null);
const suggesting = ref(false);

/** Lower-cased name the current suggestion describes, so we can tell it's stale. */
const suggestedFor = ref("");

/** Fields the user changed themselves — off-limits to autofill. */
const touched = ref(new Set<string>());

/** Fields autofill actually wrote, so the UI can label and undo them. */
const aiFilled = ref(new Set<string>());

const MIN_NAME_LENGTH = 3;
const DEBOUNCE_MS = 600;

let debounceTimer: ReturnType<typeof setTimeout> | undefined;
let inFlight: AbortController | null = null;

/** Marks a field as user-owned. Called from every control's handler. */
function markTouched(field: string) {
  touched.value.add(field);
  aiFilled.value.delete(field);
}

/** True when the suggestion on screen describes the name currently typed. */
const suggestionIsCurrent = computed(
  () => suggestedFor.value === name.value.trim().toLowerCase(),
);

/** The model recognised the name but spelled it differently — offer, don't impose. */
const spellingFix = computed(() => {
  const s = suggestion.value;
  if (!s?.isValid || !suggestionIsCurrent.value) return "";
  const typed = name.value.trim();
  return s.commonName && s.commonName.toLowerCase() !== typed.toLowerCase()
    ? s.commonName
    : "";
});

/** Warn early rather than waiting for the user to hit Add. */
const notAPlant = computed(
  () => !!suggestion.value && !suggestion.value.isValid && suggestionIsCurrent.value,
);

function applySuggestion(s: PlantSuggestion) {
  if (!s.isValid) return;

  const isKnownCategory = categories.value.some((c) => c.value === s.category);
  if (!touched.value.has("category") && s.category && isKnownCategory) {
    category.value = s.category;
    customCategory.value = "";
    aiFilled.value.add("category");
  }
  if (!touched.value.has("watering") && s.wateringFrequency) {
    wateringFrequency.value = s.wateringFrequency;
    aiFilled.value.add("watering");
  }
  if (!touched.value.has("sunlight") && s.sunlight) {
    sunlight.value = s.sunlight;
    aiFilled.value.add("sunlight");
  }
  if (!touched.value.has("notes") && s.notes && !notes.value.trim()) {
    notes.value = s.notes;
    aiFilled.value.add("notes");
  }
}

async function requestSuggestion(value: string) {
  // A newer keystroke wins: drop whatever is still in the air.
  inFlight?.abort();
  const controller = new AbortController();
  inFlight = controller;

  suggesting.value = true;
  try {
    const result = await plants.suggestPlantDetails(value, locale.value, controller.signal);
    if (controller.signal.aborted) return;

    // fromAi false means Groq was unreachable — say nothing rather than
    // showing an empty "we filled this in for you" panel.
    if (!result || !result.fromAi) return;

    suggestion.value = result;
    suggestedFor.value = value.toLowerCase();
    applySuggestion(result);
  } catch {
    // Aborted or offline — the form still works, it just isn't pre-filled.
  } finally {
    if (!controller.signal.aborted) suggesting.value = false;
  }
}

watch(name, (value) => {
  clearTimeout(debounceTimer);
  inFlight?.abort();
  suggesting.value = false;

  const trimmed = value.trim();
  if (trimmed.length < MIN_NAME_LENGTH) {
    suggestion.value = null;
    suggestedFor.value = "";
    return;
  }
  // Already answered for this exact name — no need to ask again.
  if (trimmed.toLowerCase() === suggestedFor.value) return;

  debounceTimer = setTimeout(() => void requestSuggestion(trimmed), DEBOUNCE_MS);
});

/** Puts every autofilled field back to its default and stops offering again. */
function undoAutofill() {
  if (aiFilled.value.has("category")) {
    category.value = "";
    customCategory.value = "";
  }
  if (aiFilled.value.has("watering")) wateringFrequency.value = "every-2-days";
  if (aiFilled.value.has("sunlight")) sunlight.value = "indirect";
  if (aiFilled.value.has("notes")) notes.value = "";

  aiFilled.value.clear();
  suggestion.value = null;
}

function acceptSpelling(corrected: string) {
  // Pre-set so the watcher doesn't fire a second lookup for a name we just resolved.
  suggestedFor.value = corrected.toLowerCase();
  name.value = corrected;
}

onUnmounted(() => {
  clearTimeout(debounceTimer);
  inFlight?.abort();
});

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
  if (wateringLevel.value <= 25) return t("addPlant.needsWater");
  if (wateringLevel.value <= 50) return t("addPlant.needsMisting");
  return t("addPlant.wellWatered");
});

async function submit() {
  if (!isValid.value) return;
  error.value = "";

  // The suggestion lookup already asked the model whether this is a real plant,
  // so re-validating the same name would be a second round trip for an answer
  // we hold. Only fall through to /validate when we don't.
  if (suggestionIsCurrent.value && suggestion.value?.fromAi) {
    if (!suggestion.value.isValid) {
      error.value = t("addPlant.invalidPlant", { name: name.value.trim() });
      return;
    }
  } else {
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
            error.value = t("addPlant.invalidPlant", { name: name.value.trim() });
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
          {{ t('addPlant.title') }}
        </h1>
        <p class="text-text-muted text-[0.9rem] mt-1">
          {{ t('addPlant.subtitle') }}
        </p>
      </div>
    </div>

    <form @submit.prevent="submit" class="flex flex-col gap-6">
      <!-- Plant Identity Card -->
      <div class="bg-surface rounded-xl border border-border shadow-sm p-6">
        <h2
          class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px] mb-5"
        >
          {{ t('addPlant.identity') }}
        </h2>

        <!-- Icon picker + Name row -->
        <div class="flex gap-5 mb-6 max-sm:flex-col">
          <!-- Icon preview -->
          <div class="flex flex-col gap-2 shrink-0">
            <span class="text-[0.8rem] font-bold text-text-main">{{ t('addPlant.color') }}</span>
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
              >{{ t('addPlant.pickColor') }}</span
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
            >{{ t('addPlant.nameLabel') }} <span class="text-red-400">*</span></label
          >
          <div class="relative">
            <input
              v-model="name"
              type="text"
              :placeholder="t('addPlant.namePlaceholder')"
              required
              class="w-full px-4 py-3 pr-11 rounded-xl border border-border bg-bg-app text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-success-green focus:bg-surface focus:shadow-[0_0_0_3px_rgba(55,178,126,0.1)]"
            />
            <!-- Thinking indicator, in the field so it reads as part of typing -->
            <span
              v-if="suggesting"
              class="absolute right-4 top-1/2 -translate-y-1/2 text-success-green"
              :title="t('addPlant.ai.thinking')"
            >
              <Wand2 width="17" height="17" class="animate-pulse" />
            </span>
          </div>

          <!-- Hint that autofill exists, before the user has typed enough -->
          <p v-if="!suggestion && !suggesting" class="text-[0.78rem] text-text-light leading-relaxed">
            {{ t('addPlant.ai.hint') }}
          </p>

          <!-- Spelling correction: offered, never forced -->
          <button
            v-if="spellingFix"
            type="button"
            @click="acceptSpelling(spellingFix)"
            class="self-start text-[0.8rem] text-text-muted hover:text-text-main transition-colors"
          >
            {{ t('addPlant.ai.didYouMean') }}
            <span class="font-bold text-success-green underline underline-offset-2">{{ spellingFix }}</span>
          </button>

          <!-- Name isn't a plant — flagged now rather than on submit -->
          <p
            v-if="notAPlant"
            class="flex items-start gap-2 text-[0.8rem] font-medium text-[#b45309] leading-relaxed"
          >
            <TriangleAlert width="14" height="14" class="shrink-0 mt-0.5" />
            {{ t('addPlant.ai.notAPlant', { name: name.trim() }) }}
          </p>
        </div>

        <!-- What the AI filled in, and how to undo it -->
        <div
          v-if="suggestion?.isValid && aiFilled.size > 0"
          class="mb-5 rounded-xl border border-[#cbe9dc] bg-light-green-bg px-4 py-3"
        >
          <div class="flex items-start gap-2.5">
            <Wand2 class="text-success-green shrink-0 mt-0.5" width="16" height="16" />
            <div class="flex-1 min-w-0">
              <p class="text-[0.85rem] font-bold text-primary">
                {{ t('addPlant.ai.filledTitle', { name: suggestion.commonName }) }}
              </p>
              <p v-if="suggestion.scientificName" class="text-[0.78rem] italic text-text-muted mt-0.5">
                {{ suggestion.scientificName }}
              </p>
              <p class="text-[0.78rem] text-text-muted mt-1 leading-relaxed">
                {{ t('addPlant.ai.filledDesc') }}
              </p>
            </div>
            <button
              type="button"
              @click="undoAutofill"
              class="shrink-0 inline-flex items-center gap-1.5 px-2.5 py-1.5 rounded-lg text-[0.75rem] font-bold text-text-muted hover:bg-white/70 hover:text-text-main transition-colors"
            >
              <RotateCcw width="13" height="13" />
              {{ t('addPlant.ai.undo') }}
            </button>
          </div>
        </div>

        <!-- Category -->
        <div class="flex flex-col gap-2">
          <label class="flex items-center gap-2 text-[0.85rem] font-bold text-text-main">
            {{ t('addPlant.categoryLabel') }} <span class="text-red-400">*</span>
            <span
              v-if="aiFilled.has('category')"
              class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[0.6rem] font-extrabold tracking-[0.5px] bg-light-green-bg text-success-green align-middle"
            >
              <Wand2 width="9" height="9" /> {{ t('addPlant.ai.badge') }}
            </span>
          </label>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="cat in categories"
              :key="cat.value"
              type="button"
              @click="
                markTouched('category');
                category = cat.value;
                customCategory = '';
              "
              class="px-4 py-2 rounded-[20px] text-[0.8rem] font-semibold border-2 transition-all duration-150"
              :class="
                category === cat.value
                  ? 'border-success-green bg-light-green-bg text-primary'
                  : 'border-border bg-bg-app text-text-muted hover:border-success-green hover:text-text-main'
              "
            >
              {{ cat.label }}
            </button>
            <button
              type="button"
              @click="markTouched('category'); category = '__custom__'"
              class="px-4 py-2 rounded-[20px] text-[0.8rem] font-semibold border-2 transition-all duration-150"
              :class="
                category === '__custom__'
                  ? 'border-success-green bg-light-green-bg text-primary'
                  : 'border-border bg-bg-app text-text-muted hover:border-success-green hover:text-text-main'
              "
            >
              {{ t('addPlant.other') }}
            </button>
          </div>
          <input
            v-if="category === '__custom__'"
            v-model="customCategory"
            @input="markTouched('category')"
            type="text"
            :placeholder="t('addPlant.customCategoryPlaceholder')"
            class="mt-2 px-4 py-3 rounded-xl border border-border bg-bg-app text-text-main text-[0.95rem] outline-none transition-all duration-200 placeholder:text-text-light focus:border-success-green focus:bg-surface focus:shadow-[0_0_0_3px_rgba(55,178,126,0.1)]"
          />
        </div>
      </div>

      <!-- Care Preferences -->
      <div class="bg-surface rounded-xl border border-border shadow-sm p-6">
        <h2
          class="text-[0.65rem] font-extrabold text-text-muted tracking-[1px] mb-5"
        >
          {{ t('addPlant.carePreferences') }}
        </h2>

        <!-- Watering frequency -->
        <div class="mb-6">
          <div class="flex items-center gap-2 mb-3">
            <Droplet width="16" height="16" class="text-[#3b82f6]" />
            <label class="flex items-center gap-2 text-[0.9rem] font-bold text-text-main">
              {{ t('addPlant.wateringFrequency') }}
              <span
              v-if="aiFilled.has('watering')"
              class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[0.6rem] font-extrabold tracking-[0.5px] bg-light-green-bg text-success-green align-middle"
            >
              <Wand2 width="9" height="9" /> {{ t('addPlant.ai.badge') }}
            </span>
            </label>
          </div>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="opt in wateringOptions"
              :key="opt.value"
              type="button"
              @click="markTouched('watering'); wateringFrequency = opt.value"
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
            <label class="text-[0.9rem] font-bold text-text-main">{{ t('addPlant.currentWaterLevel') }}</label>
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
            <span>{{ t('addPlant.needsWater') }}</span>
            <span>{{ t('addPlant.wellWatered') }}</span>
          </div>
        </div>

        <!-- Sunlight -->
        <div>
          <div class="flex items-center gap-2 mb-3">
            <Sun width="16" height="16" class="text-[#f59e0b]" />
            <label class="flex items-center gap-2 text-[0.9rem] font-bold text-text-main">
              {{ t('addPlant.sunlightNeeds') }}
              <span
              v-if="aiFilled.has('sunlight')"
              class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[0.6rem] font-extrabold tracking-[0.5px] bg-light-green-bg text-success-green align-middle"
            >
              <Wand2 width="9" height="9" /> {{ t('addPlant.ai.badge') }}
            </span>
            </label>
          </div>
          <div class="grid grid-cols-2 max-sm:grid-cols-1 gap-3">
            <button
              v-for="opt in sunlightOptions"
              :key="opt.value"
              type="button"
              @click="markTouched('sunlight'); sunlight = opt.value"
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
          class="flex items-center gap-2 text-[0.65rem] font-extrabold text-text-muted tracking-[1px] mb-5"
        >
          {{ t('addPlant.notesOptional') }}
          <span
              v-if="aiFilled.has('notes')"
              class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[0.6rem] font-extrabold tracking-[0.5px] bg-light-green-bg text-success-green align-middle"
            >
              <Wand2 width="9" height="9" /> {{ t('addPlant.ai.badge') }}
            </span>
        </h2>
        <textarea
          v-model="notes"
          @input="markTouched('notes')"
          rows="3"
          :placeholder="t('addPlant.notesPlaceholder')"
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
          {{ t('common.cancel') }}
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
          {{ saved && recs.loading ? t('addPlant.gettingRecs') : saved ? t('addPlant.plantAdded') : validating ? t('addPlant.checking') : saving ? t('addPlant.saving') : t('common.addPlant') }}
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
