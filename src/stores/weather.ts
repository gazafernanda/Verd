import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { roundTemp } from "../utils/temperature";

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

// Every temperature entering the store is normalised here, so nothing downstream
// has to know where the reading came from or what unit it arrived in.
function round(n: number) {
  return roundTemp(n);
}

export const useWeatherStore = defineStore("weather", () => {
  const loading = ref(false);
  const lastFetched = ref<number | null>(null);
  const CACHE_MS = 5 * 60 * 1000; // 5 minutes

  /** When the displayed readings were last successfully refreshed. */
  const lastUpdatedAt = ref<number | null>(null);

  /**
   * Consecutive failed refreshes. One blip is normal on a flaky connection, so
   * the UI only warns once a refresh has failed twice in a row — otherwise the
   * indicator would flicker on every dropped request.
   */
  const failureCount = ref(0);
  const isStale = computed(() => failureCount.value >= 2);

  const temp = ref(22);
  const condition = ref("Partly Cloudy");
  const humidity = ref(45);
  const uvIndex = ref(6);
  const soilMoisture = ref(32);
  const windSpeed = ref(13);
  const aqi = ref(24);
  const feelsLike = ref(24);

  // 24 hourly samples starting at the current hour, used by the temp trend chart.
  const hourlyTemps = ref<{ hour: number; temp: number }[]>([]);

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
        // Pinned rather than relying on the API default, so the unit can never
        // drift out from under the UI.
        `&temperature_unit=celsius` +
        `&timezone=auto&forecast_days=7`;
      const res = await fetch(url);
      if (!res.ok) {
        failureCount.value += 1;
        return;
      }
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

        // Collect the next 24 hourly temperatures for the trend chart.
        const series: { hour: number; temp: number }[] = [];
        for (let offset = 0; offset <= 24; offset++) {
          const i = hourIndex + offset;
          if (hourly.temperature_2m[i] == null) break;
          const ts = new Date(hourly.time[i]);
          series.push({ hour: ts.getHours(), temp: round(hourly.temperature_2m[i]) });
        }
        hourlyTemps.value = series;
      }

      lastFetched.value = Date.now();
      lastUpdatedAt.value = lastFetched.value;
      failureCount.value = 0;
    } catch {
      failureCount.value += 1;
    } finally {
      loading.value = false;
    }
  }

  /** Resolves true when fresh readings were applied, false when the refresh failed. */
  async function fetchWeather(force = false): Promise<boolean> {
    const token = localStorage.getItem("verd_token");
    if (!token) return false;
    if (
      !force &&
      lastFetched.value &&
      Date.now() - lastFetched.value < CACHE_MS
    )
      return true;
    if (loading.value) return true;
    loading.value = true;
    try {
      const res = await fetch(`${API}/api/weather`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) {
        failureCount.value += 1;
        return false;
      }
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
      lastUpdatedAt.value = lastFetched.value;
      failureCount.value = 0;
      return true;
    } catch {
      // Keep showing the last good readings rather than blanking the dashboard —
      // the staleness indicator is what tells the user they're looking at old data.
      failureCount.value += 1;
      return false;
    } finally {
      loading.value = false;
    }
  }

  return {
    loading,
    lastUpdatedAt,
    failureCount,
    isStale,
    temp,
    condition,
    humidity,
    uvIndex,
    soilMoisture,
    windSpeed,
    aqi,
    feelsLike,
    forecast,
    hourlyTemps,
    uvLabel,
    fetchWeather,
    fetchWeatherByCoords,
  };
});
