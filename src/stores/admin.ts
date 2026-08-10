import { ref } from "vue";
import { defineStore } from "pinia";

const API = import.meta.env.VITE_API_URL || "http://localhost:5000";

export interface AdminUser {
  id: number;
  displayName: string;
  email: string;
  location: string;
  tier: string;
  role: string;
  memberSince: string;
  weatherAlertsEnabled: boolean;
  plantCount: number;
  logCount: number;
}

export interface AdminStats {
  totalUsers: number;
  totalAdmins: number;
  totalPlants: number;
  totalLogs: number;
  plantsNeedingCare: number;
  newUsersThisWeek: number;
}

export interface SystemSetting {
  key: string;
  value: string;
  updatedAt: string;
}

export const useAdminStore = defineStore("admin", () => {
  const users = ref<AdminUser[]>([]);
  const stats = ref<AdminStats | null>(null);
  const settings = ref<SystemSetting[]>([]);
  const loading = ref(false);
  const error = ref("");

  function authHeaders(): Record<string, string> {
    const token = localStorage.getItem("verd_token") ?? "";
    return { Authorization: `Bearer ${token}`, "Content-Type": "application/json" };
  }

  /** Turns a failed response into the API's message so the UI can show why. */
  async function failure(res: Response, fallback: string) {
    const body = await res.json().catch(() => ({}));
    return new Error(body.message ?? fallback);
  }

  async function fetchStats() {
    const res = await fetch(`${API}/api/admin/stats`, { headers: authHeaders() });
    if (!res.ok) throw await failure(res, "Failed to load statistics.");
    stats.value = await res.json();
  }

  async function fetchUsers(search = "") {
    const query = search ? `?search=${encodeURIComponent(search)}` : "";
    const res = await fetch(`${API}/api/admin/users${query}`, { headers: authHeaders() });
    if (!res.ok) throw await failure(res, "Failed to load users.");
    users.value = await res.json();
  }

  async function fetchSettings() {
    const res = await fetch(`${API}/api/admin/settings`, { headers: authHeaders() });
    if (!res.ok) throw await failure(res, "Failed to load settings.");
    settings.value = await res.json();
  }

  async function loadAll(search = "") {
    loading.value = true;
    error.value = "";
    try {
      await Promise.all([fetchStats(), fetchUsers(search), fetchSettings()]);
    } catch (e) {
      error.value = e instanceof Error ? e.message : "Something went wrong.";
    } finally {
      loading.value = false;
    }
  }

  async function updateUser(id: number, data: Partial<AdminUser>) {
    const current = users.value.find((u) => u.id === id);
    if (!current) return;

    const res = await fetch(`${API}/api/admin/users/${id}`, {
      method: "PUT",
      headers: authHeaders(),
      body: JSON.stringify({
        displayName: data.displayName ?? current.displayName,
        email: data.email ?? current.email,
        location: data.location ?? current.location,
        tier: data.tier ?? current.tier,
        role: data.role ?? current.role,
        weatherAlertsEnabled: data.weatherAlertsEnabled ?? current.weatherAlertsEnabled,
      }),
    });
    if (!res.ok) throw await failure(res, "Failed to update user.");

    const updated: AdminUser = await res.json();
    const idx = users.value.findIndex((u) => u.id === id);
    if (idx !== -1) users.value[idx] = updated;
    return updated;
  }

  async function deleteUser(id: number) {
    const res = await fetch(`${API}/api/admin/users/${id}`, {
      method: "DELETE",
      headers: authHeaders(),
    });
    if (!res.ok) throw await failure(res, "Failed to delete user.");
    users.value = users.value.filter((u) => u.id !== id);
  }

  async function resetPassword(id: number, newPassword: string) {
    const res = await fetch(`${API}/api/admin/users/${id}/reset-password`, {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify({ newPassword }),
    });
    if (!res.ok) throw await failure(res, "Failed to reset password.");
  }

  async function saveSettings(changed: Record<string, string>) {
    const res = await fetch(`${API}/api/admin/settings`, {
      method: "PUT",
      headers: authHeaders(),
      body: JSON.stringify({ settings: changed }),
    });
    if (!res.ok) throw await failure(res, "Failed to save settings.");
    settings.value = await res.json();
  }

  return {
    users,
    stats,
    settings,
    loading,
    error,
    loadAll,
    fetchUsers,
    updateUser,
    deleteUser,
    resetPassword,
    saveSettings,
  };
});
