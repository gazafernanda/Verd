<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { Trophy } from 'lucide-vue-next'
const { t } = useI18n()
</script>

<template>
  <div class="bg-surface rounded-xl py-8 px-6 shadow-sm border border-border flex flex-col items-center text-center">
    <div class="w-16 h-16 rounded-full flex items-center justify-center mb-4 bg-light-green-bg text-success-green">
      <Trophy width="32" height="32" />
    </div>

    <h3 class="text-[1.35rem] font-extrabold text-text-main mb-2">{{ t('profile.achievementTitle') }}</h3>
    <p class="text-[0.95rem] text-text-muted leading-relaxed mb-6 max-w-[80%]">{{ t('profile.achievementDesc') }}</p>

    <div class="w-full">
      <div class="w-full h-2 bg-border rounded mb-2 overflow-hidden">
        <div class="h-full rounded bg-success-green" style="width: 75%"></div>
      </div>
      <span class="text-[0.7rem] font-extrabold text-success-green tracking-[1px]">750 / 1000 XP</span>
    </div>
  </div>
</template>
