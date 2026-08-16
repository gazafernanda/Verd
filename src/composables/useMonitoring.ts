import { ref, computed, onMounted, onUnmounted } from "vue";
import { usePlantsStore } from "../stores/plants";
import { useWeatherStore } from "../stores/weather";
import { useUserStore } from "../stores/user";

/** How often monitoring data is refreshed while the page is visible. */
const POLL_INTERVAL_MS = 60_000;

/**
 * Keeps sensor and garden readings current without a manual page reload.
 *
 * Polling is used rather than a websocket because the API is a plain REST
 * service on a host that sleeps when idle; a socket would spend most of its life
 * reconnecting. Polling pauses while the tab is hidden — a background tab
 * refreshing every minute is wasted work — and fires once immediately on return
 * so the user never reads stale numbers after switching back.
 */
export function useMonitoring(intervalMs: number = POLL_INTERVAL_MS) {
  const plants = usePlantsStore();
  const weather = useWeatherStore();
  const user = useUserStore();

  const refreshing = ref(false);

  // Ticks every second purely to re-evaluate the "x minutes ago" label, which
  // would otherwise freeze at "just now" until the next actual refresh.
  const now = ref(Date.now());

  let pollTimer: ReturnType<typeof setInterval> | undefined;
  let clockTimer: ReturnType<typeof setInterval> | undefined;

  async function refresh(force = true) {
    if (refreshing.value || !user.isAuthenticated) return;
    refreshing.value = true;
    try {
      // Both are independent reads; running them together halves the latency.
      await Promise.all([weather.fetchWeather(force), plants.fetchPlants()]);
    } finally {
      refreshing.value = false;
      now.value = Date.now();
    }
  }

  function startPolling() {
    stopPolling();
    pollTimer = setInterval(() => refresh(true), intervalMs);
  }

  function stopPolling() {
    clearInterval(pollTimer);
    pollTimer = undefined;
  }

  function handleVisibilityChange() {
    if (document.hidden) {
      stopPolling();
    } else {
      refresh(true);
      startPolling();
    }
  }

  onMounted(() => {
    clockTimer = setInterval(() => (now.value = Date.now()), 1000);
    if (!document.hidden) startPolling();
    document.addEventListener("visibilitychange", handleVisibilityChange);
  });

  onUnmounted(() => {
    stopPolling();
    clearInterval(clockTimer);
    document.removeEventListener("visibilitychange", handleVisibilityChange);
  });

  /** Seconds since the last successful refresh, or null if nothing has landed yet. */
  const secondsSinceUpdate = computed(() => {
    if (!weather.lastUpdatedAt) return null;
    return Math.max(0, Math.floor((now.value - weather.lastUpdatedAt) / 1000));
  });

  /** True once refreshes have been failing long enough to be worth reporting. */
  const disconnected = computed(() => weather.isStale);

  return {
    refresh,
    refreshing,
    secondsSinceUpdate,
    disconnected,
    lastUpdatedAt: computed(() => weather.lastUpdatedAt),
  };
}
