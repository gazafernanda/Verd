import { ref, computed } from "vue";
import { defineStore } from "pinia";

const API = import.meta.env.VITE_API_URL || "http://localhost:5000";

export interface ChatMessage {
  role: "user" | "assistant";
  content: string;
  sentAt: string;
}

function authHeaders() {
  const token = localStorage.getItem("verd_token");
  return {
    "Content-Type": "application/json",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

/**
 * The assistant conversation. History lives on the server keyed by account, so
 * it survives logout, a page refresh, and moving to another device — the store
 * is a cache of it, never the source of truth.
 */
export const useChatStore = defineStore("chat", () => {
  const messages = ref<ChatMessage[]>([]);
  const loading = ref(false);
  const historyLoaded = ref(false);
  const loadingHistory = ref(false);
  const error = ref("");

  const isEmpty = computed(() => messages.value.length === 0);

  /**
   * Pulls the stored conversation. Runs once per session by default; pass
   * `force` after signing in as somebody else.
   */
  async function loadHistory(force = false) {
    if (loadingHistory.value) return;
    if (historyLoaded.value && !force) return;

    const token = localStorage.getItem("verd_token");
    if (!token) return;

    loadingHistory.value = true;
    try {
      const res = await fetch(`${API}/api/chat/history`, {
        headers: authHeaders(),
      });
      if (res.ok) {
        const stored: { role: string; content: string; sentAt: string }[] =
          await res.json();
        messages.value = stored.map((m) => ({
          role: m.role === "user" ? "user" : "assistant",
          content: m.content,
          sentAt: m.sentAt,
        }));
        historyLoaded.value = true;
      }
    } catch {
      // Keep whatever is already on screen; the user can still send a message.
    } finally {
      loadingHistory.value = false;
    }
  }

  async function send(text: string) {
    if (loading.value) return;
    const trimmed = text.trim();
    if (!trimmed) return;

    error.value = "";

    // Shown immediately so the conversation feels responsive; the server stores
    // its own copy, so a failure here costs nothing but a re-render.
    messages.value.push({
      role: "user",
      content: trimmed,
      sentAt: new Date().toISOString(),
    });

    loading.value = true;
    try {
      const res = await fetch(`${API}/api/chat`, {
        method: "POST",
        headers: authHeaders(),
        body: JSON.stringify({ message: trimmed }),
      });

      if (res.ok) {
        const data = await res.json();
        messages.value.push({
          role: "assistant",
          content: data.reply,
          sentAt: new Date().toISOString(),
        });
      } else {
        const body = await res.json().catch(() => ({}));
        error.value = body.message ?? "";
        return false;
      }
      return true;
    } catch {
      error.value = "";
      return false;
    } finally {
      loading.value = false;
    }
  }

  /** Clears the conversation on the server as well as on screen. */
  async function clear() {
    const token = localStorage.getItem("verd_token");
    if (!token) return;
    try {
      const res = await fetch(`${API}/api/chat/history`, {
        method: "DELETE",
        headers: authHeaders(),
      });
      if (res.ok || res.status === 204) messages.value = [];
    } catch {
      /* silent fail — the conversation stays on screen */
    }
  }

  /** Drops the cached copy on logout without touching the server's record. */
  function reset() {
    messages.value = [];
    historyLoaded.value = false;
    error.value = "";
  }

  return {
    messages,
    loading,
    loadingHistory,
    historyLoaded,
    error,
    isEmpty,
    loadHistory,
    send,
    clear,
    reset,
  };
});
