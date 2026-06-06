import { ref } from "vue";
import { defineStore } from "pinia";
import i18n from "../i18n";

const API = import.meta.env.VITE_API_URL || "http://localhost:5000";

// The app language the AI should answer in (e.g. "id" or "en").
function currentLang() {
  return i18n.global.locale.value;
}

export interface PriorityAction {
  id: string;
  title: string;
  description: string;
  priority: "IMMEDIATE" | "RECOMMENDED" | "OPTIONAL";
  type: "water" | "mist" | "shade" | "fertilize" | "prune";
}

export interface BotanicalInsight {
  headline: string;
  detail: string;
}

export interface DayPlan {
  day: string;
  date: string;
  weather: string;
  action: string;
  type: "water" | "mist" | "shade" | "fertilize" | "prune" | "none";
}

export const useRecommendationsStore = defineStore("recommendations", () => {
  const priorityActions = ref<PriorityAction[]>([]);
  const insight = ref<BotanicalInsight | null>(null);
  const weeklyPlan = ref<DayPlan[]>([]);
  const loading = ref(false);
  const generatingFor = ref<string | null>(null); // plant name currently generating for

  async function fetchRecommendations() {
    const token = localStorage.getItem("verd_token");
    if (!token) return;

    loading.value = true;
    try {
      const res = await fetch(`${API}/api/recommendations?lang=${currentLang()}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (res.ok) {
        const data = await res.json();
        priorityActions.value = data.priorityActions ?? [];
        insight.value = data.insight;
        weeklyPlan.value = data.weeklyPlan ?? [];
      }
    } finally {
      loading.value = false;
    }
  }

  async function generateForPlant(plantId: number, plantName: string) {
    const token = localStorage.getItem("verd_token");
    if (!token) return false;
    generatingFor.value = plantName;
    weeklyPlan.value = [];
    loading.value = true;
    try {
      const res = await fetch(`${API}/api/recommendations/generate`, {
        method: "POST",
        headers: { "Content-Type": "application/json", Authorization: `Bearer ${token}` },
        body: JSON.stringify({ plantId, language: currentLang() }),
      });
      if (!res.ok) return false;
      const data = await res.json();
      priorityActions.value = data.priorityActions ?? [];
      insight.value = data.insight;
      weeklyPlan.value = data.weeklyPlan ?? [];
      return true;
    } catch {
      return false;
    } finally {
      loading.value = false;
    }
  }

  return { priorityActions, insight, weeklyPlan, loading, generatingFor, fetchRecommendations, generateForPlant };
});
