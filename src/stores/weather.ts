import { ref, computed } from "vue";
import { defineStore } from "pinia";

const API = import.meta.env.VITE_API_URL || "http://localhost:5000";

const WMO_ICONS: Record<number, string> = {
  0: "sun", 1: "sun", 2: "cloud-sun", 3: "cloud",
  45: "cloud", 48: "cloud",
  51: "rain", 53: "rain", 55: "rain", 56: "rain", 57: "rain",
  61: "rain", 63: "rain", 65: "rain", 66: "rain", 67: "rain",
  71: "cloud", 73: "cloud", 75: "cloud", 77: "cloud",
  80: "rain", 81: "rain", 82: "rain", 85: "cloud", 86: "cloud",
  95: "rain", 96: "rain", 99: "rain",
};

const DAYS = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
const MONTHS = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

function round(n: number) {
  return Math.round(n);
}

export const useWeatherStore = defineStore("weather", () => {
  const loading = ref(false);
  const lastFetched = ref<number | null>(null);
  const CACHE_MS = 5 * 60 * 1000; // 5 minutes

  const temp = ref(22);
  const condition = ref("Partly Cloudy");
  const humidity = ref(45);
  const uvIndex = ref(6);
  const soilMoisture = ref(32);
  const windSpeed = ref(13);
  const aqi = ref(24);
  const feelsLike = ref(24);

  const forecast = ref([
    { day: "TODAY", date: "May 12", icon: "sun",       tempHi: 22, tempLo: 12, active: true  },
    { day: "MON",   date: "May 13", icon: "cloud-sun", tempHi: 20, tempLo: 11, active: false },
    { day: "TUE",   date: "May 14", icon: "rain",      tempHi: 17, tempLo: 9,  active: false },
    { day: "WED",   date: "May 15", icon: "cloud",     tempHi: 18, tempLo: 10, active: false },
    { day: "THU",   date: "May 16", icon: "sun",       tempHi: 21, tempLo: 11, active: false },
  ]);

  const uvLabel = computed(() => {
    if (uvIndex.value >= 8) return "Very High";
    if (uvIndex.value >= 6) return "High";
    if (uvIndex.value >= 3) return "Moderate";
    return "Low";
  });

  async function fetchWeatherByCoords(latitude: number, longitude: number) {
    if (loading.value) return;
    loading.value = true;
    try {
      const url =
        `https://api.open-meteo.com/v1/forecast?latitude=${latitude}&longitude=${longitude}` +
        `&daily=weathercode,temperature_2m_max,temperature_2m_min` +
        `&hourly=temperature_2m,relativehumidity_2m,windspeed_10m,uv_index,apparent_temperature,soil_moisture_0_to_1cm` +
        `&timezone=auto&forecast_days=7`;
      const res = await fetch(url);
      if (!res.ok) return;
      const data = await res.json();

      const daily = data.daily;
      forecast.value = daily.time.map((dateStr: string, i: number) => {
        const d = new Date(dateStr + "T00:00:00");
        const isToday = i === 0;
        return {
          day: isToday ? "TODAY" : DAYS[d.getDay()],
          date: `${MONTHS[d.getMonth()]} ${d.getDate()}`,
          icon: WMO_ICONS[daily.weathercode[i]] ?? "cloud-sun",
          tempHi: round(daily.temperature_2m_max[i]),
          tempLo: round(daily.temperature_2m_min[i]),
          active: isToday,
        };
      });

      // use current hour's hourly data for current conditions
      const now = new Date();
      const hourIndex = now.getHours();
      const hourly = data.hourly;
      if (hourly) {
        temp.value = round(hourly.temperature_2m[hourIndex]);
        feelsLike.value = round(hourly.apparent_temperature[hourIndex]);
        humidity.value = Math.round(hourly.relativehumidity_2m[hourIndex]);
        windSpeed.value = Math.round(hourly.windspeed_10m[hourIndex]);
        uvIndex.value = Math.round(hourly.uv_index[hourIndex] ?? 0);
        soilMoisture.value = Math.min(100, Math.round((hourly.soil_moisture_0_to_1cm?.[hourIndex] ?? 0) * 100));
      }

      lastFetched.value = Date.now();
    } catch {
      // silent fail
    } finally {
      loading.value = false;
    }
  }

  async function fetchWeather(force = false) {
    const token = localStorage.getItem("verd_token");
    if (!token) return;
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
    fetchWeatherByCoords,
  };
});
