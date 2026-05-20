import { ref, computed } from "vue";
import { defineStore } from "pinia";

const API = import.meta.env.VITE_API_URL ?? "http://localhost:5000";

export const useUserStore = defineStore("user", () => {
  const token = ref<string | null>(localStorage.getItem("verd_token"));
  const name = ref(localStorage.getItem("verd_name") ?? "");
  const location = ref(localStorage.getItem("verd_location") ?? "");
  const lat = ref<number | null>(
    localStorage.getItem("verd_lat") ? Number(localStorage.getItem("verd_lat")) : null
  );
  const lon = ref<number | null>(
    localStorage.getItem("verd_lon") ? Number(localStorage.getItem("verd_lon")) : null
  );
  const tier = ref(localStorage.getItem("verd_tier") ?? "Green Thumb");
  const memberSince = ref("");
  const displayName = ref(name.value);
  const email = ref(localStorage.getItem("verd_email") ?? "");
  const weatherAlertsEnabled = ref(true);

  const isAuthenticated = computed(() => !!token.value);

  function persist(data: {
    token: string;
    displayName: string;
    email: string;
    location: string;
    tier: string;
  }) {
    token.value = data.token;
    name.value = data.displayName;
    displayName.value = data.displayName;
    email.value = data.email;
    location.value = data.location;
    tier.value = data.tier;
    localStorage.setItem("verd_token", data.token);
    localStorage.setItem("verd_name", data.displayName);
    localStorage.setItem("verd_email", data.email);
    localStorage.setItem("verd_location", data.location);
    localStorage.setItem("verd_tier", data.tier);
  }

  async function login(emailInput: string, password: string) {
    let res: Response;
    try {
      res = await fetch(`${API}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: emailInput, password }),
      });
    } catch {
      throw new Error(
        "Cannot reach the server. Make sure the .NET API is running.",
      );
    }
    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message ?? "Invalid email or password.");
    }
    persist(await res.json());
  }

  async function register(
    displayNameInput: string,
    emailInput: string,
    password: string,
  ) {
    let res: Response;
    try {
      res = await fetch(`${API}/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          displayName: displayNameInput,
          email: emailInput,
          password,
        }),
      });
    } catch {
      throw new Error(
        "Cannot reach the server. Make sure the .NET API is running.",
      );
    }
    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message ?? "Registration failed.");
    }
    persist(await res.json());
  }

  function loginDemo(displayNameInput = "Alex Rivera") {
    persist({
      token: "demo",
      displayName: displayNameInput,
      email: `${displayNameInput.toLowerCase().replace(" ", ".")}@demo.com`,
      location: "San Francisco, CA",
      tier: "Green Thumb",
    });
  }

  function logout() {
    token.value = null;
    name.value = "";
    displayName.value = "";
    email.value = "";
    location.value = "";
    tier.value = "Green Thumb";
    localStorage.removeItem("verd_token");
    localStorage.removeItem("verd_name");
    localStorage.removeItem("verd_email");
    localStorage.removeItem("verd_location");
    localStorage.removeItem("verd_tier");
  }

  async function saveSettings(updates: {
    displayName?: string;
    location?: string;
    lat?: number;
    lon?: number;
    weatherAlertsEnabled?: boolean;
  }) {
    if (updates.displayName !== undefined) {
      displayName.value = updates.displayName;
      name.value = updates.displayName;
      localStorage.setItem("verd_name", updates.displayName);
    }
    if (updates.location !== undefined) {
      location.value = updates.location;
      localStorage.setItem("verd_location", updates.location);
    }
    if (updates.lat !== undefined) {
      lat.value = updates.lat;
      localStorage.setItem("verd_lat", String(updates.lat));
    }
    if (updates.lon !== undefined) {
      lon.value = updates.lon;
      localStorage.setItem("verd_lon", String(updates.lon));
    }
    if (updates.weatherAlertsEnabled !== undefined) {
      weatherAlertsEnabled.value = updates.weatherAlertsEnabled;
    }

    const storedToken = token.value;
    if (!storedToken || storedToken === "demo") return;
    await fetch(`${API}/api/users/settings`, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${storedToken}`,
      },
      body: JSON.stringify(updates),
    });
  }

  async function fetchProfile() {
    const storedToken = token.value;
    if (!storedToken || storedToken === "demo") return;
    try {
      const res = await fetch(`${API}/api/users/profile`, {
        headers: { Authorization: `Bearer ${storedToken}` },
      });
      if (!res.ok) return;
      const data = await res.json();
      displayName.value = data.displayName;
      name.value = data.displayName;
      email.value = data.email;
      location.value = data.location;
      tier.value = data.tier;
      memberSince.value = data.memberSince;
      weatherAlertsEnabled.value = data.weatherAlertsEnabled;
      localStorage.setItem("verd_name", data.displayName);
      localStorage.setItem("verd_email", data.email);
      localStorage.setItem("verd_location", data.location);
      localStorage.setItem("verd_tier", data.tier);
    } catch {
      // silent fail — use cached localStorage values
    }
  }

  async function detectLocationFromIP() {
    if (location.value) return;
    try {
      const res = await fetch("https://ipwho.is/");
      const data = await res.json();
      if (data.success && data.city) {
        const detected = data.region
          ? `${data.city}, ${data.region}`
          : data.city;
        await saveSettings({
          location: detected,
          lat: data.latitude,
          lon: data.longitude,
        });
      }
    } catch {
      // silent fail — user can set manually
    }
  }

  return {
    token,
    name,
    location,
    lat,
    lon,
    tier,
    memberSince,
    displayName,
    email,
    weatherAlertsEnabled,
    isAuthenticated,
    login,
    register,
    loginDemo,
    logout,
    saveSettings,
    fetchProfile,
    detectLocationFromIP,
  };
});
