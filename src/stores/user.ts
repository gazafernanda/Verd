import { ref, computed } from "vue";
import { defineStore } from "pinia";

const API = import.meta.env.VITE_API_URL || "http://localhost:5000";

// The API is hosted on Render's free tier, which spins down after inactivity
// and takes a while to wake. A plain fetch throws immediately on the first hit
// to a cold instance, so retry a few times with backoff before giving up.
async function fetchWithWake(input: string, init?: RequestInit, retries = 3) {
  for (let attempt = 0; ; attempt++) {
    try {
      return await fetch(input, init);
    } catch (e) {
      if (attempt >= retries) throw e;
      await new Promise((r) => setTimeout(r, 2000 * (attempt + 1)));
    }
  }
}

/** Shape returned by every endpoint that mints a session. */
interface AuthPayload {
  token: string;
  displayName: string;
  email: string;
  location: string;
  tier: string;
  role?: string;
  isEmailVerified?: boolean;
  avatarUrl?: string;
  authProvider?: string;
}

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
  const role = ref(localStorage.getItem("verd_role") ?? "Gardener");
  const avatarUrl = ref(localStorage.getItem("verd_avatar") ?? "");
  const memberSince = ref("");
  const displayName = ref(name.value);
  const email = ref(localStorage.getItem("verd_email") ?? "");
  const weatherAlertsEnabled = ref(true);

  // Cached so the verification banner renders on first paint, before the profile
  // request comes back. Defaults to true so a returning user isn't flashed a
  // "verify your email" warning they've already dealt with.
  const isEmailVerified = ref(localStorage.getItem("verd_verified") !== "false");
  const authProvider = ref(localStorage.getItem("verd_provider") ?? "local");

  const isAuthenticated = computed(() => !!token.value);
  const isAdmin = computed(() => role.value === "Admin");

  /** True when signed in but still held out of the core features. */
  const needsEmailVerification = computed(
    () => isAuthenticated.value && !isEmailVerified.value,
  );

  function persist(data: AuthPayload) {
    token.value = data.token;
    name.value = data.displayName;
    displayName.value = data.displayName;
    email.value = data.email;
    location.value = data.location;
    tier.value = data.tier;
    role.value = data.role ?? "Gardener";
    isEmailVerified.value = data.isEmailVerified ?? true;
    authProvider.value = data.authProvider ?? "local";
    if (data.avatarUrl) avatarUrl.value = data.avatarUrl;

    localStorage.setItem("verd_role", role.value);
    localStorage.setItem("verd_token", data.token);
    localStorage.setItem("verd_name", data.displayName);
    localStorage.setItem("verd_email", data.email);
    localStorage.setItem("verd_location", data.location);
    localStorage.setItem("verd_tier", data.tier);
    localStorage.setItem("verd_verified", String(isEmailVerified.value));
    localStorage.setItem("verd_provider", authProvider.value);
    if (data.avatarUrl) localStorage.setItem("verd_avatar", data.avatarUrl);
  }

  /** Pulls the server's message out of an error response, with a sane fallback. */
  async function messageFrom(res: Response, fallback: string) {
    const body = await res.json().catch(() => ({}));
    return body.message ?? fallback;
  }

  async function login(emailInput: string, password: string) {
    let res: Response;
    try {
      res = await fetchWithWake(`${API}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: emailInput, password }),
      });
    } catch {
      throw new Error(
        "The server is waking up. Please wait a moment and try again.",
      );
    }
    if (!res.ok) {
      throw new Error(await messageFrom(res, "Invalid email or password."));
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
      res = await fetchWithWake(`${API}/api/auth/register`, {
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
        "The server is waking up. Please wait a moment and try again.",
      );
    }
    if (!res.ok) {
      throw new Error(await messageFrom(res, "Registration failed."));
    }
    persist(await res.json());
  }

  // ── Google sign-in ─────────────────────────────────────────────────────────

  /** Client id comes from the API so it isn't baked into the built bundle. */
  async function fetchGoogleConfig(): Promise<{ clientId: string; enabled: boolean }> {
    try {
      const res = await fetchWithWake(`${API}/api/auth/google/config`);
      if (!res.ok) return { clientId: "", enabled: false };
      return await res.json();
    } catch {
      return { clientId: "", enabled: false };
    }
  }

  /** Exchanges a Google ID token for a Verd session. */
  async function googleSignIn(credential: string) {
    let res: Response;
    try {
      res = await fetchWithWake(`${API}/api/auth/google`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ credential }),
      });
    } catch {
      throw new Error(
        "The server is waking up. Please wait a moment and try again.",
      );
    }
    if (!res.ok) {
      throw new Error(await messageFrom(res, "Google sign-in failed."));
    }
    persist(await res.json());
  }

  // ── Email verification ─────────────────────────────────────────────────────

  /** Confirms the emailed token and upgrades the current session to verified. */
  async function verifyEmail(verificationToken: string) {
    const res = await fetchWithWake(`${API}/api/auth/verify-email`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ token: verificationToken }),
    });
    if (!res.ok) {
      throw new Error(await messageFrom(res, "Verification failed."));
    }
    persist(await res.json());
  }

  /**
   * Asks for a fresh verification email. Returns the seconds left when the
   * server is still rate-limiting, so the UI can show a live countdown.
   */
  async function resendVerification(): Promise<{ ok: boolean; retryAfter?: number; message: string }> {
    const storedToken = token.value;
    const res = await fetchWithWake(
      `${API}/api/auth/resend-verification${storedToken ? "/me" : ""}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...(storedToken ? { Authorization: `Bearer ${storedToken}` } : {}),
        },
        body: storedToken ? undefined : JSON.stringify({ email: email.value }),
      },
    );

    const body = await res.json().catch(() => ({}));
    if (res.status === 429) {
      return { ok: false, retryAfter: body.retryAfter ?? 60, message: body.message ?? "" };
    }
    return { ok: res.ok, message: body.message ?? "" };
  }

  // ── Password reset ─────────────────────────────────────────────────────────

  /**
   * Always resolves with the server's neutral message — whether the address is
   * registered is deliberately not observable from the client either.
   */
  async function forgotPassword(emailInput: string): Promise<string> {
    const res = await fetchWithWake(`${API}/api/auth/forgot-password`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: emailInput }),
    });
    const body = await res.json().catch(() => ({}));
    if (!res.ok) throw new Error(body.message ?? "Request failed.");
    return body.message ?? "";
  }

  async function isResetTokenValid(resetToken: string): Promise<boolean> {
    try {
      const res = await fetchWithWake(
        `${API}/api/auth/reset-password/valid?token=${encodeURIComponent(resetToken)}`,
      );
      if (!res.ok) return false;
      return (await res.json()).valid === true;
    } catch {
      return false;
    }
  }

  async function resetPassword(
    resetToken: string,
    password: string,
    confirmPassword: string,
  ): Promise<string> {
    const res = await fetchWithWake(`${API}/api/auth/reset-password`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ token: resetToken, password, confirmPassword }),
    });
    const body = await res.json().catch(() => ({}));
    if (!res.ok) throw new Error(body.message ?? "Reset failed.");
    return body.message ?? "";
  }

  function logout() {
    token.value = null;
    name.value = "";
    displayName.value = "";
    email.value = "";
    location.value = "";
    avatarUrl.value = "";
    tier.value = "Green Thumb";
    role.value = "Gardener";
    isEmailVerified.value = true;
    authProvider.value = "local";

    // Clears this device's session only. Chat history lives on the server and is
    // deliberately left untouched so it comes back on the next sign-in.
    localStorage.removeItem("verd_role");
    localStorage.removeItem("verd_token");
    localStorage.removeItem("verd_name");
    localStorage.removeItem("verd_email");
    localStorage.removeItem("verd_location");
    localStorage.removeItem("verd_avatar");
    localStorage.removeItem("verd_tier");
    localStorage.removeItem("verd_verified");
    localStorage.removeItem("verd_provider");
  }

  async function saveSettings(updates: {
    displayName?: string;
    location?: string;
    avatarUrl?: string;
    lat?: number;
    lon?: number;
    weatherAlertsEnabled?: boolean;
  }) {
    if (updates.displayName !== undefined) {
      displayName.value = updates.displayName;
      name.value = updates.displayName;
      localStorage.setItem("verd_name", updates.displayName);
    }
    if (updates.avatarUrl !== undefined) {
      avatarUrl.value = updates.avatarUrl;
      localStorage.setItem("verd_avatar", updates.avatarUrl);
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
    if (!storedToken) return;
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
    if (!storedToken) return;
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
      avatarUrl.value = data.avatarUrl ?? "";
      tier.value = data.tier;
      role.value = data.role ?? role.value;
      isEmailVerified.value = data.isEmailVerified ?? true;
      authProvider.value = data.authProvider ?? authProvider.value;
      localStorage.setItem("verd_role", role.value);
      memberSince.value = data.memberSince;
      weatherAlertsEnabled.value = data.weatherAlertsEnabled;
      localStorage.setItem("verd_name", data.displayName);
      localStorage.setItem("verd_email", data.email);
      localStorage.setItem("verd_location", data.location);
      localStorage.setItem("verd_avatar", data.avatarUrl ?? "");
      localStorage.setItem("verd_tier", data.tier);
      localStorage.setItem("verd_verified", String(isEmailVerified.value));
      localStorage.setItem("verd_provider", authProvider.value);
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
    avatarUrl,
    memberSince,
    displayName,
    email,
    weatherAlertsEnabled,
    role,
    isEmailVerified,
    authProvider,
    isAuthenticated,
    isAdmin,
    needsEmailVerification,
    login,
    register,
    logout,
    fetchGoogleConfig,
    googleSignIn,
    verifyEmail,
    resendVerification,
    forgotPassword,
    isResetTokenValid,
    resetPassword,
    saveSettings,
    fetchProfile,
    detectLocationFromIP,
  };
});
