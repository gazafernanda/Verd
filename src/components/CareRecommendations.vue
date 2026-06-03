<script setup lang="ts">
import CareCard from "./CareCard.vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { usePlantsStore } from "../stores/plants";

const { t } = useI18n();
const router = useRouter();
const plants = usePlantsStore();
</script>

<template>
  <div class="flex flex-col">
    <div class="flex justify-between items-center mb-6">
      <div class="flex items-center gap-3 min-w-0">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#1a5641" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="shrink-0">
          <path d="M12 22V13M12 13C12 13 7 12.5 4 9c0 0 2.5 8 8 9M12 13C12 13 17 12.5 20 9c0 0-2.5 8-8 9M12 8a4 4 0 0 0-4-4c0 2.21 1.79 4 4 4zm0 0a4 4 0 0 1 4-4c0 2.21-1.79 4-4 4z"/>
        </svg>
        <h2 class="text-[1.4rem] max-lg:text-[1.1rem] font-bold text-text-main m-0 truncate">
          {{ t('careRecommendations.title') }}
        </h2>
      </div>
      <button
        type="button"
        @click="router.push({ name: 'recommendation' })"
        class="text-[0.95rem] font-bold text-primary transition-colors duration-200 hover:underline hover:text-primary-hover shrink-0"
        >{{ t('careRecommendations.viewSchedule') }}</button
      >
    </div>

    <div class="w-full overflow-x-auto pb-4 -mb-4 scrollbar-none">
      <div class="flex gap-6 w-max pr-6">
        <div
          v-for="plant in plants.plants"
          :key="plant.id"
          class="w-[min(300px,80vw)] shrink-0 self-stretch"
        >
          <CareCard
            :category="plant.careCard.category"
            :title="plant.careCard.title"
            :description="plant.careCard.description"
            :image="plant.careCard.image"
            :bgType="plant.careCard.bgType"
          />
        </div>
      </div>
    </div>
  </div>
</template>
