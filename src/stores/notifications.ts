import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { usePlantsStore } from "./plants";
import { useWeatherStore } from "./weather";

export type NotificationType =
  | "water"
  | "misting"
  | "weather"
  | "tip"
  | "streak";

export interface AppNotification {
  /** Stable id so read-state survives re-derivation. */
  id: string;
  type: NotificationType;
  /** i18n key for the title. */
  titleKey: string;
  /** i18n key for the body. */
  messageKey: string;
  /** Interpolation params for both keys. */
  params?: Record<string, string | number>;
  /** i18n key for the call-to-action label, if any. */
  actionKey?: string;
  /** Named route the card links to, if any. */
  route?: string;
}

const STORAGE_KEY = "verd_read_notifications";

function loadRead(): Set<string> {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return new Set(raw ? (JSON.parse(raw) as string[]) : []);
  } catch {
    return new Set();
  }
}

export const useNotificationsStore = defineStore("notifications", () => {
  const plants = usePlantsStore();
  const weather = useWeatherStore();

  const readIds = ref<Set<string>>(loadRead());

  function persist() {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify([...readIds.value]));
    } catch {
      /* storage unavailable — keep in-memory only */
    }
  }

  /** All notifications derived from the current garden + weather state. */
  const notifications = computed<AppNotification[]>(() => {
    const list: AppNotification[] = [];

    // Per-plant care reminders.
    for (const p of plants.plants) {
      if (p.status === "NEEDS WATER") {
        list.push({
          id: `water-${p.id}`,
          type: "water",
          titleKey: "notifications.waterTitle",
          messageKey: "notifications.waterMsg",
          params: { name: p.name, level: p.wateringLevel },
          actionKey: "notifications.action.plants",
          route: "plants",
        });
      } else if (p.status === "NEEDS MISTING") {
        list.push({
          id: `mist-${p.id}`,
          type: "misting",
          titleKey: "notifications.mistTitle",
          messageKey: "notifications.mistMsg",
          params: { name: p.name },
          actionKey: "notifications.action.plants",
          route: "plants",
        });
      }
    }

    // Weather-driven alerts.
    if (weather.uvIndex >= 6) {
      list.push({
        id: "weather-uv",
        type: "weather",
        titleKey: "notifications.uvTitle",
        messageKey: "notifications.uvMsg",
        params: { uv: weather.uvIndex, label: weather.uvLabel },
        actionKey: "notifications.action.weather",
        route: "weather",
      });
    }
    if (weather.temp >= 30) {
      list.push({
        id: "weather-hot",
        type: "weather",
        titleKey: "notifications.hotTitle",
        messageKey: "notifications.hotMsg",
        params: { temp: weather.temp },
        actionKey: "notifications.action.weather",
        route: "weather",
      });
    }
    if (weather.humidity > 0 && weather.humidity <= 35) {
      list.push({
        id: "weather-dry",
        type: "weather",
        titleKey: "notifications.dryTitle",
        messageKey: "notifications.dryMsg",
        params: { humidity: weather.humidity },
        actionKey: "notifications.action.weather",
        route: "weather",
      });
    }

    // Positive reinforcement.
    if (plants.totalPlants > 0 && plants.plantsNeedingCare.length === 0) {
      list.push({
        id: "tip-healthy",
        type: "tip",
        titleKey: "notifications.healthyTitle",
        messageKey: "notifications.healthyMsg",
        params: { count: plants.totalPlants },
        actionKey: "notifications.action.plants",
        route: "plants",
      });
    }
    if (plants.careStreak > 0) {
      list.push({
        id: "streak",
        type: "streak",
        titleKey: "notifications.streakTitle",
        messageKey: "notifications.streakMsg",
        params: { days: plants.careStreak },
      });
    }

    return list;
  });

  const unreadCount = computed(
    () => notifications.value.filter((n) => !readIds.value.has(n.id)).length,
  );

  function isRead(id: string) {
    return readIds.value.has(id);
  }

  function markRead(id: string) {
    if (readIds.value.has(id)) return;
    readIds.value.add(id);
    readIds.value = new Set(readIds.value);
    persist();
  }

  function markAllRead() {
    for (const n of notifications.value) readIds.value.add(n.id);
    readIds.value = new Set(readIds.value);
    persist();
  }

  return {
    notifications,
    unreadCount,
    isRead,
    markRead,
    markAllRead,
  };
});
