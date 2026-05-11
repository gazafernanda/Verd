import { ref, computed } from "vue";
import { defineStore } from "pinia";

const API = import.meta.env.VITE_API_URL ?? "http://localhost:5000";

export const useWeatherStore = defineStore("weather", () => {
  const loading = ref(false);
  const lastFetched = ref<number | null>(null);
  const CACHE_MS = 5 * 60 * 1000; // 5 minutes

  const temp = ref(72);
  const condition = ref("Partly Cloudy");
  const humidity = ref(45);
  const uvIndex = ref(6);
  const soilMoisture = ref(32);
  const windSpeed = ref(8);
  const aqi = ref(24);
  const feelsLike = ref(75);

  const forecast = ref([
    {
      day: "TODAY",
      date: "May 12",
      icon: "sun",
      tempHi: 72,
      tempLo: 54,
      active: true,
    },
    {
      day: "MON",
      date: "May 13",
      icon: "cloud-sun",
      tempHi: 68,
      tempLo: 51,
      active: false,
    },
    {
      day: "TUE",
      date: "May 14",
      icon: "rain",
      tempHi: 62,
      tempLo: 49,
      active: false,
    },
    {
      day: "WED",
      date: "May 15",
      icon: "cloud",
      tempHi: 65,
      tempLo: 50,
      active: false,
    },
    {
      day: "THU",
      date: "May 16",
      icon: "sun",
      tempHi: 70,
      tempLo: 52,
      active: false,
    },
  ]);

  const uvLabel = computed(() => {
    if (uvIndex.value >= 8) return "Very High";
    if (uvIndex.value >= 6) return "High";
    if (uvIndex.value >= 3) return "Moderate";
    return "Low";
  });

  async function fetchWeather(force = false) {
    const token = localStorage.getItem("verd_token");
    if (!token || token === "demo") return;
    // skip if recently fetched and not forced
    if (
      !force &&
      lastFetched.value &&
      Date.now() - lastFetched.value < CACHE_MS
    )
      return;
    if (loading.value) return;
    loading.value = true;
    try {
      const res = await fetch(`${API}/api/weather`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) return;
      const data = await res.json();
      temp.value = data.temp;
      condition.value = data.condition;
      humidity.value = data.humidity;
      uvIndex.value = data.uvIndex;
      soilMoisture.value = data.soilMoisture;
      windSpeed.value = data.windSpeed;
      aqi.value = data.aqi;
      feelsLike.value = data.feelsLike;
      forecast.value = data.forecast;
      lastFetched.value = Date.now();
    } catch {
      // silent fail — keep showing defaults/cached values
    } finally {
      loading.value = false;
    }
  }

  return {
    loading,
    temp,
    condition,
    humidity,
    uvIndex,
    soilMoisture,
    windSpeed,
    aqi,
    feelsLike,
    forecast,
    uvLabel,
    fetchWeather,
  };
});
