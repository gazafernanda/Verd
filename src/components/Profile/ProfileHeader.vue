<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../../stores/user'
import { Check, MapPin, UserRound } from 'lucide-vue-next'
import EditProfileModal from './EditProfileModal.vue'
const { t } = useI18n()
const user = useUserStore()
const showEdit = ref(false)
</script>

<template>
  <div class="bg-surface rounded-2xl px-10 py-8 flex justify-between items-center shadow-sm border border-border max-md:flex-col max-md:items-center max-md:text-center max-md:gap-6 max-md:px-6 max-md:py-8">

    <!-- Avatar + Info -->
    <div class="flex items-center gap-6 max-md:flex-col max-md:items-center max-md:gap-4">
      <div class="relative w-24 h-24 rounded-full border-4 border-border bg-bg-app flex items-center justify-center shrink-0 overflow-hidden">
        <img v-if="user.avatarUrl" :src="user.avatarUrl" :alt="user.name" class="w-[88px] h-[88px] rounded-full object-cover" />
        <UserRound v-else class="text-text-light" width="48" height="48" stroke-width="1.5" />
        <div class="absolute bottom-0 -right-1 bg-success-green text-white w-6 h-6 rounded-full flex items-center justify-center border-2 border-surface">
          <Check width="12" height="12" stroke-width="2" />
        </div>
      </div>

      <div class="flex flex-col max-md:items-center">
        <h1 class="text-[2rem] font-extrabold text-text-main leading-none mb-2">{{ user.name }}</h1>
        <div class="flex items-center gap-2 mb-3 text-[0.95rem] font-medium flex-wrap max-md:justify-center">
          <span class="flex items-center gap-1 text-text-muted">
            <MapPin width="14" height="14" />
            {{ user.location }}
          </span>
          <span class="text-text-muted">•</span>
          <span class="text-success-green">{{ t('profile.tier', { tier: user.tier }) }}</span>
        </div>
        <p class="text-[0.7rem] font-extrabold text-text-muted tracking-widest m-0">{{ t('profile.memberSince', { date: user.memberSince.toUpperCase() }) }}</p>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex gap-3 max-md:w-full max-md:flex-col">
      <button @click="showEdit = true" class="px-6 py-3 rounded-[24px] text-[0.9rem] font-bold cursor-pointer bg-transparent text-text-main border border-border hover:bg-bg-app transition-colors max-md:w-full">{{ t('profile.editProfile') }}</button>
    </div>
  </div>

  <EditProfileModal v-if="showEdit" @close="showEdit = false" />
</template>
