import { ref, computed } from "vue";
import { defineStore } from "pinia";

const API = import.meta.env.VITE_API_URL || "http://localhost:5000";

export interface Plant {
  id: number;
  name: string;
  status: "HEALTHY" | "NEEDS WATER" | "NEEDS MISTING";
  wateringLevel: number;
  lastWatered: string;
  category: string;
  iconBg: string;
  wateringFrequency: string;
  sunlight: string;
  notes: string;
  careCard: {
    category: string;
    title: string;
    description: string;
    image: string;
    bgType: "blue" | "yellow" | "green";
  };
}

export interface PlantLog {
  id: number;
  plantId: number;
  action: string;
  notes: string;
  loggedAt: string;
}

function authHeaders() {
  const token = localStorage.getItem("verd_token");
  return {
    "Content-Type": "application/json",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

export const usePlantsStore = defineStore("plants", () => {
  const plants = ref<Plant[]>([]);

  const careStreak = ref(0);

  const totalPlants = computed(() => plants.value.length);

  const plantsNeedingCare = computed(() =>
    plants.value.filter((p) => p.status !== "HEALTHY"),
  );

  const plantsMistingNeeded = computed(() =>
    plants.value.filter((p) => p.status === "NEEDS MISTING"),
  );

  const gardenCategories = computed(() => [
    ...new Set(plants.value.map((p) => p.category)),
  ]);

  const overallStatus = computed(() => {
    if (plantsNeedingCare.value.length === 0) return "safe";
    if (plantsNeedingCare.value.some((p) => p.status === "NEEDS WATER"))
      return "warning";
    return "attention";
  });

  async function fetchPlants(): Promise<boolean> {
    const token = localStorage.getItem("verd_token");
    if (!token) return false;
    try {
      const res = await fetch(`${API}/api/plants`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (res.ok) {
        plants.value = await res.json();
        await refreshCareStreak();
        return true;
      }
      if (res.status === 401) return false;
    } catch { /* silent fail */ }
    return false;
  }

  /** Consecutive days (ending today) with at least one care log across any plant. */
  async function refreshCareStreak() {
    if (plants.value.length === 0) {
      careStreak.value = 0;
      return;
    }
    const logsByPlant = await Promise.all(
      plants.value.map((p) => fetchLogs(p.id)),
    );
    const loggedDays = new Set(
      logsByPlant.flat().map((log) => log.loggedAt.slice(0, 10)),
    );

    let streak = 0;
    const cursor = new Date();
    while (loggedDays.has(cursor.toISOString().slice(0, 10))) {
      streak++;
      cursor.setDate(cursor.getDate() - 1);
    }
    careStreak.value = streak;
  }

  async function deletePlant(id: number) {
    const token = localStorage.getItem("verd_token");
    if (!token) return;
    const res = await fetch(`${API}/api/plants/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });
    if (res.ok || res.status === 204) {
      plants.value = plants.value.filter((p) => p.id !== id);
    }
  }

  async function updatePlant(id: number, data: Partial<Plant>) {
    const token = localStorage.getItem("verd_token");
    const plant = plants.value.find((p) => p.id === id);
    if (!plant) return;

    const merged = { ...plant, ...data };

    if (!token) return;

    const res = await fetch(`${API}/api/plants/${id}`, {
      method: "PUT",
      headers: authHeaders(),
      body: JSON.stringify({
        name: merged.name,
        status: merged.status,
        wateringLevel: merged.wateringLevel,
        lastWatered: merged.lastWatered,
        category: merged.category,
        iconBg: merged.iconBg,
        wateringFrequency: merged.wateringFrequency,
        sunlight: merged.sunlight,
        notes: merged.notes,
        careCategory: merged.careCard.category,
        careTitle: merged.careCard.title,
        careDescription: merged.careCard.description,
        careImage: merged.careCard.image,
        careBgType: merged.careCard.bgType,
      }),
    });
    if (res.ok) {
      const updated = await res.json();
      const idx = plants.value.findIndex((p) => p.id === id);
      if (idx !== -1) plants.value[idx] = updated;
    }
  }

  async function logCare(plantId: number, action: string, notes = "") {
    const token = localStorage.getItem("verd_token");
    if (!token) return;
    const res = await fetch(`${API}/api/plants/${plantId}/logs`, {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify({ action, notes }),
    });
    if (res.ok) {
      // Refresh the plant to get updated status/wateringLevel
      const plantRes = await fetch(`${API}/api/plants/${plantId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (plantRes.ok) {
        const updated = await plantRes.json();
        const idx = plants.value.findIndex((p) => p.id === plantId);
        if (idx !== -1) plants.value[idx] = updated;
      }
      await refreshCareStreak();
    }
  }

  async function fetchLogs(plantId: number): Promise<PlantLog[]> {
    const token = localStorage.getItem("verd_token");
    if (!token) return [];
    const res = await fetch(`${API}/api/plants/${plantId}/logs`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (res.ok) return await res.json();
    return [];
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
    deletePlant,
    updatePlant,
    logCare,
    fetchLogs,
  };
});
