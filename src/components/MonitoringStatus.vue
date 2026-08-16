<script setup lang="ts">
/**
 * Shows when the monitoring data last refreshed, and warns when refreshes are
 * failing so the user can tell "nothing changed" apart from "nothing is arriving".
 */
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import HelpTip from './HelpTip.vue'
import { RefreshCw, Wifi, WifiOff } from 'lucide-vue-next'

const props = defineProps<{
  secondsSinceUpdate: number | null
  disconnected: boolean
  refreshing: boolean
}>()

const emit = defineEmits<{ refresh: [] }>()

const { t } = useI18n()

const relativeTime = computed(() => {
  const seconds = props.secondsSinceUpdate
  if (seconds === null) return t('monitoring.never')
  if (seconds < 10) return t('monitoring.justNow')
  if (seconds < 60) return t('monitoring.secondsAgo', { count: seconds })
  const minutes = Math.floor(seconds / 60)
  if (minutes < 60) return t('monitoring.minutesAgo', { count: minutes })
  return t('monitoring.hoursAgo', { count: Math.floor(minutes / 60) })
})
</script>

<template>
  <div class="flex items-center gap-3 flex-wrap">
    <!-- Connection state -->
    <span
      class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-lg text-[0.75rem] font-semibold transition-colors"
      :class="disconnected
        ? 'bg-[#fff4e5] text-[#b45309]'
        : 'bg-light-green-bg text-success-green'"
      :title="disconnected ? t('monitoring.offlineHint') : t('monitoring.liveHint')"
    >
      <WifiOff v-if="disconnected" width="13" height="13" />
      <Wifi v-else width="13" height="13" />
      {{ disconnected ? t('monitoring.offline') : t('monitoring.live') }}
    </span>

    <!-- Last updated -->
    <span class="inline-flex items-center gap-1.5 text-[0.78rem] text-text-muted font-medium">
      {{ t('monitoring.lastUpdated') }}
      <span class="text-text-main font-semibold">{{ relativeTime }}</span>
      <HelpTip :label="t('help.lastUpdated')" :size="12" />
    </span>

    <!-- Manual refresh -->
    <button
      @click="emit('refresh')"
      :disabled="refreshing"
      class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-lg text-[0.75rem] font-semibold text-text-muted hover:text-text-main hover:bg-bg-app transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
      :title="t('monitoring.refreshNow')"
    >
      <RefreshCw width="13" height="13" :class="refreshing ? 'animate-spin' : ''" />
      {{ t('monitoring.refreshNow') }}
    </button>
  </div>
</template>
