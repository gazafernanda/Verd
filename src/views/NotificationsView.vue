<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import {
  useNotificationsStore,
  type AppNotification,
  type NotificationType,
} from "../stores/notifications";
import { usePlantsStore } from "../stores/plants";
import { Bell, Droplet, CloudSun, Sun, Leaf, Award, CheckCheck } from "lucide-vue-next";

const router = useRouter();
const { t } = useI18n();
const notifications = useNotificationsStore();
const plants = usePlantsStore();

const activeFilter = ref<"all" | "unread">("all");

const filters = computed(() => [
  { key: "all", label: t("notifications.filterAll") },
  { key: "unread", label: t("notifications.filterUnread") },
]);

const visible = computed(() => {
  if (activeFilter.value === "unread") {
    return notifications.notifications.filter((n) => !notifications.isRead(n.id));
  }
  return notifications.notifications;
});

const iconFor: Record<NotificationType, typeof Bell> = {
  water: Droplet,
  misting: CloudSun,
  weather: Sun,
  tip: Leaf,
  streak: Award,
};

// Tailwind needs literal class strings, so map types to full class pairs.
const styleFor: Record<NotificationType, string> = {
  water: "bg-[#fff4e5] text-[#f59e0b]",
  misting: "bg-[#ebf5ff] text-[#3b82f6]",
  weather: "bg-[#fff4e5] text-[#f59e0b]",
  tip: "bg-light-green-bg text-success-green",
  streak: "bg-[#f3e8ff] text-[#8b5cf6]",
};

function openNotification(n: AppNotification) {
  notifications.markRead(n.id);
  if (n.route) router.push({ name: n.route });
}

onMounted(() => {
  if (plants.plants.length === 0) plants.fetchPlants();
});
</script>

<template>
  <div class="flex flex-col gap-6">
    <!-- Header -->
    <div class="flex justify-between items-end max-lg:flex-col max-lg:items-start max-lg:gap-4">
      <div>
        <span class="font-semibold text-success-green text-[0.9rem]">{{
          t("notifications.breadcrumb")
        }}</span>
        <h1
          class="text-[2.2rem] max-sm:text-[1.6rem] font-extrabold text-text-main mb-2 mt-2 tracking-[-0.5px]"
        >
          {{ t("notifications.title") }}
        </h1>
        <p class="text-text-muted text-[0.95rem] font-medium">
          <template v-if="notifications.unreadCount > 0">{{
            t("notifications.subtitleUnread", notifications.unreadCount)
          }}</template>
          <template v-else>{{ t("notifications.allCaughtUp") }}</template>
        </p>
      </div>
      <button
        v-if="notifications.unreadCount > 0"
        @click="notifications.markAllRead()"
        class="flex items-center gap-2 px-5 py-[11px] bg-surface border border-border text-text-main rounded-[24px] font-semibold text-[0.95rem] hover:bg-bg-app transition-colors"
      >
        <CheckCheck width="18" height="18" />
        {{ t("notifications.markAllRead") }}
      </button>
    </div>

    <!-- Filter pills -->
    <div v-if="notifications.notifications.length > 0" class="flex gap-2">
      <button
        v-for="f in filters"
        :key="f.key"
        @click="activeFilter = f.key as typeof activeFilter"
        class="px-4 py-2.5 rounded-xl text-[0.85rem] font-semibold border-2 transition-all"
        :class="
          activeFilter === f.key
            ? 'border-success-green bg-light-green-bg text-primary'
            : 'border-border bg-surface text-text-muted hover:border-success-green hover:text-text-main'
        "
      >
        {{ f.label }}
      </button>
    </div>

    <!-- Empty: nothing at all -->
    <div
      v-if="notifications.notifications.length === 0"
      class="flex flex-col items-center justify-center py-24 gap-5 text-center"
    >
      <div class="w-20 h-20 rounded-full bg-light-green-bg flex items-center justify-center">
        <Bell class="text-success-green" width="36" height="36" />
      </div>
      <div>
        <h2 class="text-[1.4rem] font-bold text-text-main mb-2">
          {{ t("notifications.emptyTitle") }}
        </h2>
        <p class="text-text-muted text-[0.95rem]">{{ t("notifications.emptyDesc") }}</p>
      </div>
    </div>

    <!-- Empty: no unread under the unread filter -->
    <div
      v-else-if="visible.length === 0"
      class="flex flex-col items-center justify-center py-16 gap-3 text-center"
    >
      <CheckCheck class="text-success-green" width="32" height="32" />
      <p class="text-text-muted font-medium">{{ t("notifications.noUnread") }}</p>
    </div>

    <!-- Notification list -->
    <div v-else class="flex flex-col gap-3">
      <button
        v-for="n in visible"
        :key="n.id"
        @click="openNotification(n)"
        class="text-left bg-surface rounded-2xl p-4 shadow-sm border flex gap-4 items-start transition-all hover:shadow-md"
        :class="
          notifications.isRead(n.id)
            ? 'border-border opacity-75'
            : 'border-success-green/40 bg-light-green-bg/30'
        "
      >
        <div
          class="w-11 h-11 rounded-xl flex items-center justify-center shrink-0"
          :class="styleFor[n.type]"
        >
          <component :is="iconFor[n.type]" width="20" height="20" />
        </div>

        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2">
            <h3 class="font-bold text-text-main text-[0.95rem] leading-tight">
              {{ t(n.titleKey, n.params ?? {}) }}
            </h3>
            <span
              v-if="!notifications.isRead(n.id)"
              class="w-2 h-2 rounded-full bg-success-green shrink-0"
            ></span>
          </div>
          <p class="text-text-muted text-[0.85rem] mt-1 leading-relaxed">
            {{ t(n.messageKey, n.params ?? {}) }}
          </p>
          <span
            v-if="n.actionKey"
            class="inline-block mt-2 text-[0.8rem] font-semibold text-primary"
          >
            {{ t(n.actionKey) }} →
          </span>
        </div>
      </button>
    </div>
  </div>
</template>
