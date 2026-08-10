<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useAdminStore, type AdminUser } from "../stores/admin";
import { useUserStore } from "../stores/user";
import {
  Users,
  Sprout,
  ShieldCheck,
  TriangleAlert,
  Search,
  Pencil,
  Trash2,
  KeyRound,
  Save,
  X,
  RotateCcw,
} from "lucide-vue-next";

const { t } = useI18n();
const admin = useAdminStore();
const user = useUserStore();

const activeTab = ref<"users" | "settings">("users");
const search = ref("");
const actionMessage = ref("");
const actionError = ref("");

// ── UC15: Manage User Account ───────────────────────────────────────────────
const editing = ref<AdminUser | null>(null);
const editForm = ref({ displayName: "", email: "", location: "", tier: "", role: "" });
const savingUser = ref(false);

const confirmDelete = ref<AdminUser | null>(null);
const resetting = ref<AdminUser | null>(null);
const newPassword = ref("");

const tiers = ["Green Thumb", "Sprout", "Botanist", "Master Gardener"];

function startEdit(u: AdminUser) {
  editing.value = u;
  editForm.value = {
    displayName: u.displayName,
    email: u.email,
    location: u.location,
    tier: u.tier,
    role: u.role,
  };
}

function flash(message: string) {
  actionMessage.value = message;
  actionError.value = "";
  setTimeout(() => (actionMessage.value = ""), 3000);
}

function fail(e: unknown) {
  actionError.value = e instanceof Error ? e.message : t("admin.genericError");
}

async function saveUser() {
  if (!editing.value) return;
  savingUser.value = true;
  actionError.value = "";
  try {
    await admin.updateUser(editing.value.id, editForm.value);
    flash(t("admin.userUpdated"));
    editing.value = null;
  } catch (e) {
    fail(e);
  } finally {
    savingUser.value = false;
  }
}

async function removeUser() {
  if (!confirmDelete.value) return;
  try {
    await admin.deleteUser(confirmDelete.value.id);
    flash(t("admin.userDeleted"));
    confirmDelete.value = null;
  } catch (e) {
    fail(e);
    confirmDelete.value = null;
  }
}

async function submitReset() {
  if (!resetting.value || newPassword.value.length < 8) return;
  try {
    await admin.resetPassword(resetting.value.id, newPassword.value);
    flash(t("admin.passwordReset"));
    resetting.value = null;
    newPassword.value = "";
  } catch (e) {
    fail(e);
  }
}

// ── UC16: System Setting Management ─────────────────────────────────────────
const draft = ref<Record<string, string>>({});
const savingSettings = ref(false);

// Editing writes to a draft so nothing is persisted until Save is pressed.
watch(
  () => admin.settings,
  (list) => {
    draft.value = Object.fromEntries(list.map((s) => [s.key, s.value]));
  },
  { immediate: true, deep: true },
);

const dirty = computed(() =>
  admin.settings.some((s) => draft.value[s.key] !== s.value),
);

const booleanKeys = ["ai.enabled", "registration.open"];
const numericKeys = ["uv.highThreshold", "weather.hotThresholdC", "weather.dryHumidityThreshold"];

function resetDraft() {
  draft.value = Object.fromEntries(admin.settings.map((s) => [s.key, s.value]));
}

async function persistSettings() {
  savingSettings.value = true;
  actionError.value = "";
  try {
    const changed: Record<string, string> = {};
    for (const s of admin.settings) {
      const value = draft.value[s.key];
      if (value !== undefined && value !== s.value) changed[s.key] = value;
    }
    await admin.saveSettings(changed);
    flash(t("admin.settingsSaved"));
  } catch (e) {
    fail(e);
  } finally {
    savingSettings.value = false;
  }
}

let searchTimer: ReturnType<typeof setTimeout>;
watch(search, (term) => {
  clearTimeout(searchTimer);
  searchTimer = setTimeout(() => admin.fetchUsers(term).catch(fail), 300);
});

onMounted(() => admin.loadAll());
</script>

<template>
  <div class="flex flex-col gap-6">
    <!-- Header -->
    <div>
      <span class="font-semibold text-success-green text-[0.9rem]">{{
        t("admin.breadcrumb")
      }}</span>
      <h1
        class="text-[2.2rem] max-sm:text-[1.6rem] font-extrabold text-text-main mb-2 mt-2 tracking-[-0.5px]"
      >
        {{ t("admin.title") }}
      </h1>
      <p class="text-text-muted text-[0.95rem] font-medium">{{ t("admin.subtitle") }}</p>
    </div>

    <!-- Toasts -->
    <div
      v-if="actionMessage"
      class="px-5 py-3 rounded-xl bg-light-green-bg text-primary font-semibold text-[0.9rem]"
    >
      {{ actionMessage }}
    </div>
    <div
      v-if="actionError || admin.error"
      class="px-5 py-3 rounded-xl bg-[#fdecec] text-[#c0392b] font-semibold text-[0.9rem]"
    >
      {{ actionError || admin.error }}
    </div>

    <!-- Stats -->
    <div v-if="admin.stats" class="grid grid-cols-4 max-lg:grid-cols-2 max-sm:grid-cols-1 gap-4">
      <div class="bg-surface border border-border rounded-xl p-5">
        <div class="flex items-center gap-2 text-text-muted text-[0.8rem] font-bold uppercase tracking-[0.5px]">
          <Users width="16" height="16" /> {{ t("admin.stats.users") }}
        </div>
        <div class="text-[2rem] font-extrabold text-text-main mt-2">{{ admin.stats.totalUsers }}</div>
        <div class="text-text-light text-[0.8rem]">
          {{ t("admin.stats.newThisWeek", { count: admin.stats.newUsersThisWeek }) }}
        </div>
      </div>
      <div class="bg-surface border border-border rounded-xl p-5">
        <div class="flex items-center gap-2 text-text-muted text-[0.8rem] font-bold uppercase tracking-[0.5px]">
          <ShieldCheck width="16" height="16" /> {{ t("admin.stats.admins") }}
        </div>
        <div class="text-[2rem] font-extrabold text-text-main mt-2">{{ admin.stats.totalAdmins }}</div>
      </div>
      <div class="bg-surface border border-border rounded-xl p-5">
        <div class="flex items-center gap-2 text-text-muted text-[0.8rem] font-bold uppercase tracking-[0.5px]">
          <Sprout width="16" height="16" /> {{ t("admin.stats.plants") }}
        </div>
        <div class="text-[2rem] font-extrabold text-text-main mt-2">{{ admin.stats.totalPlants }}</div>
        <div class="text-text-light text-[0.8rem]">
          {{ t("admin.stats.logs", { count: admin.stats.totalLogs }) }}
        </div>
      </div>
      <div class="bg-surface border border-border rounded-xl p-5">
        <div class="flex items-center gap-2 text-text-muted text-[0.8rem] font-bold uppercase tracking-[0.5px]">
          <TriangleAlert width="16" height="16" /> {{ t("admin.stats.needsCare") }}
        </div>
        <div class="text-[2rem] font-extrabold text-text-main mt-2">
          {{ admin.stats.plantsNeedingCare }}
        </div>
      </div>
    </div>

    <!-- Tabs -->
    <div class="flex gap-2">
      <button
        v-for="tab in (['users', 'settings'] as const)"
        :key="tab"
        @click="activeTab = tab"
        class="px-4 py-2.5 rounded-xl text-[0.85rem] font-semibold border-2 transition-all"
        :class="
          activeTab === tab
            ? 'border-success-green bg-light-green-bg text-primary'
            : 'border-border bg-surface text-text-muted hover:border-success-green hover:text-text-main'
        "
      >
        {{ tab === "users" ? t("admin.tabUsers") : t("admin.tabSettings") }}
      </button>
    </div>

    <!-- ── UC15: Manage User Account ── -->
    <section v-if="activeTab === 'users'" class="flex flex-col gap-4">
      <div class="relative max-w-md">
        <Search
          class="absolute left-4 top-1/2 -translate-y-1/2 text-text-light"
          width="18"
          height="18"
        />
        <input
          v-model="search"
          type="text"
          :placeholder="t('admin.searchPlaceholder')"
          class="w-full pl-11 pr-4 py-3 rounded-xl bg-surface border border-border text-text-main placeholder:text-text-light focus:outline-none focus:border-success-green transition-colors"
        />
      </div>

      <div class="bg-surface border border-border rounded-xl overflow-x-auto">
        <table class="w-full min-w-[820px] text-left">
          <thead>
            <tr class="border-b border-border">
              <th class="px-5 py-4 text-[0.75rem] font-bold uppercase tracking-[0.5px] text-text-muted">
                {{ t("admin.table.user") }}
              </th>
              <th class="px-5 py-4 text-[0.75rem] font-bold uppercase tracking-[0.5px] text-text-muted">
                {{ t("admin.table.role") }}
              </th>
              <th class="px-5 py-4 text-[0.75rem] font-bold uppercase tracking-[0.5px] text-text-muted">
                {{ t("admin.table.tier") }}
              </th>
              <th class="px-5 py-4 text-[0.75rem] font-bold uppercase tracking-[0.5px] text-text-muted">
                {{ t("admin.table.garden") }}
              </th>
              <th class="px-5 py-4 text-[0.75rem] font-bold uppercase tracking-[0.5px] text-text-muted">
                {{ t("admin.table.joined") }}
              </th>
              <th class="px-5 py-4 text-[0.75rem] font-bold uppercase tracking-[0.5px] text-text-muted text-right">
                {{ t("admin.table.actions") }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="admin.loading">
              <td colspan="6" class="px-5 py-10 text-center text-text-muted">
                {{ t("admin.loading") }}
              </td>
            </tr>
            <tr v-else-if="admin.users.length === 0">
              <td colspan="6" class="px-5 py-10 text-center text-text-muted">
                {{ t("admin.noUsers") }}
              </td>
            </tr>
            <tr
              v-for="u in admin.users"
              :key="u.id"
              class="border-b border-border last:border-0 hover:bg-bg-app transition-colors"
            >
              <td class="px-5 py-4">
                <div class="font-semibold text-text-main">
                  {{ u.displayName }}
                  <span
                    v-if="u.email === user.email"
                    class="ml-1 text-[0.7rem] font-bold text-success-green"
                    >{{ t("admin.you") }}</span
                  >
                </div>
                <div class="text-text-muted text-[0.85rem]">{{ u.email }}</div>
              </td>
              <td class="px-5 py-4">
                <span
                  class="px-2.5 py-1 rounded-full text-[0.72rem] font-bold"
                  :class="
                    u.role === 'Admin'
                      ? 'bg-[#f3e8ff] text-[#8b5cf6]'
                      : 'bg-light-green-bg text-success-green'
                  "
                  >{{ u.role }}</span
                >
              </td>
              <td class="px-5 py-4 text-text-muted text-[0.9rem]">{{ u.tier }}</td>
              <td class="px-5 py-4 text-text-muted text-[0.9rem]">
                {{ t("admin.gardenSummary", { plants: u.plantCount, logs: u.logCount }) }}
              </td>
              <td class="px-5 py-4 text-text-muted text-[0.9rem]">
                {{ new Date(u.memberSince).toLocaleDateString() }}
              </td>
              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-1">
                  <button
                    @click="startEdit(u)"
                    :title="t('admin.edit')"
                    class="p-2 rounded-lg text-text-muted hover:bg-bg-app hover:text-primary transition-colors"
                  >
                    <Pencil width="17" height="17" />
                  </button>
                  <button
                    @click="resetting = u"
                    :title="t('admin.resetPassword')"
                    class="p-2 rounded-lg text-text-muted hover:bg-bg-app hover:text-primary transition-colors"
                  >
                    <KeyRound width="17" height="17" />
                  </button>
                  <button
                    @click="confirmDelete = u"
                    :title="t('admin.delete')"
                    class="p-2 rounded-lg text-text-muted hover:bg-[#fdecec] hover:text-[#c0392b] transition-colors"
                  >
                    <Trash2 width="17" height="17" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>

    <!-- ── UC16: System Setting Management ── -->
    <section v-else class="flex flex-col gap-4">
      <div class="bg-surface border border-border rounded-xl p-6 flex flex-col gap-5">
        <div>
          <h2 class="text-[1.1rem] font-bold text-text-main">{{ t("admin.settingsTitle") }}</h2>
          <p class="text-text-muted text-[0.9rem] mt-1">{{ t("admin.settingsHint") }}</p>
        </div>

        <div
          v-for="s in admin.settings"
          :key="s.key"
          class="flex items-center gap-4 max-sm:flex-col max-sm:items-start py-3 border-b border-border last:border-0"
        >
          <div class="flex-1">
            <div class="font-semibold text-text-main text-[0.95rem]">
              {{ t(`admin.setting.${s.key}.label`) }}
            </div>
            <div class="text-text-muted text-[0.85rem]">
              {{ t(`admin.setting.${s.key}.help`) }}
            </div>
          </div>

          <label
            v-if="booleanKeys.includes(s.key)"
            class="inline-flex items-center cursor-pointer shrink-0"
          >
            <input
              type="checkbox"
              class="sr-only peer"
              :checked="draft[s.key] === 'true'"
              @change="draft[s.key] = ($event.target as HTMLInputElement).checked ? 'true' : 'false'"
            />
            <div
              class="relative w-11 h-6 bg-border rounded-full peer-checked:bg-success-green transition-colors after:content-[''] after:absolute after:top-0.5 after:left-0.5 after:bg-white after:rounded-full after:h-5 after:w-5 after:transition-transform peer-checked:after:translate-x-5"
            ></div>
          </label>

          <input
            v-else-if="numericKeys.includes(s.key)"
            v-model="draft[s.key]"
            type="number"
            class="w-28 shrink-0 px-3 py-2 rounded-lg bg-bg-app border border-border text-text-main focus:outline-none focus:border-success-green transition-colors"
          />

          <input
            v-else
            v-model="draft[s.key]"
            type="text"
            class="w-64 max-sm:w-full shrink-0 px-3 py-2 rounded-lg bg-bg-app border border-border text-text-main focus:outline-none focus:border-success-green transition-colors"
          />
        </div>

        <div class="flex gap-3 pt-1">
          <button
            @click="persistSettings"
            :disabled="!dirty || savingSettings"
            class="flex items-center gap-2 px-5 py-[11px] rounded-[24px] font-semibold text-[0.95rem] transition-colors disabled:opacity-50 disabled:cursor-not-allowed bg-primary text-white hover:bg-primary/90"
          >
            <Save width="18" height="18" />
            {{ savingSettings ? t("admin.saving") : t("common.saveChanges") }}
          </button>
          <button
            @click="resetDraft"
            :disabled="!dirty"
            class="flex items-center gap-2 px-5 py-[11px] bg-surface border border-border text-text-main rounded-[24px] font-semibold text-[0.95rem] hover:bg-bg-app transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <RotateCcw width="18" height="18" />
            {{ t("admin.revert") }}
          </button>
        </div>
      </div>
    </section>

    <!-- Edit user modal -->
    <div
      v-if="editing"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4"
      @click.self="editing = null"
    >
      <div class="bg-surface rounded-2xl w-full max-w-md p-6 flex flex-col gap-4">
        <div class="flex items-center justify-between">
          <h3 class="text-[1.15rem] font-bold text-text-main">{{ t("admin.editUser") }}</h3>
          <button
            @click="editing = null"
            class="p-1.5 rounded-lg text-text-muted hover:bg-bg-app transition-colors"
          >
            <X width="20" height="20" />
          </button>
        </div>

        <label class="flex flex-col gap-1.5">
          <span class="text-[0.8rem] font-bold text-text-muted uppercase tracking-[0.5px]">{{
            t("admin.field.displayName")
          }}</span>
          <input
            v-model="editForm.displayName"
            class="px-4 py-2.5 rounded-lg bg-bg-app border border-border text-text-main focus:outline-none focus:border-success-green"
          />
        </label>

        <label class="flex flex-col gap-1.5">
          <span class="text-[0.8rem] font-bold text-text-muted uppercase tracking-[0.5px]">{{
            t("admin.field.email")
          }}</span>
          <input
            v-model="editForm.email"
            type="email"
            class="px-4 py-2.5 rounded-lg bg-bg-app border border-border text-text-main focus:outline-none focus:border-success-green"
          />
        </label>

        <label class="flex flex-col gap-1.5">
          <span class="text-[0.8rem] font-bold text-text-muted uppercase tracking-[0.5px]">{{
            t("admin.field.location")
          }}</span>
          <input
            v-model="editForm.location"
            class="px-4 py-2.5 rounded-lg bg-bg-app border border-border text-text-main focus:outline-none focus:border-success-green"
          />
        </label>

        <div class="grid grid-cols-2 gap-3">
          <label class="flex flex-col gap-1.5">
            <span class="text-[0.8rem] font-bold text-text-muted uppercase tracking-[0.5px]">{{
              t("admin.field.role")
            }}</span>
            <select
              v-model="editForm.role"
              class="px-4 py-2.5 rounded-lg bg-bg-app border border-border text-text-main focus:outline-none focus:border-success-green"
            >
              <option value="Gardener">Gardener</option>
              <option value="Admin">Admin</option>
            </select>
          </label>
          <label class="flex flex-col gap-1.5">
            <span class="text-[0.8rem] font-bold text-text-muted uppercase tracking-[0.5px]">{{
              t("admin.field.tier")
            }}</span>
            <select
              v-model="editForm.tier"
              class="px-4 py-2.5 rounded-lg bg-bg-app border border-border text-text-main focus:outline-none focus:border-success-green"
            >
              <option v-for="tier in tiers" :key="tier" :value="tier">{{ tier }}</option>
            </select>
          </label>
        </div>

        <div class="flex gap-3 mt-1">
          <button
            @click="saveUser"
            :disabled="savingUser"
            class="flex-1 px-5 py-[11px] rounded-[24px] font-semibold bg-primary text-white hover:bg-primary/90 transition-colors disabled:opacity-50"
          >
            {{ savingUser ? t("admin.saving") : t("common.saveChanges") }}
          </button>
          <button
            @click="editing = null"
            class="px-5 py-[11px] rounded-[24px] font-semibold bg-surface border border-border text-text-main hover:bg-bg-app transition-colors"
          >
            {{ t("common.cancel") }}
          </button>
        </div>
      </div>
    </div>

    <!-- Reset password modal -->
    <div
      v-if="resetting"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4"
      @click.self="resetting = null"
    >
      <div class="bg-surface rounded-2xl w-full max-w-sm p-6 flex flex-col gap-4">
        <h3 class="text-[1.15rem] font-bold text-text-main">{{ t("admin.resetPassword") }}</h3>
        <p class="text-text-muted text-[0.9rem]">
          {{ t("admin.resetPasswordFor", { name: resetting.displayName }) }}
        </p>
        <input
          v-model="newPassword"
          type="password"
          :placeholder="t('admin.newPasswordPlaceholder')"
          class="px-4 py-2.5 rounded-lg bg-bg-app border border-border text-text-main focus:outline-none focus:border-success-green"
        />
        <div class="flex gap-3">
          <button
            @click="submitReset"
            :disabled="newPassword.length < 8"
            class="flex-1 px-5 py-[11px] rounded-[24px] font-semibold bg-primary text-white hover:bg-primary/90 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ t("admin.confirmReset") }}
          </button>
          <button
            @click="resetting = null; newPassword = ''"
            class="px-5 py-[11px] rounded-[24px] font-semibold bg-surface border border-border text-text-main hover:bg-bg-app transition-colors"
          >
            {{ t("common.cancel") }}
          </button>
        </div>
      </div>
    </div>

    <!-- Delete confirmation -->
    <div
      v-if="confirmDelete"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4"
      @click.self="confirmDelete = null"
    >
      <div class="bg-surface rounded-2xl w-full max-w-sm p-6 flex flex-col gap-4">
        <h3 class="text-[1.15rem] font-bold text-text-main">{{ t("admin.deleteUser") }}</h3>
        <p class="text-text-muted text-[0.9rem]">
          {{ t("admin.deleteWarning", { name: confirmDelete.displayName }) }}
        </p>
        <div class="flex gap-3">
          <button
            @click="removeUser"
            class="flex-1 px-5 py-[11px] rounded-[24px] font-semibold bg-[#c0392b] text-white hover:bg-[#a93226] transition-colors"
          >
            {{ t("admin.confirmDelete") }}
          </button>
          <button
            @click="confirmDelete = null"
            class="px-5 py-[11px] rounded-[24px] font-semibold bg-surface border border-border text-text-main hover:bg-bg-app transition-colors"
          >
            {{ t("common.cancel") }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
