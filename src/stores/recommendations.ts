import { ref } from "vue";
import { defineStore } from "pinia";

const API = import.meta.env.VITE_API_URL ?? "http://localhost:5000";

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

export const useRecommendationsStore = defineStore("recommendations", () => {
  const priorityActions = ref<PriorityAction[]>([]);
  const insight = ref<BotanicalInsight | null>(null);
  const loading = ref(false);
  const generatingFor = ref<string | null>(null); // plant name currently generating for

  async function fetchRecommendations() {
    const token = localStorage.getItem("verd_token");
    if (!token || token === "demo") {
      // Default data for demo mode
      priorityActions.value = [
        {
          id: "water-plants",
          title: "Water deeply at base",
          description:
            "Apply roughly 2 liters per m² specifically for tomatoes and zucchini to prevent heat-stress wilting.",
          priority: "IMMEDIATE",
          type: "water",
        },
        {
          id: "shade-cloth",
          title: "Deploy 30% Shade Cloth",
          description:
            "Protect leafy greens from intense afternoon sun to prevent tip burn and bolting.",
          priority: "RECOMMENDED",
          type: "shade",
        },
      ];
      insight.value = {
        headline:
          "High solar radiation increases the transpiration rate of your crops.",
        detail:
          "Stomata are currently wide open to facilitate cooling, leading to rapid water loss from soil surface.",
      };
      return;
    }

    loading.value = true;
    try {
      const res = await fetch(`${API}/api/recommendations`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (res.ok) {
        const data = await res.json();
        priorityActions.value = data.priorityActions;
        insight.value = data.insight;
      }
    } finally {
      loading.value = false;
    }
  }

  async function generateForPlant(plantId: number, plantName: string) {
    const token = localStorage.getItem("verd_token");
    if (!token || token === "demo") return false;
    generatingFor.value = plantName;
    loading.value = true;
    try {
      const res = await fetch(`${API}/api/recommendations/generate`, {
        method: "POST",
        headers: { "Content-Type": "application/json", Authorization: `Bearer ${token}` },
        body: JSON.stringify({ plantId }),
      });
      if (!res.ok) return false;
      const data = await res.json();
      priorityActions.value = data.priorityActions;
      insight.value = data.insight;
      return true;
    } catch {
      return false;
    } finally {
      loading.value = false;
    }
  }

  return { priorityActions, insight, loading, generatingFor, fetchRecommendations, generateForPlant };
});
